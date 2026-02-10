using NUnit.Framework.Constraints;
using System;
using System.Collections;
using System.Collections.Generic;
using TwilightAndBlight.Map;
using Unity.VisualScripting;
using UnityEngine;
namespace TwilightAndBlight.Ability.Module
{
    [Serializable]
    public class AbilityTerrainModule : AbilityModule
    {
        [SerializeField] protected float baseMaxShift;
        [SerializeField] protected List<VariableStatScaler> maxShiftScalers = new List<VariableStatScaler>();
        [SerializeField] protected float baseShiftSpeed;
        [SerializeField] protected List<VariableStatScaler> shiftSpeedScalers = new List<VariableStatScaler>();
        [SerializeField] protected float delayRange;
        [SerializeField] protected TerrainShiftType terrainShiftType;
        [SerializeField] protected bool realativeToTarget;
        protected int nodesBeingShifted = 0;
        protected MapNode targetingOriginMemory;
        private Dictionary<MapNode, float> initialHeightMemory = new Dictionary<MapNode, float>();
        public override void GenerateStringConversionTable(ref Dictionary<string, string> dictionary)
        {
            AddElementToLookupTable(ref dictionary, "basemaxshift", baseMaxShift.ToString());
            AddElementToLookupTable(ref dictionary, "maxshiftscalers", GetStringFromScalerList(maxShiftScalers));
            AddElementToLookupTable(ref dictionary, "maxshift", GetMaxShift().ToString());
            AddElementToLookupTable(ref dictionary, "baseshiftspeed", baseShiftSpeed.ToString());
            AddElementToLookupTable(ref dictionary, "shiftspeedscalers", GetStringFromScalerList(shiftSpeedScalers));
            AddElementToLookupTable(ref dictionary, "shiftspeed", GetShiftSpeed().ToString());
            AddElementToLookupTable(ref dictionary, "terrainshifttype", terrainShiftType.ToString());
        }
        public IEnumerator PerformTerrainBehavior(IEnumerable targets, float maxRange, MapNode targetingOrigin)
        {
            targetingOriginMemory = targetingOrigin;
            initialHeightMemory.Clear();
            foreach (MapNode node in targets)
            {
                Vector3 start = new Vector3(node.transform.position.x, 0, node.transform.position.z);
                Vector3 end = new Vector3(owner.transform.position.x, 0, owner.transform.position.z);
                float delayMultiplier = Vector3.Distance(start, end) / maxRange * MapManager.gridSizeToWorldSize;
                float speed = (delayRange + UnityEngine.Random.Range(0, 0.1f)) * delayMultiplier;
                RecordNodeHeight(node);
                prePerTargetBehaviorExpansion?.Invoke(node, GetMaxShift());
                owner.StartCoroutine(ShiftNode(node, speed));
            }
            yield return new WaitUntil(() => { return nodesBeingShifted == 0; });
            moduleBehaviorCoroutine = null;
        }
       
        protected virtual float GetMaxShift()
        {
            return GetScaledStat(baseMaxShift, maxShiftScalers, 0) * MapManager.gridSizeToWorldSize;
        }
        protected virtual float GetShiftSpeed()
        {
            return GetScaledStat(baseShiftSpeed, shiftSpeedScalers, 0.1f);
        }
        protected void RecordNodeHeight(MapNode node)
        {
            if (!initialHeightMemory.ContainsKey(node)) 
            {
                initialHeightMemory.Add(node, 0);
            }
            initialHeightMemory[node] = node.transform.position.y;
        }
        protected virtual IEnumerator ShiftNode(MapNode node, float startDelay)
        {
            nodesBeingShifted++;
            yield return new WaitForSeconds(startDelay);
            TriggerNodeShift(node);
            yield return new WaitUntil(() => { return !node.MoveInProgress(); });
            postPerTargetBehaviorExpansion?.Invoke(node, node.transform.position.y - initialHeightMemory[node]);
            nodesBeingShifted--;

        }
        protected virtual void TriggerNodeShift(MapNode node)
        {
            float newHeight = 0f;
            float maxShift = 0;
            switch (terrainShiftType)
            {
                case TerrainShiftType.Raise:
                    node.ShiftTerrainHeight(GetMaxShift(), GetShiftSpeed());
                    break;

                case TerrainShiftType.Flatten:
                    if (realativeToTarget)
                    {
                        newHeight = targetingOriginMemory.transform.position.y;
                    }
                    else
                    {
                        newHeight = owner.transform.position.y;
                    }
                    if (node.transform.position.y > newHeight)
                    {
                        maxShift = GetMaxShift();
                        newHeight = Mathf.Clamp(newHeight, node.transform.position.y - maxShift, node.transform.position.y + maxShift);
                        node.SetTerrainHeight(newHeight, GetShiftSpeed());
                    }
                    break;

                case TerrainShiftType.Lower:
                    node.ShiftTerrainHeight(-GetMaxShift(), GetShiftSpeed());
                    break;

                case TerrainShiftType.Bridge:
                    if (realativeToTarget)
                    {
                        newHeight = targetingOriginMemory.transform.position.y;
                    }
                    else
                    {
                        newHeight = owner.transform.position.y;
                    }
                    if (node.transform.position.y < newHeight)
                    {
                        maxShift = GetMaxShift();
                        newHeight = Mathf.Clamp(newHeight, node.transform.position.y - maxShift, node.transform.position.y + maxShift);
                        node.SetTerrainHeight(newHeight, GetShiftSpeed());
                    }
                    break;

                case TerrainShiftType.Level:
                    if (realativeToTarget)
                    {
                        newHeight = targetingOriginMemory.transform.position.y;
                    }
                    else
                    {
                        newHeight = owner.transform.position.y;
                    }
                    maxShift = GetMaxShift();
                    newHeight = Mathf.Clamp(newHeight, node.transform.position.y - maxShift, node.transform.position.y + maxShift);
                    node.SetTerrainHeight(newHeight, GetShiftSpeed());
                    break;
            }
        }
    }
}