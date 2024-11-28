using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Code.Canvas;
using Code.NPCs;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Code.CameraTool
{
    public class CameraToolService : MonoBehaviour
    {
        public delegate void NotableObjectAction(
            NotableObject notableObject
        );

        private readonly int defaultCameraFieldOfView = 60;
        private readonly bool isRaycastVersionInsteadOfThickBox_ = true;
        private bool checkedIfPhotoFileEsistsAndIfNotCreateIt;
        private RenderTexture renderTexture_;
        private NotableObject reticleDeterminedMostNotableObject_;
        private bool takePictureInLateUpdate_;

        public Transform PlayerRoot;
        public Volume GlobalPPFXVolume;
        public float CameraMouseSensitivity = 15;

        public CameraTripod cameraTripod_;
        public AudioSource CameraClickSFX;

        public bool UsingCameraTool;
        public PlayerInput PlayerInput;
        // public InputAction PlayerInput;
        public CameraToolUI CameraToolUI;
        public NotableObjectService NotableObjectService;

        public CinemachineVirtualCamera CameraToolVcamCamera;
        public CinemachineVirtualCamera TPCharacterVCamera;
        public Transform CameraToolTarget;
        public Vector2 lookVector;

        private void Start()
        {
            PlayerInput.actions["CameraTool"].started += ToggleCameraTool;
            PlayerInput.actions["TakePicture"].started += TakePictureInputHandler;
            PlayerInput.actions["Look"].performed += LookVectorListener;
            
            Cursor.lockState = CursorLockMode.Locked;
        }

        protected void ToggleCameraTool(InputAction.CallbackContext obj)
        {
            if (!UsingCameraTool)
            {
                ActivateCameraTool();
            }
            else
            {
                DeactivateCameraTool();
            }
        }
        
        protected void LookVectorListener(InputAction.CallbackContext context)
        {
            if (!CameraToolUI.RootNode.activeSelf)
                return;
            var lookV = context.ReadValue<Vector2>() * CameraMouseSensitivity;
            MoveReticle(lookV);
        }
        
        
        public float minX = -40f;  // minimum X rotation (upwards)
        public float maxX = 70f; 
        
        private void MoveReticle(Vector2 moveDistance)
        {
            // Define your min and max rotation limits
            // maximum X rotation (downwards)
            

            // minus for invert, + for non
            if (CameraToolVcamCamera == null)
            {
                return;
            }
            var currentX = CameraToolVcamCamera.transform.localRotation.eulerAngles.x;
            currentX -= moveDistance.y;

            // Ensure the camera's X rotation stays within the specified range
            // The eulerAngles are in the range 0-360, so we need to adjust for negative rotations.
            if (currentX > 180f) {
                currentX -= 360f;
            }
            
            // Normalize to -180 to 180 range
            // Clamp the rotation between minX and maxX
            currentX = Mathf.Clamp(currentX, minX, maxX);

             // Apply the rotation back to the camera, ensuring it doesn't exceed the limits
            CameraToolVcamCamera.transform.localRotation = Quaternion.Euler(currentX, 0,0);
            
            PlayerRoot.Rotate(Vector3.up, moveDistance.x);

        }

        public CameraTripod GetCameraTripod()
        {
            return cameraTripod_;
        }

        public bool ActivateCameraTool()
        {
            
            //activate the camera tool render camera
            if (GetCameraTripod() != null && GetCameraTripod().TripodDeployed)
            {
                TPCharacterVCamera.Priority = 0;
            }
            else
            {
                TPCharacterVCamera.Priority = 0;
                CameraToolVcamCamera.Priority = 10;
            }


            Cursor.lockState = CursorLockMode.Locked;
            UsingCameraTool = true;
            CameraToolUI.RequestEnableUI();
            return true;
        }

        public bool DeactivateCameraTool()
        {
            if (GetCameraTripod() != null && GetCameraTripod().TripodDeployed)
            {
                cameraTripod_.TripodObject.VirtualCamera.enabled = false;
            }

            CameraToolUI.DisableUI();
            UsingCameraTool = false;
            
            
            // TPCharacterCamera.Activate();
            // CameraToolRenderCamera.Deactivate();
            TPCharacterVCamera.Priority = 10;
            CameraToolVcamCamera.Priority = 0;

            if (reticleDeterminedMostNotableObject_ != null)
            {
                reticleDeterminedMostNotableObject_.ExitingCameraView();
                reticleDeterminedMostNotableObject_ = null;
            }

            return true;
        }

        
        public void TakePicture()
        {
            var notableObjects = DetectWhatsInScreen();
            var mostNotable = reticleDeterminedMostNotableObject_;

            if (mostNotable != null)
            {
                var nameActionText =
                    $"{mostNotable.DisplayName}";
                
                mostNotable.SendCapturedEvent();
            }
        }
        public List<NotableObject> DetectWhatsInScreen()
        {
            var detectedNotableObjects = new List<NotableObject>();
            var notableObjectDebug = "";
            foreach (var notableObject in NotableObjectService.ReturnAllNotableObjectsInPictureOrNull())
            {
                notableObjectDebug +=
                    $" there is a {notableObject.DisplayName} in the photo with tags {string.Join(",", notableObject.tags)}\n";
                detectedNotableObjects.Add(notableObject);
            }

            return detectedNotableObjects;
        }

        public void MoveIncrementalCenterReticle(Vector2 amount)
        {
            throw new NotImplementedException();
        }

        public void MoveCenterReticleTo(Vector2 pos)
        {
            throw new NotImplementedException();
        }

        public void OnDestroy()
        {
            if (PlayerInput == null)
            {
                return;
            }
            PlayerInput.actions["TakePicture"].started -= TakePictureInputHandler;
            PlayerInput.actions["CameraTool"].started -= ToggleCameraTool;
            PlayerInput.actions["Look"].performed -= LookVectorListener;
        }

        public event NotableObjectAction NotableObjectInCameraSight;

        public void Update()
        {
            UpdateFocusObjectWhenActive();
        }

        public void UpdateFocusObjectWhenActive()
        {
            var closestDistance = float.MaxValue;
            if (UsingCameraTool)
            {
                NotableObject mostNotableObj = null;
                // if we are doing the raycast version
                if (isRaycastVersionInsteadOfThickBox_)
                {
                    mostNotableObj = NotableObjectService.ReturnNotableObjectUsingRayCastMethod(
                        CameraToolUI.GetReticleScreenPos()
                    );
                }

                //trigger change event
                if (reticleDeterminedMostNotableObject_ != mostNotableObj)
                {
                    if (reticleDeterminedMostNotableObject_ != null)
                    {
                        reticleDeterminedMostNotableObject_.ExitingCameraView();
                    }
                }

                if (mostNotableObj != null)
                {
                    mostNotableObj.CameraToolOnTarget();
                }

                reticleDeterminedMostNotableObject_ = mostNotableObj;

                if (mostNotableObj == null)
                {
                    SetPrimaryNotableObject(null,
                        false,
                        false,
                        false);
                    return;
                }

                closestDistance = Vector3.Distance(
                    mostNotableObj.transform.position,
                    PlayerRoot.position
                );
                
                SetPrimaryNotableObject(
                    mostNotableObj,
                    false,
                    false,
                    closestDistance > 50 *
                    mostNotableObj.MaxDistanceDetectableZoomDistanceMultiplier
                );

                NotableObjectInCameraSight?.Invoke(mostNotableObj);
            }
        }

        public void SetPrimaryNotableObject(
            NotableObject notableObject,
            bool isKnownNotableObject,
            bool isRecordedActionState,
            bool isTooFar)
        {
            if (notableObject == null)
            {
                CameraToolUI.PrimaryTargetDetails.text = "";
                return;
            }
            CameraToolUI.PrimaryTargetDetails.text = notableObject.DisplayName;
        }
        
        private IEnumerator CameraTakePictureEffect()
        {
            Vignette vg;
            var ppv = GlobalPPFXVolume.profile;
            ppv.TryGet(out vg);

            if (vg == null)
            {
                throw new Exception(
                    "Must have global PPFX volume in scene with vignette (even if off) and assigned to scene master entity (check downtown for demo)"
                );
            }

            var startActive = vg.active;
            var startIntensity = .3f;
            if (!startActive)
            {
                startIntensity = 0;
            }

            // Use the QuickVolume method to create a volume with a priority of 100, and assign the vignette to this volume
            // Use the QuickVolume method to create a volume with a priority of 100, and assign the vignette to this volume
            var effectDurationClose = 0.0f;
            float shutterSpeed = .1f;
            while (effectDurationClose < shutterSpeed)
            {
                effectDurationClose += Time.deltaTime;
                var val = Mathf.Lerp(startIntensity, .55f, effectDurationClose / shutterSpeed);
                vg.intensity.value = val;
                yield return new WaitForEndOfFrame();
            }

            effectDurationClose = 0.0f;
            while (effectDurationClose < shutterSpeed)
            {
                effectDurationClose += Time.deltaTime;
                var val = Mathf.Lerp(.55f, startIntensity, effectDurationClose / shutterSpeed);
                vg.intensity.value = val;
                yield return new WaitForEndOfFrame();
            }

            vg.intensity.value = startIntensity;
        }

        public NotableObject GetMostFocusedNotableObject(List<NotableObject> notableObjectsInScreenAndVisible)
        {
            if (notableObjectsInScreenAndVisible == null || notableObjectsInScreenAndVisible.Count == 0)
            {
                return null;
            }

            NotableObject currentMostNotable = null;
            var shortestDistanceSoFar = float.MaxValue;
            var startPos = CameraToolTarget.position;
            var reticleRayScreenToWorld = ReticleRayScreenToWorld();

            foreach (var notableObject in notableObjectsInScreenAndVisible)
            {
                var endPos = reticleRayScreenToWorld.origin + reticleRayScreenToWorld.direction *
                    (notableObject.MaxDistanceDetectableZoomDistanceMultiplier *
                     50);
                var ray = new Ray(startPos, endPos);
                var distancePointLine = Vector3.Cross(ray.direction, notableObject.GetCenterPosition() - ray.origin)
                    .magnitude;
                if (shortestDistanceSoFar > distancePointLine)
                {
                    shortestDistanceSoFar = distancePointLine;
                    currentMostNotable = notableObject;
                }
            }

            return currentMostNotable;
        }
        
        public Ray ReticleRayScreenToWorld()
        {
            var ray = Camera.main.ScreenPointToRay(
                new Vector3(
                    Screen.width / 2,
                    Screen.height / 2,
                    0
                )
            );
            Debug.DrawRay(ray.origin, ray.direction * 10, Color.cyan);
            return ray;
        }

        public void ToggleCamera()
        {
            if (UsingCameraTool)
            {
                DeactivateCameraTool();
            }
            else
            {
                ActivateCameraTool();
            }
        }

        private void TakePictureInputHandler(InputAction.CallbackContext context)
        {
            if(CameraToolUI.RootNode.activeSelf)
                TakePicture();
        }

        public static float GetCameraFOVMultiplierForZoom(float zoom)
        {
            zoom = Math.Max(zoom, 1);
            return (float)(1 / Math.Pow(2, zoom - 1));
        }

        public float GetCameraFOVForCurrentZoom()
        {
            return GetCameraFOVMultiplierForZoom(1);
        }

        public static float GetCameraFOVForZoomLevel(float zoomLevel)
        {
            return GetCameraFOVMultiplierForZoom(zoomLevel);
        }
    }
}