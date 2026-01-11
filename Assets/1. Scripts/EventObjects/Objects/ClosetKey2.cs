using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClosetKey2 : EventObject, IResultExecutable
{
    private void Start()
    {
        RegisterWithResultManager();
    }

    protected override bool CanInteract()
    {
        return !GameManager.Instance.GetIsBusy();
    }

    public void ExecuteAction()
    {
        HideClosetKey2();
    }

    private void HideClosetKey2()
    {
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        RegisterWithResultManager();
        bool HasClosetKey2 = (bool)GameManager.Instance.GetVariable("HasClosetKey2");
        //Debug.Log("HasClosetKey2 : " + HasClosetKey2);
        if (HasClosetKey2) gameObject.SetActive(false);
    }

    private void RegisterWithResultManager()
    {
        if (ResultManager.Instance != null)
            ResultManager.Instance.RegisterExecutable($"ClosetKey2_{sideNum}", this);
    }
}