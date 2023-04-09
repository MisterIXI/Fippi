using UnityEngine;
[RequireComponent(typeof(CommanderController))]
public class CommanderTaskController : MonoBehaviour
{
    private CommanderController _commanderController;
    public Vector2 CursorPosition { get; private set; } = Vector2.right;

    private void Awake()
    {
        _commanderController = GetComponent<CommanderController>();
        _commanderController.OnOwnershipChanged += OnOwnershipChanged;
        _commanderController.OwnActiveStateChanged += OnOwnActiveStateChanged;
        enabled = false;
    }

    private void OnOwnershipChanged(bool isMine)
    {
        if (isMine)
        {
        }
    }

    private void OnOwnActiveStateChanged(bool isActive)
    {
        if (isActive)
        {
            enabled = true;
        }
        else
        {
            enabled = false;
        }
    }
}