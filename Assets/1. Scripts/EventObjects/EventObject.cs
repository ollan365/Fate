using UnityEngine;

public class EventObject : MonoBehaviour
{
    [SerializeField]
    protected string eventId;

    protected void OnMouseDown()
    {
        if (!string.IsNullOrEmpty(eventId) && EventManager.Instance)
        {
            EventManager.Instance.CallEvent(eventId);
        }
    }

    protected void OnClick()
    {
        if (!string.IsNullOrEmpty(eventId) && EventManager.Instance)
        {
            EventManager.Instance.CallEvent(eventId);
        }
    }

    public string getEventId()
    {
        return eventId;
    }
}