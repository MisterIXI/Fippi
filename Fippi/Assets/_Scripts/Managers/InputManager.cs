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
    public void OnMoveInput(CallbackContext context)
    {
        OnMove?.Invoke(context);
    }
    private void SubscribeToActions()
    {
        _playerInput.actions["Move"].started += OnMoveInput;
        _playerInput.actions["Move"].performed += OnMoveInput;
        _playerInput.actions["Move"].canceled += OnMoveInput;
    }

    private void UnsubscribeFromActions()
    {
        _playerInput.actions["Move"].started -= OnMoveInput;
        _playerInput.actions["Move"].performed -= OnMoveInput;
        _playerInput.actions["Move"].canceled -= OnMoveInput;
    }

}