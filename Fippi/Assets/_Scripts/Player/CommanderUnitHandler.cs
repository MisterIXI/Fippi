using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class CommanderUnitHandler : MonoBehaviour
{
    private Vector2 _cursorInput;
    private Vector2 _cursorPos => _cursorInput + (Vector2)transform.position;
    private Camera _camera => CameraController.Camera;
    private CommanderController _commanderController;
    private bool IsActiveCommander => _commanderController.IsActiveCommander;
    private void Awake()
    {
        _commanderController = GetComponent<CommanderController>();
        _commanderController.OnOwnershipChanged += OnCommanderOwnershipChanged;
    }
    private void Start()
    {

    }

    private void OnCommanderOwnershipChanged(bool isOwner)
    {
        if (isOwner)
        {
            SubscribeToInputs();
        }
        else
        {
            UnsubscribeFromInputs();
        }
    }

    public void OnCursorPositionInput(InputAction.CallbackContext context)
    {
        _cursorInput = context.ReadValue<Vector2>();
        // TODO: do input stuff here
    }
    private void SubscribeToInputs()
    {
        InputManager.OnMoveCursor += OnCursorPositionInput;
    }
    private void UnsubscribeFromInputs()
    {
        InputManager.OnMoveCursor -= OnCursorPositionInput;
    }
    private void OnDestroy()
    {
        UnsubscribeFromInputs();
    }


}