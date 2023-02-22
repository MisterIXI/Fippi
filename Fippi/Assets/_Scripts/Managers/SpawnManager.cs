using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
public class SpawnManager : NetworkBehaviour
{

    [field: SerializeField] public SpawnSettings spawnSettings { get; private set; } = null;
    [field: SerializeField] public MovementSettings movementSettings { get; private set; } = null;
    public static MovementSettings MovementSettings => Instance.movementSettings;
    public static Dictionary<ulong, List<CommanderController>> commanders = new Dictionary<ulong, List<CommanderController>>();
    [field: SerializeField] private Transform _colliderParent;
    public static Transform ColliderParent => Instance._colliderParent;
    [field: SerializeField] private Transform _commanderParent;
    public static Transform CommanderParent => Instance._commanderParent;

    public static SpawnManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
    }

    [ServerRpc(RequireOwnership = false)]
    public void SpawnPlayer_ServerRpc(ServerRpcParams serverRpcParams = default)
    {
        ulong clientID = serverRpcParams.Receive.SenderClientId;
        Debug.Log("Spawning player " + clientID);
        SpawnOrTakeOverCommandersForPlayer(clientID);
    }
    public static void SpawnOrTakeOverCommandersForPlayer(ulong playerID)
    {
        if (!commanders.ContainsKey(playerID))
        {
            commanders[playerID] = new List<CommanderController>();
            int commanderCount = Instance.spawnSettings.CommanderCount;
            for (int i = 0; i < commanderCount; i++)
            {
                var commanderObject = SpawnNetworkBehaviourForPlayerAtPos(Instance.spawnSettings.CommanderPrefab.networkObject, Vector3.right * i * 2, playerID);
                CommanderController commander = commanderObject.GetComponent<CommanderController>();
                commanders[playerID].Add(commander);
            }
            // commanders[playerID][0].MakeActiveCommander();
        }
        else
        {
            Debug.LogError("Player already has commanders");
        }
    }

    public static NetworkObject SpawnNetworkBehaviourForPlayerAtPos(NetworkObject networkObject, Vector3 position, ulong playerID)
    {
        NetworkObject spawnedObj = Instantiate<NetworkObject>(networkObject, position, Quaternion.identity);
        Debug.Log("Spawning " + spawnedObj.name + " for player " + playerID);
        spawnedObj.SpawnWithOwnership(playerID);
        spawnedObj.ChangeOwnership(playerID);
        return spawnedObj;
    }

    private void OnDrawGizmos()
    {
        if (CommanderController.activeCommander != null)
            Gizmos.DrawWireSphere(CommanderController.activeCommander.transform.position, 1f);
    }
}