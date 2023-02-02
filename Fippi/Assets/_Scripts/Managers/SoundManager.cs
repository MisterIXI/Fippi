using UnityEngine;

public class SoundManager : MonoBehaviour {
    private void Awake() {
        if(RefManager.soundManager != null)
        {
            Destroy(gameObject);
            return;
        }
        RefManager.soundManager = this;
    }
}