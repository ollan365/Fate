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

    // ************************* temporary members for moving *************************
    private Vector2 originalPosition;  // ���� ��ġ 
    private List<Vector2> movedPositions = new List<Vector2> { Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero };
    private RectTransform rectTransform;  // targetPosition�� ���� origin ��ġ�� moved ��ġ�� �����ؼ� �ű���� �����̰� ��.
    [SerializeField] private float moveDuration = 0.3f; // �̵��� �ɸ��� �ð�
    private bool isMoving = false; // �����̴� ������ ����
    // ********************************************************************************

    private void Awake()
    {
        closedOrOpen = isClosedDrawers ? "Closed" : "Open";
        ResultManager.Instance.RegisterExecutable($"{closedOrOpen}{parentObjectName}Drawers", this);


        rectTransform = GetComponent<RectTransform>();

        originalPosition = rectTransform.anchoredPosition;  // �ʱ� ��ġ ����
        switch (isUpDrawer)
        {
            case true:
                // upDrawer
                movedPositions[1] = new Vector2(originalPosition.x, -75.3f);
                break;
            case false:
                // downDrawer
                movedPositions[2] = new Vector2(originalPosition.x, 191.7f);
                break;
        }
    }

    public new void OnMouseDown()
    {
        bool isBusy = GameManager.Instance.GetIsBusy();
        if (isBusy) return;

        base.OnMouseDown();
    }

    public void ExecuteAction()
    {
        ToggleDoors();
    }

    private void ToggleDoors()
    {
        isInquiry = false;  // ���� �ý��� �� �ƴϿ� ��ŵ

        //GameManager.Instance.InverseVariable($"{parentObjectName}DrawersClosed");

        // ************************* temporary codes for moving *************************
        otherDrawers.SetActive(true);
        if (!otherDrawers.GetComponent<Drawers>().isClosedDrawers)
        {
            otherDrawers.GetComponent<Drawers>().ExecuteActionMoveDrawer();
        }

        if (!isClosedDrawers)
        {
            ExecuteActionMoveDrawer();
        }
        else
        {
            gameObject.SetActive(false);
        }
        // *******************************************************************************

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


    // ************************* temporary methods for moving *************************
    public void ExecuteActionMoveDrawer()
    {
        bool UpDrawerMoved = isUpDrawer ? (bool)GameManager.Instance.GetVariable("UpDrawerMoved") 
            : (bool)GameManager.Instance.GetVariable("DownDrawerMoved");
        int movedPositionsIndex = 1;
        if (!isUpDrawer)
            movedPositionsIndex = 2;

        Vector2 targetPosition = UpDrawerMoved ? originalPosition : movedPositions[movedPositionsIndex];

        if (!isClosedDrawers) StartCoroutine(MoveDrawer(targetPosition));
    }

    IEnumerator MoveDrawer(Vector2 targetPosition)
    {
        RoomManager.Instance.SetIsInvestigating(true);
        isMoving = true;

        GameManager.Instance.SetVariable(isUpDrawer ? "isUpDrawerMoving": "isDownDrawerMoving", isMoving);

        float elapsedTime = 0;
        Vector2 startingPosition = rectTransform.anchoredPosition;

        while (elapsedTime < moveDuration)
        {
            rectTransform.anchoredPosition = Vector2.Lerp(startingPosition, targetPosition, (elapsedTime / moveDuration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        rectTransform.anchoredPosition = targetPosition;

        isMoving = false;

        GameManager.Instance.SetVariable(isUpDrawer ? "isUpDrawerMoving" : "isDownDrawerMoving", isMoving);

        GameManager.Instance.InverseVariable(isUpDrawer ? "UpDrawerMoved" : "DownDrawerMoved");
        bool DrawerMoved = (bool)GameManager.Instance.GetVariable(isUpDrawer ? "UpDrawerMoved" : "DownDrawerMoved");
        if (!DrawerMoved)
            gameObject.SetActive(false);

        RoomManager.Instance.SetIsInvestigating(false);
    }
    // *******************************************************************************
}
