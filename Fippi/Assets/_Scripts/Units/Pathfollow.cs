using System;
using System.Collections.Generic;
using UnityEngine;
public class Pathfollow : MonoBehaviour
{
    public LinkedList<Vector2> RemainingPath { get; private set; }
    private UnitSettings _unitSettings;
    private Vector2 _currentNode;
    [SerializeField] private bool _twoNodeStep = true;
    public PathFindStatus Status { get; private set; }
    public enum PathFindStatus { Idle, Searching, Following }
    private float _distanceMovedThisNode;
    /// <summary>
    /// Called when the path is finished, bool is true if the path was completed, false if it was cancelled
    /// </summary>
    public Action<bool> OnPathFinished;
    public void CancelPath()
    {
        Status = PathFindStatus.Idle;
        RemainingPath = null;
        _currentNode = Vector2.zero;
        _distanceMovedThisNode = 0;
        OnPathFinished?.Invoke(false);
    }

    public void SearchPath(Vector2 targetPos)
    {
        Status = PathFindStatus.Searching;
        Pathfinding.FindPath(transform.position, targetPos, StartPath);
    }

    public void StartPath(LinkedList<Vector2> path)
    {
        if (path == null)
        {
            // No Path found
            Status = PathFindStatus.Idle;
        }
        else
        {
            Status = PathFindStatus.Following;
            RemainingPath = path;
            _currentNode = RemainingPath.First.Value;
            RemainingPath.RemoveFirst();
            _distanceMovedThisNode = 0;
        }
    }
    public void FollowPathFixedStep()
    {
        if (Status == PathFindStatus.Following && RemainingPath != null)
        {
            if (_distanceMovedThisNode >= _unitSettings.MovementSpeed)
            {
                if (RemainingPath.Count == 0)
                {
                    // Finished path
                    _currentNode = Vector2.zero;
                    Status = PathFindStatus.Idle;
                    RemainingPath = null;
                    OnPathFinished?.Invoke(true);
                    return;
                }
                _currentNode = RemainingPath.First.Value;
                RemainingPath.RemoveFirst();
                _distanceMovedThisNode = 0;
            }
            Vector2 actualTarget;
            if (_twoNodeStep && RemainingPath.Count > 0)
                actualTarget = RemainingPath.First.Value;
            else
                actualTarget = _currentNode;
            Vector2.MoveTowards(transform.position, actualTarget, _unitSettings.MovementSpeed * Time.fixedDeltaTime);
            _distanceMovedThisNode += _unitSettings.MovementSpeed * Time.fixedDeltaTime;
        }
    }
}