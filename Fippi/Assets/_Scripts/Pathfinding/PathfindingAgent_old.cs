using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PathfindingAgent_old : MonoBehaviour
{
    [field: SerializeField] private Transform _target;
    public Action<List<Vector2>> OnPathFound;
    private List<Vector2> _path;

    private void Start()
    {
        OnPathFound += OnPathFoundFunc;
    }
    [ContextMenu(nameof(MoveToDebugTarget))]
    public void MoveToDebugTarget()
    {
        if (_target != null)
            MoveTo(_target.position);
    }

    [SerializeField] bool UpdateDebugInfo = true;
    [SerializeField] int density;
    [SerializeField] Vector2Int currPos;
    private void Update()
    {
        if (UpdateDebugInfo)
        {
            if (Pathfinding_old.Grid != null)
            {
                density = Pathfinding_old.Grid[0, 0];
            }
            currPos = Vector2Int.RoundToInt(transform.position);
        }
    }
    public void MoveTo(Vector3 target)
    {
        Pathfinding_old.FindPath(Vector2Int.RoundToInt(transform.position), Vector2Int.RoundToInt(target), OnPathFound);
    }

    public void OnPathFoundFunc(List<Vector2> path)
    {
        _path = path;

    }

    private void OnDrawGizmos()
    {
        if (_path != null)
        {
            Gizmos.color = Color.red;
            for (int i = 0; i < _path.Count - 1; i++)
            {
                Gizmos.DrawLine((Vector3)_path[i] + Vector3.back * 5, (Vector3)_path[i + 1] + Vector3.back * 5);
            }
        }
    }
}