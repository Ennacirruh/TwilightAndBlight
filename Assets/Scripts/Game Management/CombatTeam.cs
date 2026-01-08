using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
namespace TwilightAndBlight
{
    public class CombatTeam
    {
        private HashSet<CombatEntity> team = new HashSet<CombatEntity>();
        public bool TeamLost {get { return team.Count == 0; } }
        public void AddCombatant(CombatEntity entity)
        {
            if (team.Add(entity))
            {
                entity.AssignCombatTeam(this);
                GameEvents.OnTeamJoin.Invoke(this, entity);
            }
        }
        public void RemoveCombatant(CombatEntity entity) 
        {
            if (team.Remove(entity))
            {
                entity.AssignCombatTeam(null);
                GameEvents.OnTeamLeave.Invoke(this, entity);
            }

        }
       
        
    }
}
