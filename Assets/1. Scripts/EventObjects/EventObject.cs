using UnityEngine;

public class EventObject : MonoBehaviour
{
    [SerializeField]
    protected string eventId;

    [SerializeField]
    protected int sideNum;

    protected void OnMouseDown()
    {
        if (!string.IsNullOrEmpty(eventId) && EventManager.Instance)
        {
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