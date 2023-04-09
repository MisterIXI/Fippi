using System;
using Cinemachine;
using UnityEngine;


public class CameraController : MonoBehaviour
{
    public static CameraController Instance { get; private set; }
    public static CinemachineVirtualCamera VirtualCamera { get; private set; }
    public static Camera Camera { get; private set; }
    public static CinemachineBrain CMBrain { get; private set; }
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        Camera = GetComponentInChildren<Camera>();
        CMBrain = GetComponentInChildren<CinemachineBrain>();
    }


}