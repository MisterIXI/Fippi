using Unity.Netcode;
using UnityEngine;

public class MS_NetworkSync : NetworkBehaviour
{
    private MarchingSquares _marchingSquares;
    public static MS_NetworkSync Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        _marchingSquares = GetComponent<MarchingSquares>();
    }
    [ServerRpc(RequireOwnership = false)]
    public void ChangeWallInfo_ServerRpc(Vector2Int index, WallType wallType, int value)
    {
        _marchingSquares.ChangeWallAt(index, wallType, value);
        ChangeWallInfo_ClientRpc(index, wallType, value);
    }
    [ClientRpc]
    public void ChangeWallInfo_ClientRpc(Vector2Int index, WallType wallType, int value)
    {
        if (!IsServer)
            _marchingSquares.ChangeWallAt(index, wallType, value);
    }

    [ClientRpc]
    public void ReceiveWallInfo_ClientRpc(int[,,] wallInfo, ClientRpcParams clientRpcParams = default)
    {
        _marchingSquares.SetWallInfo(wallInfo);
        _marchingSquares.GenerateChunksAsync();
    }
}