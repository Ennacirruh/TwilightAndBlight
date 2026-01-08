using UnityEngine;
using System.Collections.Generic;
namespace TwilightAndBlight {
    public class EntityStats : MonoBehaviour
    {
        public enum StatType
        {
            Strength,
            Fortitude,
            Agility,
            Constition,
            Intelligence,
            Wisdom,
            Dexterity,
            Charisma,
            Cunning,
            Discipline
        }
        #region Variables
        [SerializeField] private Stat strength;
        [SerializeField] private Stat fortitude;
        [SerializeField] private Stat agility;
        [SerializeField] private Stat constitution;
        [SerializeField] private Stat intelligence;
        [SerializeField] private Stat wisdom;
        [SerializeField] private Stat dexterity;
        [SerializeField] private Stat charisma;
        [SerializeField] private Stat cunning;
        [SerializeField] private Stat discipline;
        private Dictionary<DamageType, float> resistances;
        private Dictionary<StatType, Stat> statLookupTable;
        #endregion
        #region Paramaters
        public float Strength { get { return strength.Value; } }
        public float Fortitude { get { return fortitude.Value; } }
        public float Agility { get { return agility.Value; } }
        public float Constitution { get { return constitution.Value; } }
        public float Intelligence { get { return intelligence.Value; } }
        public float Wisdom { get { return wisdom.Value; } }
        public float Dexterity { get { return dexterity.Value; } }
        public float Charisma { get { return charisma.Value; } }
        public float Cunning { get { return cunning.Value; } }
        public float Discipline { get { return discipline.Value; } }
        #endregion
        private void Awake()
        {
            statLookupTable.Add(StatType.Strength, strength);
            statLookupTable.Add(StatType.Fortitude, fortitude);
            statLookupTable.Add(StatType.Agility, agility);
            statLookupTable.Add(StatType.Constition, constitution);
            statLookupTable.Add(StatType.Intelligence, intelligence);
            statLookupTable.Add(StatType.Wisdom, wisdom);
            statLookupTable.Add(StatType.Dexterity, dexterity);
            statLookupTable.Add(StatType.Charisma, charisma);
            statLookupTable.Add(StatType.Cunning, cunning);
            statLookupTable.Add(StatType.Discipline, discipline);
        }
        public Stat GetStat(StatType statType)
        {
            return statLookupTable[statType];
        }
        public float GetResistance(DamageType damageType)
        {
            if (resistances.ContainsKey(damageType))
            {
                return resistances[damageType];
            }
            return 0;
        }
    }
}
