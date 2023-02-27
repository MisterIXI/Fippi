using System;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;
public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }
    private PlayerInput _playerInput;
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    private void Start()
    {
        _playerInput = GetComponent<PlayerInput>();
        SubscribeToActions();
    }
    private void OnDestroy()
    {
        UnsubscribeFromActions();
    }
    public static event Action<CallbackContext> OnMove;
    public static event Action<CallbackContext> OnCommanderSwitch;
    public static event Action<CallbackContext> OnCommanderSwitchNumber;
    public static event Action<CallbackContext> OnRecall;
    public void OnMoveInput(CallbackContext context)
    {
        OnMove?.Invoke(context);
    }
    public void OnCommanderSwitchInput(CallbackContext context)
    {
        OnCommanderSwitch?.Invoke(context);
    }
    public void OnCommanderSwitchNumberInput(CallbackContext context)
    {
        OnCommanderSwitchNumber?.Invoke(context);
    }

    public void OnRecallInput(CallbackContext context)
    {
        OnRecall?.Invoke(context);
    }
    private void SubscribeToActions()
    {
        _playerInput.actions["Move"].started += OnMoveInput;
        _playerInput.actions["Move"].performed += OnMoveInput;
        _playerInput.actions["Move"].canceled += OnMoveInput;

        _playerInput.actions["CommanderSwitch"].started += OnCommanderSwitchInput;
        _playerInput.actions["CommanderSwitch"].performed += OnCommanderSwitchInput;
        _playerInput.actions["CommanderSwitch"].canceled += OnCommanderSwitchInput;

        _playerInput.actions["CommanderSwitchNumber"].started += OnCommanderSwitchNumberInput;
        _playerInput.actions["CommanderSwitchNumber"].performed += OnCommanderSwitchNumberInput;
        _playerInput.actions["CommanderSwitchNumber"].canceled += OnCommanderSwitchNumberInput;

        _playerInput.actions["Recall"].started += OnRecallInput;
        _playerInput.actions["Recall"].performed += OnRecallInput;
        _playerInput.actions["Recall"].canceled += OnRecallInput;
    }

    private void UnsubscribeFromActions()
    {
        _playerInput.actions["Move"].started -= OnMoveInput;
        _playerInput.actions["Move"].performed -= OnMoveInput;
        _playerInput.actions["Move"].canceled -= OnMoveInput;

        _playerInput.actions["CommanderSwitch"].started -= OnCommanderSwitchInput;
        _playerInput.actions["CommanderSwitch"].performed -= OnCommanderSwitchInput;
        _playerInput.actions["CommanderSwitch"].canceled -= OnCommanderSwitchInput;

        _playerInput.actions["CommanderSwitchNumber"].started -= OnCommanderSwitchNumberInput;
        _playerInput.actions["CommanderSwitchNumber"].performed -= OnCommanderSwitchNumberInput;
        _playerInput.actions["CommanderSwitchNumber"].canceled -= OnCommanderSwitchNumberInput;

        _playerInput.actions["Recall"].started -= OnRecallInput;
        _playerInput.actions["Recall"].performed -= OnRecallInput;
        _playerInput.actions["Recall"].canceled -= OnRecallInput;
    }

}