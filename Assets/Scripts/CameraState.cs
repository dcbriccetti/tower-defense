using UnityEngine;

/// <summary>
/// The normal and desired camera positions and rotations
/// </summary>
internal class CameraState {
    public readonly Vector3 normalPosition;
    public readonly Quaternion normalRotation;
    public Vector3 desiredPosition;
    public Quaternion desiredRotation;

    public CameraState(Vector3 position, Quaternion rotation) {
        normalPosition = desiredPosition = position;
        normalRotation = desiredRotation = rotation;
    }
}
