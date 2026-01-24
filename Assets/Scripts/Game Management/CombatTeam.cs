using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
namespace TwilightAndBlight
{
    [Serializable]
    public class CombatTeam
    {
        public static int maxTeamSize = 5;
        private HashSet<CombatEntity> team = new HashSet<CombatEntity>();
        private CombatEntity[] teamSlots = new CombatEntity[maxTeamSize];
        public bool TeamLost {get { return team.Count == 0; } }
        public void AddCombatant(CombatEntity entity, int pos = -1)
        {
            if (pos == -1)
            {
                if (!team.Contains(entity))
                {
                    for (int i = 0; i < teamSlots.Length; i++)
                    {
                        if(teamSlots[i] == null)
                        {
                            teamSlots[i] = entity;
                            team.Add(entity);
                            entity.AssignCombatTeam(this);
                            GameEvents.OnTeamJoin?.Invoke(this, entity);
                            break;
                        }
                    }
                }
            }
            else
            {
                if (teamSlots[pos] != null)
                {
                    RemoveCombatant(pos);
                }
                if (team.Contains(entity))
                {
                    RemoveCombatant(entity);
                }
                teamSlots[pos] = entity;
                team.Add(entity);
                entity.AssignCombatTeam(this);
                GameEvents.OnTeamJoin.Invoke(this, entity);
            }
        }
        public void RemoveCombatant(CombatEntity entity) 
        {
            if (team.Contains(entity))
            {
                for (int i = 0; i < teamSlots.Length; i++)
                {
                    if( teamSlots[i] == entity)
                    {
                        teamSlots[i] = null;
                        team.Remove(entity);
                        entity.AssignCombatTeam(null);
                        GameEvents.OnTeamLeave.Invoke(this, entity);
                    }
                }
                
            }
        }
        public void RemoveCombatant(int index)
        {
            CombatEntity entity = teamSlots[index];
            if (entity != null)
            {
                teamSlots[index] = null;
                team.Remove(entity);
                entity.AssignCombatTeam(null);
                GameEvents.OnTeamLeave.Invoke(this, entity);
                
            }
        }
        public CombatEntity GetCombatEntity(int index)
        {
            if (index >= 0 && index < team.Count)
            {
                return teamSlots[index];
            }
            return null;
        }
        public List<CombatEntity> GetEntireTeam()
        {
            return new List<CombatEntity>(team);
        }

    }
}
