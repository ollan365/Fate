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
            bool skipInquiry = (bool) GameManager.Instance.skipInquiry;
            if (isInquiry && !skipInquiry)
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

            //Debug.Log("현재 inquiryObjectId : " + GameManager.Instance.getCurrentInquiryObjectId());
        }
    }

    private void Start()
    {
        // 병합 후 에러 발생해서 일단 주석 처리!!!
        // EventObjectManager.Instance.AddEventObject(this);
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


    public bool GetIsFinished()
    {
        return isFinished;
    }

    public void SetIsFinished(bool isFinished)
    {
        this.isFinished = isFinished;
    }
}
