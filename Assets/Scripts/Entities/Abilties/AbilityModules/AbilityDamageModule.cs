
using System;
using System.Collections;
using System.Collections.Generic;
using TwilightAndBlight.Map;
using UnityEngine;
using TwilightAndBlight.Events;
namespace TwilightAndBlight.Ability.Module
{
    [Serializable]
    public class AbilityDamageModule : AbilityModule
    {
        [SerializeField] protected List<DamageType> damageTypes = new List<DamageType>();
        [SerializeField] protected float baseDamage;
        [SerializeField] protected List<VariableStatScaler> damageScalers = new List<VariableStatScaler>();
        [SerializeField] protected float basePercentArmorPen;
        [SerializeField] protected List<VariableStatScaler> percentArmorPenScalers = new List<VariableStatScaler>();
        [SerializeField] protected float baseFlatArmorPen;
        [SerializeField] protected List<VariableStatScaler> flatArmorPenScalers = new List<VariableStatScaler>();
        [SerializeField] protected int baseDamageTicks;
        [SerializeField] protected List<VariableStatScaler> damageTickScalers = new List<VariableStatScaler>();
        [SerializeField] protected float timeBetweenTicks;
        
        protected HashSet<DamageType> damageTypeSet = new HashSet<DamageType>();

        public override void InitializeAbilityModule(EntityAbility ability, string lookupTablePrefix = "")
        {
            base.InitializeAbilityModule(ability, lookupTablePrefix);
            damageTypeSet = new HashSet<DamageType>(damageTypes);
        }
        public override void GenerateStringConversionTable(ref Dictionary<string, string> dictionary)
        {
            AddElementToLookupTable(ref dictionary, "damagetypes", GetStringFromDamageList(damageTypes));
            AddElementToLookupTable(ref dictionary, "basedamage", baseDamage.ToString());
            AddElementToLookupTable(ref dictionary, "damagescalers", GetStringFromScalerList(damageScalers));
            AddElementToLookupTable(ref dictionary, "basepercentarmorpen", (basePercentArmorPen).ToString());
            AddElementToLookupTable(ref dictionary, "percentarmorpenscalers", GetStringFromScalerList(percentArmorPenScalers));
            AddElementToLookupTable(ref dictionary, "baseflatarmorpen", baseFlatArmorPen.ToString());
            AddElementToLookupTable(ref dictionary, "flatarmorpenscalers", GetStringFromScalerList(flatArmorPenScalers));
            AddElementToLookupTable(ref dictionary, "basedamageticks", baseDamageTicks.ToString());
            AddElementToLookupTable(ref dictionary, "damagetickscalers", GetStringFromScalerList(damageTickScalers));
            AddElementToLookupTable(ref dictionary, "damage", GetDamage().ToString());
            AddElementToLookupTable(ref dictionary, "flatarmorpen", GetFlatArmorPen().ToString());
            AddElementToLookupTable(ref dictionary, "percentarmorpen", (GetPercentArmorPen()).ToString());
            AddElementToLookupTable(ref dictionary, "damageticks", GetDamageTicks().ToString());
        }

        public IEnumerator PerformDamageBehavior(IEnumerable<MapNode> targets, float range = 0)
        {
            for (int i = 0; i < GetDamageTicks(); i++)
            {
                foreach (MapNode node in targets)
                {
                    float damage = GetDamage();
                    if (respectLineOfSight)
                    {
                        damage *= GetCoverMultiplier(node, range);
                    }
                    CombatEntity target = node.GetCombatEntity();
                    if (target != null)
                    {

                        DamageTarget(target, node, damage);
                    }
                }
                yield return new WaitForSeconds(timeBetweenTicks);
            }
            moduleBehaviorCoroutine = null;
        }
        
        public IEnumerator PerformDamageBehavior(MapNode targetNode, float range)
        {
            float damage = GetDamage();
            if (respectLineOfSight)
            {
                damage *= GetCoverMultiplier(targetNode, range);
            }

            CombatEntity target = targetNode.GetCombatEntity();
            if (target != null)
            {
                for (int i = 0; i < GetDamageTicks(); i++)
                {
                    DamageTarget(target, targetNode, damage);
                    yield return new WaitForSeconds(timeBetweenTicks);
                }
            }
            moduleBehaviorCoroutine = null;
        }
        private void DamageTarget(CombatEntity target, MapNode targetNode, float damage)
        {
            prePerTargetBehaviorExpansion?.Invoke(targetNode, damage);
            float damageTaken = target.DamageEntity(owner.OwningCombatEntity, damage, damageTypeSet, GetPercentArmorPen() / 100f, GetFlatArmorPen());
            postPerTargetBehaviorExpansion?.Invoke(targetNode, damageTaken);
        }
        public void HighlightNodes(IEnumerable<MapNode> nodes, MapNode origin, MapNodeConditional condition, float range, ref HashSet<MapNode> validSet)
        {
            foreach (MapNode node in nodes)
            {

                bool genericHighlight = true;
                if (RespectLineOfSight)
                {
                    if (LineOfSightObstructed(origin, node, AbilityModuleCastHeightOffset, range, LineOfSightForgiveness)) //GetTrueOrigin(targetingOrigin)
                    {
                        genericHighlight = false;
                        MapManager.Instance.HighlightNodes(node, IndicatorType.AltGeneric);
                    }
                }
                if (genericHighlight)
                {
                    if (condition.Invoke(node))
                    {
                        if (!CanTargetSelf && owner.OwningCombatEntity.GetCurrentMapNode() == node)
                        {
                            MapManager.Instance.HighlightNodes(node, IndicatorType.Generic);
                        }
                        else
                        {
                            if (node.GetCombatEntity().GetCombatTeam() == owner.OwningCombatEntity.GetCombatTeam())
                            {
                                MapManager.Instance.HighlightNodes(node, IndicatorType.Warnign);
                            }
                            else
                            {
                                MapManager.Instance.HighlightNodes(node, IndicatorType.Valid);
                            }
                            validSet.Add(node);
                        }
                    }
                    else
                    {
                        MapManager.Instance.HighlightNodes(node, IndicatorType.Generic);
                    }
                }
            }
        }

        public virtual float GetDamage()
        {
            return GetScaledStat(baseDamage, damageScalers, 1);
        }
        public virtual float GetFlatArmorPen()
        {
            return GetScaledStat(baseFlatArmorPen, flatArmorPenScalers);
        }
        public virtual float GetPercentArmorPen()
        {
            return GetScaledStat(basePercentArmorPen, percentArmorPenScalers);
        }
        public virtual int GetDamageTicks()
        {
            return GetScaledStat(baseDamageTicks, damageTickScalers, 1);
        }

        public static string GetStringFromDamageList(List<DamageType> list)
        {
            string returnString = "";

            if (list == null || list.Count == 0)
            {
                return returnString;
            }
            if (list.Count == 1)
            {
                returnString += $"{list[0].ToString()}";
                return returnString;
            }

            for (int i = 0; i < list.Count - 1; i++)
            {
                DamageType type = list[i];
                returnString += $"{type.ToString()}, ";
            }
            returnString += $"and {list[list.Count - 1].ToString()}";
            return returnString;
        }
    }
}
