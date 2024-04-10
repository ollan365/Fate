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
    
    private GameObject currentView;  // 현재 뷰
    [Header("시점들")][SerializeField] private List<GameObject> sides;  // 시점들
    private int currentSideIndex = 0;  // 현재 시점 인덱스

    [Header("확대 화면들")][SerializeField] private List<GameObject> zoomViews;  // 확대 화면들
    
    // 조사 중이거나 확대 중이면 이동키로 시점 바꾸지 못하게 함
    public bool isInvestigating = false;
    public bool isZoomed = false;

    private List<GameObject> screenObjects = new List<GameObject>();

    // 다이얼로그 매니저의 isDialogueActive가 true면 다른 버튼들 비활성화시킴
    private Button[] otherButtons;

    // 이동 버튼
    [Header("이동 버튼들")] 
    [SerializeField] private Button moveButtonLeft;
    [SerializeField] private Button moveButtonRight;

    // 나가기 버튼
    [Header("나가기 버튼")] [SerializeField] private Button exitButton;

    // 이벤트오브젝트패널
    [Header("이벤트 오브젝트 패널")] [SerializeField] private GameObject eventObjectPanel;
    
    [Header("이벤트 오브젝트 확대 이미지")]
    [SerializeField] private GameObject amuletImage;
    [SerializeField] private GameObject carpetPaperImage;
    [SerializeField] private GameObject clockImage;
    [SerializeField] private GameObject keysImage;
    [SerializeField] private GameObject knifeImage;
    [SerializeField] private GameObject posterImage;
    [SerializeField] private GameObject liquorImage;


    // 시연 용 미행 씬 이동 버튼
    [SerializeField] private Button goFollowSceneButton;

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
        // "Controlled Button" 태그를 가진 모든 버튼들을 찾아서 otherButtons에 넣음.
        otherButtons = GameObject.FindGameObjectsWithTag("Controlled Button").Select(g => g.GetComponent<Button>()).ToArray();

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

        // 메모 버튼이 보이게 함
        MemoManager.Instance.HideMemoButton(false);

        // 시연 용 미행 씬 이동 버튼 기능 추가
        goFollowSceneButton.onClick.AddListener(() => SceneManager.LoadScene(2));

        DialogueManager.Instance.StartDialogue("Prologue_015");
    }

    // A키와 D키로 시점 이동
    void Update()
    {
        //if (isInvestigating || DialogueManager.Instance.isDialogueActive) return; 
        //if (Input.GetKeyDown(KeyCode.A))
        //{
        //    int newSideIndex = (currentSideIndex - 1 + sides.Count) % sides.Count;
        //    SetCurrentSide(newSideIndex);
        //}
        //else if (Input.GetKeyDown(KeyCode.D))
        //{
        //    int newSideIndex = (currentSideIndex + 1) % sides.Count;
        //    SetCurrentSide(newSideIndex);
        //}

        // 조사/다이얼로그 출력을 안 하고 있을 때만 이동 버튼이 보임.
        if (isInvestigating || DialogueManager.Instance.isDialogueActive)
            SetMoveButton(false);
        else
            SetMoveButton(true);
    }

    // 왼쪽 이동 버튼
    public void MoveLeftButton()
    {
        if (isInvestigating || DialogueManager.Instance.isDialogueActive) return;
        else
        {
            int newSideIndex = (currentSideIndex - 1 + sides.Count) % sides.Count;
            SetCurrentSide(newSideIndex);

            // 화면 이동 효과
            ScreenEffect.Instance.MoveButtonEffect(sides[newSideIndex], new Vector3(-1, 0, 0));
        }
    }

    // 오른쪽 이동 버튼
    public void MoveRightButton()
    {
        if (isInvestigating || DialogueManager.Instance.isDialogueActive) return;
        else
        {
            int newSideIndex = (currentSideIndex + 1) % sides.Count;
            SetCurrentSide(newSideIndex);

            // 화면 이동 효과
            ScreenEffect.Instance.MoveButtonEffect(sides[newSideIndex], new Vector3(1, 0, 0));
        }
    }

    // 이동 버튼들이 조사/다이얼로그 출력 시에는 화면 상에서 보이지 않게 함
    public void SetMoveButton(bool isTrue)
    {
        moveButtonLeft.gameObject.SetActive(isTrue);
        moveButtonRight.gameObject.SetActive(isTrue);
    }


    public void ControllEventButtons(bool isTrue)
    {
        foreach (Button button in otherButtons)
        {
            button.interactable = isTrue;
        }
    }

    public void SearchExitButton()
    {
        if (isInvestigating)
        {
            DeactivateObjects();
            isInvestigating = false;
        }
        else if (isZoomed)
        {
            SetCurrentView(sides[currentSideIndex]);
            isZoomed = false;
        }
        
        if (!isInvestigating && !isZoomed) exitButton.gameObject.SetActive(false); 
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
    }

    // 시점 전환
    private void SetCurrentSide(int newSideIndex)
    {
        SetCurrentView(sides[newSideIndex]);
        currentSideIndex = newSideIndex;
    }
    
    // 뷰 전환
    public void SetCurrentView(GameObject newView)
    {
        currentView.SetActive(false);
        newView.SetActive(true);
        currentView = newView;
        
        if (!sides.Contains(newView))  // side 중 하나가 아니라면 확대중인 화면이다
        {
            SetExitButton(true);
            isZoomed = true;
        }
    }

    // 나가기 버튼 필요 시, 보이게 함 (ResultManager에서 호출하게 함)
    public void SetExitButton(bool isTrue)
    {
        exitButton.gameObject.SetActive(isTrue);
    }

    // EventObjectPanel 켜서 해당 오브젝트 확대 UI 보여줌
    public void SetEventObjectPanel(bool isTrue, string objName)
    {
        eventObjectPanel.SetActive(isTrue);
        if (eventObjectPanel.activeSelf)
        {
            AddScreenObjects(eventObjectPanel);
            SetExitButton(true);
        }
        switch (objName)
        {
            case "Pillow":
                amuletImage.SetActive(isTrue);
                AddScreenObjects(amuletImage);
                break;
            case "Carpet_Paper":
                carpetPaperImage.SetActive(isTrue);
                AddScreenObjects(carpetPaperImage);
                break;
            case "clock1":
                clockImage.SetActive(isTrue);
                AddScreenObjects(clockImage);
                break;
            case "ClockKeys":
                keysImage.SetActive(isTrue);
                AddScreenObjects(keysImage);
                break;
            case "Knife":
                knifeImage.SetActive(isTrue);
                AddScreenObjects(knifeImage);
                break;
            case "Poster":
                posterImage.SetActive(isTrue);
                AddScreenObjects(posterImage);
                break;
            case "Liquor":
                liquorImage.SetActive(isTrue);
                AddScreenObjects(liquorImage);
                break;
        }
    }

}
