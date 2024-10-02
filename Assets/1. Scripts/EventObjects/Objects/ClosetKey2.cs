using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClosetKey2 : EventObject, IResultExecutable
{
    private void Start()
    {
        ResultManager.Instance.RegisterExecutable($"ClosetKey2_{sideNum}", this);
        if (sideNum == 0)
            GetComponent<Collider2D>().enabled = false;
    }

    public new void OnMouseDown()
    {
        bool isBusy = GameManager.Instance.GetIsBusy();
        if (isBusy) return;

        base.OnMouseDown();
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
        bool HasClosetKey2 = (bool)GameManager.Instance.GetVariable("HasClosetKey2");
        //Debug.Log("HasClosetKey2 : " + HasClosetKey2);
        if (HasClosetKey2) gameObject.SetActive(false);
    }
}