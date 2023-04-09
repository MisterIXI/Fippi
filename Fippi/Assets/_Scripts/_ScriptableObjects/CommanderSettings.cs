using UnityEngine;

[CreateAssetMenu(fileName = "CommanderSettings", menuName = "Fippi/CommanderSettings", order = 0)]
public class CommanderSettings : ScriptableObject
{
    [field: Header("Movement Settings")]
    [field: SerializeField][field: Range(0.1f, 10f)] public float MovementSpeed { get; private set; } = 1f;
    [field: SerializeField] public float PositionCheckOffset { get; private set; } = 0.5f;
    [field: Header("Collision Settings")]
    [field: SerializeField] public int CollisionCheckCountPerDirection { get; private set; } = 3;
    [field: Header("Commander Movement")]
    [field: SerializeField][field: Range(0.1f, 10f)] public float CMMoveSpeed { get; private set; } = 1f;
    [field: SerializeField][field: Range(0f, 1f)] public float CMInputChangeMax { get; private set; } = 0.5f;
    [field: Header("Swarm Settings")]
    [field: SerializeField][field: Range(0f, 10f)] public float SwarmCommanderDistance = 1f;
    [field: SerializeField][field: Range(0.01f, 2f)] public float SwarmIndiviualDistance { get; private set; } = 1f;
    [field: SerializeField]public float RecallDistance { get; private set; } = 10f;
    // [field: SerializeField][field: Header("TaskSettings")]
}