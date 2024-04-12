using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Carpet_closed : EventObject, IResultExecutable
{
    [SerializeField] private GameObject CarpetPaper;

    private void Awake()
    {
        ResultManager.Instance.RegisterExecutable("Carpet_Closed", this);
    }

    public new void OnMouseDown()
    {
        base.OnMouseDown();
        if((bool)GameManager.Instance.GetVariable("ChairMoved")) GameManager.Instance.IncrementVariable("CarpetClick");
    }

    public void ExecuteAction()
    {
        CarpetOpen();
    }

    [Header("������ ī�� ��ư")]
    [SerializeField] private GameObject carpetOpen;

    private void CarpetOpen()
    {
        gameObject.SetActive(false);
        carpetOpen.SetActive(true);

        CarpetPaper.SetActive(true);

        // ī�� �� ���� ��ư �����ϰ� �� ��ư �� ������ ��.
        // RoomManager.Instance.SetBlockingPanel(true);

        // RoomManager.Instance.Tutorial2_ChairAndCarpetInteractable(false);
        // CarpetPaper�� �� �� ���� �ø��� ��.
        CarpetPaper.GetComponent<RectTransform>().SetAsLastSibling();

        RoomManager.Instance.SetIsInvestigating(true);
    }
}
