using System;
using System.Collections;
using UnityEngine;

namespace TwilightAndBlight.Map
{
    public class MapNode : MonoBehaviour
    {
        [SerializeField] private CombatEntity entity;
        [SerializeField] private int movesToLeave = 1;
        public delegate void EntityMovePattern(CombatEntity entity, Vector3 targetPos);
        private Color defaultColor;
        public Renderer nodeRanderer;
        private static LayerMask mapNodeMask;
        private static bool maskGenerated = false;
        private bool spawnNode = false;
        private CombatTeam spawnTeam;
        private float pathfindingScoreModifier = 1f;
       
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
        public void AssignEntity(CombatEntity entity, Vector3 offset, EntityMovePattern movePattern = null)
        {
            this.entity = entity;
            MapNode node = entity.GetCurrentMapNode();
            
            if (movePattern == null)
            {
                TeleportMovePattern(entity, transform.position + offset);
            }
            else
            {
                movePattern?.Invoke(entity, transform.position + offset);
            }
            if (node != null)
            {
                node.UnassignCurrentEntity();
            }
            entity.SetCurrentMapNode(this);
        }
        public void UnassignCurrentEntity()
        {
            entity = null;
        }
        public void TeleportMovePattern(CombatEntity entity, Vector3 targetPos)
        {
            StartCoroutine(TeleportMovePatternCoroutine(entity, targetPos));
        }
        private static IEnumerator TeleportMovePatternCoroutine(CombatEntity entity, Vector3 targetPos)
        {
            entity.transform.position = targetPos;
            yield return null;
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
    }
}
