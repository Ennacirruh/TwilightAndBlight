using System.Collections.Generic;
using UnityEngine;
namespace TwilightAndBlight.Ability.Module
{
    public abstract class AbilityModuleData 
    {
        protected EntityAbility owner;
        protected string lookupTablePrefix;

        public virtual void InitializeAbilityModule(EntityAbility ability, string lookupTablePrefix = "")
        {
            owner = ability;
            this.lookupTablePrefix = lookupTablePrefix;
        }
        public abstract void GenerateStringConversionTable(ref Dictionary<string, string> dictionary);

        protected string GetStringFromScalerList(List<VariableStatScaler> list, string valueLable = "")
        {
            return EntityAbility.GetStringFromScalerList(list, valueLable);
        }

        protected float GetScaledStat(float baseValue, List<VariableStatScaler> scalers, float min = float.MinValue, float max = float.MaxValue)
        {
            return owner.GetScaledStat(baseValue, scalers, min, max);
        }
        protected int GetScaledStat(int baseValue, List<VariableStatScaler> scalers, int min = int.MinValue, int max = int.MaxValue)
        {
            return owner.GetScaledStat(baseValue, scalers, min, max);
        }

        protected void AddElementToLookupTable(ref Dictionary<string, string> lookupTable, string name, string data)
        {
            lookupTable.Add(lookupTablePrefix + name, data);
        }
    }
}
