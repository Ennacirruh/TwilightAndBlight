using System.Collections;
using TwilightAndBlight.Ability;
using TwilightAndBlight.Map;
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
            UIManager.Instance.PreviewAbilities(this);
            selectedAbility = null;
            yield return new WaitUntil(() => (selectedAbility != null));
            UIManager.Instance.CloseAbilityPreview();
            GameEvents.OnAbilitySelected?.Invoke(this);
            abilitySelectionCoroutine = null;
        }
        private IEnumerator TargetAcquisitionCoroutine()
        {
            while (acquiringTarget)
            {
                RaycastHit hit;
                Debug.DrawRay(Camera.main.transform.position, Camera.main.ScreenPointToRay(Input.mousePosition).direction * 1000f, Color.red);
                if (Physics.Raycast(Camera.main.transform.position, Camera.main.ScreenPointToRay(Input.mousePosition).direction, out hit, Mathf.Infinity, MapNode.GetMapNodeMask()))
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
                GameEvents.OnTargetsSelected?.Invoke(this);
            }
            else
            {
                SelectAction();
            }
        }
        private void SelectNode(InputAction.CallbackContext context)
        {
            if (CombatManager.Instance.GetCombatEntityTakingTurn() == this && selectedAbility != null && selectedAbility.IsValidTarget(targetNode))
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
