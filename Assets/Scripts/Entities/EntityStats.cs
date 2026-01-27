using UnityEngine;
using System.Collections.Generic;
namespace TwilightAndBlight {
    public class EntityStats : MonoBehaviour
    {
        
        #region Variables
        [SerializeField] private Stat power = new Stat(); //damage / primary ability driver
        [SerializeField] private Stat fortitude = new Stat(); //armor
        [SerializeField] private Stat agility = new Stat(); //speed
        [SerializeField] private Stat constitution = new Stat(); //health
        [SerializeField] private Stat intelligence = new Stat(); //crit damage
        [SerializeField] private Stat wisdom = new Stat(); //
        [SerializeField] private Stat dexterity = new Stat(); //fall resistance and Item Power
        [SerializeField] private Stat charisma = new Stat();
        [SerializeField] private Stat cunning = new Stat(); // Crit Chance
        [SerializeField] private Stat discipline = new Stat(); // Damage Roll Skew
        [SerializeField] private Stat reflex = new Stat(); // Crit Resistance
        [SerializeField] private Stat flatArmorPen = new Stat(); 
        [SerializeField] private Stat percemtArmorPen = new Stat();
        [SerializeField] private Stat spirit = new Stat(); // mana
        [SerializeField] private Stat endurance = new Stat(); // stamina
        [SerializeField] private Stat vitality = new Stat(); // health regen
        [SerializeField] private Stat tenacity = new Stat(); // stamina regen
        [SerializeField] private Stat effervescence = new Stat(); // mana regen
        [SerializeField] private Stat compassion = new Stat(); // heal power
        [SerializeField] private Stat mettle = new Stat(); // shield power
        [SerializeField] private Stat transendance = new Stat(); // Essence or cosmic essence


        private Dictionary<DamageType, float> resistances = new Dictionary<DamageType, float>();
        private Dictionary<StatType, Stat> statLookupTable = new Dictionary<StatType, Stat>();
        #endregion
        #region Paramaters
        public float Power { get { return power.Value; } }
        public float Fortitude { get { return fortitude.Value; } }
        public float Agility { get { return agility.Value; } }
        public float Constitution { get { return constitution.Value; } }
        public float Intelligence { get { return intelligence.Value; } }
        public float Wisdom { get { return wisdom.Value; } }
        public float Dexterity { get { return dexterity.Value; } }
        public float Charisma { get { return charisma.Value; } }
        public float Cunning { get { return cunning.Value; } }
        public float Discipline { get { return discipline.Value; } }
        public float Reflex { get { return reflex.Value; } }
        public float FlatArmorPen { get { return flatArmorPen.Value; } }
        public float PercentArmorPen { get { return percemtArmorPen.Value; } }
        public float Spirit { get { return spirit.Value; } }
        public float Endurance { get { return endurance.Value; } }
        public float Vitality { get { return vitality.Value; } }
        public float Tenacity { get { return tenacity.Value; } }
        public float Effervescence { get { return effervescence.Value; } }
        public float Compassion { get { return compassion.Value; } }
        public float Mettle { get { return mettle.Value; } }
        public float Transendance { get { return transendance.Value; } }
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
            statLookupTable.Add(StatType.Reflex, reflex);
            statLookupTable.Add(StatType.PercentArmorPen, percemtArmorPen);
            statLookupTable.Add(StatType.FlatArmorPen, flatArmorPen);
            statLookupTable.Add(StatType.Spirit, spirit);
            statLookupTable.Add(StatType.Endurance, endurance);
            statLookupTable.Add(StatType.Vitality, vitality);
            statLookupTable.Add(StatType.Tenacity, tenacity);
            statLookupTable.Add(StatType.Effervescence, effervescence);
            statLookupTable.Add(StatType.Compassion, compassion);
            statLookupTable.Add(StatType.Mettle, mettle);
            statLookupTable.Add(StatType.Transendance, transendance);


        }
        private void OnValidate()
        {
            InitializeLookupTable();
        }
    }
}
