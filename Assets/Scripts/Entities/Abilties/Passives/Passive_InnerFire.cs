using System.Collections.Generic;
using TwilightAndBlight.Ability.Infliction;
using TwilightAndBlight.Map;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
namespace TwilightAndBlight.Ability.Passive
{
    public class Passive_InnerFire : Passive
    {
        [SerializeField] private int heat;
        [SerializeField] private float percentCurrentHealthConsumption;
        [SerializeField] private int damageRange;
        [SerializeField] private float damagePerHealthConsumed;
        [SerializeField] private float powerPerHealthConsumed;
        [SerializeField] private float novaStrikeBonusPerHeat;
        [SerializeField] private int markCharges;
        [SerializeField] private float markDamageBonus;
        private CombatEntity combatEntity;
        private float powerGainedMemory;
        private float novaStrikeBonus;
        private void Awake()
        {
            combatEntity = GetComponent<CombatEntity>();    
        }
        private void OnEnable()
        {
            GameEvents.OnTurnStart += InnerFire;
            GameEvents.OnTurnEnd += ReleaseBonus;
        }
        private void OnDisable()
        {
            GameEvents.OnTurnStart -= InnerFire;
            GameEvents.OnTurnEnd -= ReleaseBonus;

        }
        private void InnerFire(CombatEntity entity)
        {
            if(entity == combatEntity)
            {
                float consumption = combatEntity.GetHealthCost(percentCurrentHealthConsumption * heat, ResourceCostType.PercentCurrent);
                combatEntity.DrainEntityHealth(combatEntity, consumption, true);
                powerGainedMemory = consumption * powerPerHealthConsumed;
                combatEntity.Stats.GetStat(StatType.Power).ModifyBaseValue(powerGainedMemory);
                HashSet<MapNode> nodes = MapManager.Instance.GetNodesWithinRange(combatEntity.GetCurrentMapNode(), damageRange);
                foreach (MapNode node in nodes)
                {
                    if (node.IsOccupied() && node.GetCombatEntity() != combatEntity)
                    {
                        node.GetCombatEntity().DamageEntity(combatEntity, consumption * damagePerHealthConsumed, DamageType.Fire);
                    }
                }
            }
        }
        public void AddStackOfHeat()
        {
            heat ++;
        }
        public void NovaStrikeStart()
        {
            novaStrikeBonus = 1f + (novaStrikeBonusPerHeat * heat);
            heat = 0;
            GameEvents.OnEntityDamagedOverride += DamageOrverride;
        }
        public void NovaStrikeEnd()
        {
            GameEvents.OnEntityDamagedOverride -= DamageOrverride;
        }
        public bool DamageOrverride(CombatEntity source, CombatEntity target, ref float attack, ref HashSet<DamageType> damageTypes, ref float percentPenetration, ref float flatPenetration, ref float damageRangeWeight, ref float critChance, ref float critDamage, ref bool crit)
        {
            if(source == combatEntity)
            {
                attack *= novaStrikeBonus;
            }
            return true;
        }
        public void MarkEnemies(MapNode node, float idk)
        {
            CombatEntity combatEntity = node.GetCombatEntity();
            if(node != null)
            {
                Affliction_DamageMark mark = combatEntity.AddComponent<Affliction_DamageMark>();
                mark.InitializeDamageMark(markCharges, 0, markDamageBonus);
            }
        }
        private void ReleaseBonus(CombatEntity entity)
        {
            if (entity == combatEntity)
            {
                combatEntity.Stats.GetStat(StatType.Power).ModifyBaseValue(-powerGainedMemory);
            }
        }

        public override void PerformAdditionalBehavior(float value)
        {
            heat += Mathf.FloorToInt(value);
        }
    }
}