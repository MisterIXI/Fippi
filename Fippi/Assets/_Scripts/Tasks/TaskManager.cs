using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
public class TaskManager : NetworkBehaviour
{
    public TaskManager Instance { get; private set; }
    private HashSet<Task>[,] _taskArray;
    public static HashSet<Task> Tasks { get; private set; } = new HashSet<Task>();
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        MarchingSquares.OnChunksGenerated += OnMapGenerated;
    }

    private void OnMapGenerated()
    {
        InitTaskArray();
    }

    private void InitTaskArray()
    {
        _taskArray = new HashSet<Task>[MarchingSquares.Chunks.GetLength(0), MarchingSquares.Chunks.GetLength(1)];
        for (int x = 0; x < MarchingSquares.Chunks.GetLength(0); x++)
        {
            for (int y = 0; y < MarchingSquares.Chunks.GetLength(1); y++)
            {
                _taskArray[x, y] = new HashSet<Task>();
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void CreateDrillTask_ServerRpc(Vector2Int[] wallsToDrill, Vector2 pos, ulong[] unitIDs)
    {
        DrillTask task = TaskPool.GetDrillTask(pos, new LinkedList<Vector2Int>(wallsToDrill));
        foreach (ulong unitID in unitIDs)
        {
            NetworkObject unitNetworkObject = SpawnManager.SpawnedObjects[unitID];
            unitNetworkObject.ChangeOwnership(NetworkManager.ServerClientId);
            var unitTaskController = unitNetworkObject.GetComponent<TaskController>();
            if (unitTaskController != null)
            {
                unitTaskController.SetTask(task);
            }
            else
            {
                Debug.LogError($"Unit with id: {unitID} does not have a TaskController");
            }
        }
    }
}