using System.Collections.Generic;
using System.Collections;
using TwilightAndBlight.Map;
using UnityEngine;
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
        protected Dictionary<string, string> StringConversionTable { get { return GetStringConversionTable(); }}
        protected List<MapNode> targetVisualReference = new List<MapNode>();
        protected delegate bool CombatEntityConditional(CombatEntity caster, MapNode targetOrigin);
        protected EntityStats entityStats;
        protected CombatEntity combatEntity;
        protected virtual void Awake()
        {
            entityStats = GetComponent<EntityStats>();  
            combatEntity = GetComponent<CombatEntity>();
        }

        protected abstract IEnumerator AbilityBehavior(MapNode targetingOrigin);
        public abstract void HighlightAbility(MapNode targetingOrigin);
        protected abstract Dictionary<string, string> GetStringConversionTable();
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
            if(targetNode == null) return false;
            switch (targetFilter)
            {
                case AbilityTarget.EnemyNode:
                    if (!targetNode.IsOccupied()) return false;
                    return targetNode.GetCombatEntity().GetComponent<CombatEntity>().GetCombatTeam() != combatEntity.GetCombatTeam();
                case AbilityTarget.AllyNode:
                    if(!targetNode.IsOccupied()) return false;
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
        public string GetAbilityDescription()
        {
            string returnString = "";
            if (abilityDescription.Length > 0)
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
        protected virtual void OnValidate()
        {
            if(entityStats == null)
            {
                entityStats = GetComponent<EntityStats>();
            }
            descriptionPreview = GetAbilityDescription();
        }

    }
}
