using System;
using Cinemachine;
using UnityEngine;


public class CameraController : MonoBehaviour
{
    public static CameraController Instance { get; private set; }
    public CinemachineVirtualCamera VirtualCamera { get; private set; }
    public Camera Camera { get; private set; }
    public CinemachineBrain CMBrain { get; private set; }
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