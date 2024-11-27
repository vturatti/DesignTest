using UnityEngine;

namespace Code.PhysicsHelpers
{
    public class PhysicsLayerManager
    {
        public static int GetCameraNotableObjectIgnoreLayerMask()
        {
            //bit shift all that you want to avoid
            var cullingMask = 1 << LayerMask.NameToLayer("Player");
            cullingMask = ~cullingMask;
            return cullingMask;
        }
    }
}