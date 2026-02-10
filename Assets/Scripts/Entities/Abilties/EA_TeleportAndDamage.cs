using System.Collections;
using System.Collections.Generic;
using TwilightAndBlight.Ability.Module;
using TwilightAndBlight.Map;
using Unity.XR.CoreUtils;
using UnityEngine;
namespace TwilightAndBlight.Ability
{
    public class EA_TeleportAndDamage : EA_Teleport
    {
        [SerializeField] protected AbilityTarget originTargetFilter;
        [SerializeField] protected AbilityTarget destinationTargetFilter;
        [SerializeField] protected AbilitySizeModule originSizeModule = new AbilitySizeModule();
        [SerializeField] protected AbilityDamageModule originDamageModule = new AbilityDamageModule();
        [SerializeField] protected AbilitySizeModule destinationSizeModule = new AbilitySizeModule();
        [SerializeField] protected AbilityDamageModule destinationDamageModule = new AbilityDamageModule();
        protected HashSet<MapNode> originTargets = new HashSet<MapNode>();
        protected HashSet<MapNode> destinationTargets = new HashSet<MapNode>();


        private AbilityTarget originTargetMem;// for on validate

        private AbilityTarget destinationTargetMem;// for on validate

        protected override void Awake()
        {
            base.Awake();
            originDamageModule.InitializeAbilityModule(this, "origin");
            originSizeModule.InitializeAbilityModule(this, "origin");
            destinationDamageModule.InitializeAbilityModule(this, "destination");
            destinationSizeModule.InitializeAbilityModule(this, "destination");
        }
        public override void HighlightAbility(MapNode targetingOrigin)
        {
            MapManager.Instance.ResetHighlight();
            nodesInRange = MapManager.Instance.GetNodesWithinRange(combatEntity.GetCurrentMapNode(), Mathf.FloorToInt(teleportRangeModule.GetRange()));
            foreach (MapNode node in nodesInRange)
            {
                MapManager.Instance.HighlightNodes(node, IndicatorType.AltGeneric);
            }

            originTargets.Clear();
            destinationTargets.Clear();
            HashSet<MapNode> originNodes = GetNodesInRange(combatEntity.GetCurrentMapNode(), originSizeModule.GetSize());
            originDamageModule.HighlightNodes(originNodes, combatEntity.GetCurrentMapNode(), (node) => { return IsValidTarget(node, originTargetFilter); }, originSizeModule.GetSize(), ref originTargets);
            HashSet<MapNode> destinationNodes = GetNodesInRange(targetingOrigin, destinationSizeModule.GetSize());
            destinationDamageModule.HighlightNodes(destinationNodes, targetingOrigin, (node) => { return IsValidTarget(node, destinationTargetFilter); }, destinationSizeModule.GetSize(), ref destinationTargets);
            
            if (targetingOrigin.IsOccupied() || !nodesInRange.Contains(targetingOrigin))
            {
                MapManager.Instance.HighlightNodesAsOverlay(targetingOrigin, IndicatorType.Invalid);
            }
            else
            {
                MapManager.Instance.HighlightNodesAsOverlay(targetingOrigin, IndicatorType.Valid);
            }

        }
        
        protected override IEnumerator AbilityBehavior(MapNode targetingOrigin)
        {
            originDamageModule.moduleBehaviorCoroutine = StartCoroutine(originDamageModule.PerformDamageBehavior(originTargets, originSizeModule.GetSize() * MapManager.gridDistanceToWorldDistance));
            yield return new WaitUntil(() => { return originDamageModule.moduleBehaviorCoroutine == null; });
            destinationDamageModule.moduleBehaviorCoroutine = StartCoroutine(destinationDamageModule.PerformDamageBehavior(destinationTargets, destinationSizeModule.GetSize() * MapManager.gridDistanceToWorldDistance));
            yield return new WaitUntil(() => { return destinationDamageModule.moduleBehaviorCoroutine == null; });
            TeleportToNode(targetingOrigin);
            yield return null;
            EndAbility(targetingOrigin);
        }
        protected override Dictionary<string, string> GenerateStringConversionTable()
        {
            Dictionary<string, string> dict = base.GenerateStringConversionTable();
            originDamageModule.GenerateStringConversionTable(ref dict);
            destinationDamageModule.GenerateStringConversionTable(ref dict);
            originSizeModule.GenerateStringConversionTable(ref dict);
            destinationSizeModule.GenerateStringConversionTable(ref dict);
            return dict;
        }

        protected HashSet<MapNode> GetNodesInRange(MapNode origin,float range)
        {
            int radius = Mathf.FloorToInt(range);
            HashSet<MapNode> newSet = MapManager.Instance.GetNodesWithinRange(origin, radius);
            return newSet;
        }
        protected override void OnValidate()
        {
            originDamageModule.InitializeAbilityModule(this, "origin");
            originSizeModule.InitializeAbilityModule(this, "origin");
            destinationDamageModule.InitializeAbilityModule(this, "destination");
            destinationSizeModule.InitializeAbilityModule(this, "destination");
            base.OnValidate();
            if (originTargetFilter == AbilityTarget.EmptyNode || originTargetFilter == AbilityTarget.AnyNode)
            {
                originTargetFilter = originTargetMem;
            }
            else
            {
                originTargetMem = originTargetFilter;
            }
            if (destinationTargetFilter == AbilityTarget.EmptyNode || destinationTargetFilter == AbilityTarget.AnyNode)
            {
                destinationTargetFilter = destinationTargetMem;
            }
            else
            {
                destinationTargetMem = destinationTargetFilter;
            }
        }
    }
}