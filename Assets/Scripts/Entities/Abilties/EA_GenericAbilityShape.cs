using System.Collections;
using System.Collections.Generic;
using TwilightAndBlight.Map;
using UnityEngine;
namespace TwilightAndBlight.Ability
{
    public abstract class EA_GenericAbilityShape : EntityAbility
    {
        [SerializeField] private DefaultAbilityShapes abilityShape;
        [SerializeField] private float baseRange;
        [SerializeField] private List<VariableStatScaler> rangeScalers = new List<VariableStatScaler>();
        [SerializeField] private float baseSize;
        [SerializeField] private List<VariableStatScaler> sizeScalers = new List<VariableStatScaler>();
        [SerializeField] private bool respectLineOfSight;

        public virtual HashSet<MapNode> GetTargetingNodes(MapNode targetingOrigin)
        {
            switch (abilityShape)
            {
                case DefaultAbilityShapes.Hexagon:
                    return TargetHexagon(targetingOrigin);
                case DefaultAbilityShapes.Line:
                    return TargetLine(targetingOrigin);
                case DefaultAbilityShapes.Arc:
                    return TargetArc(targetingOrigin);
            }
            return new HashSet<MapNode>();
        }
        

       

        protected override Dictionary<string, string> GetStringConversionTable()
        {
            return new Dictionary<string, string>() { };
        }
        protected float GetRange()
        {
            float value = baseRange;
            foreach(VariableStatScaler scaler in rangeScalers)
            {
                float addition = scaler.valuePerScalingStat * entityStats.GetStat(scaler.scalingStat).Value;
                value += addition;
            }
            return value;

        }
        protected float GetSize()
        {
            float value = baseSize;
            foreach (VariableStatScaler scaler in sizeScalers)
            {
                float addition = scaler.valuePerScalingStat * entityStats.GetStat(scaler.scalingStat).Value;
                value += addition;
            }
            return value;
        }

        private HashSet<MapNode> TargetHexagon(MapNode targetingOrigin)
        {
            HashSet<MapNode> newSet = new HashSet<MapNode>();
            newSet.Add(targetingOrigin);
            Vector3Int direction = new Vector3Int(0, 0, -1);
            int radius = Mathf.FloorToInt(GetSize());
            Debug.Log("Radius: " + radius);
            for (int i = 0; i < 6; i++)
            {
                direction = new Vector3Int(-direction.z, direction.x, direction.y);
                for (int j = 1; j < radius; j++)
                {
                    Vector3Int swizzel = new Vector3Int(-direction.z, direction.x, direction.y);
                    for (int k = 0; k <= radius - 1 - j; k++)
                    {
                        Vector3Int offset = (swizzel * k) + (direction * j);
                        MapNode node = MapManager.Instance.GetRealativeNode(targetingOrigin, offset);
                        if(node != null)
                        {
                            newSet.Add(node);
                        }
                    }
                    
                }
            }

            return newSet;
        }
        private HashSet<MapNode> TargetLine(MapNode targetingOrigin)
        {
            HashSet<MapNode> newSet = new HashSet<MapNode>();
            float range = GetRange() * MapManager.gridPosMultiplier;
            float size = GetSize() * MapManager.gridSizeMultiplier;
            Vector3 originPos = combatEntity.transform.position + (combatEntity.transform.up * combatEntity.EntityHeight);
            Vector3 targetPos = targetingOrigin.transform.position;
            targetPos.y = originPos.y;
            Vector3 direction = (targetPos - originPos).normalized;

            
            Vector3 boxSize = new Vector3(size / 2f, 10f, 0.05f);
            RaycastHit[] hits = Physics.BoxCastAll(originPos, boxSize , direction, Quaternion.LookRotation(direction, combatEntity.transform.up), range, MapNode.GetMapNodeMask());
            foreach (RaycastHit hit in hits)
            {
                MapNode node = hit.collider.GetComponentInParent<MapNode>();
                if (node != null)
                {
                    newSet.Add(node);
                }
            }

            return newSet;
        }
        private HashSet<MapNode> TargetArc(MapNode targetingOrigin)
        {
            return null;
        }

        protected bool LineOfSightObstructed(MapNode origin, MapNode target, float rayHeightOffset)
        {
            RaycastHit hit;
            return LineOfSightObstructed(origin, target, rayHeightOffset, out hit);
        }
        protected bool LineOfSightObstructed(MapNode origin, MapNode target, float rayHeightOffset, out RaycastHit hit)
        {
            Vector3 originPos = origin.transform.position + (target.transform.up * rayHeightOffset);

            Vector3 direction = (target.transform.position - originPos).normalized;
            float distance = GetRange();
            return Physics.Raycast(originPos, direction, out hit, distance, MapNode.GetMapNodeMask());
        }
    }
}
