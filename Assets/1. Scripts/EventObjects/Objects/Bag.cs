using System;
using UnityEngine;

public class Bag : EventObject
{
    private void Start()
    {
        GetComponent<CircleCollider2D>().enabled = false;
    }

    public new void OnMouseDown()
    {
        bool isBusy = GameManager.Instance.GetIsBusy();
        if (isBusy) return;
        
        base.OnMouseDown();
    }
}