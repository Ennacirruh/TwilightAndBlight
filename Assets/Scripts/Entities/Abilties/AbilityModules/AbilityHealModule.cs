using System;
using System.Collections;
using System.Collections.Generic;
using TwilightAndBlight.Events;
using TwilightAndBlight.Map;
using UnityEngine;
namespace TwilightAndBlight.Ability.Module
{
    [Serializable]
    public class AbilityHealModule : AbilityModule
    {
        [SerializeField] protected float baseHeal;
        [SerializeField] protected List<VariableStatScaler> healScalers = new List<VariableStatScaler>();
        [SerializeField] protected int baseHealTicks;
        [SerializeField] protected List<VariableStatScaler> healTickScalers = new List<VariableStatScaler>();
        [SerializeField] protected float timeBetweenTicks;

        public IEnumerator PerformHealBehavior(IEnumerable<MapNode> targets, MapNode origin, float range = 0)
        {
            for (int i = 0; i < GetHealTicks(); i++)
            {
                foreach (MapNode node in targets)
                {   
                    CombatEntity target = node.GetCombatEntity();
                    if (target != null)
                    {
                        ApplyCameraShake(origin.transform.position, node.transform.position);
                        float heal = GetHeal();
                        if (respectLineOfSight)
                        {
                            heal *= GetCoverMultiplier(origin, node, range);
                        }
                        prePerTargetBehaviorExpansion?.Invoke(node, heal);
                        float ammountHealed = target.ReplenishEntityHealth(owner.OwningCombatEntity, heal);
                        postPerTargetBehaviorExpansion?.Invoke(node, ammountHealed);
                    }
                }
                yield return new WaitForSeconds(timeBetweenTicks);

            }
            moduleBehaviorCoroutine = null;
        }
        public void HighlightNodes(IEnumerable<MapNode> nodes, MapNode origin, MapNodeConditional condition, float range, ref HashSet<MapNode> validSet)
        {
            foreach (MapNode node in nodes)
            {

                bool genericHighlight = true;
                if (RespectLineOfSight)
                {
                    if (LineOfSightObstructed(origin, node, AbilityModuleCastHeightOffset, range, LineOfSightForgiveness)) //GetTrueOrigin(targetingOrigin)
                    {
                        genericHighlight = false;
                        MapManager.Instance.HighlightNodes(node, IndicatorType.AltGeneric);
                    }
                }
                if (genericHighlight)
                {
                    if (condition.Invoke(node))
                    {
                        if (!CanTargetSelf && owner.OwningCombatEntity.GetCurrentMapNode() == node)
                        {
                            MapManager.Instance.HighlightNodes(node, IndicatorType.Generic);
                        }
                        else
                        {
                            if (node.GetCombatEntity().GetCombatTeam() == owner.OwningCombatEntity.GetCombatTeam())
                            {
                                MapManager.Instance.HighlightNodes(node, IndicatorType.Valid);
                            }
                            else
                            {
                                MapManager.Instance.HighlightNodes(node, IndicatorType.Warning);
                            }
                            validSet.Add(node);
                        }
                    }
                    else
                    {
                        MapManager.Instance.HighlightNodes(node, IndicatorType.Generic);
                    }
                }
            }
        }
        public override void GenerateStringConversionTable(ref Dictionary<string, string> dictionary)
        {
            AddElementToLookupTable(ref dictionary, "baseheal", baseHeal.ToString());
            AddElementToLookupTable(ref dictionary, "healscalers", GetStringFromScalerList(healScalers));
            AddElementToLookupTable(ref dictionary, "heal", GetHeal().ToString());
            AddElementToLookupTable(ref dictionary, "basehealticks", baseHealTicks.ToString());
            AddElementToLookupTable(ref dictionary, "healtickscalers", GetStringFromScalerList(healTickScalers));
            AddElementToLookupTable(ref dictionary, "healticks", GetHealTicks().ToString());
        }

        public virtual float GetHeal()
        {
            return GetScaledStat(baseHeal, healScalers, 1);
        }
        public virtual int GetHealTicks()
        {
            return GetScaledStat(baseHealTicks, healTickScalers, 1);
        }
    }
}
