using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShoppingBags : EventObject
{
    public new void OnMouseDown()
    {
        base.OnMouseDown();
        //GameManager.Instance.IncrementVariable("ShoppingBagsClick");
    }
}