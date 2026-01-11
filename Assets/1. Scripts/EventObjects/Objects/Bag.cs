using UnityEngine;

public class Bag : EventObject
{
    protected override bool CanInteract()
    {
        return !GameManager.Instance.GetIsBusy();
    }
}