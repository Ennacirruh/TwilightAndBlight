using System;
using System.Collections.Generic;
using UnityEngine;
using TwilightAndBlight.Map;
using UnityEngine.Events;
namespace TwilightAndBlight.Ability.Module
{
    [Serializable]
    public abstract class AbilityModule : AbilityModuleData
    {
        [SerializeField] protected bool canTargetSelf;
        [SerializeField] protected bool respectLineOfSight;
        [SerializeField] protected float lineOfSightForgiveness = 1f;
        [SerializeField] protected float abilityModuleCastHeightOffset = 1f;
        [SerializeField] public UnityEvent<MapNode, float> prePerTargetBehaviorExpansion; //target of action, and action value IE damage to be dealt
        [SerializeField] public UnityEvent<MapNode, float> postPerTargetBehaviorExpansion; // same as obove, but IE damage actually dealt

        public bool CanTargetSelf { get { return canTargetSelf; } }
        public bool RespectLineOfSight { get { return respectLineOfSight; } set { respectLineOfSight = value; } }
        public float LineOfSightForgiveness { get { return lineOfSightForgiveness; } }
        public float AbilityModuleCastHeightOffset { get { return abilityModuleCastHeightOffset; } }
        public Coroutine moduleBehaviorCoroutine;
        
        public bool LineOfSightObstructed(MapNode origin, MapNode target, float rayHeightOffset, float maxRange, float targetHeightOffsetMultiplier = 1)
        {
            RaycastHit hit;
            return LineOfSightObstructed(origin, target, rayHeightOffset, out hit, maxRange, targetHeightOffsetMultiplier);
        }
        public bool LineOfSightObstructed(MapNode origin, MapNode target, float rayHeightOffset, out RaycastHit hit, float maxRange, float targetHeightOffsetMultiplier = 1)
        {
            Vector3 originPos = origin.transform.position + (origin.transform.up * rayHeightOffset);
            float targetOffset = (target.IsOccupied() ? target.GetCombatEntity().EntityHeight : lineOfSightForgiveness) * targetHeightOffsetMultiplier;
            Vector3 targetPos = target.transform.position + (target.transform.up * targetOffset);
            Vector3 direction = (targetPos - originPos).normalized;

            float distance = (targetPos - originPos).magnitude * 0.99f;

            bool result = Physics.Raycast(originPos, direction, out hit, distance, MapNode.GetMapNodeMask());
            if (distance >= maxRange + (MapManager.gridDistanceToWorldDistance * 2f))
            {
                return true;
            }
            return result;
        }
        protected float GetCoverMultiplier(MapNode node, float maxRange)
        {
            float multiplier = 1f;
            if (LineOfSightObstructed(owner.OwningCombatEntity.GetCurrentMapNode(), node, abilityModuleCastHeightOffset, maxRange, 0.2f)) // Partial Cover
            {
                multiplier = .75f;
                if (LineOfSightObstructed(owner.OwningCombatEntity.GetCurrentMapNode(), node, abilityModuleCastHeightOffset, maxRange, 0.4f)) // More Partial Cover
                {
                    multiplier = .5f;
                    if (LineOfSightObstructed(owner.OwningCombatEntity.GetCurrentMapNode(), node, abilityModuleCastHeightOffset, maxRange, 0.75f)) // Even More Partial Cover
                    {
                        multiplier = .25f;
                    }
                }
            }
            return multiplier;
        }
        
    }
}