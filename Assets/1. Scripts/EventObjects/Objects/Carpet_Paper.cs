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
        // ī�� �� ���� Ŭ���ϸ� ���� Ȯ���(UI���� Ȯ��� ���� ������).
        paperCloseUp.SetActive(true);
        RoomManager.Instance.AddScreenObjects(paperCloseUp);
        RoomManager.Instance.isInvestigating = true;
        carpet.interactable = true;
    }
}
