using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClosetDoor_open : EventObject, IResultExecutable
{
    [SerializeField]
    private GameObject door_close;

    [SerializeField]
    private GameObject door_open;

    [SerializeField]
    private GameObject bag;

    [SerializeField] private GameObject MapClosetClose;
    [SerializeField] private GameObject MapClosetOpen;

    private void Awake()
    {
        ResultManager.Instance.RegisterExecutable(this.gameObject.name, this);
        //Debug.Log(this.gameObject.name );
    }

    public new void OnMouseDown()
    {
        base.OnMouseDown();
        //Debug.Log(this.gameObject.name + "��ư ����");
        //Debug.Log(GameManager.Instance.GetVariable("ClosetDoor"));
        //showDoorClose();
    }

    public void ExecuteAction()
    {
        showDoorClose();
    }

    private void showDoorClose()
    {
        if ((bool)GameManager.Instance.GetVariable("ClosetDoor"))
        {
            //Debug.Log(this.gameObject.name + "����~");
            door_close.SetActive(true);
            GameManager.Instance.SetVariable("ClosetDoor", false);
            bag.SetActive(false);
            door_open.SetActive(false);

            MapClosetOpen.SetActive(false);
            MapClosetClose.SetActive(true);
        }
    }

}