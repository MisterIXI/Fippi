using UnityEngine;

[CreateAssetMenu(fileName = "PoolSetttings", menuName = "Fippi/PoolSetttings", order = 0)]
public class PoolSetttings : ScriptableObject
{
    [field: Header("Task Pool")]
    [field: SerializeField] public int TaskPoolSize_Drill { get; private set; } = 10;
    [field: SerializeField] public int TaskPoolSize_Build { get; private set; } = 10;
    [field: SerializeField] public int TaskPoolSize_Move { get; private set; } = 10;
}