using System.Collections;
using System.Collections.Generic;
using TwilightAndBlight.Ability.Module;
using TwilightAndBlight.Map;
using UnityEngine;
namespace TwilightAndBlight.Ability
{
    public class EA_GenericHeal : EA_GenericAbilityShape
    {
        [SerializeField] protected AbilityHealModule abilityHealModule = new AbilityHealModule();


        private AbilityTarget targetMem;
        protected override void Awake()
        {
            base.Awake();
            abilityHealModule.InitializeAbilityModule(this);
        }
        public override void HighlightAbility(MapNode targetingOrigin)
        {
            MapManager.Instance.ResetHighlight();
            if (AlwaysDrawDefaultHightlight())
            {
                DefaultHighlightBehavior(targetingOrigin);
            }
            if (TargetIsInRange(targetingOrigin))
            {
                aquiredTargets.Clear();
                HashSet<MapNode> nodes = GetTargetingNodes(targetingOrigin);
                abilityHealModule.HighlightNodes(nodes, GetTrueOrigin(targetingOrigin), (node) => { return IsValidTarget(node); }, abilityRangeModule.GetRange(), ref aquiredTargets);
            }
        }
            
        
        protected override Dictionary<string, string> GenerateStringConversionTable()
        {
            Dictionary<string, string> table = base.GenerateStringConversionTable();
            abilityHealModule.GenerateStringConversionTable(ref table);
            return table;
        }
        protected override IEnumerator AbilityBehavior(MapNode targetingOrigin)
        {
            abilityHealModule.moduleBehaviorCoroutine = StartCoroutine(abilityHealModule.PerformHealBehavior(aquiredTargets,targetingOrigin, abilityRangeModule.GetRange() * MapManager.gridDistanceToWorldDistance));

            yield return new WaitUntil(() => { return abilityHealModule.moduleBehaviorCoroutine == null; });

            EndAbility(targetingOrigin);
        }

        protected override void OnValidate()
        {
            abilityHealModule.InitializeAbilityModule(this);
            base.OnValidate();
            if (targetFilter == AbilityTarget.EmptyNode || targetFilter == AbilityTarget.AnyNode)
            {
                targetFilter = targetMem;
            }
            else
            {
                targetMem = targetFilter;
            }
        }
    }
}