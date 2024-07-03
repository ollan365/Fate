using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drawers : EventObject, IResultExecutable
{
    public bool isClosedDrawers;
    public GameObject otherDrawers;
    public string parentObjectName;  // 방탈출2 옷장 서랍장. 부모 오브젝트 이름
    private string closedOrOpen;

    private void Awake()
    {
        closedOrOpen = isClosedDrawers ? "Closed" : "Open";
        ResultManager.Instance.RegisterExecutable($"{closedOrOpen}{parentObjectName}Drawers", this);
    }

    public new void OnMouseDown()
    {
        bool isBusy = GameManager.Instance.GetIsBusy();
        if (isBusy) return;

        base.OnMouseDown();

        //if (isClosedDrawers) GameManager.Instance.IncrementVariable($"{closedOrOpen}{parentObjectName}DrawersClick");
    }

    public void ExecuteAction()
    {
        ToggleDoors();
    }

    private void ToggleDoors()
    {
        isInquiry = false;  // 조사 시스템 예 아니오 스킵

        //GameManager.Instance.InverseVariable($"{parentObjectName}DrawersClosed");
        otherDrawers.SetActive(true);
        gameObject.SetActive(false);

    }
}
