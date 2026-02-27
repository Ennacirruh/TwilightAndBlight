using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TwilightAndBlight.Map;
using TwilightAndBlight.Events;
using Unity.VisualScripting;
using UnityEngine;
namespace TwilightAndBlight {
    public class CombatManager : MonoBehaviour
    {
        private static CombatManager instance;
        public static CombatManager Instance { get { return instance; } }

        private List<CombatTeam> teams = new List<CombatTeam>();
        private List<CombatEntity> entityTurnQueue = new List<CombatEntity>();
        private CombatEntity entityTakingTurn;
        public CombatTeam cursorTarget;
        [SerializeField] private bool startCombat;
        private bool combatRunning;
        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(this);
            }
        }
        private void OnEnable()
        {
            GameEvents.OnAbilitySelected += ActionSelected;
            GameEvents.OnTargetsSelected += TargetsSelected;
            GameEvents.OnAbilityPerformed += AbilityPerformed;
            GameEvents.OnEntityKilled += RemoveCombatantFromTurnQueue;
        }
        private void OnDisable()
        {
            GameEvents.OnAbilitySelected -= ActionSelected;
            GameEvents.OnTargetsSelected -= TargetsSelected;
            GameEvents.OnAbilityPerformed -= AbilityPerformed;
            GameEvents.OnEntityKilled -= RemoveCombatantFromTurnQueue;
        }
        private void Update()
        {
            if (startCombat)
            {
                startCombat = false;
                BeginCombat();
            }
        }
        public void BeginCombat()
        {
            combatRunning = true;
            GameEvents.OnCombatStart?.Invoke();
            ProgressToNextTurn();
        }
        public void EndCombat()
        {
            combatRunning = false;
            CombatTeam victor = null;
            foreach(CombatTeam team in teams)
            {
                if (!team.TeamLost)
                {
                    victor = team;
                    break;
                }
            }
            if (victor != null && victor.IsPlayerTeam)
            {
                UIManager.Instance.OpenVictoryScreen();
            }
            else
            {
                UIManager.Instance.OpenDefeatScreen();
            }
            Debug.Log("Combat Ended");
            GameEvents.OnCombatEnd?.Invoke();

        }
        public void ProgressToNextTurn()
        {
            entityTurnQueue.Sort(new SortCombatEntitiesByTurn());
            CombatEntity nextEntity = entityTurnQueue[0];
            int ticks = Mathf.CeilToInt(nextEntity.GetTicksToTurn());
            //Debug.Log($"Ticks Passed: {ticks}");
            if (ticks > 0)
            {
                foreach (CombatEntity entity in entityTurnQueue)
                {
                    entity.TickTurnProgress(ticks);
                }
            }
            entityTakingTurn = nextEntity;
            entityTakingTurn.ResetTurnProgress();
            BeginTurn();
        }
        public void BeginTurn()
        {
            GameEvents.OnTurnStart?.Invoke(new CombatEntityActionCallback(entityTakingTurn));
            entityTakingTurn.OnTurnStart();
            SelectAction();
        }
        public void EndTurn()
        {
            GameEvents.OnTurnEnd?.Invoke(new CombatEntityActionCallback(entityTakingTurn));
            CheckWinConditions();
        }
        public void SelectAction()
        {
            entityTakingTurn.SelectAction();

        }
        public void SelectTargets()
        {
            entityTakingTurn.AcquireTargets();
        }
        public void PerformAction()
        {
            entityTakingTurn.PerformAction();
        }
        public void CheckWinConditions()
        {
            
            int teamsInTheRunning = teams.Count;
            foreach (CombatTeam team in teams)
            {
                Debug.Log($"Team: {team} : {team.TeamLost}");
                if (team.TeamLost)
                {
                    teamsInTheRunning--;
                }
            }
            Debug.Log($"Teams in the running: {teamsInTheRunning}");
            if (teamsInTheRunning == 1)
            {
                EndCombat();
            }
           
        }
        private void ActionSelected(CombatEntityActionCallback callback)
        {
            if (callback.entity = entityTakingTurn)
            {
                SelectTargets();
                UIManager.Instance.CloseAbilityPreview();
            }
        }
        private void TargetsSelected(CombatEntityActionCallback callback)
        {
            if (callback.entity = entityTakingTurn)
            {
                PerformAction();
            }
        }
        private void AbilityPerformed(CombatEntityMapNodeInteractionCallback callback)
        {
            if (combatRunning && callback.entity == entityTakingTurn)
            {
                MapManager.Instance.ResetHighlight();
                ProgressToNextTurn();
            }
        }
        public void AddTeam(CombatTeam team, bool neutralTream = false)
        {
            if (!neutralTream)
            {
                teams.Add(team);
            }
            foreach (CombatEntity entity in team.GetEntireTeam())
            {
                if (entity != null)
                {
                    entityTurnQueue.Add(entity);
                    HashSet<MapNode> mapNodes = new HashSet<MapNode>(MapManager.Instance.GetSpawnNodes(team));
                    for (int i = 0; i < mapNodes.Count; i++)
                    {
                        MapNode node = mapNodes.ElementAt(UnityEngine.Random.Range(0, mapNodes.Count - 1));
                        if (!node.IsOccupied())
                        {
                            node.AssignEntity(entity, Vector2.zero);
                            break;
                        }
                        mapNodes.Remove(node);
                    }
                }
            }
        }
        private void RemoveCombatantFromTurnQueue(CombatEntityInteractionCallback callback)
        {
            
            entityTurnQueue.Remove(callback.target);
            StartCoroutine(DelayedCheckWInConditions());
        }
        private IEnumerator DelayedCheckWInConditions()
        {
            yield return new WaitForNextFrameUnit();
            CheckWinConditions();
        }
        public void AddCombatantToGame(CombatEntity entity)
        {
            // For Neutrals
        }
        public CombatEntity GetCombatEntityTakingTurn()
        {
            return entityTakingTurn;
        }
        public List<CombatTeam> GetCombatTeams()
        {
            return teams;
        }
      
       
    }
}