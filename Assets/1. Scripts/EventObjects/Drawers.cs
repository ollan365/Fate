using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drawers : EventObject, IResultExecutable
{
    public bool isClosedDrawers;
    public GameObject otherDrawers;
    public string parentObjectName;  // ��Ż��2 ���� ������. �θ� ������Ʈ �̸�
    private string closedOrOpen;

    public bool isUpDrawer;

    [Header("������ �Ʒ�ĭ")]
    public List<GameObject> sideClosedDownDrawerObjects;
    public List<GameObject> sideOpenDownDrawerObjects;

    [Header("������ ��ĭ")]
    public List<GameObject> sideClosedUpDrawerObjects;
    public List<GameObject> sideOpenUpDrawerObjects;

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

        showDrawersInSide();
    }

    private void showDrawersInSide()
    {
        // ������ ��ĭ
        if (isUpDrawer)
        {
            if (closedOrOpen == "Closed" && !gameObject.activeSelf)
            {
                // ���� ���� ����
                foreach (GameObject closedDrawer in sideClosedUpDrawerObjects)
                    closedDrawer.SetActive(false);

                foreach (GameObject openDrawer in sideOpenUpDrawerObjects)
                    openDrawer.SetActive(true);
            }
            else
            {
                // ���� ���� ����
                foreach (GameObject closedDrawer in sideClosedUpDrawerObjects)
                    closedDrawer.SetActive(true);

                foreach (GameObject openDrawer in sideOpenUpDrawerObjects)
                    openDrawer.SetActive(false);
            }
        }
        else // ������ �Ʒ�ĭ
        {
            if (closedOrOpen == "Closed" && !gameObject.activeSelf)
            {
                // ���� ���� ����
                foreach (GameObject closedDrawer in sideClosedDownDrawerObjects)
                    closedDrawer.SetActive(false);

                foreach (GameObject openDrawer in sideOpenDownDrawerObjects)
                    openDrawer.SetActive(true);
            }
            else
            {
                // ���� ���� ����
                foreach (GameObject closedDrawer in sideClosedDownDrawerObjects)
                    closedDrawer.SetActive(true);

                foreach (GameObject openDrawer in sideOpenDownDrawerObjects)
                    openDrawer.SetActive(false);
            }
        }
    }
}
