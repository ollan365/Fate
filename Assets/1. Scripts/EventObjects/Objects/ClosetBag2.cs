using UnityEngine;
using Fate.Managers;


namespace Fate.Events
{
    public class ClosetBag2 : EventObject
    {
        protected override bool CanInteract()
        {
            return !GameManager.Instance.GetIsBusy();
        }
    }
}
