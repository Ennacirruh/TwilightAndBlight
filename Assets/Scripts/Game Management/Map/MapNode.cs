
using System;
using System.Collections;
using System.Collections.Generic;
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
        private void Start()
        {
            int layer = LayerMask.NameToLayer("MapNode");
            gameObject.layer = layer;
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.layer = layer;
            }
            
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
        #region Move Patterns
        public bool MoveInProgress()
        {
            return moveInProgress;
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
        private IEnumerator FollowPathMovePatternCoroutine(CombatEntity entity, List<MapNode> path, float speed, bool takeFallDamage)
        {
            MapNode currentNode = entity.GetCurrentMapNode();
            MapNode nodeMemory = currentNode;
            int targetIndex = 0;
            MapNode endPos = path[path.Count - 1];
            while (currentNode != endPos)
            {
                float maxDelta = speed * Time.deltaTime;
                Vector3 posMemory = entity.transform.position;
                Vector3 newPos = Vector3.MoveTowards(entity.transform.position, path[targetIndex].transform.position, maxDelta);
                
                if(newPos == path[targetIndex].transform.position)
                {
                    nodeMemory = currentNode;
                    currentNode = path[targetIndex];
                    if(MapManager.WillTakeFallDamge(nodeMemory, currentNode) && takeFallDamage)
                    {
                        float fallDistance = MapManager.FallDistance(nodeMemory, currentNode);
                        float damage = (entity.MaxHealth * GameManager.Instance.PercentHealthDamageOnFallPerMeter * fallDistance) + (GameManager.Instance.FlatDamageOnFallPerMeter * fallDistance);
                        float resistance = 5f * Mathf.Clamp(entity.Stats.Dexterity, -damage / 2f, Mathf.Infinity);
                        damage = (damage / ((resistance / damage) + 1f));
                        entity.DamageEntity(null, damage, DamageType.Physical, GameManager.Instance.FallDamagePercentArmorPen);
                    }
                    currentNode.AssignEntity(entity, Vector3.zero);
                    targetIndex++;
                    if(targetIndex < path.Count)
                    {
                        float distanceTraveled = (newPos - posMemory).magnitude;
                        float difference = maxDelta - distanceTraveled;
                        newPos = Vector3.MoveTowards(newPos, path[targetIndex].transform.position, difference);
                    }
                }
                entity.transform.position = newPos;
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
