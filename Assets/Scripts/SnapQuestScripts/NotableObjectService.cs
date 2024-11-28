using System.Collections.Generic;
using System.Linq;
using Code.PhysicsHelpers;
using UnityEngine;

namespace Code.CameraTool
{
    public class NotableObjectService : MonoBehaviour
    {

        public Camera MainCamera;
        public bool UsePartialFrustumForCameraDetection = true;
        public float Tightness = .20f;
        
        public delegate void NotableObjectInReticleAction(
            NotableObject notableObject
        );

        private readonly List<NotableObject> notableObjectsInScene_;

        public NotableObjectService()
        {
            notableObjectsInScene_ = new List<NotableObject>();
        }

        public NotableObject NotableObjectHighlighted
        {
            get;
            set;
            //for subscribers to this data, send the event?
        }

        public event NotableObjectInReticleAction NotableObjectInReticle;

        /// <summary>
        ///     Returns the object under the reticle by sending a raycast. Should happen every frame (or every other frame when
        ///     camera is active)
        ///     Also how to do this if looking in a mirror or other reflective object?
        /// </summary>
        /// <returns></returns>
        public NotableObject CastForNotableObjectOrNull()
        {
            return null;
        }

        /// <summary>
        ///     Unlike the raycast, this has to use isrender or bounding box to detect if at the point of taking a picture, the
        ///     item we are looking for exists in the photo.
        /// </summary>
        /// <returns></returns>
        public List<NotableObject> ReturnAllNotableObjectsInPictureOrNull()
        {
            var objectsInView = new List<NotableObject>();
            var cameraFrustumPlanes = new List<Plane>();
            if (!UsePartialFrustumForCameraDetection)
            {
                cameraFrustumPlanes = GeometryUtility.CalculateFrustumPlanes(Camera.main).ToList();
            }
            else
            {
                
                //kind of depends on area of the camera zone. (but currently hardcoded as 1/2 screen size)
                var bottomLeftRay = MainCamera.ViewportPointToRay(new Vector3(.25f + Tightness, .25f + Tightness, 0));
                var bottomRightRay = MainCamera.ViewportPointToRay(new Vector3(.75f- Tightness, .25f + Tightness, 0));
                var topLeftRay = MainCamera.ViewportPointToRay(new Vector3(.25f + Tightness, .75f - Tightness, 0));
                var topRightRay = MainCamera.ViewportPointToRay(new Vector3(.75f - Tightness, .75f - Tightness, 0));
                //now make the planes in left, right, down, up, near, far (skip the last one since distance allowed is measured elsewhere)
                // must go clockwise looking at top of surface of plane to have correct normal
                cameraFrustumPlanes.Add(new Plane(bottomLeftRay.origin, topLeftRay.origin, topLeftRay.GetPoint(1)));
                cameraFrustumPlanes.Add(
                    new Plane(bottomRightRay.origin, bottomRightRay.GetPoint(1), topRightRay.origin)
                );
                cameraFrustumPlanes.Add(
                    new Plane(bottomRightRay.origin, bottomLeftRay.origin, bottomLeftRay.GetPoint(1))
                );
                cameraFrustumPlanes.Add(new Plane(topLeftRay.origin, topRightRay.origin, topRightRay.GetPoint(1)));
            }
            //depends on the field of view, but if we want to go to half of that:

            foreach (var notableObject in notableObjectsInScene_)
            {
                if (notableObject != null && notableObject.IsCurrentlyDetectable &&
                    notableObject.gameObject.activeInHierarchy)
                {
                    if (NotableObjectIsInViewOfCameraFrustumsAndHasLOS(notableObject, cameraFrustumPlanes, MainCamera))
                    {
                        objectsInView.Add(notableObject);
                    }
                }
            }

            return objectsInView;
        }

        public NotableObject ReturnNotableObjectUsingRayCastMethod(Vector2 reticleScreenPos)
        {
            var ray = MainCamera.ScreenPointToRay(reticleScreenPos);
            Debug.DrawRay(ray.origin, ray.direction * 10, Color.green);
            NotableObject notableObject = null;
            if (Physics.Raycast(ray, out var hit, 200, PhysicsLayerManager.GetCameraNotableObjectIgnoreLayerMask())
            ) // if something hit, send the message
            {
                if (hit.collider.gameObject.TryGetComponent<NotableObject>(out var notableObject_))
                {
                    notableObject = notableObject_;
                }

                //else try to get a notableObjectCamDetectionCollider
                if (hit.collider.gameObject.TryGetComponent<NotableObjectCamDetectionCollider>(
                    out var
                        notableObjectCamDetectionCollider
                ))
                {
                    notableObject = notableObjectCamDetectionCollider.NotableObject;
                }
            }

            if (notableObject != null)
            {
                NotableObjectInReticle?.Invoke(notableObject);
                if (!notableObject.IsCurrentlyDetectable)
                {
                    return null;
                }
            }

            return notableObject;
        }

