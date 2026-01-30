using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using TwilightAndBlight.Map;
using UnityEngine;
namespace TwilightAndBlight.Ability
{
    public abstract class EA_GenericAbilityShape : EntityAbility
    {
        [SerializeField] protected DefaultAbilityShapes abilityShape;
        [SerializeField] protected float baseRange;
        [SerializeField] protected List<VariableStatScaler> rangeScalers = new List<VariableStatScaler>();
        [SerializeField] protected float baseSize;
        [SerializeField] protected List<VariableStatScaler> sizeScalers = new List<VariableStatScaler>();
        [SerializeField] protected bool respectLineOfSight;
        [SerializeField] protected float lineOfSightForgiveness = 0.25f;

        private float angleForArc;
        private float directionOfArc;
        private int arcRangeMem;
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

        protected override Dictionary<string, string> GenerateStringConversionTable()
        {
            return new Dictionary<string, string>() { 
                { "range", GetRange().ToString()},
                { "baserange", baseRange.ToString()},
                { "size", GetRange().ToString()},
                { "basesize", baseSize.ToString()},
                { "rangescalers", GetStringFromScalerList(rangeScalers)},
                { "sizescalers", GetStringFromScalerList(sizeScalers)},
                { "respectlineofsight", respectLineOfSight.ToString()} 
            };
        }
        protected float GetRange()
        {
            return GetScaledStat(baseRange, rangeScalers);

        }
        protected float GetSize()
        {
            return GetScaledStat(baseSize, sizeScalers);
        }

        private HashSet<MapNode> TargetHexagon(MapNode targetingOrigin)
        {
            Vector3Int direction = new Vector3Int(0, 0, -1);
            int radius = Mathf.FloorToInt(GetSize());
            HashSet<MapNode> newSet = MapManager.Instance.GetNodesWithinRange(targetingOrigin, radius);

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
        protected MapNode GetTrueOrigin(MapNode defaultTargetingOrigin)
        {
            switch (abilityShape)
            {
                case DefaultAbilityShapes.Hexagon:
                    return defaultTargetingOrigin;
                case DefaultAbilityShapes.Line:
                    return combatEntity.GetCurrentMapNode();
                case DefaultAbilityShapes.Arc:
                    return combatEntity.GetCurrentMapNode();
            }
            return defaultTargetingOrigin;
        }
        private HashSet<MapNode> TargetArc(MapNode targetingOrigin)
        {
             
            Vector3 directionVector = (targetingOrigin.transform.position - combatEntity.transform.position);
            directionOfArc = Mathf.Atan2(directionVector.x, directionVector.z) * Mathf.Rad2Deg;
            //Debug.Log($"{directionOfArc} vs {Vector3.Angle(Vector3.forward, directionVector)}");
            arcRangeMem = Mathf.FloorToInt(GetRange());
            angleForArc = GetSize();

            HashSet<MapNode> newSet = MapManager.Instance.GetNodesWithinRange(combatEntity.GetCurrentMapNode(), arcRangeMem, WithinAngle); 

            return newSet;
        }
        protected bool WithinAngle(MapNode node)
        {
            if(node == null) return false;
            Vector3 directionVector = (node.transform.position - combatEntity.transform.position);
            directionVector.y = 0;
            float angle = Mathf.Atan2(directionVector.x, directionVector.z) * Mathf.Rad2Deg;
            float delta = Mathf.Abs(Mathf.DeltaAngle(directionOfArc, angle));

            if (delta <= angleForArc / 2f)
            {
                if (directionVector.magnitude <= arcRangeMem * MapManager.gridPosMultiplier)
                {
                    return true;
                }
            }
            return false;
        }

        protected bool LineOfSightObstructed(MapNode origin, MapNode target, float rayHeightOffset)
        {
            RaycastHit hit;
            return LineOfSightObstructed(origin, target, rayHeightOffset, out hit);
        }
        protected bool LineOfSightObstructed(MapNode origin, MapNode target, float rayHeightOffset, out RaycastHit hit)
        {
            Vector3 originPos = origin.transform.position + (origin.transform.up * rayHeightOffset);
            float targetOffset = target.IsOccupied() ? target.GetCombatEntity().EntityHeight : lineOfSightForgiveness;
            Vector3 targetPos = target.transform.position + (target.transform.up * lineOfSightForgiveness);
            Vector3 direction = (targetPos - originPos).normalized;
            
            float distance = (targetPos - originPos).magnitude * 0.99f;
            bool result = Physics.Raycast(originPos, direction, out hit, distance, MapNode.GetMapNodeMask());
            return result;
        }
    }
}
