using System;
using System.Collections;
using System.Collections.Generic;
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

        public IEnumerator PerformHealBehavior(IEnumerable<MapNode> targets, float range = 0)
        {
            for (int i = 0; i < GetHealTicks(); i++)
            {
                foreach (MapNode node in targets)
                {   
                    CombatEntity target = node.GetCombatEntity();
                    if (target != null)
                    {
                        float heal = GetHeal();
                        if (respectLineOfSight)
                        {
                            heal *= GetCoverMultiplier(node, range);
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
