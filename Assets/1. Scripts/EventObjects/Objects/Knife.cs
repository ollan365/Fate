using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knife : EventObject, IResultExecutable
{
    // ************************* temporary members for grab animation *************************
    [SerializeField] private Animator knifeAnimator;
    [SerializeField] private float hideTime = 0.4f;
    // ********************************************************************************

    private void Start()
    {
        RegisterWithResultManager();
    }

    public new void OnMouseDown()
    {
        base.OnMouseDown();
    }

    public void ExecuteAction()
    {
        GrabKnife();
    }

    // ************************* temporary methods for grab animation *************************
    private void GrabKnife()
    {
        if (sideNum != 0)
        {
            HideKnife();
            return;
        }

        RoomManager.Instance.SetIsInvestigating(true);

        knifeAnimator.SetBool("grab_Knife", true);
        Invoke("HideKnife", hideTime);
    }

    private void HideKnife()
    {
        gameObject.SetActive(false);
    }
    // *******************************************************************************

    private void OnEnable()
    {
        RegisterWithResultManager();
        bool hasKnife = (bool)GameManager.Instance.GetVariable("HasKnife");
        //Debug.Log("hasKnife : " + hasKnife);
        if (hasKnife) gameObject.SetActive(false);
    }

    private void RegisterWithResultManager()
    {
        if (ResultManager.Instance != null)
            ResultManager.Instance.RegisterExecutable($"Knife{sideNum}", this);
    }
}