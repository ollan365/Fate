using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorageTeddyBear : EventObject
{
    public new void OnMouseDown()
    {
        if (DialogueManager.Instance.isDialogueActive) return;
        base.OnMouseDown();
        GameManager.Instance.IncrementVariable("StorageTeddyBearClick");
    }
}