using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
[RequireComponent(typeof(CommanderController))]
[RequireComponent(typeof(CommanderCommandController))]
public class LeaderFollowHandler : MonoBehaviour
{
    public CommanderController commander { get; private set; }
    private FollowerList followers;
    public Vector2 SwarmMiddlePoint { get; private set; }
    private CommanderSettings _commanderSettings;
    private bool _recallingActive;
    private void Awake()
    {
        commander = GetComponent<CommanderController>();
        commander.OnOwnershipChanged += HandleCommanderOwnershipChanged;
        followers = new FollowerList();
    }

    private void Start()
    {
        _commanderSettings = SpawnManager.CommanderSettings;
    }

    private void FixedUpdate()
    {
        if (commander.IsOwner)
            UpdateFollowerVectors();
    }

    private void UpdateFollowerVectors()
    {
        int totalFollowers = followers.WorkerCount + followers.FighterCount;
        if (totalFollowers == 0)
            return;
        Vector2 posZero = commander.transform.position;
        Vector2 commanderHeading = commander.Heading;
        Vector2 commanderRight = commander.Right;
        int axisCount = totalFollowers / 2 + 1;
        float axisLength = axisCount * _commanderSettings.SwarmIndiviualDistance;
        float stepLength = _commanderSettings.SwarmIndiviualDistance;
        // move posZero back
        posZero -= commanderHeading * _commanderSettings.SwarmCommanderDistance;
        // offset posZero to starting point by half of axis length
        posZero -= commanderRight * axisLength / 2;
        int x = 0;
        int y = 0;
        foreach (UnitController unit in followers)
        {
            unit.PosToFollow = posZero + (commanderRight * x * stepLength) - (commanderHeading * y * stepLength);
            x++;
            if (x % axisCount == 0)
            {
                x = 0;
                y++;
            }
        }
    }
    public void FollowCommander(UnitController unit)
    {
        // netcode: get local client id

        if (!unit.IsOwner)
            unit.NetworkObject.ChangeOwnership(NetworkManager.Singleton.LocalClientId);
        if (unit is WorkerController worker)
            FollowCommander(worker);
        else if (unit is FighterController fighter)
            FollowCommander(fighter);
    }
    public void FollowCommander(WorkerController worker)
    {
        followers.AddWorker(worker);
        worker.LeaderFollowHandler = this;
    }

    public void FollowCommander(FighterController fighter)
    {
        followers.AddFighter(fighter);
        fighter.LeaderFollowHandler = this;
    }

    public void StopFollowingCommander(UnitController unit)
    {
        if (unit is WorkerController worker)
            StopFollowingCommander(worker);
        else if (unit is FighterController fighter)
            StopFollowingCommander(fighter);
    }

    public void StopFollowingCommander(WorkerController worker)
    {
        followers.RemoveWorker(worker);
        worker.LeaderFollowHandler = null;
    }

    public void StopFollowingCommander(FighterController fighter)
    {
        followers.RemoveFighter(fighter);
        fighter.LeaderFollowHandler = null;
    }
    public void OnRecallInput(InputAction.CallbackContext context)
    {
        if (context.started && commander.IsActiveCommander)
        {
            _recallingActive = true;
            Vector2Int index = MarchingSquares.GetChunkIndexFromPos(transform.position);
            var ids = MarchingSquares.GetSurroundingIDs(index);
            foreach (var id in ids)
            {
                foreach (var unit in SpawnManager.UnitsByChunk[id])
                {
                    if (unit.LeaderFollowHandler == null && Vector2.Distance(unit.transform.position, transform.position) < _commanderSettings.RecallDistance)
                        FollowCommander(unit);
                }
            }
        }
        if (context.canceled)
            _recallingActive = false;
    }

    private void HandleCommanderOwnershipChanged(bool isOwner)
    {
        if (isOwner)
            SubscribeToActions();
        else
            UnsubscribeFromActions();
    }
    private void SubscribeToActions()
    {
        InputManager.OnRecall += OnRecallInput;
    }

    private void UnsubscribeFromActions()
    {
        InputManager.OnRecall -= OnRecallInput;
    }

    private void OnDestroy()
    {
        UnsubscribeFromActions();
    }

    private void OnDrawGizmos()
    {
        if (_recallingActive)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _commanderSettings.RecallDistance);
        }
    }
}