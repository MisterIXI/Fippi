using Unity.Netcode;

using UnityEngine;

public class PlayerManager : NetworkBehaviour
{
    public static PlayerManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        if (IsServer)
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsServer)
        {
            SpawnManager.Instance.SpawnPlayer_ServerRpc();
        }
    }
    private void OnClientConnected(ulong clientId)
    {
        Debug.Log($"Client with id: {clientId} connected");
        if (IsServer)
        {
            var networkParams = new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new ulong[] { clientId }
                }
            };
            MS_NetworkSync.Instance.ReceiveWallInfo_ClientRpc(MarchingSquares.WallInfo, networkParams);
            SpawnManager.SpawnOrTakeOverCommandersForPlayer(clientId);
        }
    }

}