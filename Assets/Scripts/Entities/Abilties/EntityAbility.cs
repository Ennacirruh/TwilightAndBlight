using System.Collections.Generic;
using UnityEngine;
namespace TwilightAndBlight.Ability
{
    [RequireComponent(typeof(EntityStats))]
    public abstract class EntityAbility : MonoBehaviour
    {
        [SerializeField] protected string abilityName;
        [TextArea][SerializeField] protected string abilityDescription;
        [TextArea][SerializeField] private string descriptionPreview;
        [SerializeField] private AbilityTarget target;
        protected Dictionary<string, string> StringConversionTable { get { return GetStringConversionTable(); }}
        private List<CombatEntity> targets = new List<CombatEntity>();
        protected List<(CombatTeam, int)> targetIndexReference = new List<(CombatTeam, int)>();
        protected delegate bool CombatEntityConditional(CombatEntity entity, int cursor);
        protected EntityStats entityStats;
        protected virtual void Awake()
        {
            entityStats = GetComponent<EntityStats>();  
        }
        
        protected abstract void SelectTargets(CombatTeam team, int cursor);
        protected abstract void GenerateTargetIndexReference(CombatTeam team, int cursor);
        protected abstract Dictionary<string, string> GetStringConversionTable();
        public void AquireTargets(CombatTeam combatTeam, int cursor)
        {
            SelectTargets(combatTeam, cursor);
            GenerateTargetList();
        }
        public List<(CombatTeam, int)> GetTargetIndexReference(CombatTeam team, int cursor)
        {
            targetIndexReference.Clear();
            GenerateTargetIndexReference(team, cursor);
            return targetIndexReference;
        }
        private void GenerateTargetList()
        {
            foreach ((CombatTeam, int) target in targetIndexReference)
            {
                CombatEntity entity = target.Item1.GetCombatEntity(target.Item2);
                AddCombatEntity(entity);
            }
        }
        protected void AddCombatEntity(CombatEntity entity)
        {
            if (entity != null)
            {
                targets.Add(entity);
            }
        }
        protected void AddSingleTarget(CombatTeam combatTeam, int cursor)
        {
            targetIndexReference.Add((combatTeam, cursor));
        }
        protected void AddAdjacentTargets(CombatTeam combatTeam, int cursor, int range = 1)
        {
            range = Mathf.Max(0, range);
            targetIndexReference.Add((combatTeam, cursor));
            for (int i = 0; i < range; i++)
            {
                targetIndexReference.Add((combatTeam, cursor + i));
                targetIndexReference.Add((combatTeam, cursor - i));
            }
        }
        protected void InsertRandomTargets(CombatTeam team, int rolls = 1, bool allowRepeats = true)
        {
            InsertRandomTargets(new HashSet<CombatTeam>() { team }, rolls, allowRepeats);
        }
        protected void InsertRandomTargets(HashSet<CombatTeam> teams, int rolls = 1, bool allowRepeats = true)
        {
            List<CombatEntity> entities = new List<CombatEntity>();
            foreach (CombatTeam team in teams)
            {
                for (int i = 0; i < CombatTeam.maxTeamSize; i++)
                {
                    CombatEntity entity = team.GetCombatEntity(i);
                    if (entity != null)
                    {
                        entities.Add(entity);
                    }
                }
            }
            rolls = Mathf.Max(1, rolls);
            if (allowRepeats)
            {
                for (int i = 0; i < rolls; i++)
                {
                    int index = Random.Range(0, entities.Count);
                    AddCombatEntity(entities[index]);
                }
            }
            else
            {
                rolls = Mathf.Min(rolls, entities.Count);
                for (int i = 0; i < rolls; i++)
                {
                    int index = Random.Range(0, entities.Count);
                    AddCombatEntity(entities[index]);
                    entities.RemoveAt(index);
                }
            }
        }

        protected void AddAllTargets(CombatTeam combatTeam)
        {
            for (int i = 0; i < CombatTeam.maxTeamSize; i++)
            {
                targetIndexReference.Add((combatTeam,i));
            }
        }

        protected void InsertTargetsByConditional(CombatTeam team, int cursor, CombatEntityConditional condition)
        {
            for (int i = 0; i < CombatTeam.maxTeamSize; i++)
            {
                CombatEntity entity = team.GetCombatEntity(i);
                if (entity != null)
                {
                    bool result = condition.Invoke(entity, cursor);
                    if (result)
                    {
                        AddCombatEntity(entity);
                    }
                }
            }
        }
        public string GetDescription()
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
       
        private void OnValidate()
        {
            if(entityStats == null)
            {
                entityStats = GetComponent<EntityStats>();
            }
            descriptionPreview = GetDescription();
        }
        
    }
}
