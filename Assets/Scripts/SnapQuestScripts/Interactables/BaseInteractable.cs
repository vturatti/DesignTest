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
    [HideInInspector]
    public bool IsInteractable;
    public PlayMakerFSM EventListenerFsm;

    // does not actually use this position directly,
    // but rather averages player and this to get a mid position
    public List<Outline> Outlines = new();

    private InteractableDetector InteractableDetector;
    private InteractionManager InteractionManager;
    private void Awake()
    {
        IsInteractable = IsInteractableOnStart;

        if (EventListenerFsm == null)
        {
            EventListenerFsm = GetComponent<PlayMakerFSM>();
        }
    }

    protected void Start()
    {
        InteractionManager = GameObject.FindWithTag("GameMain").GetComponent<InteractionManager>();
        InteractableDetector = GameObject.FindWithTag("GameMain").GetComponent<InteractableDetector>();
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
        if(InteractableDetector != null)
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