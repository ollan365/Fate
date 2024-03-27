using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Carpet_Paper : EventObject, IResultExecutable
{
    [SerializeField]
    private GameObject paperCloseUp;

    private Button carpet;

    private void Start()
    {
        carpet = GameObject.Find("carpet").GetComponent<Button>();
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
        // 카펫 밑 종이 클릭하면 종이 확대됨(UI에서 확대된 종이 보여짐).
        paperCloseUp.SetActive(true);
        RoomManager.Instance.AddScreenObjects(paperCloseUp);
        RoomManager.Instance.isResearch = true;
        carpet.interactable = true;
    }
}
