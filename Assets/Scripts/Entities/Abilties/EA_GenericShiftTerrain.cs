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
                foreach (MapNode node in nodes)
                {
                    bool genericHighlight = true;
                    if (abilityTerrainModule.RespectLineOfSight)
                    {
                        if (abilityTerrainModule.LineOfSightObstructed(GetTrueOrigin(targetingOrigin), node, abilityTerrainModule.AbilityModuleCastHeightOffset, abilityTerrainModule.LineOfSightForgiveness))
                        {
                            genericHighlight = false;
                            MapManager.Instance.HighlightNodes(node, IndicatorType.AltGeneric);
                        }
                    }
                    if (genericHighlight)
                    {
                        if (IsValidTarget(node))
                        {
                            if (!abilityTerrainModule.CanTargetSelf && combatEntity.GetCurrentMapNode() == node)
                            {
                                MapManager.Instance.HighlightNodes(node, IndicatorType.Generic);
                            }
                            else
                            {
                                if (node.IsOccupied() && node.GetCombatEntity().GetCombatTeam() == combatEntity.GetCombatTeam())
                                {
                                    MapManager.Instance.HighlightNodes(node, IndicatorType.Warnign);
                                }
                                else
                                {
                                    MapManager.Instance.HighlightNodes(node, IndicatorType.Valid);
                                }
                                aquiredTargets.Add(node);
                            }
                        }
                        else
                        {
                            MapManager.Instance.HighlightNodes(node, IndicatorType.Generic);
                        }
                    }
                }
            }
        }

        protected override IEnumerator AbilityBehavior(MapNode targetingOrigin)
        {
            abilityTerrainModule.moduleBehaviorCoroutine = StartCoroutine(abilityTerrainModule.PerformTerrainBehavior(aquiredTargets, abilityRangeModule.GetRange(), targetingOrigin));
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
