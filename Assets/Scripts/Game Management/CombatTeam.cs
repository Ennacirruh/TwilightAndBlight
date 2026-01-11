using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
namespace TwilightAndBlight
{
    public class CombatTeam
    {
        public static int maxTeamSize = 5;
        private List<CombatEntity> team = new List<CombatEntity>(maxTeamSize);
        public bool TeamLost {get { return team.Count == 0; } }
        public void AddCombatant(CombatEntity entity, int pos = -1)
        {
            if (pos == -1)
            {
                if (!team.Contains(entity))
                {
                    for (int i = 0; i < team.Count; i++)
                    {
                        if(team[i] == null)
                        {
                            team[i] = entity;
                            entity.AssignCombatTeam(this);
                            GameEvents.OnTeamJoin.Invoke(this, entity);
                            break;
                        }
                    }
                }
            }
            else
            {
                if (team[pos] != null)
                {
                    RemoveCombatant(pos);
                }
                if (team.Contains(entity))
                {
                    RemoveCombatant(entity);
                }
                team[pos] = entity;
                entity.AssignCombatTeam(this);
                GameEvents.OnTeamJoin.Invoke(this, entity);
            }
        }
        public void RemoveCombatant(CombatEntity entity) 
        {
            if (team.Contains(entity))
            {
                int index = team.IndexOf(entity);
                team[index] = null;
                entity.AssignCombatTeam(null);
                GameEvents.OnTeamLeave.Invoke(this, entity);
            }
        }
        public void RemoveCombatant(int index)
        {
            CombatEntity entity = team[index];
            if (entity != null)
            { 
                team[index] = null;
                entity.AssignCombatTeam(null);
                GameEvents.OnTeamLeave.Invoke(this, entity);
                
            }
        }
        public CombatEntity GetCombatEntity(int index)
        {
            if (index >= 0 && index < team.Count)
            {
                return team[index];
            }
            return null;
        }

    }
}
