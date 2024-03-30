using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bag : EventObject { 

    public new void OnMouseDown()
    {
        base.OnMouseDown();
        GameManager.Instance.IncrementVariable("BagClick");
        GameManager.Instance.SetVariable("BagClue", true);
    }
}