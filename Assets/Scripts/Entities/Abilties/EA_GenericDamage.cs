using System.Collections;
using System.Collections.Generic;
using TwilightAndBlight.Ability.Module;
using TwilightAndBlight.Map;
using Unity.VisualScripting;
using UnityEngine;
namespace TwilightAndBlight.Ability
{
    public class EA_GenericDamage : EA_GenericAbilityShape
    {
        [SerializeField] AbilityDamageModule abilityDamageModule = new AbilityDamageModule();

        private AbilityTarget targetMem;// for on validate
        protected HashSet<DamageType> damageTypeSet = new HashSet<DamageType>();
        protected override void Awake()
        {
            base.Awake();
            abilityDamageModule.InitializeAbilityModule(this);
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
                abilityDamageModule.HighlightNodes(nodes, GetTrueOrigin(targetingOrigin), (node) => { return IsValidTarget(node); }, abilityRangeModule.GetRange(), ref aquiredTargets);
            }
        }
            
        protected override Dictionary<string, string> GenerateStringConversionTable()
        {
            Dictionary<string, string> dict = base.GenerateStringConversionTable();
            abilityDamageModule.GenerateStringConversionTable(ref dict);

            return dict;
        }
 
        protected override IEnumerator AbilityBehavior(MapNode targetingOrigin)
        {

            abilityDamageModule.moduleBehaviorCoroutine = StartCoroutine(abilityDamageModule.PerformDamageBehavior(aquiredTargets, abilityRangeModule.GetRange() * MapManager.gridDistanceToWorldDistance));
    
            yield return new WaitUntil(() => { return abilityDamageModule.moduleBehaviorCoroutine == null; });

            EndAbility(targetingOrigin);
        }

        protected override void OnValidate()
        {
            abilityDamageModule.InitializeAbilityModule(this);
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