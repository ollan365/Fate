using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fate.Managers;


namespace Fate.Events
{
    public class Book2 : EventObject, IResultExecutable
    {
        [SerializeField]
        private GameObject Book2Contents;

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
                ResultManager.Instance.RegisterExecutable("Book2", this);
        }

        public void ExecuteAction()
        {
            ReadBooks();
        }

        private void ReadBooks()
        {
            //isInquiry = false;  // ���� �ý��� �� �ƴϿ� ��ŵ
            Book2Contents.SetActive(true);
            UIManager.Instance.AnimateUI(Book2Contents, true, true);
            SetCurrentLockObjectCanvasGroup(Book2Contents);
        }
    }
}
