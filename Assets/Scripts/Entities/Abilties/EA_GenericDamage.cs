using System.Collections;
using System.Collections.Generic;
using TwilightAndBlight.Map;
using UnityEngine;
namespace TwilightAndBlight.Ability
{
    public class EA_GenericDamage : EA_GenericAbilityShape
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
        [SerializeField] protected float abilityCastHeightOffset; 
        private AbilityTarget targetMem;
        public override void HighlightAbility(MapNode targetingOrigin)
        {
            MapManager.Instance.ResetHighlight();
            HashSet<MapNode> nodes = GetTargetingNodes(targetingOrigin);
            foreach (MapNode node in nodes)
            {
                bool genericHighlight = true;
                if (respectLineOfSight)
                {
                    if (LineOfSightObstructed(GetTrueOrigin(targetingOrigin), node, abilityCastHeightOffset))
                    {
                        genericHighlight = false;
                        MapManager.Instance.HighlightNodes(node, IndicatorType.AltGeneric); 
                    }
                }
                if (genericHighlight)
                {
                    if (IsValidTarget(node))
                    {
                        if (node.GetCombatEntity().GetCombatTeam() == combatEntity.GetCombatTeam())
                        {
                            MapManager.Instance.HighlightNodes(node, IndicatorType.Warnign);
                        }
                        else
                        {
                            MapManager.Instance.HighlightNodes(node, IndicatorType.Valid);
                        }
                    }
                    else
                    {
                        MapManager.Instance.HighlightNodes(node, IndicatorType.Generic);
                    }
                }

            }
        }
        protected override Dictionary<string, string> GenerateStringConversionTable()
        {
            Dictionary<string, string> dict = base.GenerateStringConversionTable();
            dict.Add("damagetypes", GetStringFromDamageList(damageTypes));
            dict.Add("basedamage", baseDamage.ToString());
            dict.Add("damagescalers", GetStringFromScalerList(damageScalers));
            dict.Add("basepercentarmorpen", basePercentArmorPen.ToString());
            dict.Add("percentarmorpenscalers", GetStringFromScalerList(percentArmorPenScalers));
            dict.Add("baseflatarmorpen", baseFlatArmorPen.ToString());
            dict.Add("flatarmorpenscalers", GetStringFromScalerList(flatArmorPenScalers));
            dict.Add("basedamageticks", baseDamageTicks.ToString());
            dict.Add("damagetickscalers", GetStringFromScalerList(damageTickScalers));
            dict.Add("damage", GetDamage().ToString());
            dict.Add("flatarmorpen", GetFlatArmorPen().ToString());
            dict.Add("percentarmorpen", GetPercentArmorPen().ToString());
            dict.Add("damageticks", GetDamageTicks().ToString());
            return dict;
        }
        protected float GetDamage()
        {
            return GetScaledStat(baseDamage, damageScalers);
        }
        protected float GetFlatArmorPen()
        {
            return GetScaledStat(baseFlatArmorPen, flatArmorPenScalers);
        }
        protected float GetPercentArmorPen()
        {
            return GetScaledStat(basePercentArmorPen, percentArmorPenScalers);
        }
        protected int GetDamageTicks()
        {
            return GetScaledStat(baseDamageTicks, damageTickScalers);
        }
        protected override IEnumerator AbilityBehavior(MapNode targetingOrigin)
        {
            throw new System.NotImplementedException();
        }
        protected override void OnValidate()
        {
            base.OnValidate();
            if (targetFilter == AbilityTarget.EmptyNode || targetFilter == AbilityTarget.AnyNode)
            {
                targetFilter = targetMem;
            }
            else
            {
                targetMem = targetFilter;
            }
        }
    }
}