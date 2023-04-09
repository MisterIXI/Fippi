using System;
using Cinemachine;
using Unity.Netcode;
using UnityEngine;

public class FollowTargetController : MonoBehaviour
{
    public Transform CommanderToFollow { get; private set; }
    public Transform FollowOverride;
    private ulong _localPlayerID;
    private CameraSettings _cameraSettings;
    private void Awake()
    {
        _cameraSettings = SettingsManager.CameraSettings;
        _localPlayerID = NetworkManager.Singleton.LocalClientId;
        CommanderController.OnActiveCommanderChanged += OnCommanderSwitch;
    }


    private void Update()
    {
        if (FollowOverride != null)
        {
            FollowTransform(FollowOverride);
        }
        else if (CommanderToFollow != null)
        {
            FollowTransform(CommanderToFollow);
        }
    }

    private void FollowTransform(Transform transform)
    {
        transform.position = transform.position + Vector3.forward * _cameraSettings.FollowOffsetZ;
    }
    private void OnCommanderSwitch(ulong playerID, CommanderController oldCommander, CommanderController newCommander)
    {
        if (playerID == _localPlayerID)
            CommanderToFollow = newCommander.transform;
    }
}