using Unity.Netcode;
using UnityEngine;
public class FighterController : UnitController
{
    [field: SerializeField] public FighterTypes FighterType { get; private set; }

    protected override void SetSettings()
    {
        _unitSettings = SpawnManager.GetFighterSettings(FighterType);
    }
}