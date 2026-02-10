using UnityEngine;
namespace TwilightAndBlight
{
    public static class GameEvents
    {
        public static DamageEntityInteraction OnEntiyDamaged;
        public static DamageEntityOverride OnEntityDamagedOverride;
        public static ReplenishEntityInteraction OnEntityHealthReplenished;
        public static ReplenishEntityOverride OnEntityHealthReplenishedOverride;
        public static DrainEntityResourceInteraction OnEntityHealthDrained;
        public static DrainEntityResourceOverride OnEntityHealthDrainedOverride;
        public static CombatResourceChangeAction OnHealthChange;
        public static ReplenishEntityInteraction OnEntityStaminaReplenished;
        public static ReplenishEntityOverride OnEntityStaminaReplenishedOverride;
        public static DrainEntityResourceInteraction OnEntityStaminaDrained;
        public static DrainEntityResourceOverride OnEntityStaminaDrainedOverride;
        public static CombatResourceChangeAction OnStaminaChange;
        public static ReplenishEntityInteraction OnEntityManaReplenished;
        public static ReplenishEntityOverride OnEntityManaReplenishedOverride;
        public static DrainEntityResourceInteraction OnEntityManaDrained;
        public static DrainEntityResourceOverride OnEntityManaDrainedOverride;
        public static CombatResourceChangeAction OnManaChange;
        public static CombatEntityAction OnTurnStart;
        public static CombatEntityAction OnTurnEnd;
        public static CombatEntityInteraction OnEntityKilled;
        public static KillEntityOverride OnEntityKilledOverride;
        public static GenericAction OnCombatStart;
        public static GenericAction OnCombatEnd;
        public static CombatTeamInteraction OnTeamJoin;
        public static CombatTeamInteraction OnTeamLeave;
        public static CombatEntityAction OnAbilitySelected;
        public static CombatEntityAction OnTargetsSelected;
        public static CombatEntityMapNodeInteraction OnAbilityPerformed;
        public static CombatEntityMapNodeInteraction OnNodeEntered;
        public static CombatEntityMapNodeInteraction OnNodeExited;
    }
}
