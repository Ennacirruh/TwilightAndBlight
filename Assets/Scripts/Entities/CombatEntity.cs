using System.Collections;
using System.Collections.Generic;
using TwilightAndBlight.Ability;
using TwilightAndBlight.Ability.Passive;
using TwilightAndBlight.Map;
using Unity.VisualScripting;
using UnityEngine;
using TwilightAndBlight.Events;
namespace TwilightAndBlight
{
    [RequireComponent(typeof(EntityStats))]
    public abstract class CombatEntity : MonoBehaviour
    {

        [SerializeField] protected int entityLevel;
        [SerializeField] protected float health;
        [SerializeField] protected float mana;
        [SerializeField] protected float stamina;
        [SerializeField] protected float corruption;
        [SerializeField] protected float astra;
        [SerializeField] protected float totalShield;
        [SerializeField] protected float entityHeight;
        /// <summary>
        /// (float,int) --> (shield, duration)
        /// </summary>
        protected List<(float, int)> shields = new List<(float, int)> (); // shield, duration
        private MapNode currentNode;
        protected EntityStats entityStats;
        protected float turnProgress;
        protected CombatTeam combatTeam;
        protected HashSet<CombatEntity> targets = new HashSet<CombatEntity>();
        protected HashSet<EntityAbility> abilities = new HashSet<EntityAbility>();
        protected Passive passive;
        protected EntityAbility selectedAbility;
        protected HashSet<EntityAbility> freeActions = new HashSet<EntityAbility>();
        protected bool performingFreeAction;
        protected bool freeActionsPerformed;

        protected Coroutine deathCoroutine;

