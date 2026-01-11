using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fate.Managers;


namespace Fate.Events
{
    public class BrokenTeddyBear : EventObject, IResultExecutable
    {
        public GameObject FixedTeddyBear;

        private void Start()
        {
            RegisterWithResultManager();
        }

        public void ExecuteAction()
        {
            FixTeddyBear();
        }

        private void FixTeddyBear()
        {
            gameObject.SetActive(false);
            FixedTeddyBear.SetActive(true);
        }

        private void OnEnable()
        {
            RegisterWithResultManager();
            bool teddyBearFixed = (bool)GameManager.Instance.GetVariable("TeddyBearFixed");
            if (teddyBearFixed) 
            {
                gameObject.SetActive(false);
                FixedTeddyBear.SetActive(true);
            }
        }

        private void RegisterWithResultManager()
        {
            if (ResultManager.Instance != null)
                ResultManager.Instance.RegisterExecutable($"BrokenTeddyBear{sideNum}", this);
        }
    }
}
