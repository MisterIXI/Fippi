using Unity.Netcode;
using UnityEngine;

public abstract class UnitController : NetworkBehaviour
{
    public Vector2 PosToFollow;
    public LeaderFollowHandler LeaderFollowHandler;
    private bool _isFlocking;
    protected UnitSettings _unitSettings;
    public UnitSettings UnitSettings => _unitSettings;
    private Pathfollow _pathfollow;
    private Vector2Int _currentChunkID = new Vector2Int(-1, -1);
    private Rigidbody2D _rigidbody;
    [SerializeField] private bool _drawDebugGizmos;
    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _pathfollow = GetComponent<Pathfollow>();
        SetSettings();
    }
    protected abstract void SetSettings();
    private void FixedUpdate()
    {
        if (IsOwner)
        {
            Vector2 ownPos = transform.position;
            Vector2 targetPos = PosToFollow;
            if (LeaderFollowHandler != null)
            {
                if (IsFlockingPointTooFar(ownPos, targetPos))
                    BreakFormation();
                else
                    OnFlock(ownPos, targetPos);
            }
            else
            {
                //TODO: Insert code for working here
                _pathfollow.FollowPathFixedStep();
            }
        }
        CheckForChunkChange();
    }
    private void CheckForChunkChange()
    {
        if (!MarchingSquares.IsReadyAndGenerated)
            return;
        Vector2Int newChunkID = MarchingSquares.GetChunkIndexFromPos(transform.position);
        if (newChunkID != _currentChunkID)
        {
            if (SpawnManager.UnitsByChunk.ContainsKey(_currentChunkID))
                SpawnManager.UnitsByChunk[_currentChunkID]?.Remove(this);
            SpawnManager.UnitsByChunk[newChunkID].Add(this);
            _currentChunkID = newChunkID;
        }
    }
    private void OnFlock(Vector2 ownPos, Vector2 targetPos)
    {
        // if is Owner
        // if is actually following a commander
        // if still in general range
        if (_isFlocking)
        {
            // currently flocking
            if (ShouldStartPathing(ownPos, targetPos))
            {
                // start pathing
                _pathfollow.SearchPath(LeaderFollowHandler.transform.position);
                _isFlocking = false;
            }
            else
            {
                // continue flocking
                FlockingAttraction(ownPos, targetPos);
            }
        }
        else
        {
            // not currently flocking
            if (ShouldRegainFlocking(ownPos, targetPos))
            {
                // regain flocking
                _pathfollow.CancelPath();
                _isFlocking = true;
                FlockingAttraction(ownPos, targetPos);
            }
            else
            {
                // continue pathing
                _pathfollow.FollowPathFixedStep();
                if (Vector2.Distance(_pathfollow.RemainingPath.Last.Value, LeaderFollowHandler.transform.position) < _unitSettings.RepathingDistance)

                {
                    _pathfollow.CancelPath();
                    _pathfollow.SearchPath(LeaderFollowHandler.transform.position);
                }
            }
        }
    }
    private void FlockingAttraction(Vector2 ownPos, Vector2 targetPos)
    {
        // move towards target
        ownPos = Vector2.MoveTowards(ownPos, targetPos, _unitSettings.MovementSpeed * Time.fixedDeltaTime);
        // // if new Tile is wall, get moved out
        // if (MarchingSquares.IsPositionInWall(ownPos))
        // {
        //     // get moved out
        //     Vector2 wallMiddlePoint = MarchingSquares.GetPosFromIndex(MarchingSquares.GetIndexFromPos(ownPos));
        //     Vector2 pushDirection = (ownPos - wallMiddlePoint).normalized;
        //     ownPos += pushDirection * UnitSettings.MovementSpeed * Time.fixedDeltaTime;
        // }
        //TODO: get attracted to other units

        // actually move unit
        _rigidbody.MovePosition(ownPos);

    }
    private void BreakFormation()
    {
        this.LeaderFollowHandler.StopFollowingCommander(this);
        LeaderFollowHandler = null;
        _isFlocking = false;
        _pathfollow.CancelPath();
    }

    private bool IsFlockingPointTooFar(Vector2 from, Vector2 to)
    {
        return Vector2.Distance(from, to) > _unitSettings.FlockingMaxDistance;
    }
    private bool ShouldStartPathing(Vector2 from, Vector2 to)
    {
        return Vector2.Distance(from, to) > _unitSettings.FlockingPathingDistance;
    }
    private bool ShouldRegainFlocking(Vector2 from, Vector2 to)
    {
        return Vector2.Distance(from, to) < _unitSettings.FlockingRegainDistance;
    }

    private void OnDrawGizmos()
    {
        if (UnitSettings && UnitSettings.DrawDebugGizmos)
        {
            if (LeaderFollowHandler != null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(transform.position, 0.1f);
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(PosToFollow, 0.1f);
            }
            else
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawSphere(transform.position, 0.1f);
            }
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _unitSettings.FlockingMaxDistance);
            Gizmos.DrawWireSphere(transform.position, _unitSettings.FlockingPathingDistance);
            Gizmos.DrawWireSphere(transform.position, _unitSettings.FlockingRegainDistance);

        }
    }
}