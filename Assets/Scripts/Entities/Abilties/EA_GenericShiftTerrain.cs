using System.Collections.Generic;
using System.Collections;
using TwilightAndBlight.Map;
using UnityEngine;
using TwilightAndBlight.Ability.Module;
namespace TwilightAndBlight.Ability
{
    public class EA_GenericShiftTerrain : EA_GenericAbilityShape
    {
        [SerializeField] protected AbilityTerrainModule abilityTerrainModule = new AbilityTerrainModule();

        protected override void Awake()
        {
            base.Awake();
            abilityTerrainModule.InitializeAbilityModule(this);
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
                abilityTerrainModule.HighlightNodes(nodes, GetTrueOrigin(targetingOrigin), (node) => { return IsValidTarget(node); }, abilityRangeModule.GetRange(), ref aquiredTargets);
            }
        }
        

        protected override IEnumerator AbilityBehavior(MapNode targetingOrigin)
        {
            float range = (abilityShape == DefaultAbilityShapes.Hexagon) ? abilitySizeModule.GetSize() : abilityRangeModule.GetRange();
            abilityTerrainModule.moduleBehaviorCoroutine = StartCoroutine(abilityTerrainModule.PerformTerrainBehavior(aquiredTargets, range, targetingOrigin));
            yield return new WaitUntil(() => { return abilityTerrainModule.moduleBehaviorCoroutine == null; } );
            EndAbility(targetingOrigin);
        }
        
       
        protected override Dictionary<string, string> GenerateStringConversionTable()
        {
            Dictionary<string, string> dict = base.GenerateStringConversionTable();
            abilityTerrainModule.GenerateStringConversionTable(ref dict);
            return dict;
        }
        protected override void OnValidate()
        {
            abilityTerrainModule.InitializeAbilityModule(this);
            base.OnValidate();
        }
    }
}
