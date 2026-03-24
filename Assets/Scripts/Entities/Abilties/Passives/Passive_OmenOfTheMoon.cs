
using TwilightAndBlight.Map;
using System.Collections.Generic;
using UnityEngine;
namespace TwilightAndBlight.Ability.Passive
{
    public class Passive_OmenOfTheMoon : Passive
    {
        [SerializeField] private Sprite powerIcon;
        [SerializeField] private Sprite compassionIcon;
        [SerializeField] private Sprite nightsEmbraceIcon;
        [SerializeField] private float powerGainOnHeal;
        [SerializeField] private float compassionGainOnDamage;
        [SerializeField] private float embaraceOfTheNightPwerBuff;

        private CombatEntity combatEntity;
        private EntityInfoDisplay infoDisplay;
        private StatusEffectPreview compassionPreview;
        private StatusEffectPreview powerPreview;
        private Dictionary<CombatEntity, StatusEffectPreview> nightsEmbraceEffects = new Dictionary<CombatEntity, StatusEffectPreview>();
        private Dictionary<CombatEntity, float> nightsEmbraceStacks = new Dictionary<CombatEntity, float>();

        private float powerGained;
        private float compassionGained;
        private void Awake()
        {
            combatEntity = GetComponent<CombatEntity>();
            infoDisplay = GetComponent<EntityInfoDisplay>();
        }

        public void GainPowerOnHeal()
        {
            combatEntity.Stats.GetStat(StatType.Power).ModifyDecimalModifier(powerGainOnHeal);
            powerGained ++;
            if(powerPreview == null)
            {
                powerPreview = infoDisplay.AddStatusEffectVisual(powerIcon, 0);
            }
            powerPreview.StackCounter.text = Mathf.FloorToInt(powerGained).ToString();
        }
        public void GainCompassionOnDamage()
        {
            combatEntity.Stats.GetStat(StatType.Compassion).ModifyDecimalModifier(compassionGainOnDamage);
            compassionGained ++;
            if (compassionPreview == null)
            {
                compassionPreview = infoDisplay.AddStatusEffectVisual(compassionIcon, 0);
            }
            compassionPreview.StackCounter.text = Mathf.FloorToInt(compassionGained).ToString();
        }
        public void EmpowerAlly(MapNode node, float idk)
        {
            if (node != null)
            {
                CombatEntity combatEntity = node.GetCombatEntity();

                combatEntity.Stats.GetStat(StatType.Power).ModifyDecimalModifier(embaraceOfTheNightPwerBuff);
                if (!nightsEmbraceStacks.ContainsKey(combatEntity))
                {
                    nightsEmbraceStacks.Add(combatEntity, 0);

                }
                if (!nightsEmbraceEffects.ContainsKey(combatEntity))
                {
                    nightsEmbraceEffects.Add(combatEntity, combatEntity.GetComponent<EntityInfoDisplay>().AddStatusEffectVisual(nightsEmbraceIcon, 0));
                }
                nightsEmbraceStacks[combatEntity] += 1;
                nightsEmbraceEffects[combatEntity].StackCounter.text = Mathf.FloorToInt(nightsEmbraceStacks[combatEntity]).ToString();

            }
        }
    }
}