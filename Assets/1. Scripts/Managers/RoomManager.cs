using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class RoomManager : MonoBehaviour
{
    public static RoomManager Instance { get; private set; }
    
    public GameObject roomP1;
    public GameObject roomP2;
    public GameObject roomP3;

    // 조사 중이면 이동키로 시점 바꾸지 못하게 함
    public bool isResearch = false;

    public List<GameObject> ScreenObjects = new List<GameObject>();

    // 다이얼로그 매니저의 isDialogueActive가 true면 다른 버튼들 비활성화시킴
    private Button[] otherButtons;

    // 나가기 버튼
    private Button exitBtn;

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
        exitBtn = GameObject.Find("Exit Button").GetComponent<Button>();
        // "Controlled Button" 태그를 가진 모든 버튼들을 찾아서 otherButtons에 넣음.
        otherButtons = GameObject.FindGameObjectsWithTag("Controlled Button").Select(g => g.GetComponent<Button>()).ToArray();

        roomP1.SetActive(true);
        roomP2.SetActive(false);
        roomP3.SetActive(false);
        DialogueManager.Instance.StartDialogue("Prologue_015");
    }

    // A키와 D키로 시점 이동
    void Update()
    {
        //// 대사 출력되고 있으면 버튼들 클릭 비활성화시킴
        //if (DialogueManager.Instance.isDialogueActive)
        //{
        //    StartCoroutine(offControlledBtn());
        //}
        //else // 대사 출력이 끝나면 버튼들 클릭 다시 활성화시킴
        //{
        //    StartCoroutine(onControlledBtn());
        //}
        

        if (!isResearch)
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                // P1 -> P2
                // P2 -> P3
                // P3 -> P1
                if (roomP1.activeSelf)
                {
                    roomP1.SetActive(false);
                    roomP2.SetActive(true);
                }
                else if (roomP2.activeSelf)
                {
                    roomP2.SetActive(false);
                    roomP3.SetActive(true);
                }
                else
                {
                    roomP3.SetActive(false);
                    roomP1.SetActive(true);
                }
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                // P1 -> P3
                // P2 -> P1
                // P3 -> P2
                if (roomP1.activeSelf)
                {
                    roomP1.SetActive(false);
                    roomP3.SetActive(true);
                }
                else if (roomP2.activeSelf)
                {
                    roomP2.SetActive(false);
                    roomP1.SetActive(true);
                }
                else
                {
                    roomP3.SetActive(false);
                    roomP2.SetActive(true);
                }
            }
        }
        else
        {
            if (!DialogueManager.Instance.isDialogueActive)
            {
                // 조사 중이면 다른 단서들 클릭 못하게 막기... 나가기 버튼은 활성화 해둬야 함.
                offControlledBtn();

                exitBtn.interactable = true;
            }
        }
    }

    public void SearchExitBtn()
    {
        DeactivateObjects();
        onControlledBtn();
        isResearch = false;
    }

    private void DeactivateObjects()
    {
        foreach (GameObject obj in ScreenObjects)
        {
            obj.SetActive(false);
        }

        //ScreenObjects.Clear();
    }

    public void AddScreenObjects(GameObject obj)
    {
        if (!ScreenObjects.Contains(obj)) ScreenObjects.Add(obj);
    }

    // 대사 출력 동안 버튼들 클릭 비활성화
    public void offControlledBtn()
    {
        foreach (Button button in otherButtons)
        {
            button.interactable = false;
        }
    }
    //private IEnumerator offControlledBtn()
    //{
    //    foreach (Button button in otherButtons)
    //    {
    //        button.interactable = false;
    //    }

    //    yield return null;
    //}

    // 대사 출력 끝나면 버튼들 클릭 다시 활성화
    public void onControlledBtn()
    {
        foreach (Button button in otherButtons)
        {
            button.interactable = true;
        }
    }
    //private IEnumerator onControlledBtn()
    //{
    //    foreach (Button button in otherButtons)
    //    {
    //        button.interactable = true;
    //    }

    //    yield return null;
    //}

}
