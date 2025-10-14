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

    // 오브젝트 패널 매니저
    public ImageAndLockPanelManager imageAndLockPanelManager;
    [SerializeField] private Canvas imageAndLockPanelCanvas;
    // 조사 중이거나 확대 중이면 이동키로 시점 바꾸지 못하게 함
    public bool isInvestigating = false;
    [SerializeField] private bool isZoomed = false;
    public string currentZoomViewName = null;

    // 액션포인트 매니저
    [Header("행동력 매니저")]
    public ActionPointManager actionPointManager;
    public Room2ActionPointManager room2ActionPointManager;
    
    [HideInInspector] public TutorialManager tutorialManager;
    private const string restDialogueId = "RoomEscape_035";

    void Awake()
    {
        if (Instance)
            Destroy(gameObject);
        else
            Instance = this;
        
        imageAndLockPanelCanvas.worldCamera = UIManager.Instance.uiCamera;
        ResultManager.Instance.InitializeExecutableObjects();
        
        StartCoroutine(SetupMemoGaugeAfterUI());
        
        UIManager.Instance.SetUI(eUIGameObjectName.NormalVignette, true);
        UIManager.Instance.SetUI(eUIGameObjectName.ActionPoints, true);
        UIManager.Instance.SetUI(eUIGameObjectName.ActionPointsBackgroundImage, true);
        UIManager.Instance.SetUI(eUIGameObjectName.DayText, true);
        UIManager.Instance.SetUI(eUIGameObjectName.HeartParent, true);
        UIManager.Instance.SetUI(eUIGameObjectName.MemoGauge, true);
    }
    
    private IEnumerator SetupMemoGaugeAfterUI() {
        yield return new WaitForEndOfFrame();
        
        GameObject memoGaugeUI = UIManager.Instance.GetUI(eUIGameObjectName.MemoGauge);
        if (memoGaugeUI)
            MemoManager.Instance.SetMemoGauge(memoGaugeUI);
        else
            Debug.LogError("Could not find memo gauge UI");
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

        // SaveManager에 저장되어 있던 currentSideIndex으로 초기화
        currentView = sides[(int)GameManager.Instance.GetVariable("currentSideIndex")];
        SetCurrentSide((int)GameManager.Instance.GetVariable("currentSideIndex"));

        MemoManager.Instance.SetShouldHideMemoButton(false);
        SetButtons();

        // Start 시 저장 데이터를 기반으로 씬 상태를 복원하고,
        // 미완 진행(다이얼로그 진행 중 중단 등)이면 저장되어 있던 상태로 재시작.
        RestoreSceneStateFromSave();

        tutorialManager = gameObject.GetComponent<TutorialManager>();
        tutorialManager?.AddTutorialObjects();
        if (GameManager.Instance.skipTutorial ||
            GameSceneManager.Instance.GetActiveScene() != Constants.SceneType.ROOM_1 ||
            (int)GameManager.Instance.GetVariable("ReplayCount") > 0) {
            tutorialManager?.ToggleCollider(TutorialManager.eTutorialObjectName.Chair, true);
            return;
        }
        tutorialManager?.StartTutorial();
    }

    private void RestoreSceneStateFromSave()
    {
        // 귀가 스크립트 출력 미완이면 처음부터 다시 실행
        if (!(bool)GameManager.Instance.GetVariable("isHomeComingComplete"))
        {
            actionPointManager.RefillHeartsOrEndDay();
            return;
        }

        // 완료 상태면 각 요소 복원
        // 휴식 다이얼로그가 출력 중이 아닐 때만 하트 만들기
        // 휴식 다이얼로그가 출력 중이면 Day 텍스트만 업데이트.
        if ((string)GameManager.Instance.GetVariable("currentDialogueID") == restDialogueId)
            actionPointManager.TryUpdateDayTextDuringRest();
        else
            actionPointManager.CreateHearts();

        // 미완성인 상태가 있으면 재시작 처리
        if ((bool)GameManager.Instance.GetVariable("isDialogueActive"))
        {
            string currentDialogueID = (string)GameManager.Instance.GetVariable("currentDialogueID");
            if (!string.IsNullOrEmpty(currentDialogueID) &&
                !string.Equals(currentDialogueID, "NONE", System.StringComparison.OrdinalIgnoreCase))
            {
                DialogueManager.Instance.StartDialogue(currentDialogueID);
            }
        }

        if ((bool)GameManager.Instance.GetVariable("isZoomed"))
        {
            string currentZoomName = (string)GameManager.Instance.GetVariable("currentZoomViewName");
            if (!string.IsNullOrEmpty(currentZoomName) &&
                !string.Equals(currentZoomName, "NONE", System.StringComparison.OrdinalIgnoreCase))
            {
                ResultManager.Instance.ExecuteActionObject(currentZoomName);
            }
        }

        bool isImageActive = (bool)GameManager.Instance.GetVariable("isImageActive");
        bool isLockObjectActive = (bool)GameManager.Instance.GetVariable("isLockObjectActive");

        if (isImageActive)
        {
            string currentObjectImageName = (string)GameManager.Instance.GetVariable("currentEventObjectName");
            if (!string.IsNullOrEmpty(currentObjectImageName) &&
                !string.Equals(currentObjectImageName, "NONE", System.StringComparison.OrdinalIgnoreCase))
            {
                imageAndLockPanelManager.SetObjectImageGroup(true, currentObjectImageName);
            }
        }
        else if (isLockObjectActive)
        {
            string currentLockObjectName = (string)GameManager.Instance.GetVariable("currentLockObjectName");
            if (!string.IsNullOrEmpty(currentLockObjectName) &&
                !string.Equals(currentLockObjectName, "NONE", System.StringComparison.OrdinalIgnoreCase))
            {
                imageAndLockPanelManager.SetLockObject(true, currentLockObjectName);
            }
        }
    }


    public void MoveSides(int leftOrRight)  // left: -1, right: 1
    {
        if (leftOrRight != -1 && leftOrRight != 1)
        {
            Debug.Log("Value of leftOrRight must be -1 or 1!");
            return;
        }

        int newSideIndex = (currentSideIndex + sides.Count + leftOrRight) % sides.Count;
        SetCurrentSide(newSideIndex);
        
        UIManager.Instance.MoveSideEffect(sides[newSideIndex], new Vector3(leftOrRight, 0, 0));
        UIManager.Instance.StartCoroutine(UIManager.Instance.HideTemporarily(eUIGameObjectName.LeftButton, 0.5f));
        UIManager.Instance.StartCoroutine(UIManager.Instance.HideTemporarily(eUIGameObjectName.RightButton, 0.5f));

        if ((bool)GameManager.Instance.GetVariable("isTutorial"))
            tutorialManager?.SetSeenSide(newSideIndex);
    }

    public void OnExitButtonClick()
    {
        if (isInvestigating) {
            imageAndLockPanelManager.OnExitButtonClick();
        }
        else if (isZoomed) {
            UIManager.Instance.MoveSideEffect(sides[currentSideIndex], new Vector3(0, -0.5f, 0)); // 화면 전환 효과
            UIManager.Instance.StartCoroutine(UIManager.Instance.HideTemporarily(eUIGameObjectName.ExitButton, 0.5f));

            SetCurrentView(sides[currentSideIndex]);
            currentZoomViewName = null;
            isZoomed = false;
        }

        StartCoroutine(SetButtonsAfterDelay(0.5f));

        var refillHeartsOrEndDay = (bool)GameManager.Instance.GetVariable("RefillHeartsOrEndDay");
        if (refillHeartsOrEndDay)
            actionPointManager.RefillHeartsOrEndDay();

        GameManager.Instance.SetVariable("isZoomed", isZoomed);
        GameManager.Instance.SetVariable("currentZoomViewName", "NONE");

        SaveManager.Instance.SaveGameData();
    }

    // 비밀번호 무한 입력 시도 방지
    public void ProhibitInput() {
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
            currentZoomViewName = null;
            isZoomed = false;
        }
        
        SetButtons();

        GameManager.Instance.SetVariable("isZoomed", isZoomed);
        GameManager.Instance.SetVariable("currentZoomViewName", "NONE");

        SaveManager.Instance.SaveGameData();
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

        GameManager.Instance.SetVariable("isZoomed", isZoomed);
        GameManager.Instance.SetVariable("currentZoomViewName", currentZoomViewName);

        SaveManager.Instance.SaveGameData();
    }

    // 시점 전환
    private void SetCurrentSide(int newSideIndex)
    {
        SetCurrentView(sides[newSideIndex]);
        currentSideIndex = newSideIndex;
        GameManager.Instance.SetVariable("currentSideIndex", currentSideIndex);
        SaveManager.Instance.SaveGameData();
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
    
    private IEnumerator SetButtonsAfterDelay(float delayTime = 0.5f)
    {
        yield return new WaitForSeconds(delayTime);
        SetButtons();
    }
}