using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Carpet_Paper : EventObject, IResultExecutable
{
    [SerializeField]
    private GameObject paperCloseUp;

    private Button carpet;

    private void Awake()
    {
        carpet = GameObject.Find("carpet_open").GetComponent<Button>();
        ResultManager.Instance.RegisterExecutable("Carpet_Paper", this);
    }

    public new void OnMouseDown()
    {
        base.OnMouseDown();
        GameManager.Instance.IncrementVariable("PaperClick");
    }

    public void ExecuteAction()
    {
        LookPaper();
    }

    private void LookPaper()
    {
        RoomManager.Instance.imageAndLockPanelManager.SetObjectImageGroup(true, "carpetPaper");
        //paperCloseUp.SetActive(true);
        //RoomManager.Instance.AddScreenObjects(paperCloseUp);
        RoomManager.Instance.isInvestigating = true;
        carpet.interactable = true;
    }

}
