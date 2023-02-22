using UnityEngine;

[CreateAssetMenu(fileName = "SpawnSettings", menuName = "Fippi/SpawnSettings", order = 0)]
public class SpawnSettings : ScriptableObject
{
    [field: Header("Commander")]
    [field: SerializeField] public int CommanderCount { get; private set; } = 3;
    [field: SerializeField] public Color[] CommanderColors { get; private set; } = null;
    [field: Space(10)]
    [field: Header("Prefabs")]
    [field: Header("Commander")]
    [field: SerializeField] public CommanderController CommanderPrefab { get; private set; } = null;
    [field: Header("WorkerUnits")]
    [field: SerializeField] public GameObject BuilderPrefab { get; private set; } = null;
    [field: SerializeField] public GameObject DiggerPrefab { get; private set; } = null;
    [field: SerializeField] public GameObject ScoutPrefab { get; private set; } = null;
    [field: Header("CombatUnits")]
    [field: SerializeField] public GameObject SoldierPrefab { get; private set; } = null;
    [field: SerializeField] public GameObject RangedPrefab { get; private set; } = null;
}