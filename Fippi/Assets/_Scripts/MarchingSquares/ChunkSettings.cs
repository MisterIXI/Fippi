using UnityEngine;

[CreateAssetMenu(fileName = "ChunkSettings", menuName = "Fippi/ChunkSettings", order = 0)]
public class ChunkSettings : ScriptableObject
{
    [field: Header("Chunk Settings")]
    [field: SerializeField] public int TilesPerAxis { get; private set; } = 51;
    [field: SerializeField] public int ChunksPerAxis { get; private set; } = 11;
    [field: SerializeField] public float UnitSize { get; private set; } = 1f;
    [field: Header("Chunk materials")]
    [field: SerializeField] public Material FloorMaterial { get; private set; } = null;
    [field: SerializeField] public Material[] WallMaterials { get; private set; } = null;
    [field: Header("Chunk Generation")]
    [field: SerializeField] public int Seed { get; private set; } = 0;
    [field: SerializeField] public int ChunksPerFrame { get; private set; } = 10;
}