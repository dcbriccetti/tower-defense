using System;
using UnityEngine;

public class CameraPositioner : MonoBehaviour {
    public class CameraViewSetting {
        public readonly string followChildOf;
        public readonly string subpart;
        public CameraViewSetting(string followChildOf, string subpart) {
            this.followChildOf = followChildOf;
            this.subpart = subpart;
        }
    }

    public CameraViewSetting[] cameraViewSettings = {
        null, // Normal view
        new CameraViewSetting("/Guns", "Body"),
        new CameraViewSetting("/Enemies", null),
    };

    public new Transform camera;
    public int iCameraView;
    private Vector3 desiredPosition;
    private Quaternion desiredRotation;
    public Vector3 normalPosition;
    public Quaternion normalRotation;

    private void Start() {
        normalPosition = desiredPosition = camera.position;
        normalRotation  = desiredRotation = camera.rotation;
    }

    private void Update() {
        PositionCameraBehindFirstChild();
        UpdateCamera();
    }

    private void UpdateCamera() {
        camera.position = Vector3.Lerp(camera.position, desiredPosition, Time.deltaTime * 3);
        camera.rotation = Quaternion.Lerp(camera.rotation, desiredRotation, Time.deltaTime * 3);
    }

    public void PositionCameraBehindFirstChild() {
        var cvs = cameraViewSettings[iCameraView];
        if (cvs == null) {
            desiredPosition = normalPosition;
            desiredRotation = normalRotation;
            return;
        }

        Transform[] items = transform.Find(cvs.followChildOf).GetComponentsInChildren<Transform>();
        if (items.Length > 1) {
            var item = items[1];
            var part = cvs.subpart == null ? item : item.Find(cvs.subpart);
            desiredPosition = part.position - part.forward + Vector3.up / 2;
            desiredRotation = part.rotation;
        }
    }

    public void ChangeView() {
        iCameraView = (iCameraView + 1) % cameraViewSettings.Length;
    }
}
