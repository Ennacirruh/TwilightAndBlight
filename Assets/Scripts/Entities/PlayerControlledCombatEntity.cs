using System.Collections;
using TwilightAndBlight.Ability;
using TwilightAndBlight.Map;
using TwilightAndBlight.Events;
using UnityEngine;
using UnityEngine.InputSystem;
namespace TwilightAndBlight
{
    public class PlayerControlledCombatEntity : CombatEntity
    {
        private bool acquiringTarget;
        private InputSystem_Actions inputActions;
        private MapNode targetNode;
        private Coroutine abilitySelectionCoroutine;
        protected override void Awake()
        {
            inputActions = new InputSystem_Actions();
            inputActions.Enable();
            
            base.Awake();
            
        }
        protected override void OnEnable()
        {
            base.OnEnable();
            inputActions.Player.Attack.performed += SelectNode;
            inputActions.Player.Cancel.performed += ReturnToAbilitySelection;
            
        }
        protected override void OnDisable()
        {
            base.OnDisable();
            inputActions.Player.Attack.performed -= SelectNode;
            inputActions.Player.Cancel.performed -= ReturnToAbilitySelection;

        }
        public override void AcquireTargets()
        {
            acquiringTarget = true;
            StartCoroutine(TargetAcquisitionCoroutine());
        }

        public override void Action()
        {
            selectedAbility.PerformAbility(targetNode);
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
                    selectedAbility = freeAction;
                    AcquireTargets();
                    yield return new WaitUntil(() => (!performingFreeAction));
                }
            }
            UIManager.Instance.PreviewAbilities(this);
            //UIManager.Instance.PreviewStats(this);

            selectedAbility = null;
            yield return new WaitUntil(() => (selectedAbility != null));
            UIManager.Instance.CloseAbilityPreview();
            UIManager.Instance.CloseStatPreview();

            GameEvents.OnAbilitySelected?.Invoke(new CombatEntityActionCallback(this));
            abilitySelectionCoroutine = null;
        }
        private IEnumerator TargetAcquisitionCoroutine()
        {
            while (acquiringTarget)
            {
                RaycastHit hit;
                Debug.DrawRay(Camera.main.transform.position, Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue()).direction * 1000f, Color.red);
                if (Physics.Raycast(Camera.main.transform.position, Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue()).direction, out hit, Mathf.Infinity, MapNode.GetMapNodeMask()))
                {
                    MapNode node = hit.collider.GetComponentInParent<MapNode>();
                    if (node != null && node != targetNode)
                    {
                        targetNode = node;
                        selectedAbility.HighlightAbility(node);
                    }
                }
                    yield return null;
            }
            if (selectedAbility != null)
            {
                GameEvents.OnTargetsSelected?.Invoke(new CombatEntityActionCallback(this));
            }
            else
            {
                if (performingFreeAction)
                {
                    performingFreeAction = false;
                }
                else
                {
                    SelectAction(); // for return to the ability selection menu
                }
            }
        }
        public override void OnTurnStart()
        {
            base.OnTurnStart();
            //UIManager.Instance.PreviewStats(this);
        }
        private void SelectNode(InputAction.CallbackContext context)
        {
            if (CombatManager.Instance.GetCombatEntityTakingTurn() == this && selectedAbility != null && selectedAbility.IsValidAbilityCast(targetNode))
            {
                acquiringTarget = false;
            }
        }
        private void ReturnToAbilitySelection(InputAction.CallbackContext context)
        {
            if (acquiringTarget && CombatManager.Instance.GetCombatEntityTakingTurn() == this)
            {
                MapManager.Instance.ResetHighlight();
                selectedAbility = null;
                acquiringTarget = false;
            }
        }
    }
}
