using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knife : EventObject, IResultExecutable
{
    private void Start()
    {
        ResultManager.Instance.RegisterExecutable($"Knife{sideNum}", this);
    }

    public new void OnMouseDown()
    {
        base.OnMouseDown();
    }

    public void ExecuteAction()
    {
        HideKnife();
    }

    private void HideKnife()
    {
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        bool hasKnife = (bool)GameManager.Instance.GetVariable("HasKnife");
        //Debug.Log("hasKnife : " + hasKnife);
        if (hasKnife) gameObject.SetActive(false);
    }
}