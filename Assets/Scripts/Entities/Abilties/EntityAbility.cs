using System.Collections.Generic;
using System.Collections;
using TwilightAndBlight.Map;
using TwilightAndBlight.Events;
using UnityEngine;
using System.Linq;

using UnityEngine.Events;

namespace TwilightAndBlight.Ability
{
    [RequireComponent(typeof(EntityStats))]
    public abstract class EntityAbility : MonoBehaviour, IDescriptable
    {
        [SerializeField] protected Sprite abilityIcon;
        [SerializeField] protected string abilityName;
        [TextArea][SerializeField] protected string abilityDescription;
        [TextArea][SerializeField] private string descriptionPreview;
        [SerializeField] protected float endOfAbilityDelay = 1.5f;
        [SerializeField] protected AbilityTarget targetFilter;
        [SerializeField] protected float abilityCost;
        [SerializeField] protected CombatResource costResource;
        [SerializeField] protected ResourceCostType resourceCostType;
        [SerializeField] protected int abilityCooldown;
        [SerializeField] protected UnityEvent preBehaviorExpansion;
        [SerializeField] protected UnityEvent postBehaviorExpansion;
        
        private Dictionary<string, string> stringConversionTable;
        protected Dictionary<string, string> StringConversionTable { get { if (stringConversionTable == null) { stringConversionTable = GenerateStringConversionTable(); } return stringConversionTable; }}
        protected List<MapNode> targetVisualReference = new List<MapNode>();
        protected delegate bool CombatEntityConditional(CombatEntity caster, MapNode targetOrigin);
        protected EntityStats entityStats;
        protected CombatEntity combatEntity;
        public EntityStats OwningEntityStats { get { return entityStats; } }
        public CombatEntity OwningCombatEntity{ get { return combatEntity; } }
        protected int castTickets;
        protected int cooldownTimer;
        private bool subscribed;
        protected virtual void Awake()
        {
            entityStats = GetComponent<EntityStats>();  
            combatEntity = GetComponent<CombatEntity>();
        }

        protected virtual void OnEnable()
        {
        }
        protected virtual void OnDisable()
        {
            

        }
        protected abstract IEnumerator AbilityBehavior(MapNode targetingOrigin);
        public abstract void HighlightAbility(MapNode targetingOrigin);
        protected abstract Dictionary<string, string> GenerateStringConversionTable();
        public void PerformAbility(MapNode targetingOrigin)
        {
            MapManager.Instance.ResetHighlight();
            if(castTickets == 0)
            {
                PayAbilityCost();
                castTickets++;
            }
            castTickets--;
            preBehaviorExpansion?.Invoke();
            StartCoroutine(AbilityBehavior(targetingOrigin));
            
        }
        public void OnEntityKilled(CombatEntityInteractionCallback callback)
        {
            if (callback.target == combatEntity)
            {
                EndAbility(null);
            }
        }
        public virtual void OnTurnStart()
        {
            cooldownTimer--;
            if (!subscribed)
            {
                subscribed = true;
                GameEvents.OnEntityKilled += OnEntityKilled;
            }
        }

