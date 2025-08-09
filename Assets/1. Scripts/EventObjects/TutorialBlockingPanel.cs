using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialBlockingPanel : EventObject
{
    public new void OnMouseDown()
    {
        if((bool)GameManager.Instance.GetVariable("isTutorial"))
            base.OnMouseDown();
    }
}
