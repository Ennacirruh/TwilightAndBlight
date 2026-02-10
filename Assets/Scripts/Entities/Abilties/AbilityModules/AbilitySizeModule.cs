using System;
using System.Collections.Generic;
using UnityEngine;
namespace TwilightAndBlight.Ability.Module
{
    [Serializable]
    public class AbilitySizeModule : AbilityModuleData
    {
        [SerializeField] protected float baseSize;
        [SerializeField] protected List<VariableStatScaler> sizeScalers = new List<VariableStatScaler>();
        public override void GenerateStringConversionTable(ref Dictionary<string, string> dictionary)
        {
            AddElementToLookupTable(ref dictionary, "size", GetSize().ToString());
            AddElementToLookupTable(ref dictionary, "basesize", baseSize.ToString());
            AddElementToLookupTable(ref dictionary, "sizescalers", GetStringFromScalerList(sizeScalers));

        }

        public float GetSize()
        {
            return GetScaledStat(baseSize, sizeScalers, 0);
        }
    }
}
