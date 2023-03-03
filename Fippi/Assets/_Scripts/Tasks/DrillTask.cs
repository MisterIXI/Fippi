using System;
using System.Collections.Generic;
using UnityEngine;

class DrillTask : Task
{
    Vector2Int CurrentWall;
    LinkedList<Vector2Int> WallsToDrill;
    public Action OnDrillTaskComplete;
    public bool IsComplete { get; private set; } = false;
    public DrillTask(Vector2 pos, LinkedList<Vector2Int> wallsToDrill) : base(pos)
    {
        WallsToDrill = wallsToDrill;
        if (WallsToDrill.Count > 0)
        {
            CurrentWall = WallsToDrill.First.Value;
            wallsToDrill.RemoveFirst();
        }
        else
        {
            IsComplete = true;
        }
    }

    public void DrillTick()
    {
    }
}