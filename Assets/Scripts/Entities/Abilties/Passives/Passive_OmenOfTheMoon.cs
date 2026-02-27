using JetBrains.Annotations;
using TwilightAndBlight.Map;
using Unity.VisualScripting;
using UnityEngine;
namespace TwilightAndBlight.Ability.Passive
{
    public class Passive_OmenOfTheMoon : Passive
    {
        [SerializeField] private float powerGainOnHeal;
        [SerializeField] private float compassionGainOnDamage;
        [SerializeField] private float embaraceOfTheNightPwerBuff;

        private CombatEntity combatEntity;

        private void Awake()
        {
            combatEntity = GetComponent<CombatEntity>();
        }
        public override void PerformAdditionalBehavior(float value)
        {
            
        }

        public void GainPowerOnHeal()
        {
            combatEntity.Stats.GetStat(StatType.Power).ModifyDecimalModifier(powerGainOnHeal);
        }
        public void GainCompassionOnDamage()
        {
            combatEntity.Stats.GetStat(StatType.Compassion).ModifyDecimalModifier(compassionGainOnDamage);
        }
        public void EmpowerAlly(MapNode node, float idk)
        {
            if (node != null)
            {
                CombatEntity combatEntity = node.GetCombatEntity();

                combatEntity.Stats.GetStat(StatType.Power).ModifyDecimalModifier(embaraceOfTheNightPwerBuff);
            }
        }
    }
}