using UnityEngine;

[CreateAssetMenu(fileName = "MovementSettings", menuName = "Fippi/MovementSettings", order = 0)]
public class MovementSettings : ScriptableObject
{
    [field: Header("Movement Settings")]
    [field: SerializeField][field: Range(0.1f, 10f)] public float MovementSpeed { get; private set; } = 1f;
    [field: SerializeField] public float PositionCheckOffset { get; private set; } = 0.5f;
    [field: Header("Collision Settings")]
    [field: SerializeField] public int CollisionCheckCountPerDirection { get; private set; } = 3;
    [field: Header("Commander Movement")]
    [field: SerializeField][field: Range(0.1f, 10f)] public float CMMoveSpeed { get; private set; } = 1f;
    [field: SerializeField][field: Range(0f, 1f)] public float CMInputChangeMax { get; private set; } = 0.5f;
}