using System;
using System.Collections.Generic;
using UnityEngine;
public class Task
{
    public Vector2 pos;
    public List<TaskController> controllers;
    public Vector2Int ChunkID => MarchingSquares.GetChunkIndexFromPos(pos);

    public Task(Vector2 pos)
    {
        this.pos = pos;
        controllers = new List<TaskController>();
    }
}