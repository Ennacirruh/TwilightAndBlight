using UnityEngine;
namespace TwilightAndBlight.Events
{
    public static class GameEvents
    {
        public static DamageEntityInteraction OnEntityDamaged;
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
        public static ShieldEntityInteraction OnEntityShielded;
        public static ShieldEntityOverride OnEntityShieldedOverride;
        public static ShieldEntityAction OnShieldExpired;
        public static ShieldEntityInteraction OnShieldDestroyed;
        public static ShieldResourceChange OnShieldChange;
        public static CombatEntityAction OnTurnStart;
        public static CombatEntityAction OnTurnEnd;
        public static CombatEntityInteraction OnEntityKilled;
        public static KillEntityOverride OnEntityKilledOverride;
        public static GenericAction OnCombatStart;
        public static GenericAction OnCombatEnd;
        public static CombatTeamCombatEntityInteraction OnTeamJoin;
        public static CombatTeamCombatEntityInteraction OnTeamLeave;
        public static CombatEntityAction OnAbilitySelected;
        public static CombatEntityAction OnTargetsSelected;
        public static CombatEntityMapNodeInteraction OnAbilityPerformed;
        public static CombatEntityMapNodeInteraction OnNodeEntered;
        public static CombatEntityMapNodeInteraction OnNodeExited;
    }
}
