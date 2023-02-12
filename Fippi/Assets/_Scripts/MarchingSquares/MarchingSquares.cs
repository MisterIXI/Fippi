using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
public class MarchingSquares : MonoBehaviour
{
    [field: SerializeField] public ChunkSettings chunkSettings { get; private set; } = null;
    public static int[,,] WallInfo { get; private set; }
    public static MarchingSquares Instance { get; private set; }
    public static MS_Chunk[,] Chunks { get; private set; }
    private int _chunkCount => chunkSettings.ChunksPerAxis;
    private int _tileCount => chunkSettings.TilesPerAxis;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        WallInfo = MapGenTools.CreateMapArray();
        MapGenTools.FillMapAtRandom();
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
        float chunkAxisLength = _tileCount * chunkSettings.UnitSize;
        Vector3 offset = new Vector3(-(chunkAxisLength) * _chunkCount / 2f, -(chunkAxisLength) * _chunkCount / 2f, 0);
        for (int y = 0; y < _chunkCount; y++)
        {
            for (int x = 0; x < _chunkCount; x++)
            {
                GameObject chunk = new GameObject($"Chunk {x} {y}");
                chunk.transform.parent = transform;
                chunk.transform.localPosition = offset + new Vector3(x * chunkAxisLength, y * chunkAxisLength, 0);
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
    [ContextMenu("Generate Cave Walls")]
    public void GenerateCaveWalls()
    {
        WallInfo = MapGenTools.GenerateCaveWalls(1);
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
