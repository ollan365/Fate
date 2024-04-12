using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RoomManager : MonoBehaviour
{
    public static RoomManager Instance { get; private set; }

    // 이게 사라져있었습니다 할당 부탁드립니당...
    public GameObject eventObjectPanel;
    // 이외에도 인스펙터 창에 Room Manager에 할당된 것들이 많이 사라졌습니다... ㅠㅠ
    // ClockPuzzle의 Awake 부분 주석 처리했는데 수정 부탁드립니당...

    [Header("시점들")][SerializeField] private List<GameObject> sides;  // 시점들
    private GameObject currentView;  // 현재 뷰
    private int currentSideIndex = 0;  // 현재 시점 인덱스

    // [0] 시점 1번
    // [2] 시점 2번
    // [1] 시점 3번

    [Header("확대 화면들")][SerializeField] private List<GameObject> zoomViews;  // 확대 화면들
    
    // 조사 중이거나 확대 중이면 이동키로 시점 바꾸지 못하게 함
    public bool isInvestigating = false;
    public bool isZoomed = false;

    private List<GameObject> screenObjects = new List<GameObject>();

    //// 다이얼로그 매니저의 isDialogueActive가 true면 다른 버튼들 비활성화시킴
    //private Button[] otherButtons;

    // 버튼 클릭 막는 투명 패널
    [Header("블록 패널")]
    [SerializeField] private GameObject BlockingPanel1;
    [SerializeField] private GameObject BlockingPanel2;
    [SerializeField] private GameObject BlockingPanel3;

    // 이동 버튼
    [Header("이동 버튼들")] 
    [SerializeField] private Button moveButtonLeft;
    [SerializeField] private Button moveButtonRight;

    // 나가기 버튼
    [Header("나가기 버튼")] [SerializeField] private Button exitButton;

    // 이벤트 오브젝트 패널 매니저
    [FormerlySerializedAs("eventObjectPanelManager")] public ImageAndLockPanelManager imageAndLockPanelManager;
    
    //[Header("이벤트 오브젝트 확대 이미지")]
    //[SerializeField] private GameObject amuletImage;
    //[SerializeField] private GameObject carpetPaperImage;
    //[SerializeField] private GameObject clockImage;
    //[SerializeField] private GameObject keysImage;
    //[SerializeField] private GameObject knifeImage;
    //[SerializeField] private GameObject posterImage;
    //[SerializeField] private GameObject liquorImage;

    // 시연 용 미행 씬 이동 버튼
    [SerializeField] private Button goFollowSceneButton;

    // 튜토리얼 관련..
    [Header("튜토리얼 관련")]
    [SerializeField] private GameObject tutorialLogic;
    [SerializeField] private GameObject chairButton;
    [SerializeField] private GameObject carpetButton;
    //// 튜토리얼 때는 메모 버튼 볼 수 없게 함 
    //[SerializeField] private GameObject memoButton;

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

        // Side 1으로 초기화
        currentView = sides[0];
        SetCurrentSide(0);

        // 처음 시작하면 튜토리얼 끝나기 전까지는 메모 버튼이 보이지 않게 함.
        MemoManager.Instance.HideMemoButton(true);

        // 시연 용 미행 씬 이동 버튼 기능 추가
        goFollowSceneButton.onClick.AddListener(() => SceneManager.LoadScene(2));

        //// "Controlled Button" 태그를 가진 모든 버튼들을 찾아서 otherButtons에 넣음.
        //// Side1의 버튼들만 들어간 상태..
        //otherButtons = GameObject.FindGameObjectsWithTag("Controlled Button").Select(g => g.GetComponent<Button>()).ToArray();

        DialogueManager.Instance.StartDialogue("Prologue_015");
    }

    // A키와 D키로 시점 이동
    void Update()
    {
        // 조사/다이얼로그 출력을 안 하고 있을 때만 이동 버튼이 보임.
        if (isInvestigating || DialogueManager.Instance.isDialogueActive)
        {
            SetMoveButton(false);
            MemoManager.Instance.HideMemoButton(true);
        }
        else
        {
            SetMoveButton(true);
            MemoManager.Instance.HideMemoButton(false);
        }

    }

    //// 왼쪽 이동 버튼
    //public void MoveLeftButton()
    //{
    //    if (isInvestigating || DialogueManager.Instance.isDialogueActive) return;
    //    else
    //    {
    //        int newSideIndex = (currentSideIndex - 1 + sides.Count) % sides.Count;
    //        SetCurrentSide(newSideIndex);

    //        // 화면 이동 효과
    //        ScreenEffect.Instance.MoveButtonEffect(sides[newSideIndex], new Vector3(-1, 0, 0));

    //        // 튜토리얼 관련
    //        if(tutorialLogic.activeSelf)
    //            tutorialLogic.GetComponent<TutorialLogic>().Tutorial_MoveLeft();

    //    }
    //}
    //// 오른쪽 이동 버튼
    //public void MoveRightButton()
    //{
    //    if (isInvestigating || DialogueManager.Instance.isDialogueActive) return;
    //    else
    //    {
    //        int newSideIndex = (currentSideIndex + 1) % sides.Count;
    //        SetCurrentSide(newSideIndex);

    //        // 화면 이동 효과
    //        ScreenEffect.Instance.MoveButtonEffect(sides[newSideIndex], new Vector3(1, 0, 0));

    //        // 튜토리얼 관련
    //        if (tutorialLogic.activeSelf)
    //            tutorialLogic.GetComponent<TutorialLogic>().Tutorial_MoveRight();
    //    }
    //}

    public void MoveSides(int leftOrRight)  // left: -1, right: 1
    {
        if (leftOrRight != -1 && leftOrRight != 1)
        {
            Debug.Log("Value of leftOrRight must be -1 or 1!");
            return;
        }

        if (isInvestigating || DialogueManager.Instance.isDialogueActive) return;
        else
        {
            int newSideIndex = (currentSideIndex + sides.Count + leftOrRight) % sides.Count;
            SetCurrentSide(newSideIndex);

            // 화면 이동 효과
            ScreenEffect.Instance.MoveButtonEffect(sides[newSideIndex], new Vector3(leftOrRight, 0, 0));

            // 튜토리얼 관련
            if (tutorialLogic.activeSelf)
            {
                if (leftOrRight == -1)
                    tutorialLogic.GetComponent<TutorialLogic>().Tutorial_MoveLeft();
                else
                    tutorialLogic.GetComponent<TutorialLogic>().Tutorial_MoveRight();
            }
        }
    }

    // 이동 버튼들이 조사/다이얼로그 출력 시에는 화면 상에서 보이지 않게 함
    public void SetMoveButton(bool isTrue)
    {
        //moveButtonLeft.gameObject.SetActive(isTrue);
        //moveButtonRight.gameObject.SetActive(isTrue);

        if ((isInvestigating || DialogueManager.Instance.isDialogueActive)&&!isTrue)
        {
            moveButtonLeft.gameObject.SetActive(false);
            moveButtonRight.gameObject.SetActive(false);
        }
        else
        {
            if (isTrue)
            {
                // 사이드 1번에선 양쪽 버튼 다 보임
                if (currentSideIndex == 0)
                {
                    moveButtonLeft.gameObject.SetActive(true);
                    moveButtonRight.gameObject.SetActive(true);
                }
                else if (currentSideIndex == 1)  // 사이드 3번에선 왼쪽 버튼만 보임
                {
                    moveButtonLeft.gameObject.SetActive(true);
                    moveButtonRight.gameObject.SetActive(false);
                }
                else if (currentSideIndex == 2)   // 사이드 2번에선 오른쪽 버튼만 보임
                {
                    moveButtonLeft.gameObject.SetActive(false);
                    moveButtonRight.gameObject.SetActive(true);
                }
                else
                {
                    moveButtonLeft.gameObject.SetActive(false);
                    moveButtonRight.gameObject.SetActive(false);
                }
            }
        }
    }

    // ------------------------ 튜토리얼 관련 ------------------------
    // 튜토리얼1 : 이벤트버튼들 제외하고 이동 버튼들만 상호작용가능
    public void Tutorial_SetMoveButtonInteractable(bool isTrue)
    {
        moveButtonLeft.interactable = isTrue;
        moveButtonRight.interactable = isTrue;
    }

    // 튜토리얼2 : 의자, 카펫 버튼만 클릭 가능
    // 그리고 튜토리얼 끝나면 다시 BlockingPanel1을 맨 앞으로 올리기
    public void Tutorial2_ChairAndCarpetInteractable(bool isTrue)
    {
        //carpetButton.interactable = isTrue;
        //chairButton.interactable = isTrue;

        if (isTrue)
        {
            carpetButton.GetComponent<RectTransform>().SetAsLastSibling();
            chairButton.GetComponent<RectTransform>().SetAsLastSibling();
        }
        else
        {
            BlockingPanel1.GetComponent<RectTransform>().SetAsLastSibling();
        }
    }

    public void SetTutorialLogic(bool isTrue)
    {
        tutorialLogic.SetActive(isTrue);
    }
    //// 메모버튼 튜토리얼 중에는 보이지 않게 함 온오프 함수
    //public void SetMemoButton(bool isTrue)
    //{
    //    MemoManager.Instance.HideMemoButton(!isTrue);
    //}

    // ---------------------------------------------------------------

    public void SetBlockingPanel(bool isTrue)
    {
        //foreach (Button button in otherButtons)
        //{
        //    button.interactable = isTrue;
        //    //Debug.Log(button.name+ " "+button.interactable);
        //}

        // 투명한 패널 켜기
        BlockingPanel1.gameObject.SetActive(isTrue);
        BlockingPanel2.gameObject.SetActive(isTrue);
        BlockingPanel3.gameObject.SetActive(isTrue);
    }

    public void OnExitButtonClick()
    {
        if (isInvestigating)
        {
            // 지은님 코드
            DeactivateObjects();
            SetIsInvestigating(false);
            SetBlockingPanel(false);
            
            // 성환님 코드
            imageAndLockPanelManager.OnExitButtonClick();
            
            SetButtons();
            
            return;
        }
        
        if (isZoomed)
        {
            SetCurrentView(sides[currentSideIndex]);
            isZoomed = false;
            
            SetButtons();
        }

        if (!isInvestigating && !isZoomed)
        {
            exitButton.gameObject.SetActive(false);

            SetBlockingPanel(false);
            SetMoveButton(true);
        }
    }

    private void DeactivateObjects()
    {
        foreach (GameObject obj in screenObjects)
        {
            obj.SetActive(false);
        }

        //ScreenObjects.Clear();
    }

    public void AddScreenObjects(GameObject obj)
    {
        if (!screenObjects.Contains(obj)) screenObjects.Add(obj);
    }

    // ######################################## setters ########################################
    public void SetIsInvestigating(bool isTrue)
    {
        isInvestigating = isTrue;
        GameManager.Instance.SetVariable("IsInvestigating", isTrue);
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

        GameManager.Instance.SetVariable("RoomCurrentSideIndex", currentSideIndex);

        //otherButtons = GameObject.FindGameObjectsWithTag("Controlled Button")
        //.Select(g =>
        //{
        //    Button button = g.GetComponent<Button>();
        //    // 버튼 이름에 currentSideIndex 추가
        //    button.name += "_" + currentSideIndex.ToString();
        //    return button;
        //})
        //.ToArray();
    }
    
    // 뷰 전환
    public void SetCurrentView(GameObject newView)
    {
        currentView.SetActive(false);
        newView.SetActive(true);
        currentView = newView;
    }
    
    // 나가기 버튼 필요 시, 보이게 함 (ResultManager에서 호출하게 함)
    public void SetExitButton(bool isTrue)
    {
        exitButton.gameObject.SetActive(isTrue);
    }

    // 아래의 SetEventObjectPanel 대신 ImageAndLockPanelManager 적용

    //// EventObjectPanel 켜서 해당 오브젝트 확대 UI 보여줌
    //// 이 상태에는 나가기 버튼 제외 다른 오브젝트 버튼들,이동 버튼 클릭X
    //public void SetEventObjectPanel(bool isTrue, string objName)
    //{
    //    eventObjectPanel.SetActive(isTrue);
    //    if (eventObjectPanel.activeSelf)
    //    {
    //        AddScreenObjects(eventObjectPanel);
    //        SetExitButton(true);
    //        SetBlockingPanel(true);
    //        SetMoveButton(false);

    //        SetIsInvestigating(true);
    //    }
    //    switch (objName)
    //    {
    //        case "Pillow":
    //            amuletImage.SetActive(isTrue);
    //            AddScreenObjects(amuletImage);
    //            break;
    //        case "Carpet_Paper":
    //            carpetPaperImage.SetActive(isTrue);
    //            AddScreenObjects(carpetPaperImage);
    //            break;
    //        case "clock1":
    //            clockImage.SetActive(isTrue);
    //            AddScreenObjects(clockImage);
    //            break;
    //        case "ClockKeys":
    //            keysImage.SetActive(isTrue);
    //            AddScreenObjects(keysImage);
    //            break;
    //        case "Knife":
    //            knifeImage.SetActive(isTrue);
    //            AddScreenObjects(knifeImage);
    //            break;
    //        case "Poster":
    //            posterImage.SetActive(isTrue);
    //            AddScreenObjects(posterImage);
    //            break;
    //        case "Liquor":
    //            liquorImage.SetActive(isTrue);
    //            AddScreenObjects(liquorImage);
    //            break;
    //    }
    //}

    public void SetButtons()
    {
        bool isInvestigatingOrZoomed = isInvestigating || isZoomed;
        bool isDialogueActive = DialogueManager.Instance.isDialogueActive;
        
        SetExitButton(isInvestigatingOrZoomed && !isDialogueActive);
        SetMoveButton(!(isInvestigatingOrZoomed || isDialogueActive));
    }
    
}
