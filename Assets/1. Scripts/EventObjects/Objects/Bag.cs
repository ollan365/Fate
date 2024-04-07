using System;
using UnityEngine;

public class Bag : EventObject
{
    private void Awake()
    {
        GetComponent<CircleCollider2D>().enabled = false;
    }

    public new void OnMouseDown()
    {
        base.OnMouseDown();
        GameManager.Instance.IncrementVariable("BagClick"); 
    }
}