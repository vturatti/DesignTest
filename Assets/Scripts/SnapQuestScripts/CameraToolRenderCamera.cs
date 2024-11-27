// using System;
// using System.Collections.Generic;
// using Cinemachine;
// using Code.CameraTool;
// using Code.Canvas;
// using UnityEngine;
// using UnityEngine.InputSystem;
//
// namespace Code.RenderCameras
// {
//     public class CameraToolRenderCamera : MonoBehaviour
//     {
//         private readonly string StartTweenKey = "CameraToolRenderCamStartTweenKey";
//         public CameraToolUIElementsAndUIEffects UiEffects;
//         private float cameraContinuousTurningDuration_;
//         public CameraToolService CameraToolService;
//         private float currentXRotation_;
//         private bool isInSelfieMode_;
//         private float lastMoveUpdateTime_;
//         private bool reticleIsLockedInCenter_ = true;
//         private readonly Vector2 rotateBoundingBoxLimits_ = new(.2f * Screen.width, .2f * Screen.height);
//         private readonly Vector2 rotateBoundingBoxLimitsLocked_ = Vector2.zero;
//         private Transform targetTransform_;
//         private Transform targetRotationTransform_;
//         private bool tweenToStartPosComplete_;
//         private float cameraMaxZoomSpeed = 5f;
//         private float currentZoomLevel;
//         private float desiredZoomLevel;
//         private CinemachineVirtualCamera selfieVirtualCamera_;
//         public Transform CameraSwivelRoot;
//         public Transform CameraToolTarget;
//         public PlayerInput playerInput;
//         public bool IsActive;
//         public float MouseSensitivity = 5f;
//         private Vector2 lookVector;
//         public Transform PlayerRoot;
//         public CameraToolUI CameraToolUI;
//
//         public CameraToolRenderCamera()
//         {
//             targetTransform_ = CameraToolTarget;
//             targetRotationTransform_ = CameraSwivelRoot;
//             var lookAction = PlayerInput.actions["Look"];
//             lookAction.performed += LookActionOnPerformed;
//             var unlockReticleAction = PlayerInput.actions["PositionReticle"];
//             unlockReticleAction.started += UnLockReticleAction;
//             unlockReticleAction.canceled += LockReticleAction;
//         }
//
//         private void UnLockReticleAction(InputAction.CallbackContext _)
//         {
//             reticleIsLockedInCenter_ = false;
//         }
//
//         private void LockReticleAction(InputAction.CallbackContext _)
//         {
//             reticleIsLockedInCenter_ = true;
//             ReticlePos = Vector2.zero;
//             MoveReticle(Vector2.zero);
//         }
//
//         public void OnDestroy()
//         {
//             PlayerInput.actions["Look"].performed -= LookActionOnPerformed;
//             PlayerInput.actions["PositionReticle"].started -= UnLockReticleAction;
//             PlayerInput.actions["PositionReticle"].canceled -= LockReticleAction;
//         }
//
//         public Vector2 ReticlePos { get; private set; }
//
//         public void SetCameraFoVForZoomLevel(int zoomLevel, float newFoV)
//         {
//             desiredZoomLevel = zoomLevel;
//         }
//
//         private void LookActionOnPerformed(InputAction.CallbackContext obj)
//         {
//             if (IsActive)
//             {
//                 var lookV = obj.ReadValue<Vector2>() * MouseSensitivity /
//                             5f;
//                 MoveReticle(lookV);
//             }
//         }
//
//         public void Activate()
//         {
//             IsActive = true;
//             var lookAction = playerInput.actions["Look"];
//             lookAction.performed += LookVectorListener;
//             lookAction.canceled += ResetLookVectorOnInputCancel;
//             // lookVector = Vector2.zero;
//             ReticlePos = Vector2.zero;
//             targetTransform_ = CameraToolTarget;
//
//             //modify culling
//             SetCameraCullingLayer(Camera.main, LayerMask.NameToLayer("Player"), false);
//             UiEffects.MoveReticleUI(ReticlePos);
//             currentXRotation_ = 0;
//             isInSelfieMode_ = false;
//             SetCameraCullingLayer(Camera.main, LayerMask.NameToLayer("Player"), false);
//             tweenToStartPosComplete_ = false;
//         }
//         
//         public static void SetCameraCullingLayer(Camera camera, int layer, bool enable)
//         {
//             if (enable)
//             {
//                 camera.cullingMask |= 1 << layer;
//             }
//             else
//             {
//                 camera.cullingMask &= ~(1 << layer);
//             }
//         }
//         
//         protected void LookVectorListener(InputAction.CallbackContext context)
//         {
//             lookVector = context.ReadValue<Vector2>() * 1;
//         }
//
//         protected void ResetLookVectorOnInputCancel(InputAction.CallbackContext context)
//         {
//             lookVector = Vector2.zero;
//         }
//
//         private void MoveReticle(Vector2 moveDistance)
//         {
//             moveDistance *= 4;
//             ReticlePos += new Vector2(moveDistance.x, moveDistance.y);
//             if (lastMoveUpdateTime_ + Time.deltaTime * 3 < Time.time)
//             {
//                 cameraContinuousTurningDuration_ = .4f * .3f;
//             }
//
//             lastMoveUpdateTime_ = Time.time;
//             cameraContinuousTurningDuration_ = Mathf.Min(
//                 cameraContinuousTurningDuration_ + Time.deltaTime,
//                 .3f
//             );
//
//             // Sooo we are making a box IF its locked, and if not we are just treating the box as 0 size
//             // so its the same as not having the box on.
//             var currentBoundingBox =
//                 reticleIsLockedInCenter_ ? rotateBoundingBoxLimitsLocked_ : rotateBoundingBoxLimits_;
//
//             // var rotationSpeed = GTV.I.CameraReticleSpeedMultiplier *
//             //     GTV.I.CameraRotateXSpeed * GameMain.I.CameraToolService.GetCameraFOVForCurrentZoom() *
//             //     cameraContinuousTurningDuration_ / GTV.I.ContinuousMotionTimeForMaxSpeedCameraToolTurn;
//
//             var r = CameraToolService.GetCameraTripod().TripodDeployed
//                 ? CameraToolService.GetCameraTripod().TripodObject.VirtualCamera.transform.rotation
//                     .eulerAngles
//                 : PlayerRoot.rotation.eulerAngles;
//             //LEFT RIGHT happens to operate on the Y axis, but due to X input
//             ManipulateLeftRightXInputYAXIS(currentBoundingBox, r);
//
//             r = CameraToolService.GetCameraTripod().TripodDeployed
//                 ? CameraToolService.GetCameraTripod().TripodObject.CameraPivot.transform.rotation
//                     .eulerAngles
//                 : PlayerRoot.rotation.eulerAngles;
//             //UP DOWN fix Y (positive Y Rotation is down, negative is up) //happens to operate on the X axis, but due to Y input
//             ManipulateDownUpYMouseMove(currentBoundingBox);
//
//             UiEffects.MoveReticleUI(ReticlePos);
//         }
//
//
//         //UP DOWN on the X axis, but due to Y input
//         private void ManipulateDownUpYMouseMove(Vector2 currentBoundingBox)
//         {
//             if (ReticlePos.y > currentBoundingBox.y)
//             {
//                 var invert = false;
//                 var CameraYspeed = 10;
//                 var rotateBy  = CameraYspeed * (invert ? 1 : -1) *
//                                 (ReticlePos.y - currentBoundingBox.y) / GTV.I.CameraReticleSpeedMultiplier *
//                                 CameraToolService.GetCameraFOVForCurrentZoom() * cameraContinuousTurningDuration_ /
//                                 GTV.I.ContinuousMotionTimeForMaxSpeedCameraToolTurn;
//                 currentXRotation_ = Mathf.Clamp(
//                     currentXRotation_ + rotateBy,
//                     -GTV.I.MaxYAngleRotate,
//                     -GTV.I.MinYAngleRotate
//                 );
//                 if (GameMain.I.GameStateService.PlayerTempState.TripodDeployed)
//                 {
//                     GameMain.I.CameraToolService.GetCameraTripod().TripodObject.VirtualCamera.transform.localRotation =
//                         Quaternion.Euler(
//                             currentXRotation_,
//                             0,
//                             0
//                         );
//                 }
//                 else
//                 {
//                     cameraTransform_.localRotation = Quaternion.Euler(
//                         currentXRotation_,
//                         0,
//                         0
//                     );
//                 }
//
//                 ReticlePos = new Vector2(ReticlePos.x, currentBoundingBox.y);
//             }
//             else if (ReticlePos.y < -currentBoundingBox.y)
//             {
//                 var rotateBy = GTV.I.CameraRotateYSpeed * (GTV.I.InverseCameraY ? 1 : -1) *
//                                (ReticlePos.y + currentBoundingBox.y) / GTV.I.CameraReticleSpeedMultiplier *
//                                CameraToolService.GetCameraFOVForCurrentZoom() * cameraContinuousTurningDuration_ /
//                                GTV.I.ContinuousMotionTimeForMaxSpeedCameraToolTurn;
//                 currentXRotation_ = Mathf.Clamp(
//                     currentXRotation_ + rotateBy,
//                     -GTV.I.MaxYAngleRotate,
//                     -GTV.I.MinYAngleRotate
//                 );
//                 if (GameMain.I.GameStateService.PlayerTempState.TripodDeployed)
//                 {
//                     GameMain.I.CameraToolService.GetCameraTripod().TripodObject.VirtualCamera.transform.localRotation =
//                         Quaternion.Euler(
//                             currentXRotation_,
//                             0,
//                             0
//                         );
//                 }
//                 else
//                 {
//                     cameraTransform_.localRotation = Quaternion.Euler(
//                         currentXRotation_,
//                         0,
//                         0
//                     );
//                 }
//
//                 ReticlePos = new Vector2(ReticlePos.x, -currentBoundingBox.y);
//             }
//         }
//
//         //LEFT RIGHT movement
//         private void ManipulateLeftRightXInputYAXIS(Vector2 currentBoundingBox, Vector3 r)
//         {
//             //fix X, (positive X Rotation is right, negative is left)
//             if (ReticlePos.x > currentBoundingBox.x)
//             {
//                 var q = Quaternion.Euler(
//                     r.x,
//                     r.y + (ReticlePos.x - currentBoundingBox.x) / GTV.I.CameraReticleSpeedMultiplier *
//                     GTV.I.CameraRotateXSpeed * GameMain.I.CameraToolService.GetCameraFOVForCurrentZoom() *
//                     cameraContinuousTurningDuration_ / GTV.I.ContinuousMotionTimeForMaxSpeedCameraToolTurn,
//                     r.z
//                 );
//
//                 if (GameMain.I.GameStateService.PlayerTempState.TripodDeployed)
//                 {
//                     GameMain.I.CameraToolService.GetCameraTripod().TripodObject.CameraPivot.transform.localRotation =
//                         Quaternion.Euler(
//                             0,
//                             GameMain.I.CameraToolService.GetCameraTripod().TripodObject.CameraPivot.transform
//                                 .localRotation
//                                 .eulerAngles.y + (ReticlePos.x + currentBoundingBox.x) /
//                             GTV.I.CameraReticleSpeedMultiplier *
//                             GTV.I.CameraRotateXSpeed * GameMain.I.CameraToolService.GetCameraFOVForCurrentZoom() *
//                             cameraContinuousTurningDuration_ / GTV.I.ContinuousMotionTimeForMaxSpeedCameraToolTurn,
//                             0
//                         );
//                     ;
//                 }
//                 else
//                 {
//                     GameMain.I.PlayerService.ForceSetCharacterRotation(q, false);
//                 }
//
//                 ReticlePos = new Vector2(currentBoundingBox.x, ReticlePos.y);
//             }
//             else if (ReticlePos.x < -currentBoundingBox.x)
//             {
//                 // var q = Quaternion.Euler(r.x, r.y + (ReticlePos.x + currentBoundingBox.x) / rotationSpeed, r.z);
//                 var q = Quaternion.Euler(
//                     r.x,
//                     r.y + (ReticlePos.x + currentBoundingBox.x) / GTV.I.CameraReticleSpeedMultiplier *
//                     GTV.I.CameraRotateXSpeed * GameMain.I.CameraToolService.GetCameraFOVForCurrentZoom() *
//                     cameraContinuousTurningDuration_ / GTV.I.ContinuousMotionTimeForMaxSpeedCameraToolTurn,
//                     r.z
//                 );
//                 if (GameMain.I.GameStateService.PlayerTempState.TripodDeployed)
//                 {
//                     GameMain.I.CameraToolService.GetCameraTripod().TripodObject.CameraPivot.transform.localRotation =
//                         Quaternion.Euler(
//                             0,
//                             GameMain.I.CameraToolService.GetCameraTripod().TripodObject.CameraPivot.transform
//                                 .localRotation
//                                 .eulerAngles.y + (ReticlePos.x + currentBoundingBox.x) /
//                             GTV.I.CameraReticleSpeedMultiplier *
//                             GTV.I.CameraRotateXSpeed * GameMain.I.CameraToolService.GetCameraFOVForCurrentZoom() *
//                             cameraContinuousTurningDuration_ / GTV.I.ContinuousMotionTimeForMaxSpeedCameraToolTurn,
//                             0
//                         );
//                     ;
//                 }
//                 else
//                 {
//                     GameMain.I.PlayerService.ForceSetCharacterRotation(q, false);
//                 }
//
//                 ReticlePos = new Vector2(-currentBoundingBox.x, ReticlePos.y);
//             }
//         }
//         
//
//         protected override void LateUpdate()
//         {
//             if (tweenToStartPosComplete_)
//             {
//                 //camera is now active, so move based on input
//                 // stick to targetTransform_ point
//                 cameraTransform_.position = targetTransform_.position;
//             }
//             //Lerp to desired zoom
//             //needs to set the virtual camera value
//
//             if (currentZoomLevel != desiredZoomLevel)
//             {
//                 var deltaZoom = currentZoomLevel - desiredZoomLevel;
//                 if (Mathf.Abs(deltaZoom) < Time.deltaTime * cameraMaxZoomSpeed)
//                 {
//                     currentZoomLevel = desiredZoomLevel;
//                 }
//                 else
//                 {
//                     currentZoomLevel = currentZoomLevel > desiredZoomLevel
//                         ? currentZoomLevel - Time.deltaTime * cameraMaxZoomSpeed
//                         : currentZoomLevel +
//                           Time.deltaTime * cameraMaxZoomSpeed;
//                 }
//
//                 if (!CameraToolUI.CameraZoomSFX.isPlaying)
//                 {
//                     CameraToolUI.CameraZoomSFX.Play();
//                 }
//
//                 if (CameraToolService.GetCameraTripod().TripodDeployed)
//                 {
//                     CameraToolService.GetCameraTripod().TripodObject.VirtualCamera.m_Lens.FieldOfView =
//                         60 * CameraToolService.GetCameraFOVForZoomLevel(currentZoomLevel);
//                 }
//                 else
//                 {
//                     cinemachineVirtualCamera_.m_Lens.FieldOfView =
//                         60 * CameraToolService.GetCameraFOVForZoomLevel(currentZoomLevel);
//                 }
//             }
//             else
//             {
//                 CameraToolUI.CameraZoomSFX.Stop();
//             }
//         }
//
//         public override void Deactivate()
//         {
//             SetCameraCullingLayer(Camera.main, LayerMask.NameToLayer("Player"), true);
//             cinemachineVirtualCamera_.m_Lens.FieldOfView =
//                 60 * CameraToolService.GetCameraFOVForZoomLevel(1);
//             IsActive = false;
//             var lookAction = playerInput.actions["Look"];
//             lookAction.performed -= LookVectorListener;
//             lookAction.canceled -= ResetLookVectorOnInputCancel;
//             lookVector = Vector2.zero;
//         }
//
//         public Ray ReticleRayScreenToWorld()
//         {
//             var ray = Camera.main.ScreenPointToRay(
//                 new Vector3(
//                     ReticlePos.x + Screen.width / 2,
//                     ReticlePos.y + Screen.height / 2,
//                     0
//                 )
//             );
//             Debug.DrawRay(ray.origin, ray.direction * 10, Color.cyan);
//             return ray;
//         }
//
//         public void SetPrimaryNotableObject(
//             NotableObject notableObject,
//             bool isKnownNotableObject,
//             bool isRecordedActionState,
//             bool isTooFar)
//         {
//             UiEffects.SetPrimaryNotableObject(notableObject, isKnownNotableObject, isRecordedActionState, isTooFar);
//         }
//
//         public void SetSecondaryNotableObjects(List<NotableObject> notableObjects)
//         {
//             UiEffects.SetSecondaryNotableObjects(notableObjects);
//         }
//     }
// }