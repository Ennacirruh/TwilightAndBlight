using UnityEngine;
using System.Collections.Generic;
using TwilightAndBlight.Ability;
using Unity.VisualScripting;
using TwilightAndBlight.Map;
namespace TwilightAndBlight
{
    [RequireComponent(typeof(EntityStats))]
    public abstract class CombatEntity : MonoBehaviour
    {

        [SerializeField] protected int entityLevel;
        protected EntityStats entityStats;
        protected float turnProgress;
        protected CombatTeam combatTeam;
        protected HashSet<CombatEntity> targets = new HashSet<CombatEntity>();
        protected HashSet<EntityAbility> abilities = new HashSet<EntityAbility>();
        protected EntityAbility selectedAbility;
        [SerializeField] protected float health;
        [SerializeField] protected float entityHeight;
        private MapNode currentNode;
        public float MaxHealth { get { return 1000f + (100f * Stats.Constitution); } }
        public float Health { get { return health; } }
        private bool actionInProgress;
        public int Level {  get { return entityLevel; } } 
        public float TurnProgress {  get { return turnProgress; } } 
        public float EntityHeight {  get { return entityHeight; } }
        public EntityStats Stats { get { return entityStats; } }
        protected virtual void Awake()
        {
            entityStats = GetComponent<EntityStats>();
            health = MaxHealth;
        }
        protected virtual void OnEnable()
        {
            GameEvents.OnCombatStart += OnCombatStart;
        }
        protected virtual void OnDisable()
        {
            GameEvents.OnCombatStart -= OnCombatStart;
        }
        public abstract void SelectAction();
        public abstract void AcquireTargets();
        public abstract void Action();
        public void PerformAction()
        {
            actionInProgress = true;
            Action();
        }
        protected void ActionComplete()
        {
            actionInProgress=false;
        }
        public bool ActionInProgress()
        {
            return actionInProgress;
        }
        public virtual void OnCombatStart()
        {
            turnProgress = 0;
            health = MaxHealth;
            abilities.AddRange(GetComponents<EntityAbility>());
        }
        public void AssignCombatTeam(CombatTeam team)
        {
            combatTeam = team;
        }
        public CombatTeam GetCombatTeam()
        {
            return combatTeam;
        }
        public void TickTurnProgress(int ticks)
        {
            turnProgress += entityStats.Agility * ticks;
        }
        public void ProgressTowardsTurn(float decimalTowardsTurn)
        {
            turnProgress += GameManager.Instance.TurnThreshold * decimalTowardsTurn;
        }
        public void ResetTurnProgress()
        {
            turnProgress = 0;
        }
        public void SetSelectedAbility(EntityAbility ability)
        {
            selectedAbility = ability;
        }
        public void DamageEntity(CombatEntity source, float attack, DamageType damageType, float percentPenetration = 0, float flatPenetration = 0)
        {
            DamageEntity(source, attack, new HashSet<DamageType>() {damageType}, percentPenetration, flatPenetration);
        }
        public void DamageEntity(CombatEntity source, float attack, HashSet<DamageType> damageTypes, float percentPenetration = 0, float flatPenetration = 0)
        {
            float damageRangeWeight = source != null ? source.Stats.Discipline : 0;
            float critChance = source != null ? source.Stats.Cunning : 0; 
            float critDamage = source != null ?  1.5f + (source.Stats.Intelligence / 100f) : 1.5f;
            bool crit = false;
            GameEvents.OnEntityDamagedOverride?.Invoke(source, this, ref attack, ref damageTypes, ref percentPenetration, ref flatPenetration, ref damageRangeWeight, ref critChance, ref critDamage, ref crit);
            
            float armor = Stats.Fortitude - (Stats.Fortitude * percentPenetration) - flatPenetration;
            armor = Mathf.Clamp(armor, -attack / 2f, Mathf.Infinity);
            float resistanceMultiplier = 1f;
            foreach (DamageType damageType in damageTypes)
            {
                resistanceMultiplier = resistanceMultiplier * (1f - Stats.GetResistance(damageType));
            }
            float attackWeightRoll = (-Mathf.Cos(Mathf.PI * Random.Range(0f,1f))) + 1f;
            attackWeightRoll = GameManager.Instance.DamageVarianceRange * (attackWeightRoll / 2f);
            attackWeightRoll += 1f - (GameManager.Instance.DamageVarianceRange / 2f);
            attackWeightRoll += (damageRangeWeight - (GameManager.Instance.DamageVarianceRange / 2f)) / 100f;
            attack = attack * attackWeightRoll;
            float damage = (attack / ((armor / attack) + 1f)) * resistanceMultiplier;

            critChance -= Stats.Reflex;
            if (critChance > 100f)
            {
                 damage *= (1f + ((critChance-100f) / 200f));
            }
            else if (critChance < 0f)
            {
                damage *= (1f + (critChance / 200f));
            }
            crit |= Random.Range(0f, 100f) < critChance;
            if (crit)
            {
                damage *= critDamage;
            }
            health -= damage;
            Debug.Log($"Damage: {damage}\nBaseDamage: {attack}\nEfective Armor: {armor}\nResistance Mult: {resistanceMultiplier}\nCrit: {crit}\nCrit Damage{critDamage}");
            GameEvents.OnEntiyDamaged?.Invoke(attack, damage, this, source);
            GameEvents.OnHealthChange?.Invoke(this, -damage);
            if(health <= 0f)
            {
                KillEntity(source);
            }

        }
        public void KillEntity(CombatEntity source)
        {
            bool kill = true;
            GameEvents.OnEntityKilledOverride?.Invoke(this, source, ref kill);
            if (kill)
            {
                GameEvents.OnEntityKilled?.Invoke(this, source);
                if (combatTeam != null)
                {
                    combatTeam.RemoveCombatant(source);
                }
                Destroy(gameObject);
            }
            else
            {
                if(health <= 0f)
                {
                    health = 1f;
                }
            }
        }
        public float GetTicksToTurn()
        {
            if(Stats.Agility == 0)
            {
                return Mathf.Infinity;
            }
            return (GameManager.Instance.TurnThreshold - TurnProgress) / Stats.Agility;
        }
        public void SetCurrentMapNode(MapNode node)
        {
            currentNode = node;
        }
        public MapNode GetCurrentMapNode()
        {
            return currentNode;
        }
        public HashSet<EntityAbility> GetEntityAbilities()
        {
            return abilities;
        }
    }
}
