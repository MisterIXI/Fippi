using System;
using System.Collections.Generic;
using UnityEngine;

public class DrillTask : Task
{
    Vector2Int CurrentWall;
    LinkedList<Vector2Int> WallsToDrill;
    public bool IsComplete { get; private set; } = false;
    public override TaskType TaskType => TaskType.Drill;
    public DrillTask() : base() { }
    public DrillTask(Vector2 pos, LinkedList<Vector2Int> wallsToDrill) : base(pos)
    {
        InitTask(pos, wallsToDrill);
    }
    public void InitTask(Vector2 pos, LinkedList<Vector2Int> wallsToDrill)
    {
        base.InitTask(pos);
        WallsToDrill = wallsToDrill;
        if (WallsToDrill.Count > 0)
        {
            CurrentWall = WallsToDrill.First.Value;
            wallsToDrill.RemoveFirst();
            IsComplete = false;
        }
        else
        {
            IsComplete = true;
        }
    }

    public void DrillTick()
    {
    }
    protected override void ReturnTaskToPool()
    {
        TaskPool.ReturnTaskToPool(this);
    }
}