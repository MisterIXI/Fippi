using Unity.Netcode;
using UnityEngine;

[CreateAssetMenu(fileName = "SpawnSettings", menuName = "Fippi/SpawnSettings", order = 0)]
public class SpawnSettings : ScriptableObject
{
    [field: Header("Commander")]
    [field: SerializeField] public int CommanderCount { get; private set; } = 3;
    [field: SerializeField] public Color[] CommanderColors { get; private set; } = null;
    [field: SerializeField] public ColliderSettings CommanderColliderSettings { get; private set; } = null;
    [field: Space(10)]
    [field: Header("Prefabs")]
    [field: Header("Commander")]
    [field: SerializeField] public CommanderController CommanderPrefab { get; private set; } = null;
    [field: Space(10)]
    [field: Header("Worker Settings")]
    [field: SerializeField] public ColliderSettings UnitColliderSettings { get; private set; } = null;
    [field: Header("WorkerSettings")]
    [field: SerializeField] public UnitSettings BuilderSettings { get; private set; } = null;
    [field: SerializeField] public UnitSettings DiggerSettings { get; private set; } = null;
    [field: SerializeField] public UnitSettings ScoutSettings { get; private set; } = null;
    [field: Header("FighterSettings")]
    [field: SerializeField] public UnitSettings SoldierSettings { get; private set; } = null;
    [field: SerializeField] public UnitSettings RangedSettings { get; private set; } = null;
    [field: Header("WorkerUnits")]
    [field: SerializeField] public NetworkObject BuilderPrefab { get; private set; } = null;
    [field: SerializeField] public NetworkObject DiggerPrefab { get; private set; } = null;
    [field: SerializeField] public NetworkObject ScoutPrefab { get; private set; } = null;
    [field: Header("CombatUnits")]
    [field: SerializeField] public NetworkObject SoldierPrefab { get; private set; } = null;
    [field: SerializeField] public NetworkObject RangedPrefab { get; private set; } = null;
}