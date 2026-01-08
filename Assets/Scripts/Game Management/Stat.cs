using System;
using UnityEngine;
[Serializable]
public class Stat 
{
    private float totalValue;
    private float baseValue;
    private float decimalModifier;
    private float flatModifier;
    private bool recalculationQueued;
    public float Value { get { return GetValue(); } }
    private float GetValue()
    {
        if (recalculationQueued)
        {
            recalculationQueued = false;
            CalculateTotalValue();
        }
        return totalValue;
    }
    public void ModifyDecimalModifier(float modification)
    {
        decimalModifier += modification;
        recalculationQueued = true;
    }
    public void ModifyFlatModifier(float modification)
    {
        flatModifier += modification;
        recalculationQueued = true;
    }

    public void ModifyBaseValue(float modification)
    {
        baseValue += modification;
        recalculationQueued = true;
    }
    public float GetBaseValue()
    {
        return baseValue;
    }
    public float GetDecimalModifier()
    {
        return decimalModifier;
    }
    public float GetFlatModifier()
    {
        return flatModifier;
    }
    private void CalculateTotalValue()
    {
        totalValue = (baseValue * (1f + decimalModifier)) + flatModifier;
    }
}
