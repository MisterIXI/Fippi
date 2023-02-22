using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
public class CommanderController : NetworkBehaviour
{
    public NetworkObject networkObject => gameObject.GetComponent<NetworkObject>();
    public static CommanderController activeCommander { get; private set; }
    public static event Action<ulong, CommanderController, CommanderController> OnActiveCommanderChanged;
    public NetworkVariable<int> commanderIDVar = new NetworkVariable<int>(-1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    [field: SerializeField] private TextMeshPro commanderNameText;
    public ulong playerID { get; private set; }
    private event Action<bool> OwnActiveStateChanged;
    public bool IsActive => activeCommander == this;
    public event Action<bool> OnOwnershipChanged;
    public event Action<string> OnNameChanged;
    public static int owner_id_helper = 0;
    [field: SerializeField] public VisualUpdater VisualUpdater { get; private set; }
    private void Awake()
    {

        OnActiveCommanderChanged += OnActiveCommanderChangedHandler;
        commanderIDVar.OnValueChanged += (oldValue, newValue) => UpdateText();
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

    private IEnumerator DelayedStart()
    {
        yield return new WaitForEndOfFrame();
        transform.parent = SpawnManager.CommanderParent;

    }


    public void MakeActiveCommander()
    {
        if (IsOwner)
        {
            OnActiveCommanderChanged?.Invoke(playerID, activeCommander, this);
            activeCommander = this;
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
        if (oldCommander == this)
            OwnActiveStateChanged?.Invoke(false);
        else if (newCommander == this)
            OwnActiveStateChanged?.Invoke(true);

        if (playerID == NetworkManager.LocalClientId)
        {
            // on commander of own player changed
        }
        else
        {
            // other player's commander selection changed
        }
    }

    [ContextMenu("DebugPrintActiveCommanderName")]
    private void DebugPrintActiveCommanderName()
    {
        Debug.Log("Active commander is " + activeCommander.gameObject.name);
        Debug.Log("IsOwner: " + IsOwner);
    }
}