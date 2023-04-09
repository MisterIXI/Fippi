using System;
using System.Collections.Generic;
using UnityEngine;
public abstract class Task
{
    public Vector2 pos { get; private set; }
    public List<TaskController> controllers { get; private set; }
    public Vector2Int ChunkID => MarchingSquares.GetChunkIndexFromPos(pos);
    public Action OnTaskCompleted;
    abstract public TaskType TaskType { get; }
    protected virtual void InitTask(Vector2 pos)
    {
        this.pos = pos;
    }
    public Task(Vector2 pos = default)
    {
        this.pos = pos;
        controllers = new List<TaskController>();
        OnTaskCompleted += ReturnTaskToPool;
    }
    public virtual void CompleteTask()
    {
        OnTaskCompleted?.Invoke();
    }
    protected abstract void ReturnTaskToPool();
}