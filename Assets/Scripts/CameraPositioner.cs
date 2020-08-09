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
        new CameraViewSetting("/Enemies", null)
    };

    public new Transform camera;
    private int iCameraView;
    private Vector3 desiredPosition;
    private Quaternion desiredRotation;
    [Range(1, 30)] public int positionChangeSpeed = 3;
    [Range(1, 30)] public int turnSpeed = 3;
    public Vector3 NormalPosition { get; set; }
    public Quaternion NormalRotation { get; set; }

    private void Start() {
        NormalPosition = desiredPosition = camera.position;
        NormalRotation  = desiredRotation = camera.rotation;
    }

    private void Update() {
        PositionCameraBehindFirstChild();
        UpdateCamera();
    }

    private void UpdateCamera() {
        camera.position = Vector3.Lerp(camera.position, desiredPosition, Time.deltaTime * positionChangeSpeed);
        camera.rotation = Quaternion.Lerp(camera.rotation, desiredRotation, Time.deltaTime * turnSpeed);
    }

    private void PositionCameraBehindFirstChild() {
        var cvs = cameraViewSettings[iCameraView];
        if (cvs == null) {
            desiredPosition = NormalPosition;
            desiredRotation = NormalRotation;
            return;
        }

        Transform[] items = transform.Find(cvs.followChildOf).GetComponentsInChildren<Transform>();
        if (items.Length <= 1) return;
        var item = items[1];
        var part = cvs.subpart == null ? item : item.Find(cvs.subpart);
        desiredPosition = part.position - part.forward + Vector3.up / 2;
        desiredRotation = part.rotation;
    }

    public void ChangeView() {
        iCameraView = (iCameraView + 1) % cameraViewSettings.Length;
    }
}
