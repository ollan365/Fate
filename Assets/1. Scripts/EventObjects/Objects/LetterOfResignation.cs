using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LetterOfResignation : EventObject
{
    private void Start()
    {
        GetComponent<BoxCollider2D>().enabled = false;
    }

    public new void OnMouseDown()
    {
        bool isBusy = GameManager.Instance.GetIsBusy(); 
        if (isBusy) return;

        base.OnMouseDown();
    }
}
