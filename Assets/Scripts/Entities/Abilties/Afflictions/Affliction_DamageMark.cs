using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TwilightAndBlight.Events;
namespace TwilightAndBlight.Ability.Infliction
{
    public class Affliction_DamageMark : Affliction
    {
        private Sprite icon;
        private float flatDamageBonus;
        private float percentDamageBonus;
        private int charges;
        private EntityInfoDisplay infoDisplay;
        private StatusEffectPreview statusEffectPreview;
        private void OnEnable()
        {
            GameEvents.OnEntityDamagedOverride += DamageOrverride;
        }
        private void OnDisable()
        {
            GameEvents.OnEntityDamagedOverride -= DamageOrverride;
        }
        public void InitializeDamageMark(Sprite icon, int charges, float flatDamageBonus, float percentDamageBonus)
        {
            this.icon = icon;   
            this.charges = charges;
            this.flatDamageBonus = flatDamageBonus;
            this.percentDamageBonus = percentDamageBonus;
            infoDisplay = combatEntity.gameObject.GetComponent<EntityInfoDisplay>();
            statusEffectPreview = infoDisplay.AddStatusEffectVisual(icon, charges);
        }

        public bool DamageOrverride(CombatEntity source, CombatEntity target, ref float attack, ref HashSet<DamageType> damageTypes, ref float percentPenetration, ref float flatPenetration, ref float damageRangeWeight, ref float critChance, ref float critDamage, ref bool crit)
        {
            if (target == combatEntity)
            {
                attack *= percentDamageBonus;
                attack += flatDamageBonus;
                charges--;
                statusEffectPreview.DisplayNewStacks(charges);

                if (charges <= 0)
                {
                    infoDisplay.RemoveStatusEffectVisual(statusEffectPreview);
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