using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using TwilightAndBlight.Ability.Module;
using TwilightAndBlight.Map;
using UnityEngine;
namespace TwilightAndBlight.Ability
{
    public abstract class EA_GenericAbilityShape : EntityAbility
    {
        [SerializeField] protected DefaultAbilityShapes abilityShape;
        [SerializeField] protected AbilitySizeModule abilitySizeModule = new AbilitySizeModule();
        [SerializeField] protected AbilityRangeModule abilityRangeModule = new AbilityRangeModule();
        
        protected override void Awake()
        {
            base.Awake();
            abilitySizeModule.InitializeAbilityModule(this);
            abilityRangeModule.InitializeAbilityModule(this);
        }


        protected HashSet<MapNode> aquiredTargets = new HashSet<MapNode>();

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
            Dictionary<string, string> dict = new Dictionary<string, string>();
            abilitySizeModule.GenerateStringConversionTable(ref dict);
            abilityRangeModule.GenerateStringConversionTable(ref dict);
            return dict;
        }

        private HashSet<MapNode> TargetHexagon(MapNode targetingOrigin)
        {
            int radius = Mathf.FloorToInt(abilitySizeModule.GetSize());
            HashSet<MapNode> newSet = MapManager.Instance.GetNodesWithinRange(targetingOrigin, radius);
            return newSet;
        }
        private HashSet<MapNode> TargetLine(MapNode targetingOrigin)
        {
            HashSet<MapNode> newSet = new HashSet<MapNode>();
            float range = abilityRangeModule.GetRange() * MapManager.gridDistanceToWorldDistance;
            float size = abilitySizeModule.GetSize() * MapManager.gridSizeToWorldSize;
            Vector3 originPos = combatEntity.transform.position + (combatEntity.transform.up * combatEntity.EntityHeight);
            Vector3 targetPos = targetingOrigin.transform.position;
            targetPos.y = originPos.y;
            Vector3 direction = (targetPos - originPos).normalized;

            
            Vector3 boxSize = new Vector3(size / 2f, 40f, 0.05f);
            RaycastHit[] hits = Physics.BoxCastAll(originPos, boxSize , direction, Quaternion.LookRotation(direction, combatEntity.transform.up), range, MapNode.GetMapNodeMask());
            foreach (RaycastHit hit in hits)
            {
                MapNode node = hit.collider.GetComponentInParent<MapNode>();
                if (node != null)
                {
                    if (hit.distance <= range * MapManager.gridDistanceToWorldDistance)
                        newSet.Add(node);
                }
            }

            return newSet;
        }
        public override bool IsValidAbilityCast(MapNode targetNode)
        {
            if (aquiredTargets.Count > 0)
            {
                return true;
            }
            return false;
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
            arcRangeMem = Mathf.FloorToInt(abilityRangeModule.GetRange());
            angleForArc = abilitySizeModule.GetSize();

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
                if (directionVector.magnitude <= arcRangeMem * MapManager.gridDistanceToWorldDistance)
                {
                    return true;
                }
            }
            return false;
        }
        protected bool TargetIsInRange(MapNode targetingNode)
        {
            switch (abilityShape)
            {
                case DefaultAbilityShapes.Hexagon:
                    return MapManager.Instance.GetNodesWithinRange(combatEntity.GetCurrentMapNode(), Mathf.FloorToInt(abilityRangeModule.GetRange())).Contains(targetingNode);
                default:
                    return true;
            }
        }
        protected bool AlwaysDrawDefaultHightlight()
        {
            switch (abilityShape)
            {
                case DefaultAbilityShapes.Hexagon:
                    return true;
                default:
                    return false;
            }
        }
        protected virtual void DefaultHighlightBehavior(MapNode targetingOrigin)
        {
            MapManager.Instance.ResetHighlight();
            MapNode currentNode = combatEntity.GetCurrentMapNode();
            int range = Mathf.FloorToInt(abilityRangeModule.GetRange());
            HashSet<MapNode> nodesInRange = MapManager.Instance.GetNodesWithinRange(currentNode, range);
            foreach (MapNode node in nodesInRange)
            {
                MapManager.Instance.HighlightNodes(node, IndicatorType.AltGeneric);
            }
        }
      
        protected override void OnValidate()
        {
            abilitySizeModule.InitializeAbilityModule(this);
            abilityRangeModule.InitializeAbilityModule(this);
            base.OnValidate();
        }
    }
}
