using UnityEngine;
using System.Collections.Generic;
namespace TwilightAndBlight {
    public class EntityStats : MonoBehaviour
    {
        
        #region Variables
        [SerializeField] private Stat power = new Stat();
        [SerializeField] private Stat fortitude = new Stat();
        [SerializeField] private Stat agility = new Stat();
        [SerializeField] private Stat constitution = new Stat();
        [SerializeField] private Stat intelligence = new Stat();
        [SerializeField] private Stat wisdom = new Stat();
        [SerializeField] private Stat dexterity = new Stat();
        [SerializeField] private Stat charisma = new Stat();
        [SerializeField] private Stat cunning = new Stat();
        [SerializeField] private Stat discipline = new Stat();
        [SerializeField] private Stat reflexes = new Stat();
        [SerializeField] private Stat flatArmorPen = new Stat();
        [SerializeField] private Stat percemtArmorPen = new Stat();
        private Dictionary<DamageType, float> resistances = new Dictionary<DamageType, float>();
        private Dictionary<StatType, Stat> statLookupTable = new Dictionary<StatType, Stat>();
        #endregion
        #region Paramaters
        public float Strength { get { return power.Value; } }
        public float Fortitude { get { return fortitude.Value; } }
        public float Agility { get { return agility.Value; } }
        public float Constitution { get { return constitution.Value; } }
        public float Intelligence { get { return intelligence.Value; } }
        public float Wisdom { get { return wisdom.Value; } }
        public float Dexterity { get { return dexterity.Value; } }
        public float Charisma { get { return charisma.Value; } }
        public float Cunning { get { return cunning.Value; } }
        public float Discipline { get { return discipline.Value; } }
        public float Reflexes { get { return reflexes.Value; } }
        public float FlatArmorPen { get { return flatArmorPen.Value; } }
        public float PercentArmorPen { get { return percemtArmorPen.Value; } }
        #endregion
        private void Awake()
        {
            InitializeLookupTable();    
        }
        public Stat GetStat(StatType statType)
        {
            if(statLookupTable.Count == 0)
            {
                InitializeLookupTable();
            }
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
        private void InitializeLookupTable()
        {
            statLookupTable.Clear();
            statLookupTable.Add(StatType.Power, power);
            statLookupTable.Add(StatType.Fortitude, fortitude);
            statLookupTable.Add(StatType.Agility, agility);
            statLookupTable.Add(StatType.Constition, constitution);
            statLookupTable.Add(StatType.Intelligence, intelligence);
            statLookupTable.Add(StatType.Wisdom, wisdom);
            statLookupTable.Add(StatType.Dexterity, dexterity);
            statLookupTable.Add(StatType.Charisma, charisma);
            statLookupTable.Add(StatType.Cunning, cunning);
            statLookupTable.Add(StatType.Discipline, discipline);
            statLookupTable.Add(StatType.Reflexes, reflexes);

        }
        private void OnValidate()
        {
            InitializeLookupTable();
        }
    }
}
