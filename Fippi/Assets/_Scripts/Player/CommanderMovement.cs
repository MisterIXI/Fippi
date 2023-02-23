using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(ColliderUpdater))]
public class CommanderMovement : MonoBehaviour
{
    private ColliderUpdater _colliderUpdater;
    private CommanderController _commanderController;
    private Vector2 _moveInput;
    private Vector2 _moveVector;
    private Rigidbody2D _rigidbody2D;
    private MovementSettings _movementSettings;
    private UnitMovement _unitMovement;
    private void Awake()
    {
        _unitMovement = GetComponent<UnitMovement>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _commanderController = GetComponent<CommanderController>();
        _colliderUpdater = GetComponent<ColliderUpdater>();
        _commanderController.OnOwnershipChanged += OnOwnershipChanged;
    }

    private void OnOwnershipChanged(bool isNowOwner)
    {
        if (isNowOwner)
        {
            enabled = true;
            _colliderUpdater.enabled = true;
            SubscribeToInputs();
        }
        else
        {
            enabled = false;
            _colliderUpdater.enabled = false;
            UnsubscribeFromInputs();
        }
    }
    private void Start()
    {
        _movementSettings = SpawnManager.MovementSettings;
        _commanderController.OwnActiveStateChanged += OnOwnActiveStateChanged;
    }
    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        // lerp movement
        if (_commanderController.IsActiveCommander && !_unitMovement.IsFollowingPath)
            _moveVector = Vector2.MoveTowards(_moveVector, _moveInput, _movementSettings.CMInputChangeMax);
        // apply movement
        _rigidbody2D.velocity = _moveVector * _movementSettings.CMMoveSpeed;
    }
    private void OnOwnActiveStateChanged(bool isActive)
    {
        if (!isActive)
        {
            ResetInputs();
        }
    }

    private void ResetInputs()
    {
        // _moveInput = Vector2.zero;
        _moveVector = Vector2.zero;
        _rigidbody2D.velocity = Vector2.zero;
    }
    private void OnMoveInput(InputAction.CallbackContext context)
    {
        // Debug.Log("Commander " + gameObject.name + " Move Input: " + context.ReadValue<Vector2>());

        if (context.performed)
        {
            _moveInput = context.ReadValue<Vector2>();
        }
        else if (context.canceled)
        {
            _moveInput = Vector2.zero;
        }
    }



    private void SubscribeToInputs()
    {
        InputManager.OnMove += OnMoveInput;
    }
    private void UnsubscribeFromInputs()
    {
        InputManager.OnMove -= OnMoveInput;
    }
    private void OnDestroy()
    {
        UnsubscribeFromInputs();
    }
}