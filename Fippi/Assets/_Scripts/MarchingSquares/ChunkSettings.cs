using UnityEngine;

[CreateAssetMenu(fileName = "ChunkSettings", menuName = "Fippi/ChunkSettings", order = 0)]
public class ChunkSettings : ScriptableObject
{
    [Header("Chunk Settings")]
    public int TilesPerAxis = 50;
    public int ChunksPerAxis = 30;
    [Header("Chunk materials")]
    public Material FloorMaterial;
    public Material WallMaterial;
}