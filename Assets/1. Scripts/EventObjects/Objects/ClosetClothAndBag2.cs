using UnityEngine;

public class ClosetClothAndBag2 : EventObject
{
    protected override bool CanInteract()
    {
        return !GameManager.Instance.GetIsBusy();
    }
}