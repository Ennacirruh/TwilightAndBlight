using UnityEngine;
using System.Collections.Generic;
using TwilightAndBlight.Map;
namespace TwilightAndBlight.Events
{
    public delegate void GenericAction();
    public delegate float GenericFloatAction();
    public delegate void DamageEntityInteraction(DamageEntityInteractionCallback callback);
    public struct DamageEntityInteractionCallback
    {
        public float preMitigationDamage;
        public float postMitigationDamage;
        public CombatEntity target;
        public CombatEntity source;
        public bool crit;
        public DamageEntityInteractionCallback(float preMitigationDamage, float postMitigationDamage, CombatEntity target, CombatEntity source, bool crit) 
        {
            this.preMitigationDamage = preMitigationDamage;
            this.postMitigationDamage= postMitigationDamage;
            this.target = target;
            this.source = source;
            this.crit = crit;
        }  
    }
    public delegate bool DamageEntityOverride(CombatEntity source, CombatEntity target, ref float attack, ref HashSet<DamageType> damageTypes, ref float percentPenetration,ref float flatPenetration, ref float damageRangeWeight, ref float critChance, ref float critDamage, ref bool crit);   
    public delegate void ReplenishEntityInteraction(ReplenishEntityInteractionCallback callback);
    public struct ReplenishEntityInteractionCallback
    {
        public float recoveryValue;
        public float overRecoveryValue;
        public CombatEntity target;
        public CombatEntity source;
        public bool crit;
        public ReplenishEntityInteractionCallback(float recoveryValue, float overRecoveryValue, CombatEntity target, CombatEntity source, bool crit)
        {
            this.recoveryValue = recoveryValue;
            this.overRecoveryValue = overRecoveryValue;
            this.target = target;
            this.source = source;
            this.crit = crit;
        }
    }
    public delegate bool ReplenishEntityOverride(CombatEntity source, CombatEntity target, ref float recovery, ref float recoveryRangeWeight, ref float critChance, ref float critPower, ref bool crit);
    public delegate void DrainEntityResourceInteraction(DrainEntityResourceInteractionCallback callback);
    public struct DrainEntityResourceInteractionCallback
    {
        public float resourceDrain;
        public CombatEntity target;
        public CombatEntity source;
        public DrainEntityResourceInteractionCallback(float resourceDrain, CombatEntity target, CombatEntity source)
        {
            this.resourceDrain = resourceDrain;
            this.target = target;
            this.source = source;
        }
    }
    public delegate bool DrainEntityResourceOverride(CombatEntity source, CombatEntity target, ref float resourceDrain);
    public delegate void CombatEntityAction(CombatEntityActionCallback callback);
    public struct CombatEntityActionCallback
    {
        public CombatEntity entity;
        public CombatEntityActionCallback(CombatEntity entity)
        {
            this.entity = entity;
        }
    }
    public delegate void CombatEntityInteraction(CombatEntityInteractionCallback callback);
    public struct CombatEntityInteractionCallback
    {
        public CombatEntity target;
        public CombatEntity source;
        public CombatEntityInteractionCallback(CombatEntity target, CombatEntity source)
        {
            this.target = target;
            this.source = source;
        }
    }
    public delegate void KillEntityOverride(CombatEntity target, CombatEntity source, ref bool kill);
    public delegate void CombatTeamCombatEntityInteraction(CombatTeamCombatEntityInteractionCallback callback);
    public struct CombatTeamCombatEntityInteractionCallback
    {
        public CombatTeam team;
        public CombatEntity entity;
        public CombatTeamCombatEntityInteractionCallback(CombatTeam combatTeam, CombatEntity combatEntity)
        {
            this.team = combatTeam;
            this.entity = combatEntity;
        }
    }
    public delegate void CombatResourceChangeAction(CombatResourceChangeActionCallback callback);
    public struct CombatResourceChangeActionCallback
    {
        public CombatEntity entity;
        float differnece;
        public CombatResourceChangeActionCallback(CombatEntity entity, float difference)
        {
            this.entity = entity;
            this.differnece = difference;
        }
    }
    public delegate void CombatEntityMapNodeInteraction(CombatEntityMapNodeInteractionCallback callback);
    public struct CombatEntityMapNodeInteractionCallback
    {
        public CombatEntity entity;
        public MapNode mapNode;
        public CombatEntityMapNodeInteractionCallback(CombatEntity entity, MapNode mapNode)
        {
            this.entity = entity;
            this.mapNode = mapNode;
        }
    }
    public delegate bool MapNodeParentConditional(MapNode parent, MapNode neighbor);
    public delegate bool MapNodeConditional(MapNode node);


}
