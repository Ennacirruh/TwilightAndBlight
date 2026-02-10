using UnityEngine;
namespace TwilightAndBlight
{
    
    public enum DamageType
    {
        Physical,
        Fire
    }
    public enum StatType
    {
        Power,
        Fortitude,
        Agility,
        Constition,
        Intelligence,
        Wisdom,
        Dexterity,
        Reflexes,
        Charisma,
        Cunning,
        Discipline,
        FlatArmorPen,
        PercentArmorPen,
        Spirit,
        Endurance,
        Vitality,
        Tenacity,
        Effervescence,
        Compassion,
        Mettle,
        Transendance
    }
 

    public enum AbilityTarget
    {
        EnemyNode,
        AllyNode,
        OccupiedNode,
        EmptyNode,
        AnyNode
    }
    public enum IndicatorType
    {
        Generic,
        AltGeneric,
        Valid,
        Invalid,
        Warnign
    }
    public enum DefaultAbilityShapes
    {
        Hexagon, 
        Line,
        Arc
    }
    public enum TerrainShiftType
    {
        Raise,
        Bridge,
        Level,
        Flatten,
        Lower
    }
    public enum CombatResource
    {
        Health,
        Stamina,
        Mana,
        Corruption,
        Astra
    }
    public enum ResourceCostType
    {
        Flat,
        PercentMax,
        PercentCurrent,
        PercentMissing
    }

}
