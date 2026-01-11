using UnityEngine;
using Fate.Managers;


namespace Fate.Events
{
    public class TutorialBlockingPanel : EventObject
    {
        protected override bool CanInteract()
        {
            return !GameManager.Instance.GetIsBusy();
        }
    }
}
