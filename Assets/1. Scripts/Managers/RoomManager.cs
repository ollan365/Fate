using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class RoomManager : MonoBehaviour
{
    public static RoomManager Instance { get; private set; }
    
    private GameObject currentView;  // 현재 뷰
    [SerializeField] private List<GameObject> sides;  // 시점들
    private int currentSideIndex = 0;  // 현재 시점 인덱스

    // 조사 중이면 이동키로 시점 바꾸지 못하게 함
    public bool isInvestigating = false;

    private List<GameObject> screenObjects = new List<GameObject>();

    // 다이얼로그 매니저의 isDialogueActive가 true면 다른 버튼들 비활성화시킴
    private Button[] otherButtons;

    // 나가기 버튼
    [Header("나가기 버튼")] [SerializeField] private Button exitButton;

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
        
        // Side 1으로 초기화
        currentView = sides[0];
        SetCurrentSide(0);
        
        DialogueManager.Instance.StartDialogue("Prologue_015");
    }

    // A키와 D키로 시점 이동
    void Update()
    {
        if (isInvestigating || DialogueManager.Instance.isDialogueActive) return; 
        if (Input.GetKeyDown(KeyCode.A))
        {
            int newSideIndex = (currentSideIndex - 1 + sides.Count) % sides.Count;
            SetCurrentSide(newSideIndex);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            int newSideIndex = (currentSideIndex + 1) % sides.Count;
            SetCurrentSide(newSideIndex);
        }
    }

    public void SearchExitButton()
    {
        DeactivateObjects();
        isInvestigating = false;
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
    private void SetIsInvestigating(bool isTrue)
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
    private void SetCurrentView(GameObject newView)
    {
        currentView.SetActive(false);
        newView.SetActive(true);
        currentView = newView;
    }
}
