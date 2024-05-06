using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorageTeddyBear : EventObject
{
    private void Awake()
    {
        GetComponent<Collider2D>().enabled = false;
    }
    
    public new void OnMouseDown()
    {
        bool isBusy = GameManager.Instance.GetIsBusy();
        if (isBusy) return;
        
        base.OnMouseDown();
        GameManager.Instance.IncrementVariable("StorageTeddyBearClick");
    }
}