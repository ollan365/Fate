using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fate.Managers;


namespace Fate.Events
{
    public class TinCase : EventObject, IResultExecutable
    {
        [SerializeField] private GameObject tinCaseLock;
    
        private void Start()
        {
            RegisterWithResultManager();
        }

        private void OnEnable()
        {
            RegisterWithResultManager();
        }

        private void RegisterWithResultManager()
        {
            if (ResultManager.Instance != null)
                ResultManager.Instance.RegisterExecutable("TinCase", this);
        }

        protected override bool CanInteract()
        {
            return !GameManager.Instance.GetIsBusy();
        }

        public void ExecuteAction()
        {
            ActivateTinCaseLock();
        }

        // 틴케이스 잠금 장치 실행
        private void ActivateTinCaseLock()
        {
            //isInquiry = false;  // 조사 시스템 예 아니오 스킵
            UIManager.Instance.AnimateUI(tinCaseLock, true, true);
            SetCurrentLockObjectCanvasGroup(tinCaseLock);
        }
    }
}
