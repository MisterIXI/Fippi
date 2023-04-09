using UnityEngine;

[CreateAssetMenu(fileName = "PlayerSettings", menuName = "Fippi/PlayerSettings", order = 0)]
public class PlayerSettings : ScriptableObject
{
    [field: Header("Player Settings")]
    [Tooltip("The name of the player profile; this is restricted to A-Z, a-z, 0-9, and _")]
    [field: SerializeField] public string PlayerProfileName { get; private set; } = "PlayerDefaultName";
    [field: Header("Input Settings")]
    [field: Header("Cursor Settings")]
    [field: SerializeField] public bool CursorGizmos { get; private set; } = false;
    [field: SerializeField][field: Range(0f, 50f)] public float CursorRadius { get; private set; } = 10f;
}