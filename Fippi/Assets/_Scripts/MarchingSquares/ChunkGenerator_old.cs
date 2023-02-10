using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkGenerator_old : MonoBehaviour
{
    public static ChunkGenerator_old Instance { get; private set; }
    [field: SerializeField] public ChunkSettings ChunkSettings { get; private set; }
    public static Action OnChunksGenerated;
    public MS_Chunk_old[,] Chunks;
    private Vector3 _gridZeroWorldPosition;
    public static Vector3 GridZeroWorldPosition => Instance._gridZeroWorldPosition;
    public int TileCount;
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
        StartCoroutine(GenerateChunks());
    }
    private IEnumerator GenerateChunks()
    {
        Chunks = new MS_Chunk_old[ChunkSettings.ChunksPerAxis, ChunkSettings.ChunksPerAxis];
        float chunkSize = ChunkSettings.TilesPerAxis * ChunkSettings.UnitSize;
        float offset = -ChunkSettings.ChunksPerAxis / 2 * chunkSize;
        _gridZeroWorldPosition = new Vector3(offset, offset, 0);
        for (int y = 0; y < ChunkSettings.ChunksPerAxis; y++)
        {
            for (int x = 0; x < ChunkSettings.ChunksPerAxis; x++)
            {
                GameObject chunkObject = new GameObject($"Chunk {x} {y}");
                chunkObject.transform.parent = transform;
                chunkObject.transform.localPosition = new(x * chunkSize + offset, y * chunkSize + offset, 0);
                MS_Chunk_old chunk = chunkObject.AddComponent<MS_Chunk_old>();
                Chunks[x, y] = chunk;
            }
            // wait a frame after each row
            yield return null;
        }
        for (int y = 0; y < ChunkSettings.ChunksPerAxis; y++)
        {
            for (int x = 0; x < ChunkSettings.ChunksPerAxis; x++)
            {
                FillDensitiesAtRandom(x, y);
                yield return null;
            }
        }
        FillOuterEdges();
        // FillDensitiesTestPattern();
        RecalculateAllChunksAsync();
    }

    public void FillDensitiesAtRandom(int chunkX, int chunkY)
    {
        int tileCount = ChunkSettings.TilesPerAxis;
        for (int y = 0; y < tileCount; y++)
        {
            for (int x = 0; x < tileCount; x++)
            {
                SetDensitiyAt(new Vector2Int(x + (chunkX * tileCount), y + (chunkY * tileCount)), UnityEngine.Random.Range(0, 2), false);
            }
        }
    }
    public void RecalculateAllChunksAsync()
    {
        StartCoroutine(RecalculateAllChunks());
    }

    public IEnumerator RecalculateAllChunks()
    {
        for (int y = 0; y < ChunkSettings.ChunksPerAxis; y++)
        {
            for (int x = 0; x < ChunkSettings.ChunksPerAxis; x++)
            {
                Chunks[x, y].RecalculateChunk();
            }
            // wait a frame after each row
            yield return null;
        }
        OnChunksGenerated?.Invoke();
    }

    private void FillOuterEdges()
    {
        int chunkCount = ChunkSettings.ChunksPerAxis;
        int tileCount = ChunkSettings.TilesPerAxis + 1;
        for (int i = 0; i < chunkCount; i++)
        {
            // top row
            int[,] currentDens = Chunks[i, chunkCount - 1].Densities;
            for (int j = 0; j < tileCount; j++)
                currentDens[j, tileCount - 1] = 1;
            // right column
            currentDens = Chunks[chunkCount - 1, i].Densities;
            for (int j = 0; j < tileCount; j++)
                currentDens[tileCount - 1, j] = 1;
            // bottom row
            currentDens = Chunks[i, 0].Densities;
            for (int j = 0; j < tileCount; j++)
                currentDens[j, 0] = 1;
            // left column
            currentDens = Chunks[0, i].Densities;
            for (int j = 0; j < tileCount; j++)
                currentDens[0, j] = 1;
        }

    }
    public static void SetDensitiyAt(Vector2Int pos, int value, bool recalculate = true)
    {
        var Chunks = Instance.Chunks;
        int tileCount = Instance.ChunkSettings.TilesPerAxis;
        int chunkX = pos.x / tileCount;
        int chunkY = pos.y / tileCount;
        int tileX = pos.x % tileCount;
        int tileY = pos.y % tileCount;
        // check if at left edge
        if (tileX == 0 && chunkX > 0)
            Chunks[chunkX - 1, chunkY].Densities[tileCount, tileY] = value;
        // check if at bottom edge
        if (tileY == 0 && chunkY > 0)
            Chunks[chunkX, chunkY - 1].Densities[tileX, tileCount] = value;
        // check if in Bottom left corner
        if (tileX == 0 && chunkX > 0 && tileY == 0 && chunkY > 0)
            Chunks[chunkX - 1, chunkY - 1].Densities[tileCount, tileCount] = value;
        // Debug.Log("Chunk Densities: " + Chunks[chunkX, chunkY].Densities);
        Chunks[chunkX, chunkY].Densities[tileX, tileY] = value;
        if (recalculate)
            Chunks[chunkX, chunkY].RecalculateChunk();
    }
    public static int GetAxisTotalPointCount()
    {
        return Instance.ChunkSettings.ChunksPerAxis * Instance.ChunkSettings.TilesPerAxis;
    }
    public static int GetDensityAt(Vector2Int pos)
    {
        var Chunks = Instance.Chunks;
        int tileCount = Instance.ChunkSettings.TilesPerAxis;
        int chunkX = pos.x / tileCount;
        int chunkY = pos.y / tileCount;
        int tileX = pos.x % tileCount;
        int tileY = pos.y % tileCount;
        return Chunks[chunkX, chunkY].Densities[tileX, tileY];
    }

    public static Vector2Int GetIndexFromPos(Vector2 pos, bool CutToChunk = false)
    {
        var Chunks = Instance.Chunks;
        var settings = Instance.ChunkSettings;
        int tileCount = settings.TilesPerAxis;
        int chunkCount = settings.ChunksPerAxis;
        int tileX = Mathf.FloorToInt((pos.x + GridZeroWorldPosition.x));
        int tileY = Mathf.FloorToInt((pos.y + GridZeroWorldPosition.y));
        int maxTileX = tileCount * chunkCount;
        if(tileX > settings.ChunksPerAxis * settings.TilesPerAxis || tileY > settings.ChunksPerAxis * settings.TilesPerAxis)
            return new Vector2Int(-1, -1);
        return new Vector2Int(tileX, tileY);
    }

    public struct PointInfo
    {}
}
