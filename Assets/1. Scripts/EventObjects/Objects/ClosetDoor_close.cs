using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClosetDoor_close : EventObject, IResultExecutable
{
    [SerializeField]
    private GameObject door_open;

    [SerializeField]
    private GameObject bag;


    [SerializeField] private GameObject MapClosetClose;
    [SerializeField] private GameObject MapClosetOpen;


    private void Start()
    {
        ResultManager.Instance.RegisterExecutable("ClosetDoor_close", this);

    }

    public new void OnMouseDown()
    {
        base.OnMouseDown();
        GameManager.Instance.IncrementVariable("ClosetOffClick");
    }

    public void ExecuteAction()
    {
        showDoorOpen();
    }

    private void showDoorOpen()
    {
        if (!(bool)GameManager.Instance.GetVariable("ClosetDoor"))
        {
            door_open.SetActive(true);
            bag.SetActive(true);
            if(this.gameObject.activeSelf)
                this.gameObject.SetActive(false);
            GameManager.Instance.SetVariable("ClosetDoor", true);

            MapClosetClose.SetActive(false);
            MapClosetOpen.SetActive(true);
        }
    }

}