using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using UnityEngine.InputSystem;
public class CommanderController : NetworkBehaviour
{
    public NetworkObject networkObject => gameObject.GetComponent<NetworkObject>();
    public static CommanderController ActiveCommander { get; private set; }
    public static event Action<ulong, CommanderController, CommanderController> OnActiveCommanderChanged;
    public NetworkVariable<int> commanderIDVar = new NetworkVariable<int>(-1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    [field: SerializeField] private TextMeshPro commanderNameText;
    public event Action<bool> OwnActiveStateChanged;
    public bool IsActiveCommander => ActiveCommander == this;
    public event Action<bool> OnOwnershipChanged;
    public event Action<string> OnNameChanged;
    public static int owner_id_helper = 0;
    public Vector2 Heading => transform.right;
    public Vector2 Right => transform.up;

    [field: SerializeField] public VisualUpdater VisualUpdater { get; private set; }
    private void Awake()
    {
        OnActiveCommanderChanged += OnActiveCommanderChangedHandler;
        commanderIDVar.OnValueChanged += (oldValue, newValue) => UpdateText();
        commanderIDVar.OnValueChanged += HandleIdChange;

    }
    private void Start()
    {
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsOwner)
        {
            commanderIDVar.Value = owner_id_helper++;
            if (commanderIDVar.Value == 0)
                MakeActiveCommander();
        }
        // StartCoroutine(DelayedStart());
        UpdateText();
        Debug.Log("IsOwner: " + IsOwner + " " + gameObject.name);
        VisualUpdater.UpdateVisuals();
    }
    private void HandleIdChange(int oldId, int newId)
    {
        Debug.Log("Commander ID changed from " + oldId + " to " + newId);
        if (newId != -1)
            SpawnManager.RegisterNewCommander(OwnerClientId, newId, this);
    }
    private IEnumerator DelayedStart()
    {
        yield return new WaitForEndOfFrame();
        transform.parent = SpawnManager.CommanderParent;

    }


    public void MakeActiveCommander()
    {
        if (IsOwner && ActiveCommander != this)
        {
            ActiveCommander = this;
            OnActiveCommanderChanged?.Invoke(OwnerClientId, ActiveCommander, this);
            Debug.Log("Made myself the active commander! " + gameObject.name);
        }
    }
    private void UpdateText()
    {
        gameObject.name = $"Commander {OwnerClientId}|{commanderIDVar.Value}";
        commanderNameText.text = $"{OwnerClientId}|{commanderIDVar.Value}";
        OnNameChanged?.Invoke(commanderNameText.text);
    }
    public override void OnGainedOwnership()
    {
        base.OnGainedOwnership();
        Debug.Log("Gained ownership of " + gameObject.name);
        if (IsOwner || (IsServer && IsOwner))
            OnOwnershipChanged?.Invoke(true);
    }
    public override void OnLostOwnership()
    {
        base.OnLostOwnership();
        Debug.Log("Lost ownership of " + gameObject.name);
        OnOwnershipChanged?.Invoke(false);
    }
    private void OnActiveCommanderChangedHandler(ulong playerID, CommanderController oldCommander, CommanderController newCommander)
    {

        if (playerID == NetworkManager.LocalClientId)
        {
            // on commander of own player changed
            if (oldCommander == this)
                UnsubscribeFromInputs();
            else if (newCommander == this)
                SubscribeToInputs();
        }
        else
        {
            // other player's commander selection changed
        }
        if (oldCommander == this)
            OwnActiveStateChanged?.Invoke(false);
        else if (newCommander == this)
            OwnActiveStateChanged?.Invoke(true);
    }

    private void OnSwitchCommanderInput(InputAction.CallbackContext context)
    {
        if (IsActiveCommander && context.performed)
        {
            float input = context.ReadValue<float>();
            if (input > 0)
            {
                // switch to next commander
                int newCommanderID = commanderIDVar.Value + 1;
                newCommanderID %= SpawnManager.SpawnSettings.CommanderCount;
                Debug.Log("Switching from " + commanderIDVar.Value + " to " + newCommanderID + "");

                SpawnManager.commanders[OwnerClientId][newCommanderID].MakeActiveCommander();
            }
            else
            {
                // switch to previous commander
                int newCommanderID = commanderIDVar.Value - 1;
                if (newCommanderID < 0)
                    newCommanderID = SpawnManager.SpawnSettings.CommanderCount - 1;
                SpawnManager.commanders[OwnerClientId][newCommanderID].MakeActiveCommander();
            }
        }
    }

    private void OnSwitchCommanderInputNumber(InputAction.CallbackContext context)
    {
        if (context.performed)
        {

            int input = 1;
            int.TryParse(context.control.name, out input);
            if (input == 0)
                input = 10;
            input--;
            if (input >= 0 && input < SpawnManager.SpawnSettings.CommanderCount)
            {
                SpawnManager.commanders[OwnerClientId][input].MakeActiveCommander();
            }
        }
    }

    public override void OnDestroy()
    {
        UnsubscribeFromInputs();
        base.OnDestroy();
    }

    private void SubscribeToInputs()
    {
        InputManager.OnCommanderSwitch += OnSwitchCommanderInput;
        InputManager.OnCommanderSwitchNumber += OnSwitchCommanderInputNumber;
    }

    private void UnsubscribeFromInputs()
    {
        InputManager.OnCommanderSwitch -= OnSwitchCommanderInput;
        InputManager.OnCommanderSwitchNumber -= OnSwitchCommanderInputNumber;
    }

    [ContextMenu("DebugPrintActiveCommanderName")]
    private void DebugPrintActiveCommanderName()
    {
        Debug.Log("Active commander is " + ActiveCommander.gameObject.name);
        Debug.Log("IsOwner: " + IsOwner);
    }
}