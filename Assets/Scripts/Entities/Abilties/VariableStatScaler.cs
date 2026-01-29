using System;
using UnityEngine;
namespace TwilightAndBlight.Ability
{
    [Serializable]
    public struct VariableStatScaler
    {
        [SerializeField] public StatType scalingStat;
        [SerializeField] public float valuePerScalingStat;

    }
}
