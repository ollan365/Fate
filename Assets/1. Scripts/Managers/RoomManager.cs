using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class RoomManager : MonoBehaviour
{
    public static RoomManager Instance { get; private set; }
    
    [Header("시점들")][SerializeField] public List<GameObject> sides;  // 시점들
    private GameObject currentView;  // 현재 뷰
    public int currentSideIndex = 0;  // 현재 시점 인덱스
    
    [Header("확대 화면들")][SerializeField] private List<GameObject> zoomViews;  // 확대 화면들

    // 이벤트 오브젝트 패널 매니저
    public ImageAndLockPanelManager imageAndLockPanelManager;
    // 조사 중이거나 확대 중이면 이동키로 시점 바꾸지 못하게 함
    public bool isInvestigating = false;
    private bool isZoomed = false;
    
    // 튜토리얼 매니저
    public TutorialManager tutorialManager;

    // 액션포인트 매니저
    public ActionPointManager actionPointManager;
    public Room2ActionPointManager room2ActionPointManager;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
        
        SceneManager.Instance.ChangeSceneEffect();
        ResultManager.Instance.InitializeExecutableObjects();
        MemoManager.Instance.SetMemoGauge(UIManager.Instance.GetUI(eUIGameObjectName.MemoGauge));
        
        UIManager.Instance.SetUI(eUIGameObjectName.NormalVignette, true);
        UIManager.Instance.SetUI(eUIGameObjectName.ActionPoints, true);
        UIManager.Instance.SetUI(eUIGameObjectName.ActionPointsBackgroundImage, true);
        UIManager.Instance.SetUI(eUIGameObjectName.DayText, true);
        UIManager.Instance.SetUI(eUIGameObjectName.HeartParent, true);
        UIManager.Instance.SetUI(eUIGameObjectName.MemoGauge, true);
    }
    
    void Start()
    {
        // 모든 Side 켰다 끄기
        foreach (GameObject side in sides)
        {
            side.SetActive(true);
            side.SetActive(false);
        }
        
        // 모든 확대 화면 켰다 끄기
        foreach (GameObject zoomView in zoomViews)
        {
            zoomView.SetActive(true);
            zoomView.SetActive(false);
        }

        // Side 1으로 초기화?
        currentView = sides[currentSideIndex];
        SetCurrentSide(currentSideIndex);

        MemoManager.Instance.HideMemoButton = false;
        SetButtons();

        actionPointManager.CreateHearts();  // create hearts on room start

        // 첫 대사 출력 후 튜토리얼 1페이즈 시작(현재 씬 이름이 Room1일 때만)
        if ((int)GameManager.Instance.GetVariable("CurrentScene") == Constants.SceneType.ROOM_1.ToInt())
        {
            if (!GameManager.Instance.skipTutorial
                && (int)GameManager.Instance.GetVariable("ReplayCount") == 0
                && !(bool)GameManager.Instance.GetVariable("EndTutorial_ROOM_1"))
                DialogueManager.Instance.StartDialogue("Prologue_015");
        }
    }

    public void MoveSides(int leftOrRight)  // left: -1, right: 1
    {
        if (leftOrRight != -1 && leftOrRight != 1)
        {
            Debug.Log("Value of leftOrRight must be -1 or 1!");
            return;
        }

        //int newSideIndex = (currentSideIndex + sides.Count + leftOrRight) % sides.Count;
        int[] rightOrder = { 0, 1, 3, 2 }; // 오른쪽 side 이동 순서
        int[] leftOrder = { 0, 2, 3, 1 };  // 왼쪽 side 이동 순서

        // 현재 위치를 해당 이동 방향의 배열에서 찾고, 이동 방향에 맞게 다음 인덱스를 구함
        int currentPos = System.Array.IndexOf((leftOrRight == 1) ? rightOrder : leftOrder, currentSideIndex);
        int newSideIndex = (leftOrRight == 1)
            ? rightOrder[(currentPos + 1) % rightOrder.Length]
            : leftOrder[(currentPos + 1) % leftOrder.Length];
        SetCurrentSide(newSideIndex);
        
        ScreenEffect.Instance.MoveButtonEffect(sides[newSideIndex], new Vector3(leftOrRight, 0, 0));

        // 시점에 맞춰서 버튼 끄고 키게 함.(사이드 2번에선 오른쪽 버튼만 3번에선 왼쪽 버튼만 나오게 함)
        SetMoveButtons(true);

        // 튜토리얼 1 페이즈 관련
        if ((int)GameManager.Instance.GetVariable("CurrentScene") == Constants.SceneType.ROOM_1.ToInt())
            if(!GameManager.Instance.skipTutorial)
                tutorialManager.SetSeenSides(newSideIndex);
    }

    public void OnExitButtonClick()
    {
        if (isInvestigating) 
            imageAndLockPanelManager.OnExitButtonClick();
        else if (isZoomed)
        {
            // 화면 전환 효과
            ScreenEffect.Instance.MoveButtonEffect(sides[currentSideIndex], new Vector3(0, -0.5f, 0));

            SetCurrentView(sides[currentSideIndex]);
            isZoomed = false;
        }

        SetButtons();

        var refillHeartsOrEndDay = (bool)GameManager.Instance.GetVariable("RefillHeartsOrEndDay");
        if (refillHeartsOrEndDay)
            actionPointManager.RefillHeartsOrEndDay();

    }

    // 비밀번호 무한 입력 시도 방지
    public void ProhibitInput()
    {
        if (UIManager.Instance && UIManager.Instance.heartParent.transform.childCount < 1)
            OnExitButtonClick();
    }


    // exit to root: turn off all the panels and zoom out to the root view
    public void ExitToRoot()
    {
        if (isInvestigating) 
            imageAndLockPanelManager.OnExitButtonClick();
        if (isZoomed)
        {
            SetCurrentView(sides[currentSideIndex]);
            isZoomed = false;
        }
        
        SetButtons();
    }

    public bool GetIsInvestigating()
    {
        return isInvestigating;
    }

    // ######################################## setters ########################################
    public void SetIsInvestigating(bool isTrue)
    {
        isInvestigating = isTrue;
    }
    
    public void SetIsZoomed(bool isTrue)
    {
        isZoomed = isTrue;
    }

    // 시점 전환
    private void SetCurrentSide(int newSideIndex)
    {
        SetCurrentView(sides[newSideIndex]);
        currentSideIndex = newSideIndex;
        GameManager.Instance.SetVariable("currentSideIndex", currentSideIndex);
    }
    
    // 뷰 전환
    public void SetCurrentView(GameObject newView)
    {
        currentView.SetActive(false);
        newView.SetActive(true);
        currentView = newView;
    }
    
    // 나가기 버튼 필요 시, 보이게 함 (ResultManager에서 호출하게 함)
    private void SetExitButton(bool isTrue)
    {
        UIManager.Instance.SetUI(eUIGameObjectName.ExitButton, isTrue);
    }
    
    // 이동 버튼들이 조사/다이얼로그 출력 시에는 화면 상에서 보이지 않게 함
    private void SetMoveButtons(bool isTrue)
    {
        UIManager.Instance.SetUI(eUIGameObjectName.LeftButton, isTrue);
        UIManager.Instance.SetUI(eUIGameObjectName.RightButton, isTrue);
        
        switch (currentSideIndex)
        {
            case 1:
                UIManager.Instance.SetUI(eUIGameObjectName.RightButton, false);
                break;
            case 2:
                UIManager.Instance.SetUI(eUIGameObjectName.LeftButton, false);
                break;
        }
    }
    
    public void SetButtons()
    {
        bool isInvestigatingOrZoomed = isInvestigating || isZoomed;
        bool isDialogueActive = DialogueManager.Instance.isDialogueActive;
        bool isMemoOpen = MemoManager.Instance.isMemoOpen;
        bool isLaptopOpen = (bool)GameManager.Instance.GetVariable("isLaptopOpen");
        bool isLaptopAppOpen = (bool)GameManager.Instance.GetVariable("isLaptopAppOpen");
        
        SetExitButton((isInvestigatingOrZoomed && !isDialogueActive) 
                      || (!isDialogueActive && isLaptopOpen && !isLaptopAppOpen)
                      || isMemoOpen);
        SetMoveButtons(!isInvestigatingOrZoomed && !isDialogueActive && !isMemoOpen);
        
        UIManager.Instance.SetUI(eUIGameObjectName.ActionPoints, !isLaptopOpen);
        UIManager.Instance.SetUI(eUIGameObjectName.NormalVignette, !isLaptopOpen);
        UIManager.Instance.SetUI(eUIGameObjectName.MemoGauge, !isLaptopOpen);
        // MemoManager.Instance.SetMemoButton(!isDialogueActive && !isMemoOpen);
        
        UIManager.Instance.SetCursorAuto();
    }
}