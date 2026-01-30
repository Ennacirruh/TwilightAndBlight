using System.Collections.Generic;
using System.Collections;
using TwilightAndBlight.Map;
using UnityEngine;
using System.Linq;
using UnityEngine.Android;
using UnityEngine.Rendering;

namespace TwilightAndBlight.Ability
{
    [RequireComponent(typeof(EntityStats))]
    public abstract class EntityAbility : MonoBehaviour
    {
        [SerializeField] protected Sprite abilityIcon;
        [SerializeField] protected string abilityName;
        [TextArea][SerializeField] protected string abilityDescription;
        [TextArea][SerializeField] private string descriptionPreview;
        [SerializeField] protected AbilityTarget targetFilter;
        private Dictionary<string, string> stringConversionTable;
        protected Dictionary<string, string> StringConversionTable { get { if (stringConversionTable == null) { stringConversionTable = GenerateStringConversionTable(); } return stringConversionTable; }}
        protected List<MapNode> targetVisualReference = new List<MapNode>();
        protected delegate bool CombatEntityConditional(CombatEntity caster, MapNode targetOrigin);
        protected EntityStats entityStats;
        protected CombatEntity combatEntity;
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
   
            StartCoroutine(AbilityBehavior(targetingOrigin));
            
        }
        protected void EndAbility(MapNode targetingOrigin)
        {
            GameEvents.OnAbilityPerformed?.Invoke(combatEntity, targetingOrigin);
        }
        public virtual bool IsValidTarget(MapNode targetNode)
        {
            return NodeMatchesAbilityTarget(targetNode);
        }
        protected bool NodeMatchesAbilityTarget(MapNode targetNode)
        {
            if (targetNode == null) return false;
            switch (targetFilter)
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
            return returnString;
        }
        public Sprite GetAbilityIcon()
        {
            return abilityIcon;
        }
        public string GetAbilityName()
        {
            return abilityName;
        }
    
        protected string GetStringFromScalerList( List<VariableStatScaler> list, string valueLable = "")
        {
            string returnString = "";

            if (list == null || list.Count == 0)
            {
                return returnString;
            }
            if (list.Count == 1)
            {
                returnString += $"{list[0].valuePerScalingStat} {valueLable} per {list[0].scalingStat}";
            }

            for(int i = 0; i < list.Count - 1; i++)
            {
                VariableStatScaler scaler = list[i];
                returnString += $"{scaler.valuePerScalingStat} {valueLable} per {scaler.scalingStat}, ";
            }
            returnString += $"and {list[list.Count - 1].valuePerScalingStat} {valueLable} per {list[list.Count - 1].scalingStat}";
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
        protected virtual void OnValidate()
        {
            if(entityStats == null)
            {
                entityStats = GetComponent<EntityStats>();
            }
            descriptionPreview = GetAbilityDescription();
        }
        protected float GetScaledStat(float baseValue, List<VariableStatScaler> scalers)
        {
            float value = baseValue;
            foreach (VariableStatScaler scaler in scalers)
            {
                float addition = scaler.valuePerScalingStat * entityStats.GetStat(scaler.scalingStat).Value;
                value += addition;
            }
            return value;
        }
        protected int GetScaledStat(int baseValue, List<VariableStatScaler> scalers)
        {
            float value = baseValue;
            foreach (VariableStatScaler scaler in scalers)
            {
                float addition = scaler.valuePerScalingStat * entityStats.GetStat(scaler.scalingStat).Value;
                value += addition;
            }
            return Mathf.FloorToInt(value);
        }
    }
}
