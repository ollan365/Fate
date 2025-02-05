using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewTeddyBear : EventObject
{
    private void Start()
    {
        if ((int)GameManager.Instance.GetVariable("ReplayCount") > 0)
            gameObject.SetActive(true);
        else
            gameObject.SetActive(false);
    }

    public new void OnMouseDown()
    {
        base.OnMouseDown();
    }
}