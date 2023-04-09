using System;
using System.Collections;
using System.Collections.Generic;
using Unity;
using Unity.Netcode;
using UnityEngine;
public class TaskPool : MonoBehaviour
{
    private Queue<DrillTask> drillPool;
    public static TaskPool Instance { get; private set; }
    private PoolSetttings _poolSettings => SettingsManager.PoolSetttings;
    private void Awake()
    {
        if (!NetworkManager.Singleton.IsServer)
        {
            Destroy(gameObject);
            return;
        }
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    private void Start()
    {
        InitPools();
        StartCoroutine(FillPools());
    }

    private void InitPools()
    {
        drillPool = new Queue<DrillTask>();

    }
    private IEnumerator FillPools()
    {
        for (int i = 0; i < _poolSettings.TaskPoolSize_Drill; i++)
        {
            drillPool.Enqueue(new DrillTask());
            yield return null;
        }
    }
    public static void ResetPool()
    {

    }

    public static DrillTask GetDrillTask(Vector2 pos, LinkedList<Vector2Int> wallsToDrill)
    {
        if (Instance.drillPool.Count > 0)
        {
            DrillTask task = Instance.drillPool.Dequeue();
            task.InitTask(pos, wallsToDrill);
            return task;
        }
        else
        {
            return new DrillTask(pos, wallsToDrill);
        }
    }

    public static void ReturnTaskToPool(Task task)
    {
        switch (task.TaskType)
        {
            case TaskType.Drill:
                Instance.drillPool.Enqueue((DrillTask)task);
                break;
            default:
                Debug.LogError($"Task type: {task.TaskType} is not yet supported");
                break;
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}

public enum TaskType
{
    Drill,
    Build,
    Repair,
    Harvest,
    Move,
    Attack,
    Defend,
    Patrol
}