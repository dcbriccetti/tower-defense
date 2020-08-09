using System;
using UnityEngine;

public class CameraPositioner : MonoBehaviour {
    private abstract class CameraView {
        protected internal abstract void SetCamera(CameraState cameraState);
        public abstract bool IsActive();
    }

    private class FollowItemCameraView : CameraView {
        private readonly string tag;
        private readonly int itemIndex;

        public FollowItemCameraView(string tag, int itemIndex) {
            this.tag = tag;
            this.itemIndex = itemIndex;
        }

        protected internal override void SetCamera(CameraState cameraState) {
            GameObject[] items = GetItems();
            if (items.Length < 1) return;
            var t = items[Math.Min(items.Length - 1, itemIndex)].transform;
            var behindAndAbove = -t.forward + Vector3.up / 2;
            cameraState.desiredPosition = t.position + behindAndAbove;
            cameraState.desiredRotation = t.rotation;
        }

        private GameObject[] GetItems() => GameObject.FindGameObjectsWithTag(tag);

        public override bool IsActive() => GetItems().Length > 0;
    }

    private class NormalView : CameraView {
        protected internal override void SetCamera(CameraState cameraState) {
            cameraState.desiredPosition = cameraState.normalPosition;
            cameraState.desiredRotation = cameraState.normalRotation;
        }

        public override bool IsActive() => true;
    }

    private class CameraState {
        public readonly Vector3 normalPosition;
        public readonly Quaternion normalRotation;
        public Vector3 desiredPosition;
        public Quaternion desiredRotation;

        public CameraState(Vector3 position, Quaternion rotation) {
            normalPosition = desiredPosition = position;
            normalRotation = desiredRotation = rotation;
        }
    }

    public new Transform camera;
    [SerializeField] [Range(1, 30)] private int positionChangeSpeed = 3;
    [SerializeField] [Range(1, 30)] private int turnSpeed = 3;

    private readonly CameraView[] cameraViews = {
        new NormalView(),
        new FollowItemCameraView("Weapon", 0),
        new FollowItemCameraView("Enemy", 2)
    };

    private CameraState cameraState;
    private int iCameraView;

    private void Start() {
        cameraState = new CameraState(camera.position, camera.rotation);
    }

    private void LateUpdate() {
        cameraViews[iCameraView].SetCamera(cameraState);
        SmoothlyMoveAndTurnCamera();
    }

    private void SmoothlyMoveAndTurnCamera() {
        camera.position = Vector3.Lerp(camera.position, cameraState.desiredPosition,
            Time.deltaTime * positionChangeSpeed);
        camera.rotation = Quaternion.Lerp(camera.rotation, cameraState.desiredRotation, Time.deltaTime * turnSpeed);
    }

    public void ChangeView() {
        var activeViewFound = false;
        while (! activeViewFound) {
            iCameraView = (iCameraView + 1) % cameraViews.Length;
            if (cameraViews[iCameraView].IsActive()) activeViewFound = true;
        }
    }
    
    public bool IsNormalView() => cameraViews[iCameraView].GetType() == typeof(NormalView);
}
