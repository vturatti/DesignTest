using UnityEngine;

public class BaseCameraFocusReactor : MonoBehaviour
{
    public PlayMakerFSM EventReceiverFsm;

    public void CameraToolOnTarget()
    {
        EventReceiverFsm.SendEvent("OnEnterCameraFocus");
    }

    public void ExitingCameraView()
    {
        EventReceiverFsm.SendEvent("OnExitCameraFocus");
    }
}
