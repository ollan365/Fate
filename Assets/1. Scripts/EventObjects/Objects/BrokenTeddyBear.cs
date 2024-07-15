using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrokenTeddyBear : EventObject, IResultExecutable
{
    public GameObject FixedTeddyBear;

    private void Awake()
    {
        ResultManager.Instance.RegisterExecutable($"BrokenTeddyBear{sideNum}", this);
    }

    public new void OnMouseDown()
    {
        base.OnMouseDown();
    }

    public void ExecuteAction()
    {
        FixTeddyBear();
    }

    private void FixTeddyBear()
    {
        gameObject.SetActive(false);
        FixedTeddyBear.SetActive(true);
    }

    private void OnEnable()
    {
        bool teddyBearFixed = (bool)GameManager.Instance.GetVariable("TeddyBearFixed");
        if (teddyBearFixed) 
        {
            gameObject.SetActive(false);
            FixedTeddyBear.SetActive(true);
        }
    }
}