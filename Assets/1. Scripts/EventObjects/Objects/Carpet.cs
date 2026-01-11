using UnityEngine;
using Fate.Managers;


namespace Fate.Events
{
    public class Carpet : EventObject, IResultExecutable
    {
        public bool isClosedCarpet; 
        public GameObject otherCarpet;
        public Collider2D objectBehindCollider;
        private string closedOrOpen;

        protected override void Awake()
        {
            base.Awake();
            closedOrOpen = isClosedCarpet ? "Closed" : "Open";
            ResultManager.Instance.RegisterExecutable($"{closedOrOpen}Carpet{sideNum}", this);
            if(objectBehindCollider!=null)
                objectBehindCollider = objectBehindCollider.GetComponent<Collider2D>();

            //Debug.Log($"{closedOrOpen}Carpet{sideNum}");

            //if(!isClosedCarpet)
            //    gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            UpdateImageState();
        }

        protected override bool CanInteract()
        {
            return !GameManager.Instance.GetIsBusy();
        }

        public void ExecuteAction()
        {
            //if (isClosedCarpet) GameManager.Instance.IncrementVariable("ClosedCarpetClick");
        
            ToggleCarpet();
        }

        private void ToggleCarpet()
        {
            if (objectBehindCollider != null)
                objectBehindCollider.enabled = isClosedCarpet;
            GameManager.Instance.InverseVariable("CarpetClosed");
            otherCarpet.SetActive(true);
            gameObject.SetActive(false);
        }

        private void UpdateImageState()
        {
            bool carpetClosed = (bool)GameManager.Instance.GetVariable("CarpetClosed");
            bool shouldActivateSelf = (carpetClosed == isClosedCarpet);

            gameObject.SetActive(shouldActivateSelf);
            otherCarpet.SetActive(!shouldActivateSelf);
        }

    }
}
