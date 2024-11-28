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
    public bool IsCurrentlyDetectable = true;
    public float MaxDistanceDetectableZoomDistanceMultiplier = 1;
    private NotableObjectService NotableObjectService;

    public List<Collider> CameraTargetColliders = new ();

    [Tooltip(
        "These points must ALL be visible for the camera to count it as detecting notable object" +
        "\nIf null we use a straight ray from camera to try to hit collider"
    )]
    private Collider collider_;
    public PlayMakerFSM LinkedFSM;

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
        NotableObjectService = GameObject.FindWithTag("GameMain").GetComponent<NotableObjectService>();
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
        LinkedFSM.SendEvent("OnExitCameraFocus");
    }

    public void CameraToolOnTarget()
    {
        LinkedFSM.SendEvent("OnEnterCameraFocus");
    }

    public void SendCapturedEvent()
    {
        if (LinkedFSM != null)
        {
            LinkedFSM.SendEvent("Captured");
        }
    }
}