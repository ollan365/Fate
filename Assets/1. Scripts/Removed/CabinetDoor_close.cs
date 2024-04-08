using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CabinetDoor_close : EventObject, IResultExecutable
{
    [SerializeField]
    private GameObject door_open;

    //[SerializeField]
    //private GameObject calendar;

    private void Awake()
    {
        ResultManager.Instance.RegisterExecutable("CabinetDoor_Close", this);

    }

    public new void OnMouseDown()
    {
        base.OnMouseDown();
        GameManager.Instance.IncrementVariable("CabinetOffClick");
    }

    public void ExecuteAction()
    {
        showDoorOpen();
    }

    private void showDoorOpen()
    {
        if (!(bool)GameManager.Instance.GetVariable("CabinetDoor"))
        {
            door_open.SetActive(true);
            //calendar.SetActive(true);
            if (this.gameObject.activeSelf)
                this.gameObject.SetActive(false);
            GameManager.Instance.SetVariable("CabinetDoor", true);
        }
    }

}