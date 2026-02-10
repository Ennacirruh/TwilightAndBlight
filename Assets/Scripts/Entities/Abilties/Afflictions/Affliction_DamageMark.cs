using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace TwilightAndBlight.Ability.Infliction
{
    public class Affliction_DamageMark : Affliction
    {
        float flatDamageBonus;
        float percentDamageBonus;
        int charges;
        private void OnEnable()
        {
            GameEvents.OnEntityDamagedOverride += DamageOrverride;
        }
        private void OnDisable()
        {
            GameEvents.OnEntityDamagedOverride -= DamageOrverride;
        }
        public void InitializeDamageMark(int charges, float flatDamageBonus, float percentDamageBonus)
        {
            this.charges = charges;
            this.flatDamageBonus = flatDamageBonus;
            this.percentDamageBonus = percentDamageBonus;
        }

        public bool DamageOrverride(CombatEntity source, CombatEntity target, ref float attack, ref HashSet<DamageType> damageTypes, ref float percentPenetration, ref float flatPenetration, ref float damageRangeWeight, ref float critChance, ref float critDamage, ref bool crit)
        {
            if (target == combatEntity)
            {
                attack *= percentDamageBonus;
                attack += flatDamageBonus;
                charges--;
                if (charges <= 0)
                {
                    StartCoroutine(SelfDestruct());
                }
            }
            return true;

        }
        private IEnumerator SelfDestruct()
        {
            yield return new WaitForEndOfFrame();
            Destroy(this);
        }
    }
}