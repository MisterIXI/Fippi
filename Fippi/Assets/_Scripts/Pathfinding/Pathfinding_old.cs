using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding_old : MonoBehaviour
{
    [field: SerializeField] public PathfindingSettings PathfindingSettings { get; private set; }
    public static int[,] Grid { get; private set; }
    public static Pathfinding_old Instance { get; private set; }


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
        // ChunkGenerator.Instance.Chunks;
        ChunkGenerator_old.OnChunksGenerated += CalculateWeightGrid;
    }


    public void CalculateWeightGrid()
    {
        int chunkCount = ChunkGenerator_old.Instance.ChunkSettings.ChunksPerAxis;
        int tileCount = ChunkGenerator_old.Instance.ChunkSettings.TilesPerAxis;

        Grid = new int[chunkCount * tileCount, chunkCount * tileCount];
        for (int y = 0; y < chunkCount; y++)
        {
            for (int x = 0; x < chunkCount; x++)
            {
                MS_Chunk_old chunk = ChunkGenerator_old.Instance.Chunks[x, y];
                for (int tileY = 0; tileY < tileCount; tileY++)
                {
                    for (int tileX = 0; tileX < tileCount; tileX++)
                    {
                        int gridX = x * tileCount + tileX;
                        int gridY = y * tileCount + tileY;
                        Grid[gridX, gridY] = chunk.Densities[tileX, tileY];
                    }
                }
            }
        }
    }

    public static void FindPath(Vector2Int start, Vector2Int end, Action<List<Vector2>> action)
    {
        if (Grid == null)
        {
            Debug.LogError("Pathfinding grid is still null");
            return;
        }
        Instance.StartCoroutine(FindPathAsync(start, end, action));
        Debug.Log("Finding path...");
    }

    static List<Vector2Int> openSet;
    static HashSet<Vector2Int> closedSet;
    private static IEnumerator FindPathAsync(Vector2Int start, Vector2Int end, Action<List<Vector2>> action)
    {
        int steps = 0;
        int StepsPerFrame = Instance.PathfindingSettings.StepsPerFrame;

        // a* pathfinding 
        openSet = new List<Vector2Int>();
        closedSet = new HashSet<Vector2Int>();
        openSet.Add(start);
        Dictionary<Vector2Int, Vector2Int> cameFrom = new Dictionary<Vector2Int, Vector2Int>();
        Dictionary<Vector2Int, int> gScore = new Dictionary<Vector2Int, int>();
        Dictionary<Vector2Int, int> fScore = new Dictionary<Vector2Int, int>();
        gScore[start] = 0;
        fScore[start] = gScore[start] + ManhattanDistance(start, end);

        while (openSet.Count > 0)
        {
            Vector2Int current = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (fScore[openSet[i]] < fScore[current])
                {
                    current = openSet[i];
                }
            }

            if (current == end)
            {
                List<Vector2> path = ReconstructPath(start, end, cameFrom);
                action?.Invoke(path);
                Debug.Log("Path found!");
                // print path
                string pathString = "";
                foreach (Vector2 node in path)
                {
                    pathString += node + " -> ";
                }
                Debug.Log(pathString);
                yield break;
            }

            openSet.Remove(current);
            closedSet.Add(current);

            foreach (Vector2Int neighbor in GetNeighbors(current, Grid))
            {
                if (closedSet.Contains(neighbor))
                {
                    continue;
                }

                int tentativeGScore = gScore[current] + ManhattanDistance(current, neighbor);
                if (!openSet.Contains(neighbor) )
                {
                    openSet.Add(neighbor);
                }
                else if (tentativeGScore >= gScore[neighbor])
                {
                    continue;
                }

                cameFrom[neighbor] = current;
                gScore[neighbor] = tentativeGScore;
                fScore[neighbor] = gScore[neighbor] + ManhattanDistance(neighbor, end);
            }

            steps++;
            if (steps >= StepsPerFrame)
            {
                steps = 0;
                yield return new WaitForSeconds(0.5f);
            }
        }
        Debug.LogWarning("Path not found");
        yield return null;
    }

    private static int ManhattanDistance(Vector2Int a, Vector2Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

    private static List<Vector2Int> GetNeighbors(Vector2Int pos, int[,] grid = null)
    {
        List<Vector2Int> neighbors = new List<Vector2Int>();
        if (grid != null && pos.x < grid.GetLength(0) - 1)
        {
            neighbors.Add(new Vector2Int(pos.x + 1, pos.y));
        }
        if (grid != null && pos.x > 0)
        {
            neighbors.Add(new Vector2Int(pos.x - 1, pos.y));
        }
        if (grid != null && pos.y < grid.GetLength(1) - 1)
        {
            neighbors.Add(new Vector2Int(pos.x, pos.y + 1));
        }
        if (grid != null && pos.y > 0)
        {
            neighbors.Add(new Vector2Int(pos.x, pos.y - 1));
        }
        return neighbors;
    }


    private static List<Vector2> ReconstructPath(Vector2Int start, Vector2Int end, Dictionary<Vector2Int, Vector2Int> cameFrom)
    {
        List<Vector2> path = new List<Vector2>();
        Vector2Int current = end;
        while (current != start)
        {
            path.Add(current);
            current = cameFrom[current];
        }
        path.Reverse();
        return path;
    }
    private void OnDrawGizmos()
    {
        if (closedSet != null && openSet != null)
        {
            Gizmos.color = Color.red;
            foreach (Vector2Int node in closedSet)
            {
                Gizmos.DrawCube(node + (Vector2)ChunkGenerator_old.GridZeroWorldPosition, Vector3.one * 0.5f);
            }
            Gizmos.color = Color.green;
            foreach (Vector2Int node in openSet)
            {
                Gizmos.DrawCube(node + (Vector2)ChunkGenerator_old.GridZeroWorldPosition, Vector3.one * 0.5f);
            }
        }
     }
}