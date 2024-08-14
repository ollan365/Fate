using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClosetKey2 : EventObject
{
    private void Start()
    {
        GetComponent<Collider2D>().enabled = false;
    }

    public new void OnMouseDown()
    {
        bool isBusy = GameManager.Instance.GetIsBusy();
        if (isBusy) return;

        base.OnMouseDown();
    }
}