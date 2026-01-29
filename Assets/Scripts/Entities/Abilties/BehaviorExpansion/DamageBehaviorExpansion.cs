using UnityEngine;

namespace TwilightAndBlight.Ability {
    public abstract class DamageBehaviorExpansion : ScriptableObject
    {
        public abstract void PerformAdditionalBehavior(CombatEntity source, CombatEntity target, ref float damage);
   

    }
}