        public float MaxHealth { get { return 100f + (20f * Stats.Constitution); } }
        public float MaxMana { get { return 50f + (25f * Stats.Spirit); } }
        public float MaxStamina { get { return 20f + (10f * Stats.Endurance); } }
        public float Health { get { return health; } }
        public float Mana { get { return mana; } }
        public float Stamina { get { return stamina; } }
        public float Corruption { get { return corruption; } }
        public float Shield { get { return totalShield; } }
        public float Astra { get { return astra; } }
        private bool actionInProgress;
        public int Level {  get { return entityLevel; } } 
        public float TurnProgress {  get { return turnProgress; } } 
        public float EntityHeight {  get { return entityHeight; } }
        public EntityStats Stats { get { return entityStats; } }
        protected virtual void Awake()
        {
            entityStats = GetComponent<EntityStats>();
            health = MaxHealth;
            mana = MaxMana;
            stamina = MaxStamina;
        }
        protected virtual void OnEnable()
        {
            GameEvents.OnCombatStart += OnCombatStart;
        }
        protected virtual void OnDisable()
        {
            GameEvents.OnCombatStart -= OnCombatStart;
        }
        public virtual void OnTurnStart()
        {
            freeActionsPerformed = false;
            float healthRegen = Stats.Vitality / 10f; 
            float manaRegen = Stats.Effervescence / 15f;
            float staminaRegen = Stats.Tenacity / 3f;
            float corruptionRegen = corruption * 0.2f;
            float astraRegen = 1f / (101f - Stats.Transendance);
            ReplenishEntityHealth(this, healthRegen, false);
            ReplenishEntityMana(this, manaRegen, false, false);
            ReplenishEntityStamina(this, staminaRegen, false);
            //add corruption
            astra += astraRegen;
            List<int> expiredShields = new List<int>();
            for (int i = 0; i < shields.Count; i++)
            {
                int newDuration = shields[i].Item2 - 1;
                shields[i] = (shields[i].Item1, newDuration);
                if(newDuration <= 0)
                {
                    expiredShields.Add(i);
                    totalShield -= shields[i].Item1;
                }
            }
            expiredShields.Reverse();
            for (int i = 0; i < expiredShields.Count; i++)
            {
                shields.RemoveAt(expiredShields[i]);
            }
            foreach (EntityAbility ability in abilities)
            {
                ability.OnTurnStart();
            }
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
            passive = GetComponent<Passive>();
            foreach (EntityAbility ability in abilities)
            {
                ability.OnCombatStart();
            }
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
        #region Resource Management
        #region Calculations
        public float GetHealthCost(float ammount, ResourceCostType costType)
        {
            return GetResourceCost(ammount, MaxHealth, Health, costType);
        }
        public float GetStaminaCost(float ammount, ResourceCostType costType)
        {
            return GetResourceCost(ammount, MaxStamina, Stamina, costType);
        }
        public float GetManaCost(float ammount, ResourceCostType costType)
        {
            return GetResourceCost(ammount, MaxMana, Mana, costType);
        }
        public float GetCorruptionCost(float ammount, ResourceCostType costType)
        {
            return GetResourceCost(ammount, MaxHealth + MaxMana + MaxStamina, Corruption, costType);
        }
        public float GetAstraCost(float ammount, ResourceCostType costType)
        {
            return GetResourceCost(ammount, GameManager.Instance.MaxAstra, Astra, costType);
        }
        
        private float GetResourceCost(float value, float max, float current, ResourceCostType resourceCostType)
        {
            switch (resourceCostType)
            {
                case ResourceCostType.Flat:
                    return value;
                case ResourceCostType.PercentMax:
                    return (value / 100f) * max;
                case ResourceCostType.PercentCurrent:
                    return (value / 100f) * current;
                case ResourceCostType.PercentMissing:
                    return (value / 100f) * (max - current);
                default:
                    return 0;
            }
        }
        #endregion
        #region Replenish
        /// <summary>
        /// 
        /// </summary>
        /// <param name="shield"></param>
        /// <param name="duration"></param>
        /// <returns>Returns shield gained</returns>
        public float AddShield(float shield, float duration)
        {
            shields.Add((shield, Mathf.FloorToInt(duration))); // TODO: Expand to utilize events and random values
            totalShield += shield;
            return shield;
        }
       /// <summary>
       /// 
       /// </summary>
       /// <param name="source"></param>
       /// <param name="healthRecovery"></param>
       /// <param name="utilizeReflexBonus"></param>
       /// <returns>Returns health gained.</returns>
        public float ReplenishEntityHealth(CombatEntity source, float healthRecovery, bool utilizeReflexBonus = true)
        {
           return ReplenishEntityResource(ref health, MaxHealth, this, GameEvents.OnEntityHealthReplenished, GameEvents.OnEntityHealthReplenishedOverride, GameEvents.OnHealthChange, healthRecovery, utilizeReflexBonus);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="staminaRecovery"></param>
        /// <param name="utilizeReflexBonus"></param>
        /// <returns>Returns the stamina gained.</returns>
        public float ReplenishEntityStamina(CombatEntity source, float staminaRecovery, bool utilizeReflexBonus = true)
        {
            return ReplenishEntityResource(ref stamina, MaxStamina, this, GameEvents.OnEntityStaminaReplenished, GameEvents.OnEntityStaminaReplenishedOverride, GameEvents.OnStaminaChange, staminaRecovery, utilizeReflexBonus);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="manaRecovery"></param>
        /// <param name="canOvercharge"></param>
        /// <param name="utilizeReflexBonus"></param>
        /// <returns>Returns the mana gained.</returns>
        public float ReplenishEntityMana(CombatEntity source, float manaRecovery, bool canOvercharge = true, bool utilizeReflexBonus = true)
        {
            return ReplenishEntityResource(ref mana, MaxMana, this, GameEvents.OnEntityManaReplenished, GameEvents.OnEntityManaReplenishedOverride, GameEvents.OnManaChange, manaRecovery, utilizeReflexBonus);
            // corruption
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="targetResource"></param>
        /// <param name="resourceMax"></param>
        /// <param name="source"></param>
        /// <param name="replenishInteraction"></param>
        /// <param name="interactionOverride"></param>
        /// <param name="resourceChangeAction"></param>
        /// <param name="baseRecoveryValue"></param>
        /// <param name="utilizeReflexBonus"></param>
        /// <returns>Returns the ammount actually recovered value</returns>
        protected virtual float ReplenishEntityResource(ref float targetResource, float resourceMax, CombatEntity source, ReplenishEntityInteraction replenishInteraction, ReplenishEntityOverride interactionOverride, CombatResourceChangeAction resourceChangeAction, float baseRecoveryValue, bool utilizeReflexBonus)
        {
            bool crit;
            float recoveryValue = GetResourceRecoveryValue(interactionOverride, source, baseRecoveryValue, utilizeReflexBonus, out crit);
            if (recoveryValue > 0)
            {
                targetResource += recoveryValue;
                float overRecovery = 0;
                if (targetResource > resourceMax)
                {
                    overRecovery = targetResource - resourceMax;
                    targetResource = resourceMax;
                }
                replenishInteraction?.Invoke(new ReplenishEntityInteractionCallback(recoveryValue, overRecovery, this, source, crit));
                resourceChangeAction?.Invoke(new CombatResourceChangeActionCallback(this, recoveryValue - overRecovery));
                return recoveryValue - overRecovery;
            }
            return 0;

        }
        protected virtual float GetResourceRecoveryValue(ReplenishEntityOverride eventOverride, CombatEntity source, float baseValue, bool utilizeReflexBonus, out bool isCrit)
        {
            float recoveryRangeWeight = source != null ? source.Stats.Discipline : 0;
            float critChance = source != null ? source.Stats.Cunning : 0;
            float critPower = source != null ? 1.5f + (source.Stats.Intelligence / 100f) : 1.5f;
            bool crit = false;
            bool proceed = true;
            proceed &= eventOverride?.Invoke(source, this, ref baseValue, ref recoveryRangeWeight, ref critChance, ref critPower, ref crit) ?? true;
            if (proceed)
            {
                float resourceWeightRoll = (-Mathf.Cos(Mathf.PI * Random.Range(0f, 1f))) + 1f;
                resourceWeightRoll = GameManager.Instance.ResourceInteractionVarianceRange * (resourceWeightRoll / 2f);
                resourceWeightRoll += 1f - (GameManager.Instance.ResourceInteractionVarianceRange / 2f);
                resourceWeightRoll += (recoveryRangeWeight - (GameManager.Instance.ResourceInteractionVarianceRange / 2f)) / 100f;
                baseValue = baseValue * resourceWeightRoll;
                if (utilizeReflexBonus) critChance += Stats.Reflex;
                if (critChance > 100f)
                {
                    baseValue *= (1f + ((critChance - 100f) / 200f));
                }
                else if (critChance < 0f)
                {
                    baseValue *= (1f + (critChance / 200f));
                }
                crit |= Random.Range(0f, 100f) < critChance;
                if (crit)
                {
                    baseValue *= critPower;
                }
                isCrit = crit;
                return Mathf.Max(baseValue,0);
            }
            isCrit = crit;
            return -1;
        }
        #endregion
        #region Drain
        public float DamageShield(float damage)
        {
            while (shields.Count > 0 && damage > 0)
            {
                int minDurationIndex = 0;
                int minDuration = shields[0].Item2;
                for (int i = 1; i < shields.Count; i++)// get shield with the shortest remaining duration
                {
                    if (shields[i].Item2 < minDuration)
                    {
                        minDurationIndex = i;
                        minDuration = shields[i].Item2;
                    }
                }
                float initialShieldValue = shields[minDurationIndex].Item1;
                float newShieldValue = Mathf.Max(0, initialShieldValue - damage);
                float difference = initialShieldValue - newShieldValue;
                totalShield -= difference;
                if (newShieldValue > 0)
                {
                    shields[minDurationIndex] = (newShieldValue, shields[minDurationIndex].Item2);
                }
                else
                {
                    shields.RemoveAt(minDurationIndex);
                }

            }

            return damage;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="healthDrain"></param>
        /// <param name="canKill"></param>
        /// <returns>Ammount Actually Drained</returns>
        public virtual float DrainEntityHealth(CombatEntity source, float healthDrain, bool canKill = true)
        {
            float overkill = DrainEntityResource(ref health, GameEvents.OnEntityHealthDrained, GameEvents.OnEntityHealthDrainedOverride, GameEvents.OnHealthChange, healthDrain, source);
            if(health <= 0)
            {
                if (canKill)
                {
                    KillEntity(source);
                }
                else
                {
                    health = 1;
                }
            }
            return healthDrain - overkill;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="staminaDrain"></param>
        /// <returns>Ammount Actually Drained</returns>
        public virtual float DrainEntityStamina(CombatEntity source, float staminaDrain)
        {
            float overkill = DrainEntityResource(ref stamina, GameEvents.OnEntityStaminaDrained, GameEvents.OnEntityStaminaDrainedOverride, GameEvents.OnStaminaChange, staminaDrain, source);
            return stamina - overkill;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="manaDrain"></param>
        /// <returns>Ammount Actually Drained</returns>
        public virtual float DrainEntityMana(CombatEntity source, float manaDrain)
        {
            float overkill = DrainEntityResource(ref mana, GameEvents.OnEntityManaDrained, GameEvents.OnEntityManaDrainedOverride, GameEvents.OnManaChange, manaDrain, source);
            return manaDrain - overkill;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="targetResource"></param>
        /// <param name="drainInteracction"></param>
        /// <param name="drainOverride"></param>
        /// <param name="resourceChangeAction"></param>
        /// <param name="drainValue"></param>
        /// <returns>Returns the overkill value</returns>
        protected virtual float DrainEntityResource(ref float targetResource, DrainEntityResourceInteraction drainInteracction, DrainEntityResourceOverride drainOverride, CombatResourceChangeAction resourceChangeAction, float drainValue, CombatEntity source)
        {
            bool proceed = true;
            proceed &= drainOverride?.Invoke(source, this, ref drainValue) ?? true;
            if (proceed)
            {
                if (drainValue > 0)
                {
                    targetResource -= drainValue;
                    float overkill = 0;
                    if (targetResource < 0)
                    {
                        overkill = -targetResource;
                        targetResource = 0;
                    }
                    drainInteracction?.Invoke(new DrainEntityResourceInteractionCallback(drainValue, this, source));
                    resourceChangeAction?.Invoke(new CombatResourceChangeActionCallback(this, drainValue - overkill));
                    return overkill;
                }
            }
            return 0;
        }
        #endregion
        public virtual void DamageEntity(CombatEntity source, float attack, DamageType damageType, float percentPenetration = 0, float flatPenetration = 0, bool ignoreShield = false)
        {
            DamageEntity(source, attack, new HashSet<DamageType>() {damageType}, percentPenetration, flatPenetration, ignoreShield);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="attack"></param>
        /// <param name="damageTypes"></param>
        /// <param name="percentPenetration">1 = 100% penetration</param>
        /// <param name="flatPenetration"></param>
        /// <param name="ignoreShield"></param>
        /// <returns>Damage Actually Taken</returns>
        public virtual float DamageEntity(CombatEntity source, float attack, HashSet<DamageType> damageTypes, float percentPenetration = 0, float flatPenetration = 0, bool ignoreShield = false)
        {
            float damageRangeWeight = source != null ? source.Stats.Discipline : 0;
            float critChance = source != null ? source.Stats.Cunning : 0; 
            float critPower = source != null ?  1.5f + (source.Stats.Intelligence / 100f) : 1.5f;
            bool crit = false;
            bool proceed = true;
            proceed &= GameEvents.OnEntityDamagedOverride?.Invoke(source, this, ref attack, ref damageTypes, ref percentPenetration, ref flatPenetration, ref damageRangeWeight, ref critChance, ref critPower, ref crit) ?? true;
            float returnValue = 0;
            if (proceed)
            {
                float armor = Stats.Fortitude - (Stats.Fortitude * percentPenetration) - flatPenetration;
                armor = Mathf.Clamp(armor, -attack / 2f, Mathf.Infinity);
                float resistanceMultiplier = 1f;
                foreach (DamageType damageType in damageTypes)
                {
                    resistanceMultiplier = resistanceMultiplier * (1f - Stats.GetResistance(damageType));
                }
                float attackWeightRoll = (-Mathf.Cos(Mathf.PI * Random.Range(0f, 1f))) + 1f;
                attackWeightRoll = GameManager.Instance.ResourceInteractionVarianceRange * (attackWeightRoll / 2f);
                attackWeightRoll += 1f - (GameManager.Instance.ResourceInteractionVarianceRange / 2f);
                attackWeightRoll += (damageRangeWeight - (GameManager.Instance.ResourceInteractionVarianceRange / 2f)) / 100f;
                attack = attack * attackWeightRoll;
                float damage = (attack / ((armor / attack) + 1f)) * resistanceMultiplier;

                critChance -= Stats.Reflex;
                if (critChance > 100f)
                {
                    damage *= (1f + ((critChance - 100f) / 200f));
                }
                else if (critChance < 0f)
                {
                    damage *= (1f + (critChance / 200f));
                }
                crit |= Random.Range(0f, 100f) < critChance;
                if (crit)
                {
                    damage *= critPower;
                }
                if (damage > 0f)
                {
                    if (!ignoreShield)
                    {
                        damage = DamageShield(damage);
                    }
                    if (damage > 0f)
                    {
                        health -= damage;
                        returnValue = damage;
                        //Debug.Log($"Damage: {damage}\nBaseDamage: {attack}\nEfective Armor: {armor}\nResistance Mult: {resistanceMultiplier}\nCrit: {crit}\nCrit Damage{critPower}");
                        GameEvents.OnEntiyDamaged?.Invoke(new DamageEntityInteractionCallback(attack, damage, this, source, crit));
                        GameEvents.OnHealthChange?.Invoke(new CombatResourceChangeActionCallback(this, -damage));
                        if (health <= 0f)
                        {
                            KillEntity(source);
                        }
                    }
                }
            }
            return returnValue;
        }
        #endregion
        public virtual void KillEntity(CombatEntity source)
        {
            bool kill = true;
            GameEvents.OnEntityKilledOverride?.Invoke(this, source, ref kill);
            if (kill)
            {
                GameEvents.OnEntityKilled?.Invoke(new CombatEntityInteractionCallback(this, source));
                if (combatTeam != null)
                {
                    combatTeam.RemoveCombatant(source);
                }
                if (deathCoroutine == null)
                {
                    deathCoroutine = StartCoroutine(DestroyEntity());
                }
            }
            else
            {
                if(health <= 0f)
                {
                    health = 1f;
                }
            }
        }
        protected virtual IEnumerator DestroyEntity()
        {
            yield return new WaitForEndOfFrame();
            Destroy(gameObject);
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
        public Passive GetEntityPassive()
        {
            return passive;
        }
        public void RegisterFreeAction(EntityAbility ability)
        {
            freeActions.Add(ability);
        }
        public void UnregisterFreeAction(EntityAbility ability)
        {
            freeActions.Remove(ability);
        }
        public bool IsPerformingFreeAction()
        {
            return performingFreeAction;
        }
        public void FreeActionComplete()
        {
            performingFreeAction = false;
        }
    }
}
