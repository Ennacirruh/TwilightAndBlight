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
        Reflexes,
        Charisma,
        Cunning,
        Discipline    
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
