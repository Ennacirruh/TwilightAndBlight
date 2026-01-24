using System.Collections;
using System.Collections.Generic;
using TwilightAndBlight;
using TwilightAndBlight.Ability;
using TwilightAndBlight.Map;
using UnityEngine;

public class EA_DamageSingleTarget : EntityAbility
{
    [SerializeField] private DamageType damageType;
    [SerializeField] private float baseDamage;
    [SerializeField] private float damagePerStat;
    [SerializeField] private StatType scalingStat;
    [SerializeField] private float flatPen;
    [SerializeField] private float percentPen;
    protected override IEnumerator AbilityBehavior(MapNode targetingOrigin)
    {
        if (targetingOrigin.IsOccupied())
        {
            targetingOrigin.GetCombatEntity().GetComponent<CombatEntity>().DamageEntity(combatEntity, GetDamageValue(), damageType, percentPen + entityStats.PercentArmorPen, flatPen + entityStats.FlatArmorPen);
        }
        yield return null;
        EndAbility(targetingOrigin);
    }

    public override void HighlightAbility(MapNode targetingOrigin)
    {
        if (IsValidTarget(targetingOrigin))
        {
            MapManager.Instance.HighlightNodesAsOverlay(targetingOrigin, IndicatorType.Valid);
        }
        else
        {
            MapManager.Instance.HighlightNodesAsOverlay(targetingOrigin, IndicatorType.Invalid);
        }
    }

    protected override Dictionary<string, string> GetStringConversionTable()
    {
        return new Dictionary<string, string>() {
        {"damagetype", damageType.ToString()} ,
        {"basedamage", baseDamage.ToString()} ,
        {"damageperstat", damagePerStat.ToString()} ,
        {"scalingstat", scalingStat.ToString()} ,
        {"damage", (baseDamage + (damagePerStat * entityStats.GetStat(scalingStat).Value)).ToString() },
        {"flatpen", (flatPen + entityStats.FlatArmorPen).ToString() },
        {"percentpen", (percentPen + entityStats.PercentArmorPen).ToString() }
        };
    }
    protected float GetDamageValue()
    {
        return baseDamage + damagePerStat * entityStats.GetStat(scalingStat).Value;
    }

}
