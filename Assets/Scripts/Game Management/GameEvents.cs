using UnityEngine;
namespace TwilightAndBlight
{
    public static class GameEvents
    {
        //private static GameEvents instance;
        //public GameEvents Instance { get { return GetInstance(); } }
        //private static GameEvents GetInstance()
        //{
        //    if(instance == null)
        //    {
        //        instance = new GameEvents();
        //    }
        //    return instance;
        //}

        public static DamageEntityInteraction OnEntiyDamaged;
        public static DamageEntityOverride OnEntityDamagedOverride;
        public static HealEntityInteraction OnEntityHealed;
        public static CombatEntityAction OnTurnStart;
        public static CombatEntityAction OnTurnEnd;
        public static CombatEntityInteraction OnEntityKilled;
        public static KillEntityOverride OnEntityKilledOverride;
        public static GenericAction OnCombatStart;
        public static GenericAction OnCombatEnd;
        public static CombatTeamInteraction OnTeamJoin;
        public static CombatTeamInteraction OnTeamLeave;
        public static CombatResourceChangeAction OnHealthChange;

    }
}
