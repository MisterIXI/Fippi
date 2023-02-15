using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnitMovement : MonoBehaviour
{
    [field: SerializeField] public Transform DebugTarget { get; private set; } = null;
    [field: SerializeField] private LinkedList<Vector2> _path;
    [field: SerializeField] public MovementSettings MovementSettings { get; private set; } = null;
    public Action<LinkedList<Vector2>> PathFound { get; set; }
    private void Start()
    {
        PathFound += OnPathFound;
        InputManager.OnMove += OnMove;
    }

    private void UncheckedMovement(Vector2 target)
    {

    }

    private void Update()
    {
        CheckedMovement(transform.position + (Vector3)_movement);
    }
    private void CheckedMovement(Vector2 target)
    {
        if (target == (Vector2)transform.position)
            return;
        Vector2 nDir = (target - (Vector2)transform.position).normalized;
        Vector2Int currIndex = MarchingSquares.GetIndexFromPos(transform.position);
        Vector3 offset = nDir * MovementSettings.PositionCheckOffset;
        Vector3 movement = nDir * MovementSettings.MovementSpeed * Time.deltaTime;
        // // Raycast version
        // RaycastHit hit;
        // if (Physics.Raycast(transform.position + movement + offset + Vector3.back, Vector3.forward, out hit, 5))
        // {
        //     if (hit.)
        //     {
        //         return;
        //     }
        // }

        // Position version
        Vector2Int targetIndex = MarchingSquares.GetIndexFromPos(transform.position + movement + offset);
        if (MarchingSquares.WallInfo[targetIndex.x, targetIndex.y, 0] == 0)
        { // if target point is free
            transform.position += movement;
        }
    }
    private Vector2 _movement;
    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _movement = context.ReadValue<Vector2>();
        }
        else if (context.canceled)
        {
            _movement = Vector2.zero;
        }
    }
    public void PrintPath()
    {
        if (_path == null)
        {
            Debug.Log("No path");
            return;
        }
        string pathString = "";
        foreach (Vector2 pos in _path)
        {
            pathString += pos + " -> ";
        }
        Debug.Log(pathString);
    }
    public void DebugFindPath()
    {
        _path = null;
        Pathfinding.FindPath(transform.position, DebugTarget.position, PathFound);
    }

    private void OnPathFound(LinkedList<Vector2> path)
    {
        Debug.Log("Pathfinding complete." + (path != null ? (" Path found with length " + path.Count) : " No path found."));
        _path = path;
    }

    private void OnDrawGizmos()
    {
        if (_path != null)
        {
            Gizmos.color = Color.red;
            Vector2 offset = Vector2.zero; //Vector2.one * MarchingSquares.TileLength / 2;
            Vector2 lastPos = transform.position + (Vector3)offset;
            foreach (Vector2 pos in _path)
            {
                Gizmos.DrawLine(lastPos - offset, (Vector3)pos + Vector3.back - (Vector3)offset);
                lastPos = pos;
            }
        }
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(transform.position, 0.3f);
        if (MarchingSquares.Instance != null)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawSphere(MarchingSquares.GetPosFromIndex(MarchingSquares.GetIndexFromPos(transform.position)), 0.3f);
        }
    }
}