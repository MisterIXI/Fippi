using UnityEngine;

[CreateAssetMenu(fileName = "PathfindingSettings", menuName = "Fippi/PathfindingSettings", order = 0)]
public class PathfindingSettings : ScriptableObject
{
    [field: Header("Pathfinding Settings")]
    [field: Range(50, 500)] public int StepsPerFrame { get; private set; } = 150;
}