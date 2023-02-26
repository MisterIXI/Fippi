using UnityEngine;
[RequireComponent(typeof(CommanderController))]
[RequireComponent(typeof(CommanderCommandController))]
public class LeaderFollowHandler : MonoBehaviour
{
    public CommanderController commander { get; private set; }
    private FollowerList followers;
    public Vector2 SwarmMiddlePoint { get; private set; }
    private CommanderSettings _commanderSettings;
    private void Awake()
    {
        commander = GetComponent<CommanderController>();
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
        Vector2 currentPos = commander.transform.position;
        Vector2 commanderHeading = commander.Heading;
        currentPos -= commanderHeading * _commanderSettings.SwarmCommanderDistance;
        float axisLength = totalFollowers / 2 * _commanderSettings.SwarmIndiviualDistance;
        float stepLength = _commanderSettings.SwarmCommanderDistance;
        for (int y = 0; y < totalFollowers / 2; y++)
        {
            for (int x = 0; x < totalFollowers / 2; x++)
            {
            }
        }
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
}