using UnityEngine;
using System.Collections.Generic;
namespace TwilightAndBlight
{
    [RequireComponent(typeof(EntityStats))]
    public abstract class CombatEntity : MonoBehaviour
    {
        private void OnEnable()
        {
            GameEvents.OnCombatStart += OnCombatStart;
        }
        private void OnDisable()
        {
            GameEvents.OnCombatStart -= OnCombatStart;
        }

        [SerializeField] protected int entityLevel;
        protected EntityStats entityStats;
        protected float turnProgress;
        protected CombatTeam combatTeam;
        protected HashSet<CombatEntity> targets = new HashSet<CombatEntity>();
        protected float health;
        public float MaxHealth { get { return 100f * Stats.Constitution; } }
        private bool actionInProgress;
        public int Level {  get { return entityLevel; } } 
        public float TurnProgress {  get { return turnProgress; } } 
        public EntityStats Stats { get { return entityStats; } }
        public abstract void SelectAction();
        public abstract void AcquireTargets();
        public abstract void Action();
        public void PerformAction()
        {
            actionInProgress = true;
            Action();

        }
        protected void ActionComplete()
        {
            actionInProgress=false;
        }
        public bool ActionInProgress()
        {
            return actionInProgress;
        }
        public virtual void OnCombatStart()
        {
            turnProgress = 0;
            health = MaxHealth;
        }
        public void AssignCombatTeam(CombatTeam team)
        {
            combatTeam = team;
        }
        
        public void TickTurnProgress(int ticks)
        {
            turnProgress += entityStats.Agility * ticks;
            if(turnProgress >= CombatManager.TurnThreshold)
            {
                turnProgress = 0;
            }

        }
        public void DamageEntity(float damage, float percentPenetration, float flatPenetration, CombatEntity source)
        {
            float armor = source.
        }
        public void KillEntity(CombatEntity source)
        {
            GameEvents.OnEntityKilled?.Invoke(this, source);
            combatTeam.RemoveCombatant(source);
            Destroy(gameObject);
        }
        public float GetTicksToTurn()
        {
            return (CombatManager.TurnThreshold - TurnProgress) / Stats.Agility;
        }
    }
}
