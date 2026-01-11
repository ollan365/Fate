using UnityEngine;

namespace Fate.Managers
{
    public class InquiryHandler : ExactMatchResultHandler
    {
        public InquiryHandler() : base("ResultInquiry") { }

        public override void Execute(string resultID)
        {
            if (GameManager.Instance != null)
            {
                string inquiryObjectId = GameManager.Instance.GetCurrentInquiryObjectId();
                if (!string.IsNullOrEmpty(inquiryObjectId) && !GameManager.Instance.GetEventStatus(inquiryObjectId))
                {
                    if (EventManager.Instance != null)
                    {
                        EventManager.Instance.CallEvent(inquiryObjectId);
                        GameManager.Instance.SetVariable("isInquiry", false);
                    }
                    else
                        Debug.LogWarning("ResultManager: EventManager.Instance is null, cannot call event");
                }
                else
                {
                    if (DialogueManager.Instance != null)
                        DialogueManager.Instance.StartDialogue("RoomEscape_Inquiry2");
                    else
                        Debug.LogWarning("ResultManager: DialogueManager.Instance is null, cannot start dialogue");
                }
            }
            else
                Debug.LogWarning("ResultManager: GameManager.Instance is null, cannot process inquiry");
        }
    }

    public class InquiryYesHandler : ExactMatchResultHandler
    {
        public InquiryYesHandler() : base("ResultInquiryYes") { }

        public override void Execute(string resultID)
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.SetVariable("isInquiry", true);
                string inquiryObjectId = GameManager.Instance.GetCurrentInquiryObjectId();
                if (!string.IsNullOrEmpty(inquiryObjectId))
                {
                    if (EventManager.Instance != null)
                        EventManager.Instance.CallEvent(inquiryObjectId);
                    else
                        Debug.LogWarning("ResultManager: EventManager.Instance is null, cannot call event");
                }
                GameManager.Instance.SetVariable("isInquiry", false);
            }
            else
                Debug.LogWarning("ResultManager: GameManager.Instance is null, cannot process inquiry yes");
        }
    }

    public class InquiryNoHandler : ExactMatchResultHandler
    {
        public InquiryNoHandler() : base("ResultInquiryNo") { }

        public override void Execute(string resultID)
        {
            if (GameManager.Instance != null)
                GameManager.Instance.SetVariable("isInquiry", false);
            else
                Debug.LogWarning("ResultManager: GameManager.Instance is null, cannot process inquiry no");
        }
    }

    public class BlanketCheckHandler : ExactMatchResultHandler
    {
        public BlanketCheckHandler() : base("ResultBlanketCheck") { }

        public override void Execute(string resultID)
        {
            if (GameManager.Instance != null && EventManager.Instance != null)
            {
                GameManager.Instance.SetVariable("isInquiry", true);
                GameManager.Instance.SetCurrentInquiryObjectId("EventBlanket");
                EventManager.Instance.CallEvent("Event_Inquiry");
            }
            else
                Debug.LogWarning("ResultManager: GameManager or EventManager instance is null");
        }
    }
}
