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

    // ************************* temporary members for moving *************************
    private Vector2 originalPosition;  // 기존 위치 
    private List<Vector2> movedPositions = new List<Vector2> { Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero };
    private RectTransform rectTransform;  // targetPosition에 위의 origin 위치랑 moved 위치를 대입해서 거기까지 움직이게 함.
    [SerializeField] private float moveDuration = 0.3f; // 이동에 걸리는 시간
    private bool isMoving = false; // 움직이는 중인지 여부
    // ********************************************************************************

    private void Awake()
    {
        closedOrOpen = isClosedDrawers ? "Closed" : "Open";
        ResultManager.Instance.RegisterExecutable($"{closedOrOpen}{parentObjectName}Drawers", this);


        rectTransform = GetComponent<RectTransform>();

        originalPosition = rectTransform.anchoredPosition;  // 초기 위치 저장
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
        isInquiry = false;  // 조사 시스템 예 아니오 스킵

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
