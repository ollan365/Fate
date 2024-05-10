using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiquorAndPills : EventObject
{
    public new void OnMouseDown()
    {
        //base.OnMouseDown();
        if((bool)GameManager.Instance.GetVariable("isInquiry"))
            GameManager.Instance.IncrementVariable("LiquorAndPillsClick");
    }
}
