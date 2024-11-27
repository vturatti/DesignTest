using System.Collections;
using System.Collections.Generic;
using Code.CameraTool;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Code.Canvas
{
    public class CameraToolUI : MonoBehaviour
    {
        public Image CameraToolCrosshair;
        public AudioSource CameraZoomSFX;
        public List<Image> CrosshairBoxCorners;
        public GameObject TripodHints;
        public GameObject NonTripodHints;
        public GameObject PlaceTripodHint;
        public TextMeshProUGUI PrimaryTargetDetails;
        public TextMeshProUGUI SecondaryTargetsDetails;
        public GameObject RootNode;
        public CameraToolService CameraToolService;

        private void ToggleTripodOut(bool TripodDeployed)
        {
            TripodHints.SetActive(TripodDeployed);
            NonTripodHints.SetActive(TripodDeployed);
        }

        public void PreEnableInitialization()
        {
            CursorService.LockCursor();
        }
        
        public void DisableUI()
        {
            //CameraZoomSFX.Stop();
            RootNode.SetActive(false);
        }

        public void RequestEnableUI()
        {
            RootNode.SetActive(true);
            //TripodHints.SetActive(CameraToolService.GetCameraTripod().TripodDeployed);
            //NonTripodHints.SetActive(true);
        }

        public Vector2 GetReticleScreenPos()
        {
            return CameraToolCrosshair.transform.position;
        }
    }
}