using UnityEngine;
using Fate.Managers;


namespace Fate.Events
{
    public class Bag : EventObject
    {
        protected override bool CanInteract()
        {
            return !GameManager.Instance.GetIsBusy();
        }
    }
}
