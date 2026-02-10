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
                foreach (MapNode node in nodes)
                {

                    bool genericHighlight = true;
                    if (abilityHealModule.RespectLineOfSight)
                    {
                        if (abilityHealModule.LineOfSightObstructed(GetTrueOrigin(targetingOrigin), node, abilityHealModule.AbilityModuleCastHeightOffset, abilityHealModule.LineOfSightForgiveness))
                        {
                            genericHighlight = false;
                            MapManager.Instance.HighlightNodes(node, IndicatorType.AltGeneric);
                        }
                    }
                    if (genericHighlight)
                    {
                        if (IsValidTarget(node))
                        {
                            if (!abilityHealModule.CanTargetSelf && combatEntity.GetCurrentMapNode() == node)
                            {
                                MapManager.Instance.HighlightNodes(node, IndicatorType.Generic);
                            }
                            else
                            {
                                if (node.GetCombatEntity().GetCombatTeam() != combatEntity.GetCombatTeam())
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
            
        
        protected override Dictionary<string, string> GenerateStringConversionTable()
        {
            Dictionary<string, string> table = base.GenerateStringConversionTable();
            abilityHealModule.GenerateStringConversionTable(ref table);
            return table;
        }
        protected override IEnumerator AbilityBehavior(MapNode targetingOrigin)
        {
            abilityHealModule.moduleBehaviorCoroutine = StartCoroutine(abilityHealModule.PerformHealBehavior(aquiredTargets, abilityRangeModule.GetRange() * MapManager.gridDistanceToWorldDistance));

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