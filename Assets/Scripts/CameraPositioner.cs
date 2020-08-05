using UnityEngine;

public class CameraPositioner : MonoBehaviour {
    public class CameraViewSetting {
        public string followChildOf;
        public string subpart;
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

    public int iCameraView;

    public void PositionCameraBehindFirstChild(Transform camera) {
        var cvs = cameraViewSettings[iCameraView];
        if (cvs == null) return;
        Transform[] items = transform.Find(cvs.followChildOf).GetComponentsInChildren<Transform>();
        if (items.Length > 1) {
            var item = items[1];
            var part = cvs.subpart == null ? item : item.Find(cvs.subpart);
            camera.position = part.position - part.forward + Vector3.up / 2;
            camera.rotation = part.rotation;
            camera.LookAt(part.position + part.forward);
        }
    }

    public void ChangeView() {
        iCameraView = (iCameraView + 1) % cameraViewSettings.Length;
    }
}
