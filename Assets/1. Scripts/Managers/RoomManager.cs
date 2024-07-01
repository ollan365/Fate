using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityEditor.SceneManagement;

public class RoomManager : MonoBehaviour
{
    public static RoomManager Instance { get; private set; }
    
    [Header("시점들")][SerializeField] private List<GameObject> sides;  // 시점들
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

        // Side 1으로 초기화?
        currentView = sides[currentSideIndex];
        SetCurrentSide(currentSideIndex);
        
        SetButtons();


        // 첫 대사 출력 후 튜토리얼 1페이즈 시작(현재 씬 이름이 Room1일 때만)
        if (EditorSceneManager.GetActiveScene().name == "Room1")
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
        if (EditorSceneManager.GetActiveScene().name == "Room1")
        {
            if(!GameManager.Instance.skipTutorial)
                tutorialManager.SetSeenSides(newSideIndex);
        }
    }

    public void OnExitButtonClick()
    {
        if (isInvestigating)
        {
            imageAndLockPanelManager.OnExitButtonClick();
            
            SetButtons();
            
            return;
        }
        
        if (isZoomed)
        {
            // 화면 전환 효과
            ScreenEffect.Instance.MoveButtonEffect(sides[currentSideIndex], new Vector3(0, -0.5f, 0));

            SetCurrentView(sides[currentSideIndex]);
            isZoomed = false;
            
            SetButtons();
        }
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
    public void SetExitButton(bool isTrue)
    {
        exitButton.gameObject.SetActive(isTrue);
    }
    
    // 이동 버튼들이 조사/다이얼로그 출력 시에는 화면 상에서 보이지 않게 함
    private void SetMoveButtons(bool isTrue)
    {
        moveButtonLeft.gameObject.SetActive(isTrue);
        moveButtonRight.gameObject.SetActive(isTrue);
        
        if (currentSideIndex == 1) moveButtonRight.gameObject.SetActive(false);
        else if (currentSideIndex == 2) moveButtonLeft.gameObject.SetActive(false);
    }

    public void SetButtons()
    {
        bool isInvestigatingOrZoomed = isInvestigating || isZoomed;
        bool isDialogueActive = DialogueManager.Instance.isDialogueActive;
        
        SetExitButton(isInvestigatingOrZoomed && !isDialogueActive);
        SetMoveButtons(!(isInvestigatingOrZoomed || isDialogueActive));
    }

    //[SerializeField] private GameObject restButton;
    //public void HideRestButton(bool flag)
    //{
    //    // 휴식 버튼을 보이지 않게 or 보이게 할 수 있음
    //    restButton.SetActive(!flag);
    //}
}