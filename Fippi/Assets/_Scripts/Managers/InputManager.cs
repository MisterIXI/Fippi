using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;
public class InputManager : MonoBehaviour {
    private void Awake() {
        if(RefManager.inputManager != null)
        {
            Destroy(gameObject);
            return;
        }
        RefManager.inputManager = this;
    }
}