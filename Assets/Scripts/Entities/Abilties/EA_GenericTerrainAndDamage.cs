using System.Collections;
using System.Collections.Generic;
using TwilightAndBlight.Ability.Module;
using TwilightAndBlight.Map;
using UnityEngine;
namespace TwilightAndBlight.Ability
{
    public class EA_GenericTerrainAndDamage : EA_GenericShiftTerrain
    {
        [SerializeField] protected AbilityDamageModule abilityDamageModule = new AbilityDamageModule();

        private int damageInProgress;
        protected override void Awake()
        {
            base.Awake();
            abilityDamageModule.InitializeAbilityModule(this);
        }
        protected override void OnEnable()
        {
            abilityTerrainModule.prePerTargetBehaviorExpansion.AddListener(DamageOnTerrainShift);

            base.OnEnable();
        }
        protected override void OnDisable()
        {
            abilityTerrainModule.prePerTargetBehaviorExpansion.RemoveListener(DamageOnTerrainShift);
            base.OnDisable();
        }
        protected override void OnValidate()
        {
            abilityDamageModule.InitializeAbilityModule(this);
            abilityDamageModule.RespectLineOfSight = false;
            base.OnValidate();
        }

        public virtual void DamageOnTerrainShift(MapNode node, float idc)
        {
            StartCoroutine(abilityDamageModule.PerformDamageBehavior(node, node, idc));
        }
        protected override Dictionary<string, string> GenerateStringConversionTable()
        {
            Dictionary<string, string> dict = base.GenerateStringConversionTable();
            abilityDamageModule.GenerateStringConversionTable(ref dict);
            return dict;
        }
    }
}
