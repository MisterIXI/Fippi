using System;
using UnityEngine;

class MapGenTools
{
    private static ChunkSettings _chunkSettings => MarchingSquares.Instance.chunkSettings;
    private static int[,,] _wallInfo
    {
        get
        {
            if (MarchingSquares.WallInfo == null)
            {
                Debug.LogError("WallInfo is null");
                throw new NullReferenceException();
            }
            return MarchingSquares.WallInfo;
        }
    }

    public static int[,,] CreateMapArray()
    {
        int _tileCount = _chunkSettings.TilesPerAxis;
        int _chunkCount = _chunkSettings.ChunksPerAxis;
        return new int[_tileCount * _chunkCount + 1, _tileCount * _chunkCount + 1, 2];
    }

    public static void FillMapEdgesWithSolidWall(int width = 1)
    {
        int[,,] wallInfo = _wallInfo;
        int length = wallInfo.GetLength(0);
        for (int y = 0; y < length; y++)
        {
            for (int x = 0; x < length; x++)
            {
                if (x < width || x == length - width || y < width || y == length - width)
                {
                    wallInfo[x, y, 0] = 1;
                    wallInfo[x, y, 1] = 1;
                }
            }
        }
    }

    public static int[,,] GenerateCaveWalls(int iterations = 1, int wallThreshold = 4, int floorThreshold = 7)
    {
        int[,,] wallInfo = _wallInfo;
        int[,,] wallInfoClone = new int[wallInfo.GetLength(0), wallInfo.GetLength(1), wallInfo.GetLength(2)];
        int length = wallInfo.GetLength(0);
        for (int i = 0; i < iterations; i++)
        {
            for (int y = 0; y < length; y++)
            {
                for (int x = 0; x < length; x++)
                {
                    int wallCount = GetSurroundingWallCount(x, y);
                    if (wallCount > wallThreshold)
                        wallInfoClone[x, y, 0] = 1;
                    else if (wallCount < floorThreshold)
                    {
                        wallInfoClone[x, y, 0] = 0;
                        wallInfoClone[x, y, 1] = 0;
                    }
                }
            }
        }
        return wallInfoClone;
    }

    public static void FillMapAtRandom(bool forceRandomSeed = false)
    {
        var seed = _chunkSettings.Seed;
        if (forceRandomSeed || seed == 0)
            seed = Guid.NewGuid().GetHashCode();
        var rand = new System.Random(seed);
        int[,,] wallInfo = _wallInfo;
        int length = wallInfo.GetLength(0);
        for (int y = 0; y < length; y++)
        {
            for (int x = 0; x < length; x++)
            {
                wallInfo[x, y, 0] = rand.Next(0, 2);
                wallInfo[x, y, 1] = rand.Next(0, _chunkSettings.WallMaterials.Length);
            }
        }
    }

    #region Helper Methods
    private static int GetSurroundingWallCount(int gridX, int gridY)
    {
        int wallCount = 0;
        for (int neighbourY = gridY - 1; neighbourY <= gridY + 1; neighbourY++)
        {
            for (int neighbourX = gridX - 1; neighbourX <= gridX + 1; neighbourX++)
            {
                if (IsInMapRange(neighbourX, neighbourY))
                {
                    if (neighbourX != gridX || neighbourY != gridY)
                    {
                        wallCount += (int)Mathf.Clamp01(_wallInfo[neighbourX, neighbourY, 0]);
                    }
                }
                else
                {
                    wallCount++;
                }
            }
        }
        return wallCount;
    }

    private static bool IsInMapRange(int x, int y)
    {
        return x >= 0 && x < _wallInfo.GetLength(0) && y >= 0 && y < _wallInfo.GetLength(1);
    }
    #endregion
}