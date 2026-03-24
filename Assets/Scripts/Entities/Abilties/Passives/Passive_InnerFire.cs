using System.Collections.Generic;
using TwilightAndBlight.Ability.Infliction;
using TwilightAndBlight.Map;
using TwilightAndBlight.Events;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using TwilightAndBlight.Map.Modifier;
namespace TwilightAndBlight.Ability.Passive
{
    public class Passive_InnerFire : Passive
    {
        [SerializeField] private Sprite heatIcon;
        [SerializeField] private Sprite powerIcon;
        [SerializeField] private Sprite markIcon;
        [SerializeField] private int heat;
        [SerializeField] private float percentCurrentHealthConsumption;
        [SerializeField] private int damageRange;
        [SerializeField] private float damagePerHealthConsumed;
        [SerializeField] private float powerPerHealthConsumed;
        [SerializeField] private float novaStrikeBonusPerHeat;
        [SerializeField] private int markCharges;
        [SerializeField] private float markDamageBonus;
        [SerializeField] private GameObject damageZoneVisualPrefab;
        [SerializeField] private float terrainDamagePerStack;
        [SerializeField] private float terrainDamageDecayPerTurn;
        private CombatEntity combatEntity;
        private float powerGainedMemory;
        private float novaStrikeBonus;
        private StatusEffectPreview heatPreview;
        private StatusEffectPreview powerPreview;
        private EntityInfoDisplay infoDisplay;
        private Dictionary<MapNode, TerrainModifier_DamageZone> damageZones = new Dictionary<MapNode, TerrainModifier_DamageZone>();
        private void Awake()
        {
            combatEntity = GetComponent<CombatEntity>();    
            infoDisplay = GetComponent<EntityInfoDisplay>();
        }
        private void OnEnable()
        {
            GameEvents.OnTurnStart += InnerFire;
            GameEvents.OnEntityDamaged += OnDamageTaken;
            GameEvents.OnShieldChange += OnDamageTaken;
        }
        private void OnDisable()
        {
            GameEvents.OnTurnStart -= InnerFire;
            GameEvents.OnEntityDamaged -= OnDamageTaken;
            GameEvents.OnShieldChange -= OnDamageTaken;


        }
        private void InnerFire(CombatEntityActionCallback callback)
        {
            if(callback.entity == combatEntity)
            {
                float consumption = combatEntity.GetHealthCost(percentCurrentHealthConsumption * heat, ResourceCostType.PercentCurrent);
                combatEntity.DrainEntityHealth(combatEntity, consumption, true);
                List<MapNode> nodesToRecycle = new List<MapNode>();
                foreach(MapNode node in damageZones.Keys)
                {
                    TerrainModifier_DamageZone damageZone = damageZones[node];
                    damageZone.TerrainDamage -= terrainDamageDecayPerTurn;
                    if (damageZone.TerrainDamage <= 0)
                    {
                        Destroy(damageZone);
                        nodesToRecycle.Add(node);
                    }
                }
                foreach(MapNode node in nodesToRecycle)
                {
                    damageZones.Remove(node);
                }
            }
        }
        private void OnDamageTaken(DamageEntityInteractionCallback callback)
        {
            if (callback.target == combatEntity)
            {
                HashSet<MapNode> nodes = MapManager.Instance.GetNodesWithinRange(combatEntity.GetCurrentMapNode(), damageRange);
                foreach (MapNode node in nodes)
                {
                    if (node.IsOccupied() && node.GetCombatEntity().GetCombatTeam() != combatEntity.GetCombatTeam())
                    {
                        node.GetCombatEntity().DamageEntity(combatEntity, callback.preMitigationDamage * damagePerHealthConsumed, DamageType.Fire);
                    }
                }
            }
        }
        private void OnDamageTaken(ShieldResourceChangeCallback callback)
        {
            if (callback.entity == combatEntity && callback.shieldValueChange < 0)
            {
                HashSet<MapNode> nodes = MapManager.Instance.GetNodesWithinRange(combatEntity.GetCurrentMapNode(), damageRange);
                foreach (MapNode node in nodes)
                {
                    if (node.IsOccupied() && node.GetCombatEntity().GetCombatTeam() != combatEntity.GetCombatTeam())
                    {
                        node.GetCombatEntity().DamageEntity(combatEntity, Mathf.Abs(callback.shieldValueChange) * damagePerHealthConsumed, DamageType.Fire);
                    }
                }
            }
        }
        public void AddStackOfHeat()
        {
            heat ++;
            if(heatPreview == null)
            {
                heatPreview = infoDisplay.AddStatusEffectVisual(heatIcon, heat);
            }
            else
            {
                heatPreview.StackCounter.text = heat.ToString();
            }
        }
        public void NovaStrikeStart()
        {
            novaStrikeBonus = 1f + (novaStrikeBonusPerHeat * heat);
            
            GameEvents.OnEntityDamagedOverride += DamageOrverride;
        }
        public void NovaStrikeEnd()
        {
            GameEvents.OnEntityDamagedOverride -= DamageOrverride;
            heat = 0;
            infoDisplay.RemoveStatusEffectVisual(heatPreview);
            heatPreview = null;
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
            if(combatEntity != null)
            {
                Affliction_DamageMark mark = combatEntity.AddComponent<Affliction_DamageMark>();
                mark.InitializeDamageMark(markIcon ,markCharges, 0, markDamageBonus);
            }
        }
        public void ApplyTerrainModifier(MapNode node, float idk)
        {
            if (!damageZones.ContainsKey(node))
            {
                damageZones.Add(node, node.AddComponent<TerrainModifier_DamageZone>());
                damageZones[node].Initialize(damageZoneVisualPrefab);
            }
            //if (damageZones[node] == null)
            //{
            //    damageZones[node] = node.AddComponent<TerrainModifier_DamageZone>();
            //    damageZones[node].Initialize(damageZoneVisualPrefab);
            //}
            float damageIncrease = heat * terrainDamagePerStack;
            damageZones[node].TerrainDamage += damageIncrease;
        }
        private void OnDestroy()
        {
            foreach(TerrainModifier_DamageZone damageZone in damageZones.Values)
            {
                Destroy(damageZone);
            }
        }

    }
}