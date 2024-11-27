// using Cinemachine;
// using UnityEngine;
// using UnityEngine.InputSystem;
//
// namespace Code.RenderCameras
// {
//     public abstract class CameraControllerBase
//     {
//         protected Transform cameraTransform_;
//         protected Transform cameraSwivelRootTransform_;
//         protected ICharacterController characterController_;
//         protected KinematicCharacterMotor characterMotor_;
//         protected CinemachineVirtualCamera cinemachineVirtualCamera_;
//         public bool IsActive;
//
//         protected Vector2 lookVector;
//         protected PlayerInput playerInput_;
//         protected float zoomAxis;
//
//         public CameraControllerBase(
//             ICharacterController characterController,
//             PlayerInput playerInput,
//             KinematicCharacterMotor characterMotor,
//             CinemachineVirtualCamera cinemachineVirtualCamera,
//             PlayerService playerService)
//         {
//             characterController_ = characterController;
//             playerInput_ = playerInput;
//             characterMotor_ = characterMotor;
//             cameraTransform_ = cinemachineVirtualCamera.transform;
//             cinemachineVirtualCamera_ = cinemachineVirtualCamera;
//             cameraSwivelRootTransform_ = playerService.GetPlayersCamTargetsTransformMapping().CameraSwivelRoot;
//         }
//
//         protected Camera camera_ => GameMain.I.MainCamera;
//
//         public virtual void Activate()
//         {
//             IsActive = true;
//             var lookAction = playerInput_.actions["Look"];
//             var zoomAction = playerInput_.actions["Zoom"];
//             lookAction.performed += LookVectorListener;
//             lookAction.canceled += ResetLookVectorOnInputCancel;
//             zoomAction.performed += ZoomActionListener;
//             zoomAction.canceled += ResetZoomActionListenerOnCancel;
//             lookVector = Vector2.zero;
//         }
//
//         public virtual void Deactivate()
//         {
//             IsActive = false;
//             var lookAction = playerInput_.actions["Look"];
//             var zoomAction = playerInput_.actions["Zoom"];
//             lookAction.performed -= LookVectorListener;
//             lookAction.canceled -= ResetLookVectorOnInputCancel;
//             zoomAction.performed -= ZoomActionListener;
//             zoomAction.canceled -= ResetZoomActionListenerOnCancel;
//             lookVector = Vector2.zero;
//         }
//
//         public void SetCameraFoV(float newFoV)
//         {
//             cinemachineVirtualCamera_.m_Lens.FieldOfView = newFoV;
//         }
//
//         public Transform GetCameraTransform()
//         {
//             return camera_.transform;
//         }
//
//         public virtual GameContolMode GetGameCameraType()
//         {
//             return GameContolMode.MainThirdPerson;
//         }
//
//         protected void LookVectorListener(InputAction.CallbackContext context)
//         {
//             lookVector = PlayerTempStateBulkQueryHelper.CanPlayerRotateCamera()
//                 ? context.ReadValue<Vector2>() * GameMain.I.GameStateService.SettingsSaveState.MouseSensitivity/5f
//                 : Vector2.zero;
//         }
//
//         protected void ResetLookVectorOnInputCancel(InputAction.CallbackContext context)
//         {
//             lookVector = Vector2.zero;
//         }
//
//         protected void ZoomActionListener(InputAction.CallbackContext context)
//         {
//             zoomAxis = context.ReadValue<float>();
//         }
//
//         protected void ResetZoomActionListenerOnCancel(InputAction.CallbackContext context)
//         {
//             zoomAxis = 0;
//         }
//
//         protected virtual void LateUpdate()
//         {
//         }
//
//         public void SetZoomLocationOfDialogueCamera()
//         {
//             //this will zoom in the camera and move it slightly to the left/right?
//         }
//     }
// }