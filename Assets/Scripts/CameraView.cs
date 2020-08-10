using System;
using UnityEngine;

/// <summary>
/// Base class for camera views
/// </summary>
internal abstract class CameraView  {
    /// <summary>
    /// Sets the camera’s position and rotation
    /// </summary>
    /// <param name="cameraState">The camera’s state</param>
    protected internal abstract void SetCamera(CameraState cameraState);
    /// <summary>
    /// Returns whether the view is active. If there are no guns, a guns view will be inactive.
    /// </summary>
    /// <returns>Whether the view is active</returns>
    public abstract bool IsActive();
}

/// <summary>
/// A camera view that follows one of a set of tagged items (enemies, guns)
/// </summary>
internal class FollowItemCameraView : CameraView {
    private readonly string tag;
    private readonly int itemIndex;

    /// <summary>
    /// Creates a camera view that can follow a moving enemy in a group of enemies,
    /// or a still object such as a gun
    /// </summary>
    /// <param name="tag">The tag of an item or group of items</param>
    /// <param name="itemIndex">The index of the item in a group to follow</param>
    public FollowItemCameraView(string tag, int itemIndex) {
        this.tag = tag;
        this.itemIndex = itemIndex;
    }

    protected internal override void SetCamera(CameraState cameraState) {
        GameObject[] items = GetItems();
        if (items.Length < 1) return;
        var selectedItemTransform = items[Math.Min(items.Length - 1, itemIndex)].transform;
        var behindAndAbove = -selectedItemTransform.forward + Vector3.up / 2;
        cameraState.desiredPosition = selectedItemTransform.position + behindAndAbove;
        cameraState.desiredRotation = selectedItemTransform.rotation;
    }

    private GameObject[] GetItems() => GameObject.FindGameObjectsWithTag(tag);

    public override bool IsActive() => GetItems().Length > 0;
}

/// <summary>
/// A normal view, centered on the scene from above
/// </summary>
internal class NormalView : CameraView {
    protected internal override void SetCamera(CameraState cameraState) {
        cameraState.desiredPosition = cameraState.normalPosition;
        cameraState.desiredRotation = cameraState.normalRotation;
    }

    public override bool IsActive() => true;
}
