using System.Collections;
using System.Collections.Generic;
using TwilightAndBlight.Map;
using UnityEngine;

namespace TwilightAndBlight.Ability
{
    public abstract class EA_DamageBase : EntityAbility
    {
        [SerializeField] protected DamageType damageType;
        [SerializeField] protected float baseDamage;
        [SerializeField] protected float damagePerStat;
        [SerializeField] protected StatType scalingStat;
        [SerializeField] protected float flatPen;
        [SerializeField] protected float percentPen;
        [SerializeField] protected int ticksOfDamage;
        [SerializeField] protected float delayBetweenTicks;

        

        protected override Dictionary<string, string> GetStringConversionTable()
        {
            return new Dictionary<string, string>() {
        {"damagetype", damageType.ToString()} ,
        {"basedamage", baseDamage.ToString()} ,
        {"damageperstat", damagePerStat.ToString()} ,
        {"scalingstat", scalingStat.ToString()} ,
        {"damage", (baseDamage + (damagePerStat * entityStats.GetStat(scalingStat).Value)).ToString() },
        {"flatpen", (flatPen + entityStats.FlatArmorPen).ToString() },
        {"percentpen", (percentPen + entityStats.PercentArmorPen).ToString() },
        {"ticksofdamage", (ticksOfDamage).ToString() },
        {"delaybetweenticks", (delayBetweenTicks).ToString() }
        };
        }
        protected float GetDamageValue()
        {
            return baseDamage + damagePerStat * entityStats.GetStat(scalingStat).Value;
        }
        protected override void OnValidate()
        {
            base.OnValidate();
            ticksOfDamage = Mathf.Max(ticksOfDamage, 1);
            delayBetweenTicks = Mathf.Max(delayBetweenTicks, 0f);
        }
    }
}