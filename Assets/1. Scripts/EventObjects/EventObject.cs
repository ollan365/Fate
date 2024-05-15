using UnityEngine;

public class EventObject : MonoBehaviour
{
    [SerializeField]
    protected string eventId;

    [SerializeField]
    protected int sideNum;

    [SerializeField] protected bool isInquiry = false;

    protected bool isFinished = false;

    protected void OnMouseDown()
    {
        if (!string.IsNullOrEmpty(eventId) && EventManager.Instance)
        {
            if (isInquiry)
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

            //Debug.Log("현재 inquiryObjectId : " + GameManager.Instance.getCurrentInquiryObjectId());
        }
    }

    private void Start()
    {
        EventObjectManager.Instance.AddEventObject(this);
    }

    public string GetEventId()
    {
        return eventId;
    }

    public int GetSideNum()
    {
        return sideNum;
    }

    public bool GetIsFinished()
    {
        return isFinished;
    }

    public void SetIsFinished(bool isFinished)
    {
        this.isFinished = isFinished;
    }
}
