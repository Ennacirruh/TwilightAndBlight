using UnityEngine;
namespace TwilightAndBlight
{
    
    public enum DamageType
    {
        Physical
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
        Reflex,
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
        Valid,
        Invalid,
        Warnign
    }

}
