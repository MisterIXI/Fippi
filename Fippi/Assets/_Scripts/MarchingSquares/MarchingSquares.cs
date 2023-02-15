using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
public class MarchingSquares : MonoBehaviour
{
    public static int[,,] WallInfo { get; private set; }
    public static MarchingSquares Instance { get; private set; }
    public static MS_Chunk[,] Chunks { get; private set; }
    public static event Action OnWallInfoPointerChanged;
    public static event Action OnWallInfoChanged;
    [field: SerializeField] public ChunkSettings chunkSettings { get; private set; } = null;
    private static int _chunkCount => Instance.chunkSettings.ChunksPerAxis;
    private static int _tileCount => Instance.chunkSettings.TilesPerAxis;
    private static int _tileCountPerAxis => _chunkCount * _tileCount + 1;
    public static float TileLength => Instance.chunkSettings.UnitSize;
    private static float _axisLength => _tileCountPerAxis * TileLength;
    private static float _chunkAxisLength => _tileCount * TileLength;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        WallInfo = MapGenTools.CreateMapArray();
    }

    private void Start()
    {
        MapGenTools.FillMapWithPerlinNoise(chunkSettings.PerlinScale);
        // MapGenTools.FillMapAtRandom();
        MapGenTools.FillMapEdgesWithSolidWall();
        GenerateChunksAsync();
    }


    public void RefrehMapAsync()
    {
        StartCoroutine(RefreshMap());
    }

    private IEnumerator RefreshMap()
    {
        int chunksThisFrame = 0;
        for (int y = 0; y < _chunkCount; y++)
        {
            for (int x = 0; x < _chunkCount; x++)
            {
                Chunks[x, y].RecalculateChunk();
                chunksThisFrame++;
                if (chunksThisFrame >= chunkSettings.ChunksPerFrame)
                {
                    chunksThisFrame = 0;
                    yield return null;
                }
            }
        }
    }
    public void GenerateChunksAsync()
    {
        if (WallInfo == null)
        {
            Debug.LogError("WallInfo is null, please generate a map first");
            return;
        }
        Chunks = new MS_Chunk[_chunkCount, _chunkCount];
        StartCoroutine(GenerateChunks());
    }

    private IEnumerator GenerateChunks()
    {
        int chunksThisFrame = 0;
        Vector3 offset = new(-(_chunkAxisLength) * (_chunkCount - 1) / 2f, -(_chunkAxisLength) * (_chunkCount - 1) / 2f, 0);
        for (int y = 0; y < _chunkCount; y++)
        {
            for (int x = 0; x < _chunkCount; x++)
            {
                GameObject chunk = new GameObject($"Chunk {x} {y}");
                chunk.transform.parent = transform;
                chunk.transform.localPosition = offset + new Vector3(x * _chunkAxisLength, y * _chunkAxisLength, 0);
                chunk.tag = "Wall";
                MS_Chunk msChunk = chunk.AddComponent<MS_Chunk>();
                msChunk.InitMesh(chunkSettings, new Vector2Int(x, y));
                msChunk.RecalculateChunk();
                Chunks[x, y] = msChunk;
                chunksThisFrame++;
                if (chunksThisFrame >= chunkSettings.ChunksPerFrame)
                {
                    chunksThisFrame = 0;
                    yield return null;
                }
            }
        }
    }
    public static Vector2Int GetIndexFromPos(Vector2 position)
    {
        // calculate offset to move position to positive quadrant
        float offset = _axisLength / 2f;
        int x = Mathf.RoundToInt((position.x + offset) / TileLength);
        int y = Mathf.RoundToInt((position.y + offset) / TileLength);
        return new Vector2Int(x, y);
    }

    public static Vector2 GetPosFromIndex(Vector2Int index)
    {
        float offset = _axisLength / 2f;
        float x = (index.x * TileLength) - offset;
        float y = (index.y * TileLength) - offset;
        return new Vector2(x, y);
    }

    public static bool IsIndexInBounds(Vector2Int index)
    {
        return index.x >= 0 && index.x < _tileCountPerAxis && index.y >= 0 && index.y < _tileCountPerAxis;
    }

    [ContextMenu("Generate Cave Walls")]
    public void GenerateCaveWalls()
    {
        // WallInfo = MapGenTools.GenerateCaveWalls(1);
        MapGenTools.FillMapWithPerlinNoise(chunkSettings.PerlinScale);
        RefrehMapAsync();
    }

    [ContextMenu("Regenerate Mesh With Random Seed")]
    public void RegenerateMeshWithRandomSeed()
    {
        MapGenTools.FillMapAtRandom(true);
        MapGenTools.FillMapEdgesWithSolidWall();
        RefrehMapAsync();
    }


    #region Debug
    [ContextMenu("Debug: Print WallInfo Length")] public void PrintWallInfoLength() => Debug.Log($"WallInfo Length: {WallInfo?.GetLength(0)}, {WallInfo?.GetLength(1)}, {WallInfo?.GetLength(2)}");

    #endregion
}
