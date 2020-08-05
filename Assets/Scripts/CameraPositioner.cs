using UnityEngine;

public class CameraPositioner : MonoBehaviour {
    public static void PositionCameraBehindFirstChild(Transform camera, string parent, string subpart) {
        if (parent == null) return;
        Transform[] items = camera.Find(parent).GetComponentsInChildren<Transform>();
        if (items.Length > 1) {
            var item = items[1];
            var part = subpart == null ? item : item.Find(subpart);
            camera.position = part.position - part.forward + Vector3.up / 2;
            camera.rotation = part.rotation;
            camera.LookAt(part.position + part.forward);
        }
    }
}
