using System.Collections;
using System.Collections.Generic;
using TwilightAndBlight.Ability;
using TwilightAndBlight.Map;
using TwilightAndBlight.Events;
using UnityEngine;
namespace TwilightAndBlight
{
    public class BasicAIControlledCombatEntity : CombatEntity
    {
        private Coroutine abilitySelectionCoroutine;


        public override void AcquireTargets()
        {
            
        }

        public override void Action()
        {
            
        }

        public override void SelectAction()
        {
            if (abilitySelectionCoroutine == null)
            {
                abilitySelectionCoroutine = StartCoroutine(ActionSelectCoroutine());
            }
        }
        private IEnumerator ActionSelectCoroutine()
        {
            if (freeActions.Count > 0 && !freeActionsPerformed)
            {
                freeActionsPerformed = true;
                performingFreeAction = true;
                foreach (EntityAbility freeAction in freeActions)
                {
                    if (freeAction.HasValidTargetInRange())
                    {
                        selectedAbility = freeAction;
                        AcquireTargets();
                        yield return new WaitUntil(() => (!performingFreeAction));
                    }
                }
            }
            UIManager.Instance.PreviewAbilities(this);
            //UIManager.Instance.PreviewStats(this);

            selectedAbility = null;
            yield return new WaitUntil(() => (selectedAbility != null));

            GameEvents.OnAbilitySelected?.Invoke(new CombatEntityActionCallback(this));
            abilitySelectionCoroutine = null;
        }
    }
}
