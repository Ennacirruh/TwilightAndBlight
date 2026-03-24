
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TwilightAndBlight.Events;
using TwilightAndBlight.Ability.Module;

namespace TwilightAndBlight.Map
{
    [SelectionBase]
    public class MapNode : MonoBehaviour
    {
        [SerializeField] private CombatEntity groundedEntity;
        //[SerializeField] private CombatEntity airbornEntity;
        //[SerializeField] private CombatEntity burrowedEntity;
        [SerializeField] private int movesToLeaveGround = 1;
        //[SerializeField] private int movesToLeaveAir = 1;
        //[SerializeField] private int movesToLeaveUnderground = 1;
        [SerializeField] private float pathfindingScoreModifier = 1f;
        [SerializeField] private Vector2Int positionInMap;
        [SerializeField] private bool spawnNode = false;

        public delegate void EntityMovePattern(CombatEntity entity, Vector3 targetPos);
        private Color defaultColor;
        public Renderer nodeRanderer;
        private static LayerMask mapNodeMask;
        private static bool maskGenerated = false;
        private CombatTeam spawnTeam;
        private bool moveInProgress;
        public Vector2Int PositionInMap { get { return positionInMap; } set{ positionInMap = value; } }
        private Coroutine terrainHeightShiftCoroutine;
        private Coroutine entityShiftCoroutine;
        private float targetTerrainHeight;
        private float currentTerrainShiftSpeed;
        private void Start()
        {
            int layer = LayerMask.NameToLayer("MapNode");
            gameObject.layer = layer;
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.layer = layer;
            }
            targetTerrainHeight = transform.position.y;
            defaultColor = nodeRanderer.material.color;
        }
        public bool IsOccupied()
        {
            return groundedEntity != null;
        }
        public CombatEntity GetCombatEntity()
        {
            return groundedEntity;
        }
        public void AssignEntity(CombatEntity entity, Vector3 offset = default)
        {
            MapNode node = entity.GetCurrentMapNode();
            if (node != this) // check if already assigned
            {
                this.groundedEntity = entity;

                TeleportMovePattern(entity, transform.position + offset);
                entity.SetCurrentMapNode(this);
                GameEvents.OnNodeEntered?.Invoke(new CombatEntityMapNodeInteractionCallback(entity, this));
                if (node != null)
                {
                    node.UnassignCurrentEntity();
                }

            }
        }
        public void UnassignCurrentEntity()
        {
            if (groundedEntity != null)
            {
                GameEvents.OnNodeExited?.Invoke(new CombatEntityMapNodeInteractionCallback(groundedEntity, this));
                groundedEntity = null;
            }
        }
        

