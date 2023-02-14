using UnityEngine;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour {
    public static GameManager Instance { get; private set; }
    private void Awake() {
        if(Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        SceneManager.activeSceneChanged += OnSceneChanged;
        DontDestroyOnLoad(transform.parent.gameObject);
    }


    private void OnSceneChanged(Scene current, Scene next) {
        if (next.name == "Game") {
            // Do something
        }
    }
}