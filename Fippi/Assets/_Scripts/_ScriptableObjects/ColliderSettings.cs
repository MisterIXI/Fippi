using UnityEngine;

[CreateAssetMenu(fileName = "ColliderSettings", menuName = "Fippi/ColliderSettings", order = 0)]
public class ColliderSettings : ScriptableObject
{
    [field: Header("Colliders")]
    [field: SerializeField] public int ColliderCountPerDirection { get; private set; } = 1;
    [field: Header("Debug")]
    [field: SerializeField] public bool DrawDebugGizmos;
}