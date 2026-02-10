using System;
using TwilightAndBlight.Ability;
using UnityEngine;
[Serializable]
public class BehaviorExpansionContainer
{
    [SerializeField] private AbilityBehaviorExpansion behaviorExpansion;
    [SerializeField] private float behaviourExpansionValue;
    public AbilityBehaviorExpansion BehaviourExpansion { get { return behaviorExpansion; } } 
    public float BehaviourExpansionValue { get { return behaviourExpansionValue; } } 
}
