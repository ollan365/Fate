using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drawers : EventObject, IResultExecutable
{
    public bool isClosedDrawers;
    public GameObject otherDrawers;
    public string parentObjectName;  // 방탈출2 옷장 서랍장. 부모 오브젝트 이름
    private string closedOrOpen;

    public bool isUpDrawer;

    [Header("서랍장 아랫칸")]
    public List<GameObject> sideClosedDownDrawerObjects;
    public List<GameObject> sideOpenDownDrawerObjects;

    [Header("서랍장 윗칸")]
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
        isInquiry = false;  // 조사 시스템 예 아니오 스킵

        //GameManager.Instance.InverseVariable($"{parentObjectName}DrawersClosed");
        otherDrawers.SetActive(true);
        gameObject.SetActive(false);

        showDrawersInSide();
    }

    private void showDrawersInSide()
    {
        // 서랍장 윗칸
        if (isUpDrawer)
        {
            if (closedOrOpen == "Closed" && !gameObject.activeSelf)
            {
                // 문이 열린 상태
                foreach (GameObject closedDrawer in sideClosedUpDrawerObjects)
                    closedDrawer.SetActive(false);

                foreach (GameObject openDrawer in sideOpenUpDrawerObjects)
                    openDrawer.SetActive(true);
            }
            else
            {
                // 문이 닫힌 상태
                foreach (GameObject closedDrawer in sideClosedUpDrawerObjects)
                    closedDrawer.SetActive(true);

                foreach (GameObject openDrawer in sideOpenUpDrawerObjects)
                    openDrawer.SetActive(false);
            }
        }
        else // 서랍장 아랫칸
        {
            if (closedOrOpen == "Closed" && !gameObject.activeSelf)
            {
                // 문이 열린 상태
                foreach (GameObject closedDrawer in sideClosedDownDrawerObjects)
                    closedDrawer.SetActive(false);

                foreach (GameObject openDrawer in sideOpenDownDrawerObjects)
                    openDrawer.SetActive(true);
            }
            else
            {
                // 문이 닫힌 상태
                foreach (GameObject closedDrawer in sideClosedDownDrawerObjects)
                    closedDrawer.SetActive(true);

                foreach (GameObject openDrawer in sideOpenDownDrawerObjects)
                    openDrawer.SetActive(false);
            }
        }
    }
}
