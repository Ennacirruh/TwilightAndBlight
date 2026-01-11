using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;
namespace TwilightAndBlight {
    public class CombatManager : MonoBehaviour
    {
        private static CombatManager instance;
        public static CombatManager Instance { get { return instance; } }
        public const float TurnThreshold = 10000;
        public const float damageVarianceRange = 0.25f;
        private List<CombatTeam> teams = new List<CombatTeam>();
        private List<CombatEntity> entityTurnQueue = new List<CombatEntity>();
        private CombatEntity entityTakingTurn;
        public int cursorPos;
        public CombatTeam cursorTarget;
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
        public void BeginCombat()
        {
            GameEvents.OnCombatStart?.Invoke();
            ProgressToNextTurn();
        }
        public void EndCombat()
        {
            GameEvents.OnCombatEnd?.Invoke();

        }
        public void ProgressToNextTurn()
        {
            entityTurnQueue.Sort(new SortCombatEntitiesByTurn());
            CombatEntity nextEntity = entityTurnQueue[0];
            int ticks = Mathf.CeilToInt(nextEntity.GetTicksToTurn());
            foreach (CombatEntity entity in entityTurnQueue)
            {
                entity.TickTurnProgress(ticks);
            }
            entityTakingTurn = nextEntity;
            BeginTurn();
        }
        public void BeginTurn()
        {
            GameEvents.OnTurnStart.Invoke(entityTakingTurn);
            SelectAction();
        }
        public void EndTurn()
        {
            GameEvents.OnTurnEnd.Invoke(entityTakingTurn);
            CheckWinConditions();
        }
        public void SelectAction()
        {
            entityTakingTurn.SelectAction();
            SelectTargets();
        }
        public void SelectTargets()
        {
            entityTakingTurn.AcquireTargets();
            PerformAction();
        }
        public void PerformAction()
        {
            entityTakingTurn.PerformAction();
            StartCoroutine(WaitUntilActionCompleteCoroutine());
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
        private bool CurrentActionComplete()
        {
            return !entityTakingTurn.ActionInProgress();
        }

        private IEnumerator WaitUntilActionCompleteCoroutine()
        {
            yield return new WaitUntil(CurrentActionComplete);
            EndTurn();
        }
    }
}