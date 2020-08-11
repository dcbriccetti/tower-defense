using System;
using UnityEngine;
using UnityEngine.InputSystem.Controls;

/// Base class for camera views
internal abstract class CameraView  {
    /// <summary>
    /// Sets the camera’s position and rotation
    /// </summary>
    /// <param name="cameraState">The camera’s state</param>
    protected internal abstract void SetCamera(CameraState cameraState);

    /// Returns whether the view is active. If there are no guns, a guns view will be inactive.
    public abstract bool IsActive();

    public virtual void ProcessMouseWheelInput(float displacement) { }

    protected internal virtual void ProcessUpDown(int upDown) { }
}

/// A camera view that follows one of a set of tagged itemsCache (enemies, guns)
internal class FollowItemCameraView : CameraView {
    private readonly string tag;
    private int itemIndex;
    private float followDistance = .5f;

    /// <summary>
    /// Creates a camera view that can follow a moving enemy in a group of enemies,
    /// or a still object such as a gun
    /// </summary>
    /// <param name="tag">The tag of an item or group of items</param>
    public FollowItemCameraView( string tag) {
        this.tag = tag;
    }

    protected internal override void SetCamera(CameraState cameraState) {
        GameObject[] items = GetItems();
        if (items.Length < 1) return;
        var item = items[Math.Min(items.Length - 1, itemIndex)].transform;
        var behindAndAbove = followDistance * (-item.forward + Vector3.up / 2);
        cameraState.desiredPosition = item.position + behindAndAbove;
        cameraState.desiredRotation = item.rotation;
    }

    protected internal override void ProcessUpDown(int upDown) {
        var num = GetItems().Length;
        itemIndex = (itemIndex + num - upDown) % num; // Up: closer to first element, Down: closer to last
    }

    public override void ProcessMouseWheelInput(float displacement) {
        followDistance = Mathf.Clamp(followDistance + displacement, 0, 10);
    }

    private GameObject[] GetItems() => GameObject.FindGameObjectsWithTag(tag);

    public override bool IsActive() => GetItems().Length > 0;
}

/// A normal view, centered on the scene from above
internal class NormalView : CameraView {
    protected internal override void SetCamera(CameraState cameraState) {
        cameraState.desiredPosition = cameraState.normalPosition;
        cameraState.desiredRotation = cameraState.normalRotation;
    }

    public override bool IsActive() => true;
}
