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
        if((bool)GameManager.Instance.GetVariable("ChairMoved"))
            GameManager.Instance.IncrementVariable("CarpetClick");
    }

    public void ExecuteAction()
    {
        CarpetOpen();
    }

    [Header("뒤집은 카펫 버튼")]
    [SerializeField] private GameObject carpetOpen;

    private void CarpetOpen()
    {
        gameObject.SetActive(false);
        carpetOpen.SetActive(true);

        CarpetPaper.SetActive(true);

        // 카펫 밑 종이 버튼 제외하고 다 버튼 안 눌리게 함.
        RoomManager.Instance.SetBlockingPanel(true);

        RoomManager.Instance.Tutorial2_ChairAndCarpetInteractable(false);
        // CarpetPaper을 또 맨 위로 올리게 함.
        CarpetPaper.GetComponent<RectTransform>().SetAsLastSibling();

        RoomManager.Instance.SetIsInvestigating(true);
    }
}
