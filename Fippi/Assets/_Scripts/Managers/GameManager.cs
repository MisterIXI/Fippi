using UnityEngine;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour {
    
    private void Awake() {
        if(RefManager.gameManager != null)
        {
            Destroy(gameObject);
            return;
        }
        RefManager.gameManager = this;
        SceneManager.activeSceneChanged += OnSceneChanged;
        DontDestroyOnLoad(transform.parent.gameObject);
    }


    private void OnSceneChanged(Scene current, Scene next) {
        if (next.name == "Game") {
            // Do something
        }
    }
}