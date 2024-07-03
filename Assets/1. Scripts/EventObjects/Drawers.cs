using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drawers : EventObject, IResultExecutable
{
    public bool isClosedDrawers;
    public GameObject otherDrawers;
    public string parentObjectName;  // ��Ż��2 ���� ������. �θ� ������Ʈ �̸�
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
        isInquiry = false;  // ���� �ý��� �� �ƴϿ� ��ŵ

        //GameManager.Instance.InverseVariable($"{parentObjectName}DrawersClosed");
        otherDrawers.SetActive(true);
        gameObject.SetActive(false);

    }
}
