using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShoppingBag : EventObject
{
    public new void OnMouseDown()
    {
        base.OnMouseDown();
        GameManager.Instance.IncrementVariable("ShoppingBagClick");
    }
}