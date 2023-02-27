using UnityEngine;

[CreateAssetMenu(fileName = "UnitSettings", menuName = "Fippi/UnitSettings", order = 0)]
public class UnitSettings : ScriptableObject
{
    [field: Header("Movement")]
    [field: SerializeField][field: Range(0f, 10f)] public float MovementSpeed { get; private set; } = 0.5f;
    [field: Header("Flocking")]
    [field: SerializeField] public float FlockingMaxDistance { get; private set; } = 3f;
    [field: SerializeField] public float FlockingPathingDistance { get; private set; } = 3f;
    [field: SerializeField] public float FlockingRegainDistance { get; private set; } = 1f;
    [field: SerializeField][field: Range(0f, 2f)] public float CenterAttraction { get; private set; } = 0.5f;
    [field: SerializeField][field: Range(0f, 2f)] public float UnitRepulsion { get; private set; } = 0.5f;
    [field: SerializeField][field: Range(0f, 2f)] public float ObstacleRepulsion { get; private set; } = 0.5f;
    [field: Header("Pathing")]
    [field: SerializeField][field: Range(0.5f, 3f)] public float PathingMovementMultiplier { get; private set; } = 1.2f;
    [field: SerializeField] public float RepathingDistance { get; private set; } = 5f;
    [field: Header("Debug")]
    [field: SerializeField] public bool DrawDebugGizmos;
}