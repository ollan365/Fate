using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityEditor.SceneManagement;

public class RoomManager : MonoBehaviour
{
    public static RoomManager Instance { get; private set; }
    
    [Header("시점들")][SerializeField] public List<GameObject> sides;  // 시점들
    private GameObject currentView;  // 현재 뷰
    public int currentSideIndex = 0;  // 현재 시점 인덱스
    
    [Header("확대 화면들")][SerializeField] private List<GameObject> zoomViews;  // 확대 화면들

    // 이동 버튼
    [Header("이동 버튼들")] 
    [SerializeField] private Button moveButtonLeft;
    [SerializeField] private Button moveButtonRight;

    // 나가기 버튼
    [Header("나가기 버튼")] [SerializeField] private Button exitButton;

    // 이벤트 오브젝트 패널 매니저
    public ImageAndLockPanelManager imageAndLockPanelManager;
    // 조사 중이거나 확대 중이면 이동키로 시점 바꾸지 못하게 함
    public bool isInvestigating = false;
    private bool isZoomed = false;
    
    // 튜토리얼 매니저
    public TutorialManager tutorialManager;

    // 액션포인트 매니저
    public ActionPointManager actionPointManager;

    // ************************* temporary members for action points *************************
    [SerializeField] GameObject heartParent;
    [SerializeField] TextMeshProUGUI dayText;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        
        ResultManager.Instance.InitializeExecutableObjects();
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
        
        SetButtons();

        actionPointManager.heartParent = heartParent;
        actionPointManager.dayText = dayText;
        actionPointManager.CreateHearts();  // create hearts on room start

        // 아래는 뭘 살려야할지 모르겠어서 두개 모두 살려뒀습니다
        // 참고로 SceneManager.Instance.CurrentScene == SceneType.ROOM_1이 true 이면 현재 씬이 Room1 입니당...!
        // 도움되실까 싶어서 남겨둡니당...
        
        //// 첫 대사 출력 후 튜토리얼 1페이즈 시작(현재 씬 이름이 Room1일 때만) - 겜메에서 현재 씬 이름 저장하고 가져오는 방식으로 변경 필요
        //if (!GameManager.Instance.skipTutorial && EditorSceneManager.GetActiveScene().name == "Room1") DialogueManager.Instance.StartDialogue("Prologue_015");

        // 첫 대사 출력 후 튜토리얼 1페이즈 시작(현재 씬 이름이 Room1일 때만)
        if (SceneManager.Instance.CurrentScene == Constants.SceneType.ROOM_1)
        {
            if(!GameManager.Instance.skipTutorial)
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
        
        int newSideIndex = (currentSideIndex + sides.Count + leftOrRight) % sides.Count;
        SetCurrentSide(newSideIndex);
        
        ScreenEffect.Instance.MoveButtonEffect(sides[newSideIndex], new Vector3(leftOrRight, 0, 0));

        // 시점에 맞춰서 버튼 끄고 키게 함.(사이드 2번에선 오른쪽 버튼만 3번에선 왼쪽 버튼만 나오게 함)
        SetMoveButtons(true);

        // 튜토리얼 1 페이즈 관련
        if (SceneManager.Instance.CurrentScene == Constants.SceneType.ROOM_1)
        {
            if(!GameManager.Instance.skipTutorial)
                tutorialManager.SetSeenSides(newSideIndex);
        }
    }

    public void OnExitButtonClick()
    {
        if (isInvestigating) imageAndLockPanelManager.OnExitButtonClick();
        else if (isZoomed)
        {
            // 화면 전환 효과
            ScreenEffect.Instance.MoveButtonEffect(sides[currentSideIndex], new Vector3(0, -0.5f, 0));

            SetCurrentView(sides[currentSideIndex]);
            isZoomed = false;
        }

        SetButtons();
    }
    
    // exit to root: turn off all the panels and zoom out to the root view
    public void ExitToRoot()
    {
        while (isInvestigating) imageAndLockPanelManager.OnExitButtonClick();
        if (isZoomed)
        {
            SetCurrentView(sides[currentSideIndex]);
            isZoomed = false;
        }
        
        SetButtons();
    }

    public void EmptyActionPoint()
    {
        GameManager.Instance.SetVariable("ActionPoint", 1);
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
        exitButton.gameObject.SetActive(isTrue);
    }
    
    // 이동 버튼들이 조사/다이얼로그 출력 시에는 화면 상에서 보이지 않게 함
    private void SetMoveButtons(bool isTrue)
    {
        moveButtonLeft.gameObject.SetActive(isTrue);
        moveButtonRight.gameObject.SetActive(isTrue);
        
        switch (currentSideIndex)
        {
            case 1:
                moveButtonRight.gameObject.SetActive(false);
                break;
            case 2:
                moveButtonLeft.gameObject.SetActive(false);
                break;
        }
    }
    
    public void SetButtons()
    {
        bool isInvestigatingOrZoomed = isInvestigating || isZoomed;
        bool isDialogueActive = DialogueManager.Instance.isDialogueActive;
        bool isMemoOpen = MemoManager.Instance.isMemoOpen;
        
        SetExitButton(isInvestigatingOrZoomed && !isDialogueActive && !isMemoOpen);
        SetMoveButtons(!isInvestigatingOrZoomed && !isDialogueActive && !isMemoOpen);
        // MemoManager.Instance.SetMemoButton(!isDialogueActive && !isMemoOpen);
    }
}