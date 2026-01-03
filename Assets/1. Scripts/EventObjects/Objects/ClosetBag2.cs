using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClosetBag2 : EventObject
{
    public new void OnMouseDown()
    {
        bool isBusy = GameManager.Instance.GetIsBusy();
        if (isBusy) return;

        base.OnMouseDown();
    }
}