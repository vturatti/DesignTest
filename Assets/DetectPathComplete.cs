using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;

public class DetectPathComplete : MonoBehaviour
{
    public Seeker Seeker;
    public PlayMakerFSM FSMToSendEventTo;
    
    // Start is called before the first frame update
    void Start()
    {
        Seeker.pathCallback += PathCompletedSendFSMEvent;
    }

    private void PathCompletedSendFSMEvent(Path p)
    {
        FSMToSendEventTo.SendEvent("PathComplete");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
