using System;
using System.Collections.Generic;
using System.Linq;
using TwilightAndBlight.Events;
namespace TwilightAndBlight
{
    [Serializable]
    public class CombatTeam
    {
        public static int maxTeamSize = 6;
        private HashSet<CombatEntity> team = new HashSet<CombatEntity>();
        private CombatEntity[] teamSlots = new CombatEntity[maxTeamSize];
        private bool playerTeam;
        public bool IsPlayerTeam { get { return playerTeam; } }
        public bool TeamLost {get 
            {
                foreach (CombatEntity e in teamSlots)
                {
                    if (e != null)
                    {
                        return false;
                    }
                }
                return true;
            } }
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
                            GameEvents.OnTeamJoin?.Invoke(new CombatTeamCombatEntityInteractionCallback(this, entity));
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
                GameEvents.OnTeamJoin.Invoke(new CombatTeamCombatEntityInteractionCallback(this, entity));
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
                        GameEvents.OnTeamLeave?.Invoke(new CombatTeamCombatEntityInteractionCallback(this, entity));
                        break;
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
                GameEvents.OnTeamLeave.Invoke(new CombatTeamCombatEntityInteractionCallback(this, entity));
                
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
        public void SetPlayerTeam(bool isPlayerTeam)
        {
            playerTeam = isPlayerTeam;
        }
        public string GetTeamString()
        {
            return teamSlots.ToString();
        }
    }
}
