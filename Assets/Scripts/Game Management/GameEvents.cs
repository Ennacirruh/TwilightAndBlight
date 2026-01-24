using UnityEngine;
namespace TwilightAndBlight
{
    public static class GameEvents
    {
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
        public static CombatEntityAction OnAbilitySelected;
        public static CombatEntityAction OnTargetsSelected;
        public static CombatEntityMapNodeInteraction OnAbilityPerformed;
    }
}
