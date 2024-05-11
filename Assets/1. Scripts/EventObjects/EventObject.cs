using UnityEngine;

public class EventObject : MonoBehaviour
{
    [SerializeField]
    protected string eventId;

    [SerializeField]
    protected int sideNum;

    [SerializeField] protected bool isInquiry = false;

    protected void OnMouseDown()
    {
        if (!string.IsNullOrEmpty(eventId) && EventManager.Instance)
        {
            eventId = isInquiry ? "Event_Inquiry" : eventId;
            EventManager.Instance.CallEvent(eventId);
        }
    }

    public string GetEventId()
    {
        return eventId;
    }

    public int GetSideNum()
    {
        return sideNum;
    }
}