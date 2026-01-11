using Fate.Managers;
using UnityEngine;

namespace Fate.Events
{
    public class EventObject : MonoBehaviour
    {
        [SerializeField]
        protected string eventId;

        [SerializeField]
        protected int sideNum;

        [SerializeField] protected bool isInquiry = true;

        protected virtual bool CanInteract()
        {
            return true;
        }

        protected void OnMouseDown()
        {
            if (!CanInteract())
                return;

            if (string.IsNullOrEmpty(eventId))
            {
                Debug.LogWarning($"EventObject: OnMouseDown called but eventId is null or empty on {gameObject.name}");
                return;
            }

            if (EventManager.Instance == null)
            {
                Debug.LogWarning($"EventObject: EventManager.Instance is null, cannot call event '{eventId}' on {gameObject.name}");
                return;
            }

            if (GameManager.Instance == null)
            {
                Debug.LogWarning($"EventObject: GameManager.Instance is null, cannot process event '{eventId}' on {gameObject.name}");
                return;
            }

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

        protected virtual void Awake()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.AddEventObject(this);
            }
            else
            {
                Debug.LogWarning($"EventObject: GameManager.Instance is null in Awake on {gameObject.name}, cannot register event object");
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

        public bool GetIsInquiry()
        {
            return isInquiry;
        }

        public void SetIsInquiry(bool isInquiry)
        {
            this.isInquiry = isInquiry;
        }

        protected void SetCurrentLockObjectCanvasGroup(GameObject lockObject)
        {
            if (lockObject == null)
            {
                Debug.LogWarning($"EventObject: SetCurrentLockObjectCanvasGroup called with null lockObject on {gameObject.name}");
                return;
            }

            RoomManager roomManager = RoomManager.Instance;
            if (roomManager == null)
            {
                Debug.LogWarning($"EventObject: RoomManager.Instance is null on {gameObject.name}");
                return;
            }

            CanvasGroup canvasGroup = lockObject.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                Debug.LogWarning($"EventObject: lockObject does not have CanvasGroup component on {gameObject.name}");
                return;
            }

            if (roomManager.imageAndLockPanelManager == null)
            {
                Debug.LogWarning($"EventObject: RoomManager.imageAndLockPanelManager is null on {gameObject.name}");
                return;
            }

            roomManager.imageAndLockPanelManager.currentLockObjectCanvasGroup = canvasGroup;
            if (DialogueManager.Instance != null && DialogueManager.Instance.isDialogueActive)
                canvasGroup.blocksRaycasts = false;
        }
    }
}
