using UnityEngine;
using Unity.Netcode;
public class WorkerController : UnitController
{
    [field: SerializeField] public WorkerTypes WorkerType { get; private set; }
    protected override void SetSettings()
    {
        _unitSettings = SpawnManager.GetWorkerSettings(WorkerType);
    }
}