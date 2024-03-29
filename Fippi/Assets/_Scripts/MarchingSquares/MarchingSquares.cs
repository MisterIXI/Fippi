using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
public class MarchingSquares : MonoBehaviour
{
    public static Action OnChunksGenerated;
    public static bool IsReadyAndGenerated;
    /// <summary>
    /// Wallinfo is the big array that holds all the wall info for the map. [x,y,i] x,y is xy coords, i is: 0 = WallType, 1 = Density/health
    /// </summary>
    public static int[,,] WallInfo { get; private set; }
    public static MarchingSquares Instance { get; private set; }
    public static MS_Chunk[,] Chunks { get; private set; }
    [field: SerializeField] public ChunkSettings chunkSettings { get; private set; } = null;
    private static int _chunkCount => Instance.chunkSettings.ChunksPerAxis;
    private static int _tileCount => Instance.chunkSettings.TilesPerAxis;
    private static int _tileCountPerAxis => _chunkCount * _tileCount + 1;
    public static float TileLength => Instance.chunkSettings.UnitSize;
    private static float _axisLength => _tileCountPerAxis * TileLength;
    private static float _chunkAxisLength => _tileCount * TileLength;
    public static Action<Vector2Int, WallType, int> OnWallInfoChanged;
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
        DebugMapStart();
    }

    public void SetWallInfo(int[,,] wallInfo)
    {
        WallInfo = wallInfo;
        // OnWallInfoPointerChanged?.Invoke();
    }

    public void ChangeWallAt(Vector2Int index, WallType wallType, int value)
    {
        WallInfo[index.x, index.y, (int)wallType] = value;
        OnWallInfoChanged?.Invoke(index, wallType, value);
    }

    public void DebugMapStart()
    {
        // MapGenTools.FillEverythingSolid();
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
        IsReadyAndGenerated = false;
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
        IsReadyAndGenerated = true;
        OnChunksGenerated?.Invoke();
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
    public static Vector2Int GetChunkIndexFromPos(Vector2 position)
    {
        Vector2Int index = GetIndexFromPos(position);
        int x = Mathf.FloorToInt(index.x / (float)_tileCount);
        int y = Mathf.FloorToInt(index.y / (float)_tileCount);
        return new Vector2Int(x, y);
    }

    public static Vector2 GetPosFromChunkIndex(Vector2Int index)
    {
        float offset = _axisLength / 2f;
        float x = (index.x * _chunkAxisLength) - offset;
        float y = (index.y * _chunkAxisLength) - offset;
        return new Vector2(x, y);
    }
    public static bool IsIndexInBounds(Vector2Int index)
    {
        return index.x >= 0 && index.x < _tileCountPerAxis && index.y >= 0 && index.y < _tileCountPerAxis;
    }

    public static bool IsChunkIndexInBounds(Vector2Int index)
    {
        return index.x >= 0 && index.x < _chunkCount && index.y >= 0 && index.y < _chunkCount;
    }

    public static byte GetWallByte(Vector2 position)
    {
        Vector2Int index = GetIndexFromPos(position);
        byte points = 0;
        int x = index.x;
        int y = index.y;
        if (WallInfo[x, y, 0] > 0) points |= 0b1000;
        if (WallInfo[x + 1, y, 0] > 0) points |= 0b0100;
        if (WallInfo[x + 1, y + 1, 0] > 0) points |= 0b0010;
        if (WallInfo[x, y + 1, 0] > 0) points |= 0b0001;
        return points;
    }
    public static List<Vector2Int> GetSurroundingIDs(Vector2Int id, bool includeDiagonals = true, bool includeSelf = false, bool excludeCardinals = false)
    {
        List<Vector2Int> surroundingIDs = new List<Vector2Int>();
        for (int y = -1; y <= 1; y++)
        {
            for (int x = -1; x <= 1; x++)
            {
                if (x == 0 && y == 0) continue;
                Vector2Int surroundingID = new Vector2Int(id.x + x, id.y + y);
                if (IsIndexInBounds(surroundingID))
                {
                    if (excludeCardinals && (x == 0 || y == 0)) continue;
                    if (includeDiagonals || (x == 0 || y == 0))
                        surroundingIDs.Add(surroundingID);
                }
            }
        }
        return surroundingIDs;
    }

    public static List<Vector2Int> GetSurroundingChunkIDs(Vector2Int id, bool includeDiagonals = true, bool includeSelf = false, bool excludeCardinals = false)
    {
        List<Vector2Int> surroundingIDs = new List<Vector2Int>();
        for (int y = -1; y <= 1; y++)
        {
            for (int x = -1; x <= 1; x++)
            {
                if (!includeSelf && x == 0 && y == 0) continue;
                Vector2Int surroundingID = new Vector2Int(id.x + x, id.y + y);
                if (IsChunkIndexInBounds(surroundingID))
                {
                    if (excludeCardinals && (x == 0 || y == 0)) continue;
                    if (includeDiagonals || (x == 0 || y == 0))
                        surroundingIDs.Add(surroundingID);
                }
            }
        }
        return surroundingIDs;
    }
    public static bool IsPositionInWall(Vector2 position)
    {
        Vector2Int index = GetIndexFromPos(position);
        int wallValue = WallInfo[index.x, index.y, 0];
        return wallValue != (int)WallType.Wall && wallValue != (int)WallType.None;
    }
    [ContextMenu("Generate Cave Walls")]
    public void GenerateCaveWalls()
    {
        // WallInfo = MapGenTools.GenerateCaveWalls(1);
        MapGenTools.FillMapWithPerlinNoise(chunkSettings.PerlinScale);
        RefrehMapAsync();
    }
    public static Vector2? FindSafePathfindingAlternative(Vector2 position)
    {
        Vector2Int index = GetIndexFromPos(position);
        List<Vector2Int> diagonalNeighbours = GetSurroundingIDs(index, true, false, true);
        float minDistance = float.MaxValue;
        Vector2? closestNeighbour = null;
        foreach (Vector2Int neighbour in diagonalNeighbours)
        {
            Vector2 neighbourPos = GetPosFromIndex(neighbour);
            if (IsPositionInWall(neighbourPos)) continue;
            float distance = Vector2.Distance(position, neighbourPos);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestNeighbour = neighbourPos;
            }
        }
        return closestNeighbour;
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
