using UnityEngine;

public class CameraPositioner : MonoBehaviour {
    [SerializeField] [Range(1, 30)] private int positionChangeSpeed = 3;
    [SerializeField] [Range(1, 30)] private int turnSpeed = 3;
    private new Transform camera;

    /// <summary>
    /// Advances to the next enabled view
    /// </summary>
    public void ChangeView() {
        var activeViewFound = false;
        while (! activeViewFound) {
            iCameraView = (iCameraView + 1) % cameraViews.Length;
            if (cameraViews[iCameraView].IsActive()) activeViewFound = true;
        }
    }
    
    /// <summary>
    /// Returns whether the current view is the normal one, i.e., the view from above
    /// </summary>
    /// <returns>Whether the current view is the normal one</returns>
    public bool IsNormalView() => cameraViews[iCameraView].GetType() == typeof(NormalView);

    private readonly CameraView[] cameraViews = {
        new NormalView(),
        new FollowItemCameraView("Weapon", 0),
        new FollowItemCameraView("Enemy", 1)
    };

    private CameraState cameraState;
    private int iCameraView;

    private void Start() {
        camera = Camera.main.transform;
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
}