        public virtual void OnCombatStart()
        {
            cooldownTimer = 0;
        }
        protected void EndAbility(MapNode targetingOrigin)
        {
            GameEvents.OnEntityKilled -= OnEntityKilled;
            subscribed = false;
            postBehaviorExpansion?.Invoke();
            StartCoroutine(EndOfAbilityDelay(targetingOrigin));

            
        }
        private IEnumerator EndOfAbilityDelay(MapNode targetingOrigin)
        {
            if (combatEntity.Health > 0)
            {
                yield return new WaitForSeconds(endOfAbilityDelay);
            }

            if (combatEntity.IsPerformingFreeAction())
            {
                combatEntity.FreeActionComplete();
            }
            else
            {
                GameEvents.OnAbilityPerformed?.Invoke(new CombatEntityMapNodeInteractionCallback(combatEntity, targetingOrigin));
                if (castTickets == 0)
                {
                    cooldownTimer = abilityCooldown + 1;
                }
            }
        }
        public virtual bool IsValidTarget(MapNode targetNode)
        {
            return NodeMatchesAbilityTarget(targetNode);
        }
        public virtual bool IsValidTarget(MapNode targetNode, AbilityTarget targetingParamater)
        {
            return NodeMatchesAbilityTarget(targetNode, targetingParamater);
        }
        protected virtual void PayAbilityCost()
        {
            switch (costResource)
            {
                case CombatResource.Health:
                    combatEntity.DrainEntityHealth(combatEntity, combatEntity.GetHealthCost(abilityCost, resourceCostType), false);
                    break;
                case CombatResource.Stamina:
                    combatEntity.DrainEntityStamina(combatEntity, combatEntity.GetStaminaCost(abilityCost, resourceCostType));
                    break;
                case CombatResource.Mana:
                    combatEntity.DrainEntityMana(combatEntity, combatEntity.GetManaCost(abilityCost, resourceCostType));
                    break;
                case CombatResource.Corruption:
                    break;
                case CombatResource.Astra:
                    break;
            }
        }
        //public abstract bool HasValidTargetInRange();
        public virtual bool CanAffordAbility()
        {
            if(cooldownTimer > 0) return false;
            switch (costResource)
            {
                case CombatResource.Health:
                    return combatEntity.Health > combatEntity.GetHealthCost(abilityCost, resourceCostType);
                case CombatResource.Stamina:
                    return combatEntity.Stamina > combatEntity.GetStaminaCost(abilityCost, resourceCostType);
                case CombatResource.Mana:
                    return combatEntity.Mana > combatEntity.GetManaCost(abilityCost, resourceCostType);
                case CombatResource.Corruption:
                    return combatEntity.Corruption > combatEntity.GetCorruptionCost(abilityCost, resourceCostType);
                case CombatResource.Astra:
                    return combatEntity.Astra > combatEntity.GetAstraCost(abilityCost, resourceCostType);
                default:
                    return true;
            }
        }

        protected bool NodeMatchesAbilityTarget(MapNode targetNode, AbilityTarget targetingParameter)
        {
            if (targetNode == null) return false;
            switch (targetingParameter)
            {
                case AbilityTarget.EnemyNode:
                    if (!targetNode.IsOccupied()) return false;
                    return targetNode.GetCombatEntity().GetComponent<CombatEntity>().GetCombatTeam() != combatEntity.GetCombatTeam();
                case AbilityTarget.AllyNode:
                    if (!targetNode.IsOccupied()) return false;
                    return targetNode.GetCombatEntity().GetComponent<CombatEntity>().GetCombatTeam() == combatEntity.GetCombatTeam();
                case AbilityTarget.OccupiedNode:
                    return targetNode.IsOccupied();
                case AbilityTarget.EmptyNode:
                    return !targetNode.IsOccupied();
                case AbilityTarget.AnyNode:
                    return true;
            }
            return false;
        }
        protected bool NodeMatchesAbilityTarget(MapNode targetNode)
        {
           return NodeMatchesAbilityTarget(targetNode, targetFilter);
        }
        public virtual bool IsValidAbilityCast(MapNode targetNode)
        {
            return IsValidTarget(targetNode);
        }
        public string GetAbilityDescription()
        {
            string returnString = "";
            if (abilityDescription != null && abilityDescription.Length > 0)
            {
                string[] words = abilityDescription.Split(' ');
                foreach (string word in words)
                {
                    if (word.Length > 0)
                    {
                        if (word.Contains(':'))
                        {
                            int startIndex = word.IndexOf(':');
                            int endIndex = word.Length - 1;
                            string start = "";
                            string end = "";
                            if (startIndex > 0)
                            {
                                start = word.Substring(0, startIndex);
                            }
                            if (word.Contains(';'))
                            {
                                endIndex = word.IndexOf(';') - 1;
                                end = word.Substring(endIndex + 2);

                            }
                            string candidate = word.Substring(startIndex + 1, endIndex  - startIndex);
                            if (StringConversionTable.ContainsKey(candidate))
                            {
                                returnString += start + StringConversionTable[candidate] + end + " ";
                            }
                            else 
                            {
                                returnString += start + end + " ";
                            }
                        }
                        else
                        {
                            returnString += word + " ";
                        }
                    }

                }
                returnString.TrimEnd(' ');
            }
            if(abilityCost > 0)
            {
                string a = (resourceCostType == ResourceCostType.Flat) ? "" : "%";
                string b = "";
                switch (resourceCostType)
                {
                    case ResourceCostType.Flat:
                        b = "";
                        break;
                    case ResourceCostType.PercentMax:
                        b = " max";
                        break;
                    case ResourceCostType.PercentCurrent:
                        b = " current";

                        break;
                    case ResourceCostType.PercentMissing:
                        b = " missing";
                        break;
                }
                string c = costResource.ToString();
                returnString += $" Costs {abilityCost}{a}{b} {c}.";
            }
            if (abilityCooldown > 0)
            {
                returnString += $" Has a cooldown of {abilityCooldown} turns.";
            }
            return returnString;
        }
        public Sprite GetAbilityIcon()
        {
            return abilityIcon;
        }
       
    
        public static string GetStringFromScalerList( List<VariableStatScaler> list, string valueLable = "")
        {
            string returnString = "";

            if (list == null || list.Count == 0)
            {
                return returnString;
            }
            if (list.Count == 1)
            {
                returnString += $"{list[0].valuePerScalingStat * 100f}% {valueLable} {list[0].scalingStat}";
                return returnString;
            }

            for(int i = 0; i < list.Count - 1; i++)
            {
                VariableStatScaler scaler = list[i];
                returnString += $"{scaler.valuePerScalingStat * 100f}% {valueLable} {scaler.scalingStat}, ";
            }
            returnString += $"and {list[list.Count - 1].valuePerScalingStat * 100f}% {valueLable} {list[list.Count - 1].scalingStat}";
            return returnString;
        }
        protected string GetStringFromDamageList(List<DamageType> list)
        {
            string returnString = "";

            if (list == null || list.Count == 0)
            {
                return returnString;
            }
            if (list.Count == 1)
            {
                returnString += $"{list[0].ToString()}";
            }

            for (int i = 0; i < list.Count - 1; i++)
            {
                DamageType type = list[i];
                returnString += $"{type.ToString()}, ";
            }
            returnString += $"and {list[list.Count - 1].ToString()}";
            return returnString;
        }
        
