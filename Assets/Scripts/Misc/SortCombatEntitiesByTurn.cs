using System.Collections;
using System.Collections.Generic;
using TwilightAndBlight;
using UnityEngine;

public class SortCombatEntitiesByTurn : IComparer<CombatEntity>
{
    public int Compare(CombatEntity x, CombatEntity y)
    {
        float a = x.GetTicksToTurn();
        float b = y.GetTicksToTurn();
        if (a < b) //x is closer to taking a turn
        {
            return -1;
        }
        if (a > b) // y is closer to taking a turn
        {
            return 1;
        }
        if (x.Stats.Agility < y.Stats.Agility) // y is faster tie breaker
        {
            return 1;
        }
        if (x.Stats.Agility > y.Stats.Agility) // x is fater tie breaker
        {
            return -1;
        }
        if (x.Level < y.Level) // y is a higher lever tie breaker
        {
            return 1;
        }
        if (x.Level > y.Level) // x is a heigher level tie breaker
        {
            return -1;
        }
        return 0; //tie
    }
    
}
