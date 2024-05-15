using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventObjectManager : MonoBehaviour
{
    public static EventObjectManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private Dictionary<string, bool> eventObjectsStatusDict = new Dictionary<string, bool>();

    public void AddEventObject(EventObject eventObject)
    {
        if (!eventObjectsStatusDict.ContainsKey(eventObject.GetEventId()))
        {
            eventObjectsStatusDict.Add(eventObject.GetEventId(), eventObject.GetIsFinished());
        }
    }

    public void SetEventFinished(string eventId)
    {
        if (eventObjectsStatusDict.ContainsKey(eventId))
        {
            eventObjectsStatusDict[eventId] = true;
        }
    }

    public bool GetEventStatus(string eventId)
    {
        if (eventObjectsStatusDict.ContainsKey(eventId))
        {
            return eventObjectsStatusDict[eventId];
        }
        else
        {
            return false;
        }
    }
}
