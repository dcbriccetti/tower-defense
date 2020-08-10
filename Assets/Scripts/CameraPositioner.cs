using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class CameraPositioner : MonoBehaviour {
    [SerializeField] [Range(1, 30)] private int positionChangeSpeed = 3;
    [SerializeField] [Range(1, 30)] private int turnSpeed = 3;
    private new Transform camera;
    private readonly CameraView[] cameraViews = {
        new NormalView(),
        new FollowItemCameraView("Weapon"),
        new FollowItemCameraView("Enemy")
    };
    private CameraView cameraView;
    private CameraState cameraState;
    private int iCameraView;

    /// Advances to the next enabled view 
    public void ChangeView() {
        var activeViewFound = false;
        while (! activeViewFound) {
            iCameraView = (iCameraView + 1) % cameraViews.Length;
            if (cameraViews[iCameraView].IsActive()) activeViewFound = true;
        }

        cameraView = cameraViews[iCameraView];
    }
    
    /// Returns whether the current view is the normal one, i.e., the view from above
    public bool IsNormalView() => cameraView.GetType() == typeof(NormalView);

    private void Start() {
        cameraView = cameraViews[0];
        camera = Camera.main.transform;
        cameraState = new CameraState(camera.position, camera.rotation);
    }

    private void Update() {
        var kb = Keyboard.current;
        if (kb.upArrowKey.wasPressedThisFrame)
            cameraView.ProcessUpDown(1);
        else if (kb.downArrowKey.wasPressedThisFrame)
            cameraView.ProcessUpDown(-1);
    }

    private void LateUpdate() {
        cameraView.SetCamera(cameraState);
        SmoothlyMoveAndTurnCamera();
    }

    private void SmoothlyMoveAndTurnCamera() {
        camera.position = Vector3.Lerp(camera.position, cameraState.desiredPosition,
            Time.deltaTime * positionChangeSpeed);
        camera.rotation = Quaternion.Lerp(camera.rotation, cameraState.desiredRotation, Time.deltaTime * turnSpeed);
    }
}
