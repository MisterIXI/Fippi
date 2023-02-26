using System;
using System.Collections.Generic;
using UnityEngine;


class FollowerList
{
    public bool WorkersAtFront { get; private set; } = true;
    private Dictionary<WorkerTypes, LinkedList<WorkerController>> workers;
    private LinkedList<WorkerTypes> workerOrder;
    public int WorkerCount { get; private set; }
    private Dictionary<FighterTypes, LinkedList<FighterController>> fighters;
    private LinkedList<FighterTypes> fighterOrder;
    public int FighterCount { get; private set; }
    public FollowerList()
    {
        workers = new Dictionary<WorkerTypes, LinkedList<WorkerController>>();
        fighters = new Dictionary<FighterTypes, LinkedList<FighterController>>();
        workerOrder = new LinkedList<WorkerTypes>();
        fighterOrder = new LinkedList<FighterTypes>();
        foreach (WorkerTypes type in Enum.GetValues(typeof(WorkerTypes)))
        {
            workers.Add(type, new LinkedList<WorkerController>());
            workerOrder.AddLast(type);
        }
        foreach (FighterTypes type in Enum.GetValues(typeof(FighterTypes)))
        {
            fighters.Add(type, new LinkedList<FighterController>());
            fighterOrder.AddLast(type);
        }

    }
    public System.Collections.Generic.IEnumerator<UnitController> GetEnumerator()
    {
        if (WorkersAtFront)
        {
            foreach (WorkerTypes type in workerOrder)
            {
                foreach (WorkerController worker in workers[type])
                {
                    yield return worker;
                }
            }
            foreach (FighterTypes type in fighterOrder)
            {
                foreach (FighterController fighter in fighters[type])
                {
                    yield return fighter;
                }
            }
        }
        else
        {
            foreach (FighterTypes type in fighterOrder)
            {
                foreach (FighterController fighter in fighters[type])
                {
                    yield return fighter;
                }
            }
            foreach (WorkerTypes type in workerOrder)
            {
                foreach (WorkerController worker in workers[type])
                {
                    yield return worker;
                }
            }
        }
    }
    public void AddWorker(WorkerController worker)
    {
        workers[worker.WorkerType].AddLast(worker);
        WorkerCount++;
    }

    public void AddFighter(FighterController fighter)
    {
        fighters[fighter.FighterType].AddLast(fighter);
        FighterCount++;
    }

    public void RemoveWorker(WorkerController worker)
    {
        if (workers[worker.WorkerType].Remove(worker))
            WorkerCount--;
    }

    public void RemoveFighter(FighterController fighter)
    {
        if (fighters[fighter.FighterType].Remove(fighter))
            FighterCount--;
    }

}