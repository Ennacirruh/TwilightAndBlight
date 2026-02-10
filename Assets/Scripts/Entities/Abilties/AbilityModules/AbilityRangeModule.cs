using System;
using System.Collections.Generic;
using UnityEngine;
namespace TwilightAndBlight.Ability.Module
{
    [Serializable]
    public class AbilityRangeModule : AbilityModuleData
    {
        [SerializeField] protected float baseRange;
        [SerializeField] protected List<VariableStatScaler> rangeScalers = new List<VariableStatScaler>();
        public override void GenerateStringConversionTable(ref Dictionary<string, string> dictionary)
        {
            AddElementToLookupTable(ref dictionary, "range", GetRange().ToString());
            AddElementToLookupTable(ref dictionary, "baserange", baseRange.ToString());
            AddElementToLookupTable(ref dictionary, "rangescalers", GetStringFromScalerList(rangeScalers));
        }
        public float GetRange()
        {
            return GetScaledStat(baseRange, rangeScalers, 0);

        }
    }
}