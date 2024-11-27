using System;
using UnityEngine;

namespace Code.CameraTool
{
    [RequireComponent(typeof(Collider))]
    public class NotableObjectCamDetectionCollider : MonoBehaviour
    {
        public NotableObject NotableObject;

        private void Start()
        {
            if (NotableObject == null)
            {
                throw new Exception(
                    $"Forgot to assign notable object to notableObjectCamDetectionCollider on {gameObject}"
                );
            }
        }
    }
}