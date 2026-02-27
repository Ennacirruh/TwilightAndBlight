using System.Collections.Generic;
using TwilightAndBlight;
using TwilightAndBlight.Map;
using TwilightAndBlight.Events;
using UnityEngine;


public class CombatTeamAssembly : MonoBehaviour
{
    private CombatTeam combatTeam = new CombatTeam();

    public List<CombatEntity> teamMembers = new List<CombatEntity>();
    public bool isPlayerTeam;

    private void Awake()
    {
        combatTeam.SetPlayerTeam(isPlayerTeam);
        foreach(CombatEntity combatEntity in teamMembers)
        {
            if(combatEntity != null)
            {
                combatTeam.AddCombatant(combatEntity);
            }
        }
    }
    private void OnEnable()
    {
        GameEvents.OnCombatStart += InitializeCombatTeams;
    }
    private void OnDisable()
    {
        GameEvents.OnCombatStart -= InitializeCombatTeams;
    }
    private void InitializeCombatTeams()
    {
        CombatManager.Instance.AddTeam(combatTeam);
    }
}
