using UnityEngine;

public class ClosetBag2 : EventObject
{
    protected override bool CanInteract()
    {
        return !GameManager.Instance.GetIsBusy();
    }
}