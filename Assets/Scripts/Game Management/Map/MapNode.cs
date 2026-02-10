
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace TwilightAndBlight.Map
{
    [SelectionBase]
    public class MapNode : MonoBehaviour
    {
        [SerializeField] private CombatEntity entity;
        [SerializeField] private int movesToLeave = 1;
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
            return entity != null;
        }
        public CombatEntity GetCombatEntity()
        {
            return entity;
        }
        public void AssignEntity(CombatEntity entity, Vector3 offset)
        {
            MapNode node = entity.GetCurrentMapNode();
            if (node != this) // check if already assigned
            {
                this.entity = entity;

                TeleportMovePattern(entity, transform.position + offset);
                entity.SetCurrentMapNode(this);
                GameEvents.OnNodeEntered?.Invoke(entity, this);
                if (node != null)
                {
                    node.UnassignCurrentEntity();
                }

            }
        }
        public void UnassignCurrentEntity()
        {
            if (entity != null)
            {
                GameEvents.OnNodeExited?.Invoke(entity, this);
                entity = null;
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
            return movesToLeave;
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
                bool falling = newHeight <= transform.position.y;
                transform.position = new Vector3(transform.position.x, newHeight, transform.position.z);
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
            
            while (entity.transform.position.y != transform.position.y)
            {
                if(fallHeight == -1000 && speed < 0)
                {
                    fallHeight = entity.transform.position.y;
                }
                speed -= GameManager.Instance.Gravity * Time.deltaTime;
                float newHeight = entity.transform.position.y + (speed * Time.deltaTime);
                if (newHeight < transform.position.y)
                {
                    newHeight = transform.position.y;
                }
                entity.transform.position = new Vector3(entity.transform.position.x, newHeight, entity.transform.position.z);
                yield return null;
            }
            float fallDistance = fallHeight - entity.transform.position.y;
            ApplyFallDamage(fallDistance, entity);
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
            while (currentNode != endPos)
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
