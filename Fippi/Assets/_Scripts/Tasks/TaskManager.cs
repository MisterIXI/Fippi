using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
public class TaskManager : NetworkBehaviour
{
    public TaskManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    [ServerRpc(RequireOwnership = false)]
    public void CreateDrillTask_ServerRpc(Vector2Int[] wallsToDrill, Vector2 pos, ulong[] unitIDs)
    {
        DrillTask task = new DrillTask(pos, new LinkedList<Vector2Int>(wallsToDrill));
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