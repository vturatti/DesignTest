using System.Collections.Generic;
using Cinemachine;
using Code.Interactables;
using UnityEngine;

public class BaseInteractable : MonoBehaviour, IInteractable
{
    public Sprite IconToDisplay;
    public string TextToDisplay;
    public float InteractDistance = 1.25f;
    public bool IsInteractableOnStart = true;
    public bool ExemptFromOutline;
    public bool IsInteractable;
    public CinemachineVirtualCamera VirtualCamera;
    public PlayMakerFSM EventListenerFsm;

    // does not actually use this position directly,
    // but rather averages player and this to get a mid position
    public Transform InteractCameraFocusPosition;

    public List<Outline> Outlines = new();
    private CinemachineVirtualCamera tempVcamForInteraction_;

    public InteractableDetector InteractableDetector;
    public InteractionManager InteractionManager;
    private void Awake()
    {
        IsInteractable = IsInteractableOnStart;
        if (VirtualCamera != null)
        {
            VirtualCamera.enabled = false;
        }

        if (EventListenerFsm == null)
        {
            EventListenerFsm = GetComponent<PlayMakerFSM>();
        }
    }

    protected void Start()
    {
        if (Outlines.Count == 0 && !ExemptFromOutline)
        {
            Debug.LogWarning(
                $"WARNING!!!. The interactable object {name} has no outline object. If you dont want an outline on an interactable, ensure that you set ExemptFromOutline to true."
            );
        }

        DisableAllOutlines();
        gameObject.layer = LayerMask.NameToLayer("Interactable");
        InteractableDetector.RegisterInteractableObject(this, transform);
    }

    private void OnDestroy()
    {
        InteractableDetector.UnregisterInteractableObject(transform);
    }

    private void OnDisable()
    {
        InteractableDetector.UnregisterInteractableObject(transform);
    }

    private void OnEnable()
    {
        InteractableDetector.RegisterInteractableObject(this, transform);
    }

    //this is called by the interaction Manager ONLY
    public virtual void HandleInteraction()
    {
        if (EventListenerFsm != null)
        {
            EventListenerFsm.SendEvent("StartInteract");
        }
    }

    public void SetCurrentInteractable()
    {
        //set as current interactable in manager
        InteractionManager.SetCurrentInteractableTarget(this);
    }

    public virtual void EndInteraction()
    {
        if (!InteractionManager.IsInInteraction)
        {
            return;
        }
        InteractionManager.EndInteraction();
        if (VirtualCamera != null)
        {
            VirtualCamera.enabled = false;
            VirtualCamera.Priority = 0;
        }
    }

    public void UpdateFSMForEventListening(PlayMakerFSM fsm)
    {
        EventListenerFsm = fsm;
    }

    public void SetInteractable(bool isInteractable)
    {
        IsInteractable = isInteractable;
    }

    public bool GetInteractableAtThisTime()
    {
        return IsInteractable;
    }

    public void EnableAllOutlines()
    {
        foreach (var outline in Outlines)
        {
            outline.enabled = true;
        }
    }

    public void DisableAllOutlines()
    {
        foreach (var outline in Outlines)
        {
            outline.enabled = false;
        }
    }
}