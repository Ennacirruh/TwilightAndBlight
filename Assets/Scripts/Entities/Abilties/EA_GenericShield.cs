using System.Collections.Generic;
using TwilightAndBlight.Map;
using UnityEngine;
using System.Collections;
using TwilightAndBlight.Ability.Module;
namespace TwilightAndBlight.Ability
{
    public class EA_GenericShield : EA_GenericAbilityShape
    {
        [SerializeField] AbilityShieldModule abilityShieldModule = new AbilityShieldModule();

        private AbilityTarget targetMem;
        protected override void Awake()
        {
            base.Awake();
            abilityShieldModule.InitializeAbilityModule(this);
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
                    if (abilityShieldModule.RespectLineOfSight)
                    {
                        if (abilityShieldModule.LineOfSightObstructed(GetTrueOrigin(targetingOrigin), node, abilityShieldModule.AbilityModuleCastHeightOffset, abilityShieldModule.LineOfSightForgiveness))
                        {
                            genericHighlight = false;
                            MapManager.Instance.HighlightNodes(node, IndicatorType.AltGeneric);
                        }
                    }
                    if (genericHighlight)
                    {
                        if (IsValidTarget(node))
                        {
                            if (!abilityShieldModule.CanTargetSelf && combatEntity.GetCurrentMapNode() == node)
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
            abilityShieldModule.GenerateStringConversionTable(ref table);


            return table;
        }
        protected override IEnumerator AbilityBehavior(MapNode targetingOrigin)
        {
            abilityShieldModule.moduleBehaviorCoroutine = StartCoroutine(abilityShieldModule.PerformShieldAbility(aquiredTargets, abilityRangeModule.GetRange() * MapManager.gridDistanceToWorldDistance));
            yield return new WaitUntil(() => { return abilityShieldModule.moduleBehaviorCoroutine == null; });
            EndAbility(targetingOrigin);
        }

        protected override void OnValidate()
        {
            abilityShieldModule.InitializeAbilityModule(this);
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