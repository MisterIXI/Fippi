using UnityEngine;

public class TaskController : MonoBehaviour
{
    public Task CurrentTask { get; private set; }
    public void SetTask(Task task)
    {
        CurrentTask = task;
        CurrentTask.controllers.Add(this);
    }
}