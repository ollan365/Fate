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

    private const string UP_DRAWER_MOVED = "UpDrawerMoved";
    private const string DOWN_DRAWER_MOVED = "DownDrawerMoved";

    protected override void Awake()
    {
        base.Awake();
        closedOrOpen = isClosedDrawers ? "Closed" : "Open";
        ResultManager.Instance.RegisterExecutable($"{closedOrOpen}{parentObjectName}Drawers", this);

        rectTransform = GetComponent<RectTransform>();
        originalPosition = rectTransform.anchoredPosition;  // 초기 위치 저장
        if (isUpDrawer)
        {
            // up drawer move target
            movedPositions[1] = new Vector2(originalPosition.x, -75.3f);
        }
        else
        {
            // down drawer move target
            movedPositions[2] = new Vector2(originalPosition.x, 191.7f);
        }
    }

    private void OnEnable()
    {
        UpdateImageState();
    }

    protected override bool CanInteract()
    {
        return !GameManager.Instance.GetIsBusy();
    }

    public void ExecuteAction()
    {
        ToggleDrawers();
    }

    private void ToggleDrawers()
    {
        // ************************* temporary codes for moving *************************
        otherDrawers.SetActive(true);

        Drawers other = otherDrawers.GetComponent<Drawers>();
        if (!other.isClosedDrawers)
        {
            other.ExecuteActionMoveDrawer();
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
    }

    private void UpdateImageState()
    {
        string key = isUpDrawer ? UP_DRAWER_MOVED : DOWN_DRAWER_MOVED;
        bool drawerMoved = (bool)GameManager.Instance.GetVariable(key);

        if (isUpDrawer)
        {
            // 서랍장 윗칸
            foreach (var obj in sideClosedUpDrawerObjects)
                obj.SetActive(!drawerMoved);
            foreach (var obj in sideOpenUpDrawerObjects)
                obj.SetActive(drawerMoved);
        }
        else
        {
            // 서랍장 아랫칸
            foreach (var obj in sideClosedDownDrawerObjects)
                obj.SetActive(!drawerMoved);
            foreach (var obj in sideOpenDownDrawerObjects)
                obj.SetActive(drawerMoved);
        }
    }

    // ************************* temporary methods for moving *************************
    public void ExecuteActionMoveDrawer()
    {
        string key = isUpDrawer ? UP_DRAWER_MOVED : DOWN_DRAWER_MOVED;

        bool drawerMoved = (bool)GameManager.Instance.GetVariable(key);

        int movedPositionsIndex = isUpDrawer ? 1 : 2;
        Vector2 targetPosition = drawerMoved ? originalPosition : movedPositions[movedPositionsIndex];

        // 서랍 애니메이션 재생
        if (!isClosedDrawers)
            StartCoroutine(MoveDrawer(targetPosition));
    }

    IEnumerator MoveDrawer(Vector2 targetPosition)
    {
        isMoving = true;
        GameManager.Instance.SetVariable(isUpDrawer ? "isUpDrawerMoving" : "isDownDrawerMoving", isMoving);

        float elapsedTime = 0;
        Vector2 startingPosition = rectTransform.anchoredPosition;

        while (elapsedTime < moveDuration)
        {
            rectTransform.anchoredPosition =
                Vector2.Lerp(startingPosition, targetPosition, (elapsedTime / moveDuration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        rectTransform.anchoredPosition = targetPosition;

        isMoving = false;
        GameManager.Instance.SetVariable(isUpDrawer ? "isUpDrawerMoving" : "isDownDrawerMoving", isMoving);

        // 서랍 이동 후 변수 토글
        string key = isUpDrawer ? UP_DRAWER_MOVED : DOWN_DRAWER_MOVED;
        GameManager.Instance.InverseVariable(key);

        // 이동 끝난 후 이미지 업데이트
        UpdateImageState();

        // 서랍 닫힌 상태라면 현재 Drawer 오브젝트 숨김
        bool drawerMoved = (bool)GameManager.Instance.GetVariable(key);
        if (!drawerMoved)
            gameObject.SetActive(false);
    }
    // *******************************************************************************
}
