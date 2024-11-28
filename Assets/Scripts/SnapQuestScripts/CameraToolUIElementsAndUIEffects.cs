using System.Collections.Generic;
using Code.Canvas;
using Code.NPCs;
using UnityEngine;

namespace Code.CameraTool
{
    //shows the reticle, zoom info, target in sight, flash effect on picture taken, etc.
    public class CameraToolUIElementsAndUIEffects
    {
        public CameraToolUI cameraToolUI_;
        private RectTransform reticleRectTransform_;

        public CameraToolUIElementsAndUIEffects()
        {
            reticleRectTransform_ = cameraToolUI_.CameraToolCrosshair.GetComponent<RectTransform>();
        }

        public void EnableCameraToolUIEffects()
        {
            cameraToolUI_.PrimaryTargetDetails.text = "";
            cameraToolUI_.SecondaryTargetsDetails.text = "";
        }

        public void MoveReticleUI(Vector2 reticlePos)
        {
            if (reticleRectTransform_ == null)
            {
                reticleRectTransform_ = cameraToolUI_.CameraToolCrosshair.GetComponent<RectTransform>();
            }
            reticleRectTransform_.anchoredPosition = reticlePos;
        }

        public void SetPrimaryNotableObject(
            NotableObject notableObject,
            bool isKnownNotableObject,
            bool isKnownActionState,
            bool isTooFar)
        {
            if (notableObject == null)
            {
                cameraToolUI_.PrimaryTargetDetails.text = "";
                cameraToolUI_.CameraToolCrosshair.color = Color.white;
                cameraToolUI_.CameraToolCrosshair.rectTransform.localScale = Vector3.one;
                foreach (var corner in cameraToolUI_.CrosshairBoxCorners)
                {
                    corner.color = new Color(1,1,1,.5f);
                }
            }
            else if (isTooFar)
            {
                cameraToolUI_.PrimaryTargetDetails.text = "<i><size=60%>subject too far</size></i>";
                cameraToolUI_.CameraToolCrosshair.color = new Color(1f, 0.19f, 0.22f, 0.35f);
                cameraToolUI_.CameraToolCrosshair.rectTransform.localScale = Vector3.one;
                foreach (var corner in cameraToolUI_.CrosshairBoxCorners)
                {
                    corner.color = new Color(1f, 0.19f, 0.22f, 0.35f);
                }
            }
            else if (!isKnownNotableObject)
            {
                cameraToolUI_.PrimaryTargetDetails.text = "???";
                cameraToolUI_.CameraToolCrosshair.color = new Color(0.49f, 1f, 0.93f, 0.25f);
                cameraToolUI_.CameraToolCrosshair.rectTransform.localScale = new Vector3(1.3f, 1.3f, 0);
                foreach (var corner in cameraToolUI_.CrosshairBoxCorners)
                {
                    corner.color = new Color(0.49f, 1f, 0.93f, 0.35f);
                }
            }
            else
            {
                var txt = notableObject.DisplayName;

                cameraToolUI_.CameraToolCrosshair.color =
                    isKnownActionState ? new Color(0.88f, 0.6f, 0.27f, 0.35f) : new Color(0.61f, 1f, 0.22f, 0.35f);
                //set larger?
                cameraToolUI_.CameraToolCrosshair.rectTransform.localScale = new Vector3(1.3f, 1.3f, 0);
                cameraToolUI_.PrimaryTargetDetails.text = txt;
                foreach (var corner in cameraToolUI_.CrosshairBoxCorners)
                {
                    corner.color = cameraToolUI_.CameraToolCrosshair.color;
                }
            }
        }

        public void SetSecondaryNotableObjects(List<NotableObject> notableObjects)
        {
            var txt = "";
            if (notableObjects == null || notableObjects.Count == 0)
            {
                cameraToolUI_.SecondaryTargetsDetails.text = txt;
                return;
            }

            txt += "Also In Photo:\n";
            foreach (var notableObject in notableObjects)
            {
                txt += notableObject.DisplayName + "\n";
            }

            cameraToolUI_.SecondaryTargetsDetails.text = txt;
        }
    }
}