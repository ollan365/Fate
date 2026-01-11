using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fate.Managers;


namespace Fate.Events
{
    public class Calendar : EventObject, IResultExecutable
    {
        [SerializeField]
        private GameObject calendarPanel;

        private void Start() {
            RegisterWithResultManager();
        }

        private void OnEnable()
        {
            RegisterWithResultManager();
        }

        private void RegisterWithResultManager()
        {
            if (ResultManager.Instance != null)
                ResultManager.Instance.RegisterExecutable("Calendar", this);
        }

        protected override bool CanInteract()
        {
            return !GameManager.Instance.GetIsBusy();
        }

        public void ExecuteAction() {
            ActivateCalendarPanel();
        }

        private void ActivateCalendarPanel() {
            UIManager.Instance.AnimateUI(calendarPanel, true, true);
            SetCurrentLockObjectCanvasGroup(calendarPanel);
        }
    }
}
