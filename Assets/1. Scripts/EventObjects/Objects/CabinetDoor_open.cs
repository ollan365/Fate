using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CabinetDoor_open : EventObject, IResultExecutable
{
    [SerializeField]
    private GameObject door_close;

    [SerializeField]
    private GameObject door_open;

    private void Start()
    {
        ResultManager.Instance.RegisterExecutable(this.gameObject.name, this);
    }

    public new void OnMouseDown()
    {
        base.OnMouseDown();
    }

    public void ExecuteAction()
    {
        showDoorClose();
    }

    private void showDoorClose()
    {
        if ((bool)GameManager.Instance.GetVariable("CabinetDoor"))
        {
            door_close.SetActive(true);
            GameManager.Instance.SetVariable("CabinetDoor", false);
            door_open.SetActive(false);
        }
    }

}