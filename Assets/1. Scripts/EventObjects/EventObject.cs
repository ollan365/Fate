using UnityEngine;

public class EventObject : MonoBehaviour
{
    [SerializeField]
    protected string eventId;

    [SerializeField]
    protected int sideNum;

    [SerializeField] protected bool isInquiry = true;

    protected void OnMouseDown() {
        if (string.IsNullOrEmpty(eventId) || !EventManager.Instance)
            return;

        if (isInquiry && !GameManager.Instance.skipInquiry)
        {
            GameManager.Instance.SetVariable("isInquiry", true);
            GameManager.Instance.SetCurrentInquiryObjectId(eventId);
            EventManager.Instance.CallEvent("Event_Inquiry");
        }
        else
        {
            GameManager.Instance.SetVariable("isInquiry", false);
            GameManager.Instance.SetCurrentInquiryObjectId(null);
            EventManager.Instance.CallEvent(eventId);
        }
    }

    private void Awake()
    {
        GameManager.Instance.AddEventObject(this);
    }

    public string GetEventId()
    {
        return eventId;
    }

    public int GetSideNum()
    {
        return sideNum;
    }

    public bool GetIsInquiry()
    {
        return isInquiry;
    }

    public void SetIsInquiry(bool isInquiry)
    {
        this.isInquiry = isInquiry;
    }
}
