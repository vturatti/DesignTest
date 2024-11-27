using Cinemachine;
using Code.CameraTool;
using UnityEngine;

public class TripodObject : MonoBehaviour
{
    public CinemachineVirtualCamera VirtualCamera;
    public GameObject CameraLegs;
    public GameObject TopOfStand;
    public GameObject CameraPivot;
    public float ScaleOfLegsStanding = 1f;
    public float ScaleOfLegsCrouch = .7f;
    private CameraTripod parentScript_;

    public void AdjustHeightAndSetPivotAtTopOfStand()
    {
        CameraLegs.transform.localScale = Vector3.one;
        CameraPivot.transform.position = TopOfStand.transform.position;
    }
    
    public void PickupTripod()
    {
        gameObject.SetActive(false);

        parentScript_.TripodDeployed = false;
    }

    public void SetParentScript(CameraTripod cameraTripod)
    {
        parentScript_ = cameraTripod;
    }
}