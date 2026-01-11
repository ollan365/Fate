using UnityEngine;

public class StorageTeddyBear : EventObject
{
    protected override bool CanInteract()
    {
        return !GameManager.Instance.GetIsBusy();
    }
}