        public void ColorMaterial(Color color)
        {
            nodeRanderer.material.color = color;
        }
        public void ResetMaterial()
        {
            //Debug.Log("Material Reset");
            nodeRanderer.material.color = defaultColor;
        }
        public static LayerMask GetMapNodeMask()
        {
            if (!maskGenerated)
            {
                maskGenerated = true;
                mapNodeMask = LayerMask.GetMask("MapNode");
            }
            return mapNodeMask;
        }
        public bool IsSpawnNode()
        {
            return spawnNode;
        }
        public void SetSpawnNode(bool spawnNode)
        {
            this.spawnNode = spawnNode; 
        }
        public void AssignSpanNodeFilter(CombatTeam combatTeam)
        {
            spawnTeam = combatTeam;
        }
        public CombatTeam GetSpawnFilter()
        {
            return spawnTeam;
        }
        public int GetMovesToLeaveNode()
        {
            return movesToLeaveGround;
        }
        public float GetPathFindingScoreModifier()
        {
            return pathfindingScoreModifier;
        }
        public void ShiftTerrainHeight(float offset, float speed)
        {
            currentTerrainShiftSpeed = speed;
            targetTerrainHeight = targetTerrainHeight + offset;
            targetTerrainHeight = Mathf.Clamp(targetTerrainHeight, GameManager.Instance.MinimumTerrainHeight, GameManager.Instance.MaximumTerrainHeight);
            if (terrainHeightShiftCoroutine == null)
            {
                terrainHeightShiftCoroutine = StartCoroutine(MoveTerrainToHeight());
            }
        }
        public void SetTerrainHeight(float newTerrainHeight, float speed)
        {
            currentTerrainShiftSpeed = speed;
            float offset = 0;
            if(targetTerrainHeight != transform.position.y)
            {
                offset = targetTerrainHeight - transform.position.y;
            }
            targetTerrainHeight = newTerrainHeight + offset;
            targetTerrainHeight = Mathf.Clamp(targetTerrainHeight, GameManager.Instance.MinimumTerrainHeight, GameManager.Instance.MaximumTerrainHeight);

            if (terrainHeightShiftCoroutine == null)
            {
                terrainHeightShiftCoroutine = StartCoroutine(MoveTerrainToHeight());
            }
        }
        private IEnumerator MoveTerrainToHeight()
        {
            moveInProgress = true;
            
            while (Mathf.Abs(transform.position.y - targetTerrainHeight) > 0.005f)
            {
                float speed = Mathf.Pow(10f * currentTerrainShiftSpeed, 0.75f) * Mathf.Max(.025f, (Mathf.Abs(transform.position.y - targetTerrainHeight) + 1) / 30f);
                float newHeight = Mathf.MoveTowards(transform.position.y, targetTerrainHeight, speed * Time.deltaTime);
                bool falling = newHeight < transform.position.y;
                transform.position = new Vector3(transform.position.x, newHeight, transform.position.z);
                CombatManager.Instance.ImpuseSource.GenerateImpulse(GameManager.Instance.TerrainShiftCameraShake.defaultCameraShakeDirection * GameManager.Instance.TerrainShiftCameraShake.cameraShakeForce * speed * Time.deltaTime);
                AdjustMapVerticalScale();
                if (IsOccupied() )
                {
                    if(falling) { speed = 0; }
                    if (entityShiftCoroutine == null)
                    {
                        entityShiftCoroutine = StartCoroutine(MoveEntityWithHeight(speed));
                    }
                }
                yield return null;
            }
            terrainHeightShiftCoroutine = null;
            transform.position = new Vector3(transform.position.x, targetTerrainHeight, transform.position.z);
            moveInProgress = false;
            
        }
        private IEnumerator MoveEntityWithHeight(float initialSpeed)
        {
            float speed = initialSpeed;
            float fallHeight = -1000;

            while (groundedEntity != null && (groundedEntity.transform.position.y != transform.position.y || speed > 0))
            {
                if(fallHeight == -1000 && speed <= 0)
                {
                    fallHeight = groundedEntity.transform.position.y;
                }
                speed -= GameManager.Instance.Gravity * Time.deltaTime;
                float newHeight = groundedEntity.transform.position.y + (speed * Time.deltaTime);
                if (newHeight < transform.position.y)
                {
                    newHeight = transform.position.y;
                }
                groundedEntity.transform.position = new Vector3(groundedEntity.transform.position.x, newHeight, groundedEntity.transform.position.z);
                yield return null;
            }
            if (groundedEntity != null)
            {
                float fallDistance = fallHeight - groundedEntity.transform.position.y;
                ApplyFallDamage(fallDistance, groundedEntity);
            }
            entityShiftCoroutine = null;


        }
        private void ApplyFallDamage(float fallDistance, CombatEntity targetEntity)
        {
            if (MapManager.WillTakeFallDamage(fallDistance))
            {
                float damage = (targetEntity.MaxHealth * GameManager.Instance.PercentHealthDamageOnFallPerMeter * fallDistance) + (GameManager.Instance.FlatDamageOnFallPerMeter * fallDistance);
                float resistance = 5f * Mathf.Clamp(targetEntity.Stats.Dexterity, -damage / 2f, Mathf.Infinity);
                damage = (damage / ((resistance / damage) + 1f));
                targetEntity.DamageEntity(null, damage, DamageType.Physical, GameManager.Instance.FallDamagePercentArmorPen);
            }
        }
        public void AdjustMapVerticalScale()
        {
            float currentHeight = transform.position.y;
            float diff = (GameManager.Instance.MinimumTerrainHeight - 2f) - currentHeight;
            transform.localScale = new Vector3(transform.localScale.x, - diff, transform.localScale.z);
        }

        #region Move Patterns
        public bool MoveInProgress()
        {
            return (moveInProgress || entityShiftCoroutine != null);
        }
        public void TeleportMovePattern(CombatEntity entity, Vector3 targetPos)
        {
            StartCoroutine(TeleportMovePatternCoroutine(entity, targetPos));
        }
        private IEnumerator TeleportMovePatternCoroutine(CombatEntity entity, Vector3 targetPos)
        {
            entity.transform.position = targetPos;
            yield return null;
        }
        public void FollowPathMovePattern(CombatEntity entity, List<MapNode> path, float moveSpeed, bool takeFallDamage = true)
        {
            moveInProgress = true;
            StartCoroutine(FollowPathMovePatternCoroutine(entity, path, moveSpeed, takeFallDamage));
        }
        private IEnumerator FollowPathMovePatternCoroutine(CombatEntity targetEntity, List<MapNode> path, float speed, bool takeFallDamage)
        {
            MapNode currentNode = targetEntity.GetCurrentMapNode();
            MapNode nodeMemory = currentNode;
            int targetIndex = 0;
            MapNode endPos = path[path.Count - 1];
            while (currentNode != endPos && targetEntity != null)
            {
                float maxDelta = speed * Time.deltaTime;
                Vector3 posMemory = targetEntity.transform.position;
                Vector3 newPos = Vector3.MoveTowards(targetEntity.transform.position, path[targetIndex].transform.position, maxDelta);
                
                if(newPos == path[targetIndex].transform.position)
                {
                    nodeMemory = currentNode;
                    currentNode = path[targetIndex];
                    if(takeFallDamage)
                    {
                        float fallDistance = MapManager.FallDistance(nodeMemory, currentNode);
                        ApplyFallDamage(fallDistance, targetEntity);
                    }
                    currentNode.AssignEntity(targetEntity, Vector3.zero);
                    targetIndex++;
                    if(targetIndex < path.Count)
                    {
                        float distanceTraveled = (newPos - posMemory).magnitude;
                        float difference = maxDelta - distanceTraveled;
                        newPos = Vector3.MoveTowards(newPos, path[targetIndex].transform.position, difference);
                    }
                }
                targetEntity.transform.position = newPos;
                if(targetEntity.Health <= 0)
                {
                    break;
                }
                yield return null;
            }
            moveInProgress = false;
        }
        #endregion
        private void OnDrawGizmos()
        {
            if (spawnNode)
            {
            Gizmos.DrawLine(transform.position, transform.position + transform.up * 2.5f);

            }
        }
    }
}
