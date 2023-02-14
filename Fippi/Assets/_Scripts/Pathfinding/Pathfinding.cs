using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
public class Pathfinding : MonoBehaviour
{
    [SerializeField] public bool PathfindingSignal = false;
    [SerializeField] public bool UseConsistentHeuristic = true;
    [Range(0.01f, 0.1f)] public float RenderDelay = 0.05f;
    public static Pathfinding Instance { get; private set; }
    private static bool _searchingPath = false;
    private HashSet<Vector2Int> _closedSet;
    private HashSet<Vector2Int> _openSet;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }


    public static void FindPath(Vector2 start, Vector2 end, Action<LinkedList<Vector2>> callback)
    {
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        // a* pathfinding
        PriorityQueue<Vector2Int> openSetQueue = new PriorityQueue<Vector2Int>();
        HashSet<Vector2Int> openSet = new HashSet<Vector2Int>();
        HashSet<Vector2Int> closedSet = new HashSet<Vector2Int>();
        Instance._openSet = openSet;
        Instance._closedSet = closedSet;
        Dictionary<Vector2Int, Vector2Int> cameFrom = new Dictionary<Vector2Int, Vector2Int>();
        Dictionary<Vector2Int, float> gScore = new Dictionary<Vector2Int, float>();
        Dictionary<Vector2Int, float> fScore = new Dictionary<Vector2Int, float>();
        Vector2Int startInt = MarchingSquares.GetIndexFromPos(start);
        Vector2Int endInt = MarchingSquares.GetIndexFromPos(end);
        // Debug.Log("Pathfinding started with start: " + startInt + " and end: " + endInt);

        openSetQueue.Enqueue(startInt, 0);
        openSet.Add(startInt);
        gScore[startInt] = 0;
        fScore[startInt] = Vector2Int.Distance(startInt, endInt);
        while (openSet.Count > 0)
        {
            Vector2Int current = openSetQueue.Dequeue();
            openSet.Remove(current);
            if (current == endInt)
            {
                callback.Invoke(ReconstructPath(cameFrom, current));
                _searchingPath = false;
                stopwatch.Stop();
                UnityEngine.Debug.Log("Pathfinding took: " + stopwatch.ElapsedMilliseconds + "ms");
                return;
            }
            closedSet.Add(current);
            foreach (Vector2Int neighbour in GetNeighboursNoDiagonal(current))
            {
                if (closedSet.Contains(neighbour))
                {
                    continue;
                }
                if (MarchingSquares.WallInfo[neighbour.x, neighbour.y, 0] == 1)
                { // if neighbour is a wall
                    closedSet.Add(neighbour);
                    continue;
                }
                float tentativeGScore = gScore[current] + 1;
                if (!openSet.Contains(neighbour))
                {
                    // if (UseConsistentHeuristic)

                    // openSetQueue.Enqueue(neighbour, (int)(Vector2Int.Distance(neighbour, endInt)));
                    // else
                    openSetQueue.Enqueue(neighbour, (int)(tentativeGScore + Vector2Int.Distance(neighbour, endInt)));
                    openSet.Add(neighbour);
                }
                else if (tentativeGScore >= gScore[neighbour])
                {
                    continue;
                }
                cameFrom[neighbour] = current;
                gScore[neighbour] = tentativeGScore;
                // if (UseConsistentHeuristic)
                // fScore[neighbour] = Vector2Int.Distance(neighbour, endInt);
                // else
                fScore[neighbour] = gScore[neighbour] + Vector2Int.Distance(neighbour, endInt);
            }
            // yield return new WaitUntil(() => PathfindingSignal);
            // yield return new WaitForSeconds(RenderDelay);
            // yield return null;
            // PathfindingSignal = false;
            // Debug.Log("Pathfinding is running with openSetQueue.Count: " + openSetQueue.Count);
        }
        callback.Invoke(null);
        stopwatch.Stop();
        UnityEngine.Debug.Log("Pathfinding took: " + stopwatch.ElapsedMilliseconds + "ms");
        // _searchingPath = false;
        // Instance.StartCoroutine(Instance.FindPathCoroutine(start, end, callback));
        // _searchingPath = true;
    }

    private static List<Vector2Int> GetNeighboursNoDiagonal(Vector2Int index)
    {
        List<Vector2Int> neighbours = new List<Vector2Int>();
        if (MarchingSquares.IsIndexInBounds(new(index.x + 1, index.y)))
            neighbours.Add(new Vector2Int(index.x + 1, index.y));
        if (MarchingSquares.IsIndexInBounds(new(index.x - 1, index.y)))
            neighbours.Add(new Vector2Int(index.x - 1, index.y));
        if (MarchingSquares.IsIndexInBounds(new(index.x, index.y + 1)))
            neighbours.Add(new Vector2Int(index.x, index.y + 1));
        if (MarchingSquares.IsIndexInBounds(new(index.x, index.y - 1)))
            neighbours.Add(new Vector2Int(index.x, index.y - 1));
        return neighbours;
    }

    private static LinkedList<Vector2> ReconstructPath(Dictionary<Vector2Int, Vector2Int> cameFrom, Vector2Int current)
    {
        LinkedList<Vector2> totalPath = new LinkedList<Vector2>();
        totalPath.AddFirst(MarchingSquares.GetPosFromIndex(current));
        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            totalPath.AddFirst(MarchingSquares.GetPosFromIndex(current));
        }
        return totalPath;
    }

    private void OnDestroy()
    {
        _searchingPath = false;
    }

    private void OnDrawGizmos()
    {
        if (_searchingPath)
        {
            Gizmos.color = Color.green;
            foreach (Vector2Int index in _openSet)
            {
                Gizmos.DrawCube(MarchingSquares.GetPosFromIndex(index), Vector3.one * 0.5f);
            }
            Gizmos.color = Color.red;
            foreach (Vector2Int index in _closedSet)
            {
                Gizmos.DrawCube(MarchingSquares.GetPosFromIndex(index), Vector3.one * 0.5f);
            }
        }
    }
}