        public float GetScaledStat(float baseValue, List<VariableStatScaler> scalers, float min = float.MinValue, float max = float.MaxValue)
        {
            float value = baseValue;
            foreach (VariableStatScaler scaler in scalers)
            {
                float addition = scaler.valuePerScalingStat * entityStats.GetStat(scaler.scalingStat).Value;
                value += addition;
            }
            value = Mathf.Clamp(value, min, max);
            return value;
        }
        public int GetScaledStat(int baseValue, List<VariableStatScaler> scalers, int min = int.MinValue, int max = int.MaxValue)
        {
            float value = baseValue;
            foreach (VariableStatScaler scaler in scalers)
            {
                float addition = scaler.valuePerScalingStat * entityStats.GetStat(scaler.scalingStat).Value;
                value += addition;
            }
            value = Mathf.Clamp(value, min, max);
            return Mathf.FloorToInt(value);
        }
        protected virtual void OnValidate()
        {
            if (entityStats == null)
            {
                entityStats = GetComponent<EntityStats>();
            }
            stringConversionTable = GenerateStringConversionTable();
            descriptionPreview = GetAbilityDescription();
        }
        protected virtual bool ValidTargetInRangeExists(MapNode origin, float range, MapNodeConditional conditional)
        {
            foreach (MapNode node in MapManager.Instance.GetNodesWithinRange(origin,Mathf.FloorToInt(range)))
            {
                if (conditional.Invoke(node))
                {
                    return true;
                }
            }
            return false;
        }
        public string GetName()
        {
            string returnString = abilityName;
            if (abilityCooldown > 0 && cooldownTimer > 0)
            {
                string plural = (cooldownTimer == 1) ? "" : "s";
                returnString += $": {cooldownTimer} turn{plural} remaining";   
            }
            return returnString;
        }

        public string GetDescription()
        {
            return GetAbilityDescription();
        }
    
    }
}
