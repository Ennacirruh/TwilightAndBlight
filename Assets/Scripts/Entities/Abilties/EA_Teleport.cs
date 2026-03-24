using System.Collections;
using System.Collections.Generic;
using TwilightAndBlight.Ability.Module;
using TwilightAndBlight.Map;
using UnityEngine;
using UnityEngine.Playables;
namespace TwilightAndBlight.Ability
{
    public class EA_Teleport : EntityAbility
    {
        [SerializeReference] protected AbilityRangeModule teleportRangeModule = new AbilityRangeModule();
        protected HashSet<MapNode> nodesInRange = new HashSet<MapNode>();
        protected override void Awake()
        {
            base.Awake();
            teleportRangeModule.InitializeAbilityModule(this);
        }
        public override void HighlightAbility(MapNode targetingOrigin)
        {
            nodesInRange = MapManager.Instance.GetNodesWithinRange(combatEntity.GetCurrentMapNode(), Mathf.FloorToInt(teleportRangeModule.GetRange()));
            foreach (MapNode node in nodesInRange)
            {
                MapManager.Instance.HighlightNodes(node, IndicatorType.AltGeneric);
            }
            if (targetingOrigin.IsOccupied() || !nodesInRange.Contains(targetingOrigin))
            {
                MapManager.Instance.HighlightNodesAsOverlay(targetingOrigin, IndicatorType.Invalid);
            }
            else
            {
                MapManager.Instance.HighlightNodesAsOverlay(targetingOrigin, IndicatorType.Valid);
            }
        }
        protected void TeleportToNode(MapNode targetingOrigin)
        {
            targetingOrigin.AssignEntity(combatEntity, Vector3.zero);

        }
        protected override IEnumerator AbilityBehavior(MapNode targetingOrigin)
        {
            TeleportToNode(targetingOrigin);
            yield return null;
            EndAbility(targetingOrigin);
        }
        public override bool IsValidAbilityCast(MapNode targetNode)
        {
            if (base.IsValidAbilityCast(targetNode) && nodesInRange.Contains(targetNode)) return true;
            return false;
        }
        protected override Dictionary<string, string> GenerateStringConversionTable()
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            teleportRangeModule.GenerateStringConversionTable(ref dict);
            return dict;
        }
        protected override void OnValidate()
        {
            teleportRangeModule.InitializeAbilityModule(this);
            base.OnValidate();
            if(targetFilter != AbilityTarget.EmptyNode)
            {
                targetFilter = AbilityTarget.EmptyNode;
            }
        }

        //public override bool HasValidTargetInRange()
        //{
        //    return ValidTargetInRangeExists(combatEntity.GetCurrentMapNode(), teleportRangeModule.GetRange(), (MapNode node) => { return IsValidTarget(node); });
        //}
    }
}
