using System;
using System.Collections;
using System.Collections.Generic;
using TwilightAndBlight.Map;
using UnityEngine;
namespace TwilightAndBlight.Ability.Module
{
    [Serializable]
    public class AbilityShieldModule : AbilityModule
    {
        [SerializeField] protected float baseShield;
        [SerializeField] protected List<VariableStatScaler> shieldScalers = new List<VariableStatScaler>();
        [SerializeField] protected float baseShieldDuration;
        [SerializeField] protected List<VariableStatScaler> shieldDurationScalers = new List<VariableStatScaler>();
        [SerializeField] protected int baseShieldTicks;
        [SerializeField] protected List<VariableStatScaler> shieldTickScalers = new List<VariableStatScaler>();
        [SerializeField] protected float timeBetweenTicks;

        public IEnumerator PerformShieldAbility(IEnumerable<MapNode> targets,MapNode origin, float range = 0)
        {
            for (int i = 0; i < GetShieldTicks(); i++)
            {
                foreach (MapNode node in targets)
                {
                    CombatEntity target = node.GetCombatEntity();
                    if (target != null)
                    {
                        float shield = GetShield();
                        if (respectLineOfSight)
                        {
                            shield *= GetCoverMultiplier(origin, node, range);
                        }
                        ApplyCameraShake(origin.transform.position, node.transform.position);
                        prePerTargetBehaviorExpansion?.Invoke(node, shield);
                        float shieldAmmount = target.AddShield(shield, GetShieldDuration(), owner.OwningCombatEntity);
                        postPerTargetBehaviorExpansion?.Invoke(node, shield);
                    }
                }
                yield return new WaitForSeconds(timeBetweenTicks);
            }
            moduleBehaviorCoroutine = null;
        }
        public override void GenerateStringConversionTable(ref Dictionary<string, string> dictionary)
        {
            AddElementToLookupTable(ref dictionary, "baseshield", baseShield.ToString());
            AddElementToLookupTable(ref dictionary, "shieldscalers", GetStringFromScalerList(shieldScalers));
            AddElementToLookupTable(ref dictionary, "shield", GetShield().ToString());
            AddElementToLookupTable(ref dictionary, "baseshieldticks", baseShieldTicks.ToString());
            AddElementToLookupTable(ref dictionary, "shieldtickscalers", GetStringFromScalerList(shieldTickScalers));
            AddElementToLookupTable(ref dictionary, "shieldticks", GetShieldTicks().ToString());
            AddElementToLookupTable(ref dictionary, "shielddurationscalers", GetStringFromScalerList(shieldDurationScalers));
            AddElementToLookupTable(ref dictionary, "shieldduration", GetShieldDuration().ToString());
            AddElementToLookupTable(ref dictionary, "baseshieldduration", baseShieldDuration.ToString());

        }

        public virtual float GetShieldDuration()
        {
            return GetScaledStat(baseShieldDuration, shieldDurationScalers);
        }
        public virtual float GetShield()
        {
            return GetScaledStat(baseShield, shieldScalers, 1);
        }
        public virtual int GetShieldTicks()
        {
            return GetScaledStat(baseShieldTicks, shieldTickScalers, 1);
        }
    }
}
