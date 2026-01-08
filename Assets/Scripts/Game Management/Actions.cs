using UnityEngine;
namespace TwilightAndBlight
{
    public delegate void GenericAction();
    public delegate float GenericFloatAction();
    public delegate void DamageEntityInteraction(float preMitigationDamage, float postMitigationDamage, CombatEntity target, CombatEntity source);
    public delegate void HealEntityInteraction(float heal, CombatEntity target, CombatEntity source);
    public delegate void CombatEntityAction(CombatEntity entity);
    public delegate void CombatEntityInteraction(CombatEntity target, CombatEntity source);
    public delegate void CombatTeamInteraction(CombatTeam team, CombatEntity entity);
}
