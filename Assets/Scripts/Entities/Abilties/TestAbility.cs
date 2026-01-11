using System.Collections.Generic;
using TwilightAndBlight;
using TwilightAndBlight.Ability;
using UnityEngine;

public class TestAbility : EntityAbility
{
    [SerializeField] private DamageType damageType;
    [SerializeField] private float baseDamage;
    [SerializeField] private float damagePerStat;
    [SerializeField] private StatType scalingStat;

    protected override Dictionary<string, string> GetStringConversionTable()
    {
        return new Dictionary<string, string>() {
        {"damagetype", damageType.ToString()} ,
        {"basedamage", baseDamage.ToString()} ,
        {"damageperstat", damagePerStat.ToString()} ,
        {"scalingstat", scalingStat.ToString()} ,
        {"damage", (baseDamage + (damagePerStat * entityStats.GetStat(scalingStat).Value)).ToString() }
        };
    }

    protected override void SelectTargets(CombatTeam team, int cursor)
    {
        AddAdjacentTargets(team, cursor);
    }

    protected override void GenerateTargetIndexReference(CombatTeam team, int cursor)
    {
        AddAdjacentTargets(team, cursor);
    }
}
