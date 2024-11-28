using System.Collections.Generic;
using Pathfinding;
using UnityEngine;

public class DetectPathComplete : MonoBehaviour
{
    public Seeker Seeker;
    public AIDestinationSetter DestinationSetter;
    public PlayMakerFSM FSMToSendEventTo;

    public List<Transform> Locations = new ();
    public int currentLoc;
    
    // Start is called before the first frame update
    void Start()
    {
        Seeker.pathCallback += PathCompletedSendFSMEvent;
    }

    private void PathCompletedSendFSMEvent(Path p)
    {
        FSMToSendEventTo.SendEvent("PathComplete");
        GoToNextLocation();
    }

    // Update is called once per frame
    public void GoToNextLocation()
    {
        currentLoc++;
        if (currentLoc >= Locations.Count)
        {
            currentLoc = 0;
        }
        //set loc
        DestinationSetter.Target = Locations[currentLoc];
    }
}
