using UnityEngine;

[CreateAssetMenu(fileName = "CameraSettings", menuName = "Fippi/CameraSettings", order = 0)]
public class CameraSettings : ScriptableObject
{
    [field: Header("Follow Target Settings")]
    [field: SerializeField] public float FollowOffsetZ { get; private set; } = -10f;
    [field: Header("Virtual Camera Settings")]
    [field: SerializeField][field: Range(0f, 20f)] public float VirtDamping { get; private set; } = 3f;
    [field: Header("Camera Settings")]
    [field: SerializeField][field: Range(0.01f, 20f)] public float CameraZoomMult { get; private set; } = 3f;
}