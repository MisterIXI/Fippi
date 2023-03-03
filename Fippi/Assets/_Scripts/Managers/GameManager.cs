using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameManager : NetworkBehaviour
{
    public static GameManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        SceneManager.activeSceneChanged += OnSceneChanged;
        DontDestroyOnLoad(gameObject);
    }
    private void Start()
    {

    }
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        Debug.Log("Game manager spawned");
        // MarchingSquares.Instance.DebugMapStart();
        // SpawnManager.SpawnOrTakeOverCommandersForPlayer(NetworkManager.LocalClientId);
        // SpawnManager.Instance.SpawnPlayer_ServerRpc();
    }
    private void OnSceneChanged(Scene current, Scene next)
    {
        if (next.name == "Game")
        {
            // Do something
        }
    }
}