        public List<NotableObject> ReturnAllObjectsInScreenPixelBoxWithDirectSiteOrNull(
            Vector2 pixelsCenterOfBox,
            Vector2 pixelWidthHeight)
        {
            var bottomLeftRay = MainCamera.ScreenPointToRay(
                new Vector2(
                    pixelsCenterOfBox.x - pixelWidthHeight.x,
                    pixelsCenterOfBox.y - pixelWidthHeight.y
                )
            );
            var bottomRightRay = MainCamera.ScreenPointToRay(
                new Vector2(
                    pixelsCenterOfBox.x + pixelWidthHeight.x,
                    pixelsCenterOfBox.y - pixelWidthHeight.y
                )
            );
            var topLeftRay = MainCamera.ScreenPointToRay(
                new Vector2(
                    pixelsCenterOfBox.x - pixelWidthHeight.x,
                    pixelsCenterOfBox.y + pixelWidthHeight.y
                )
            );
            var topRightRay = MainCamera.ScreenPointToRay(
                new Vector2(
                    pixelsCenterOfBox.x + pixelWidthHeight.x,
                    pixelsCenterOfBox.y + pixelWidthHeight.y
                )
            );
            //now make the planes in left, right, down, up, near, far (skip the last one since distance allowed is measured elsewhere)
            // must go clockwise looking at top of surface of plane to have correct normal
            List<Plane> cameraFrustumPlanes = new();
            cameraFrustumPlanes.Add(new Plane(bottomLeftRay.origin, topLeftRay.origin, topLeftRay.GetPoint(1)));
            cameraFrustumPlanes.Add(new Plane(bottomRightRay.origin, bottomRightRay.GetPoint(1), topRightRay.origin));
            cameraFrustumPlanes.Add(new Plane(bottomRightRay.origin, bottomLeftRay.origin, bottomLeftRay.GetPoint(1)));
            cameraFrustumPlanes.Add(new Plane(topLeftRay.origin, topRightRay.origin, topRightRay.GetPoint(1)));

            Debug.DrawLine(bottomLeftRay.origin, bottomLeftRay.GetPoint(20));
            Debug.DrawLine(bottomRightRay.origin, bottomRightRay.GetPoint(20));
            Debug.DrawLine(topLeftRay.origin, topLeftRay.GetPoint(20));
            Debug.DrawLine(topRightRay.origin, topRightRay.GetPoint(20));

            var objectsInView = new List<NotableObject>();
            foreach (var notableObject in notableObjectsInScene_)
            {
                if (notableObject.IsCurrentlyDetectable)
                {
                    if (NotableObjectIsInViewOfCameraFrustumsAndHasLOS(notableObject, cameraFrustumPlanes, MainCamera))
                    {
                        objectsInView.Add(notableObject);
                    }
                }
            }

            return objectsInView;
        }

        //we should specify which of the colliders 
        private static bool NotableObjectIsInViewOfCameraFrustumsAndHasLOS(
            NotableObject notableObject,
            List<Plane> cameraFrustumPlanes,
            Camera MainCamera)
        {
            // check if we see ANY (some portion) of it the camera target colliders in the reticle planes box
            foreach (var cameraTargetCollider in notableObject.CameraTargetColliders)
            {
                //apply a scaler to the bounds to bring it in a little? 
                var scaledDownBounds = new Bounds(
                    cameraTargetCollider.bounds.center,
                    cameraTargetCollider.bounds.size // *.8f <- use this to scale in the collider
                );
                if (GeometryUtility.TestPlanesAABB(cameraFrustumPlanes.ToArray(), scaledDownBounds))
                {
                    if (NotableObjectBoundPointHasDirectLineOfSightToCameraBackPlane(
                        MainCamera.transform.position,
                        notableObject,
                        cameraTargetCollider.bounds,
                        cameraFrustumPlanes
                    ))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        //Raycast to screen from point of reticle to ensure only thing hit on the way is the object (not behind wall for example)
        private static bool NotableObjectBoundPointHasDirectLineOfSightToCameraBackPlane(
            Vector3 cameraPos,
            NotableObject notableObject,
            Bounds boundsInView,
            List<Plane> cameraFrustumPlanes)
        {
            //if the notable object has points that must be visible, then lets use that as well after we do a direct cast
            if (Physics.Linecast(
                cameraPos,
                boundsInView.center,
                out var hit,
                PhysicsLayerManager.GetCameraNotableObjectIgnoreLayerMask(),
                QueryTriggerInteraction.Ignore
            ))
            {
                //if the thing we hit has a notable object component, lets check to ensure it matches
                if (hit.collider.gameObject.TryGetComponent<NotableObject>(out var hitNotableObject))
                {
                    return hitNotableObject == notableObject;
                }

                //if the thing we hit does not have a NotableObject, thats a problem.
                return false;
            }

            //else we hit nothing, even the object we are checking, the collider is not wrapped right
            return true;
        }
        
        private static bool PointIsInViewOfCameraFrustums(List<Plane> cameraFrustumPlanes, Vector3 position)
        {
            // check each plane of the camera and insure - distance.
            foreach (var plane in cameraFrustumPlanes)
            {
                if (plane.GetDistanceToPoint(position) < 0)
                {
                    return false;
                }
            }

            return true;
        }

        public void RegisterSceneNotableObject(NotableObject notableObject)
        {
            notableObjectsInScene_.Add(notableObject);
        }

        public void UnregisterSceneNotableObject(NotableObject notableObject)
        {
            notableObjectsInScene_.Remove(notableObject);
        }
    }
}