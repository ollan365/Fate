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
            bool skipInquiry = (bool) GameManager.Instance.skipInquiry;
            if (isInquiry && !skipInquiry)
            {
                GameManager.Instance.SetVariable("isInquiry", true);
                GameManager.Instance.setCurrentInquiryObjectId(eventId);
                EventManager.Instance.CallEvent("Event_Inquiry");
            }
            else
            {
                GameManager.Instance.SetVariable("isInquiry", false);
                GameManager.Instance.setCurrentInquiryObjectId(null);
                EventManager.Instance.CallEvent(eventId);
            }

            Debug.Log("ÇöÀç inquiryObjectId : " + GameManager.Instance.getCurrentInquiryObjectId());
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
