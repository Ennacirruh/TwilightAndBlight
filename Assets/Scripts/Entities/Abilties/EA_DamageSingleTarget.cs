using System.Collections;
using System.Collections.Generic;
using TwilightAndBlight.Map;
using UnityEngine;

namespace TwilightAndBlight.Ability
{
    public class EA_DamageSingleTarget : EA_DamageBase
    {

        protected override IEnumerator AbilityBehavior(MapNode targetingOrigin)
        {
            if (targetingOrigin.IsOccupied())
            {
                CombatEntity target = targetingOrigin.GetCombatEntity().GetComponent<CombatEntity>();

                for (int i = 0; i < ticksOfDamage; i++)
                {
                    target.DamageEntity(combatEntity, GetDamageValue(), damageType, percentPen + entityStats.PercentArmorPen, flatPen + entityStats.FlatArmorPen);
                    if(i != ticksOfDamage - 1) yield return new WaitForSeconds(delayBetweenTicks);
                }
               
            }
            yield return null;
            EndAbility(targetingOrigin);
        }

        public override void HighlightAbility(MapNode targetingOrigin)
        {
            if (IsValidTarget(targetingOrigin))
            {
                MapManager.Instance.HighlightNodesAsOverlay(targetingOrigin, IndicatorType.Valid);
            }
            else
            {
                MapManager.Instance.HighlightNodesAsOverlay(targetingOrigin, IndicatorType.Invalid);
            }
        }

        protected override Dictionary<string, string> GetStringConversionTable()
        {
            return base.GetStringConversionTable();
        }

    }
}