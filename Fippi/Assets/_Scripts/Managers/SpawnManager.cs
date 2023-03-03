using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
public class SpawnManager : NetworkBehaviour
{
    [field: SerializeField] public SpawnSettings spawnSettings { get; private set; } = null;
    public static SpawnSettings SpawnSettings => Instance.spawnSettings;
    [field: SerializeField] public CommanderSettings movementSettings { get; private set; } = null;
    public static CommanderSettings CommanderSettings => Instance.movementSettings;
    public static Dictionary<ulong, List<CommanderController>> commanders = new Dictionary<ulong, List<CommanderController>>();
    [field: SerializeField] private Transform _colliderParent;
    public static Transform ColliderParent => Instance._colliderParent;
    [field: SerializeField] private Transform _commanderParent;
    public static Transform CommanderParent => Instance._commanderParent;
    public static Dictionary<Vector2Int, HashSet<UnitController>> UnitsByChunk { get; private set; } = new Dictionary<Vector2Int, HashSet<UnitController>>();
    public static Dictionary<ulong, NetworkObject> SpawnedObjects = new Dictionary<ulong, NetworkObject>();
    public static SpawnManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        MarchingSquares.OnChunksGenerated += OnChunksGenerated;
    }

    private void OnChunksGenerated()
    {
        UnitsByChunk = new Dictionary<Vector2Int, HashSet<UnitController>>();
        foreach (var chunk in MarchingSquares.Chunks)
        {
            UnitsByChunk.Add(chunk.ID, new HashSet<UnitController>());
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void SpawnPlayer_ServerRpc(ServerRpcParams serverRpcParams = default)
    {
        ulong clientID = serverRpcParams.Receive.SenderClientId;
        Debug.Log("Spawning player " + clientID);
        SpawnOrTakeOverCommandersForPlayer(clientID);
        SpawnDiggers();
    }
    public static void SpawnOrTakeOverCommandersForPlayer(ulong playerID)
    {
        if (!commanders.ContainsKey(playerID))
        {
            // commanders[playerID] = new List<CommanderController>();
            int commanderCount = Instance.spawnSettings.CommanderCount;
            for (int i = 0; i < commanderCount; i++)
            {
                var commanderObject = SpawnNetworkBehaviourForPlayerAtPos(Instance.spawnSettings.CommanderPrefab.networkObject, Vector3.right * i * 2, playerID);
                // CommanderController commander = commanderObject.GetComponent<CommanderController>();
                // commanders[playerID].Add(commander);
            }
            // commanders[playerID][0].MakeActiveCommander();
        }
        else
        {
            Debug.LogError("Player already has commanders");
        }
    }

    public static void RegisterNewCommander(ulong ownerID, int commanderID, CommanderController commander)
    {
        if (!commanders.ContainsKey(ownerID))
        {
            commanders[ownerID] = new List<CommanderController>();
        }
        commanders[ownerID].Remove(commander);
        commanders[ownerID].Insert(commanderID, commander);
    }
    public static void SpawnDiggers()
    {
        for (int i = 0; i < SpawnSettings.DiggerCount; i++)
        {
            var digger = SpawnNetworkObjectAtPos(Instance.spawnSettings.DiggerPrefab, Vector3.right * i * 2);
        }
    }
    public static NetworkObject SpawnNetworkBehaviourForPlayerAtPos(NetworkObject networkObject, Vector3 position, ulong playerID)
    {
        NetworkObject spawnedObj = Instantiate<NetworkObject>(networkObject, position, Quaternion.identity);
        Debug.Log("Spawning " + spawnedObj.name + " for player " + playerID);
        spawnedObj.SpawnWithOwnership(playerID);
        spawnedObj.ChangeOwnership(playerID);
        SpawnedObjects.Add(spawnedObj.NetworkObjectId, spawnedObj);
        return spawnedObj;
    }

    public static NetworkObject SpawnNetworkObjectAtPos(NetworkObject networkObject, Vector3 position)
    {
        NetworkObject spawnedObj = Instantiate<NetworkObject>(networkObject, position, Quaternion.identity);
        Debug.Log("Spawning " + spawnedObj.name);
        spawnedObj.Spawn();
        SpawnedObjects.Add(spawnedObj.NetworkObjectId, spawnedObj);
        return spawnedObj;
    }
    [ContextMenu("Print Commander Dict")]
    private void DebugCommanderDictPrint()
    {
        Debug.Log("Commander Dict:");
        foreach (var kvp in commanders)
        {
            string commanderNames = "";
            foreach (var commander in kvp.Value)
            {
                commanderNames += commander.name + ", ";
            }
            Debug.Log("Player " + kvp.Key + " has " + kvp.Value.Count + " commanders: " + commanderNames);
        }
    }

    public static UnitSettings GetWorkerSettings(WorkerTypes workerType)
    {
        switch (workerType)
        {
            case WorkerTypes.Builder:
                return Instance.spawnSettings.BuilderSettings;
            case WorkerTypes.Digger:
                return Instance.spawnSettings.DiggerSettings;
            case WorkerTypes.Scout:
                return Instance.spawnSettings.ScoutSettings;
            default:
                return null;
        }
    }

    public static UnitSettings GetFighterSettings(FighterTypes fighterType)
    {
        switch (fighterType)
        {
            case FighterTypes.Soldier:
                return Instance.spawnSettings.SoldierSettings;
            case FighterTypes.Ranged:
                return Instance.spawnSettings.RangedSettings;
            default:
                return null;
        }
    }

    private void OnDrawGizmos()
    {
        if (CommanderController.activeCommander != null)
            Gizmos.DrawWireSphere(CommanderController.activeCommander.transform.position, 1f);
    }
}