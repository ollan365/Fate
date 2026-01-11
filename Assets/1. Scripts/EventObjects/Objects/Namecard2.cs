using UnityEngine;

public class Namecard2 : EventObject
{
    protected override bool CanInteract()
    {
        return !GameManager.Instance.GetIsBusy();
    }
}