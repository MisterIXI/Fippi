using TMPro;
using UnityEngine;
public class VisualUpdater : MonoBehaviour
{
    [SerializeField] private CommanderController _commanderController;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    // [SerializeField] private TextMeshPro _textMeshPro;
    [ContextMenu("UpdateVisuals")]
    public void UpdateVisuals()
    {
        if (_commanderController == null)
        {
            Debug.LogWarning("CommanderController not found, trying to get it from parent");
            _commanderController = GetComponentInParent<CommanderController>();
        }
        var spawnSettings = SpawnManager.Instance.spawnSettings;
        Color tint = spawnSettings.CommanderColors[(int)_commanderController.OwnerClientId % spawnSettings.CommanderColors.Length];
        _spriteRenderer.color = tint;
    }
}