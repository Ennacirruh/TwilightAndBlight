using UnityEngine;
using System.Collections.Generic;
using TwilightAndBlight.Map;
namespace TwilightAndBlight
{
    public delegate void GenericAction();
    public delegate float GenericFloatAction();
    public delegate void DamageEntityInteraction(float preMitigationDamage, float postMitigationDamage, CombatEntity target, CombatEntity source);
    public delegate void DamageEntityOverride(CombatEntity source, CombatEntity target, ref float attack, ref HashSet<DamageType> damageTypes, ref float percentPenetration,ref float flatPenetration, ref float damageRangeWeight, ref float critChance, ref float critDamage, ref bool crit);   
    public delegate void HealEntityInteraction(float heal, CombatEntity target, CombatEntity source);
    public delegate void CombatEntityAction(CombatEntity entity);
    public delegate void CombatEntityInteraction(CombatEntity target, CombatEntity source);
    public delegate void KillEntityOverride(CombatEntity target, CombatEntity source, ref bool kill);
    public delegate void CombatTeamInteraction(CombatTeam team, CombatEntity entity);
    public delegate void CombatResourceChangeAction(CombatEntity entity, float differnece);
    public delegate void CombatEntityMapNodeInteraction(CombatEntity entity, MapNode mapNode);
}
