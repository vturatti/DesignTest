using System.Collections.Generic;
using Code.CameraTool;
using Code.NPCs;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class NotableObject : MonoBehaviour
{
    public List<NotableObjectTag> tags;
    public string DisplayName;
    public TextMeshProUGUI DisplayFont;
    public string ObjectName;
    public bool IsCurrentlyDetectable = true;
    public float MaxDistanceDetectableZoomDistanceMultiplier = 1;
    public BaseCameraFocusReactor BaseCameraFocusReactor;
    public NotableObjectService NotableObjectService;

    public List<Collider> CameraTargetColliders = new ();

    [Tooltip(
        "These points must ALL be visible for the camera to count it as detecting notable object" +
        "\nIf null we use a straight ray from camera to try to hit collider"
    )]
    public List<Transform> MustBeVisiblePoints = new ();

    public ActionState ActionState = ActionState.NoneOrDefault;
    private Collider collider_;
    public Dictionary<string, string> ExtraData;
    public bool DontSaveScreenshotOnPictureTaken;

    public PlayMakerFSM OnCaptureFSM;
    public string FSMMessage = "Captured";

    public void Awake()
    {
        collider_ = GetComponent<Collider>();
        if (CameraTargetColliders.Count == 0)
        {
            CameraTargetColliders.Add(collider_);
        }
    }


    public void Start()
    {
        if (NotableObjectService != null)
        {
            NotableObjectService.RegisterSceneNotableObject(this);
        }
        else
        {
            Debug.LogError("GameMain.I.NotableObjectService == null");
        }
    }

    public void OnDestroy()
    {
        NotableObjectService.UnregisterSceneNotableObject(this);
    }

    //called through PM
    public void SetCurrentActionState(ActionState newState)
    {
        ActionState = newState;
    }

    public ActionState GetCurrentActionState()
    {
        return ActionState;
    }

    public void ToggleCurrentlyDetectable(bool enableNotableObject)
    {
        IsCurrentlyDetectable = enableNotableObject;
    }

    public Vector3 GetCenterPosition()
    {
        return collider_.bounds.center;
    }

    public List<Vector3> GetBoundingPositions()
    {
        var boundPositions = new List<Vector3>();
        boundPositions.Add(GetCenterPosition());
        return boundPositions;
    }

    public void ExitingCameraView()
    {
        if (BaseCameraFocusReactor != null)
        {
            BaseCameraFocusReactor.ExitingCameraView();
        }
    }

    public void CameraToolOnTarget()
    {
        if (BaseCameraFocusReactor != null)
        {
            BaseCameraFocusReactor.CameraToolOnTarget();
        }
    }

    public void SendCapturedEvent()
    {
        if (OnCaptureFSM != null)
        {
            OnCaptureFSM.SendEvent(FSMMessage);
        }
    }
}