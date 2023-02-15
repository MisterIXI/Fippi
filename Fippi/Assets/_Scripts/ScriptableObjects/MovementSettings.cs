using UnityEngine;

[CreateAssetMenu(fileName = "MovementSettings", menuName = "Fippi/MovementSettings", order = 0)]
public class MovementSettings : ScriptableObject
{
    [field: Header("Movement Settings")]
    [field: SerializeField][field: Range(0.1f, 3f)] public float MovementSpeed { get; private set; } = 1f;
    [field: SerializeField] public float PositionCheckOffset { get; private set; } = 0.5f;
}