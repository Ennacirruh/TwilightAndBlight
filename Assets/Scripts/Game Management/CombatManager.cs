using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TwilightAndBlight.Map;

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
        }
        private void OnDisable()
        {
            GameEvents.OnAbilitySelected -= ActionSelected;
            GameEvents.OnTargetsSelected -= TargetsSelected;
            GameEvents.OnAbilityPerformed -= AbilityPerformed;
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
            GameEvents.OnCombatStart?.Invoke();
            ProgressToNextTurn();
        }
        public void EndCombat()
        {
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
            GameEvents.OnTurnStart?.Invoke(entityTakingTurn);
            entityTakingTurn.OnTurnStart();
            SelectAction();
        }
        public void EndTurn()
        {
            GameEvents.OnTurnEnd?.Invoke(entityTakingTurn);
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
            foreach(CombatTeam team in teams)
            {
                if (team.TeamLost)
                {
                    teamsInTheRunning--;
                }
            }
            if (teamsInTheRunning == 1)
            {
                EndCombat();
            }
            else
            {
                ProgressToNextTurn();
            }
        }
        private void ActionSelected(CombatEntity entity)
        {
            if(entity = entityTakingTurn)
            {
                SelectTargets();
                UIManager.Instance.CloseAbilityPreview();
            }
        }
        private void TargetsSelected(CombatEntity entity)
        {
            if(entity = entityTakingTurn)
            {
                PerformAction();
            }
        }
        private void AbilityPerformed(CombatEntity entity, MapNode origin) 
        {
            if(entity = entityTakingTurn)
            {
                MapManager.Instance.ResetHighlight();
                CheckWinConditions();
            }
        }
        public void AddTeam(CombatTeam team) 
        {
            teams.Add(team);
            foreach (CombatEntity entity in team.GetEntireTeam())
            {
                if(entity != null)
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