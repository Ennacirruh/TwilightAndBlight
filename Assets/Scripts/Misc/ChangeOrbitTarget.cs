using TwilightAndBlight;
using TwilightAndBlight.Map;
using TwilightAndBlight.Events;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class ChangeOrbitTarget : MonoBehaviour
{
    [SerializeField] private CinemachineCamera cinemachineCamera;
    private Vector3 defaultPosition;
    InputSystem_Actions inputActions;
    private Transform defaultTarget;
    private bool targetDefault;
    private CombatEntity targetMemory;
    private void Awake()
    {
        inputActions = new InputSystem_Actions();
        defaultTarget = cinemachineCamera.Target.TrackingTarget;
    }

    private void OnEnable()
    {
        inputActions.Enable();
        GameEvents.OnTurnStart += ChangeTarget;
        inputActions.Player.Toggle.performed += ToggleTargeting;
        inputActions.Player.Target.performed += SelectTarget;
    }
    private void OnDisable()
    {
        GameEvents.OnTurnStart -= ChangeTarget;
        inputActions.Player.Toggle.performed -= ToggleTargeting;
        inputActions.Player.Target.performed -= SelectTarget;
        inputActions.Disable();
    }
    private void ChangeTarget(CombatEntityActionCallback callback)
    {
        if (callback.entity != null)
        {
            targetMemory = callback.entity;
        }
        if (targetDefault || targetMemory == null)
        {
            ApplyTarget(defaultTarget);
        }
        else
        {
            ApplyTarget(targetMemory.transform);
        }
    }

    private void ToggleTargeting(InputAction.CallbackContext context)
    {
        targetDefault = !targetDefault;
        ChangeTarget(new CombatEntityActionCallback(null));
    }
    private void ApplyTarget(Transform target)
    {
        cinemachineCamera.Target.TrackingTarget = target;
    }
    private void SelectTarget(InputAction.CallbackContext context)
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue()).direction, out hit, Mathf.Infinity))
        {
            ApplyTarget(hit.transform);
        }
    }
}
