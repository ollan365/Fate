using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class ResultManager : MonoBehaviour
{
    public static ResultManager Instance { get; private set; }

    private TextAsset resultsCSV;
    
    // results: dictionary of "Results"s indexed by string "Result ID"
    public Dictionary<string, Result> results = new Dictionary<string, Result>();
    
    // 이벤트 오브젝트 참조
    private Dictionary<string, IResultExecutable> executableObjects = new Dictionary<string, IResultExecutable>();
    
    public void RegisterExecutable(string objectName, IResultExecutable executable)
    {
        if (!executableObjects.ContainsKey(objectName))
        {
            executableObjects[objectName] = executable;
        }
    }

    void Awake()
    {
        if (Instance == null)
        {
            resultsCSV = Resources.Load<TextAsset>("Datas/results");
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public void ParseResults()
    {
        string[] lines = resultsCSV.text.Split('\n');
        for (int i = 1; i < lines.Length; i++)
        {
            string[] fields = lines[i].Split(',');
            
            if ((string.IsNullOrWhiteSpace(lines[i])) || (fields[0] == "" && fields[1] == "")) continue;
            
            Result result = new Result(
                fields[0].Trim(),
                fields[1].Trim(),
                fields[2].Trim()
                );
            
            results[result.ResultID] = result;
        }
    }
    
    public void ExecuteResult(string resultID)
    {
        if (GameManager.Instance.isDebug) Debug.Log(resultID);
        
        // ------------------------ 이곳에 모든 동작을 수동으로 추가 ------------------------
        switch (resultID)
        {
            case "Result_girl":  // 우연의 성별을 여자로 설정
                GameManager.Instance.SetVariable("AccidyGender", 0);
                DialogueManager.Instance.StartDialogue("Prologue_006");
                break;
            
            case "Result_boy":  // 우연의 성별을 남자로 설정
                GameManager.Instance.SetVariable("AccidyGender", 1);
                DialogueManager.Instance.StartDialogue("Prologue_007");
                break;
            
            case "Result_001": // 눈 뜨는 효과
                executableObjects["StartLogic"].ExecuteAction();
                break;
            
            case "Result_002": // 설명창 띄우기 - ### 추후 구현 필요 ###
                SceneManager.LoadScene(1);
                break;
            
            case "Result_003": // 행동력 감소
                GameManager.Instance.DecrementVariable("ActionPoint");
                break;
            
            case "Result_004": // 조사 완료 스크립트
                DialogueManager.Instance.StartDialogue("RoomEscape_008");
                break;

            // 튜토리얼
            case "Tutorial_Result_001":
                DialogueManager.Instance.StartDialogue("Tutorial_001");
                break;

            case "Tutorial_Result_002":
                GameManager.Instance.SetVariable("Tutorial_Now", 1);
                break;

            case "Tutorial_Result_003":
                RoomManager.Instance.SetBlockingPanel(true);
                RoomManager.Instance.Tutorial_SetMoveButtonInteractable(true);
                break;

            case "Tutorial_Result_004":
                GameManager.Instance.SetVariable("Tutorial_Now", 2);
                break;

            case "Tutorial_Result_005":
                //이동버튼 가능, 시점1번 가기 전까지 다른 이벤트오브젝트버튼들 클릭X
                DialogueManager.Instance.StartDialogue("Tutorial_002_A");
                RoomManager.Instance.SetBlockingPanel(true);
                RoomManager.Instance.Tutorial_SetMoveButtonInteractable(true);
                break;

            case "Tutorial_Result_006":
                DialogueManager.Instance.StartDialogue("Tutorial_002_B");
                //스크립트 끝나면 이동버튼 클릭X, 의자, 카펫 버튼 이외 이벤트오브젝트버튼들 클릭안됨
                RoomManager.Instance.SetBlockingPanel(true);
                RoomManager.Instance.Tutorial_SetMoveButtonInteractable(false);
                RoomManager.Instance.Tutorial2_ChairAndCarpetInteractable(true);
                break;

            case "Tutorial_Result_007":
                DialogueManager.Instance.StartDialogue("Tutorial_003");
                GameManager.Instance.SetVariable("Tutorial_Now", 3);
                break;

            case "Tutorial_Result_008":
                DialogueManager.Instance.StartDialogue("Tutorial_004");
                // Tutorial_Now를 4로 바꿔서 튜토리얼 끝나게 함.
                GameManager.Instance.SetVariable("Tutorial_Now", 4);
                // TutorialLogic.setActcie를 false로 바꿔서 튜토리얼 관련으로 동작 안 일어나게 함.
                RoomManager.Instance.SetTutorialLogic(false);

                // 튜토리얼 거의 끝나가면 의자랑 카펫 버튼 맨 위로 올렸던거 때문에 
                // BlockingPanel1 맨앞으로 다시 올림
                RoomManager.Instance.Tutorial2_ChairAndCarpetInteractable(false);
                // 다른 이벤트오브젝트 버튼들 안 눌리게 했던 투명 패널 끔.
                RoomManager.Instance.SetBlockingPanel(false);
                // 이동버튼 누를 수 있게 함.
                RoomManager.Instance.Tutorial_SetMoveButtonInteractable(true);
                break;

            case "Tutorial_Result_009":
                GameManager.Instance.SetVariable("Tutorial_RoomSide2_Look", true);
                break;

            case "Tutorial_Result_010":
                GameManager.Instance.SetVariable("Tutorial_RoomSide3_Look", true);
                break;


            case "HideMoveButton":
                RoomManager.Instance.SetMoveButton(false);
                break;

            case "HideMemoButton":
                RoomManager.Instance.SetMemoButton(false);
                break;

            case "ShowMemoButton":
                RoomManager.Instance.SetMemoButton(true);
                break;



            case "Result_005": // 곰인형에 대한 설명
                DialogueManager.Instance.StartDialogue("RoomEscape_001");
                break;

            case "Result_006": // 이불에 대한 설명
                DialogueManager.Instance.StartDialogue("RoomEscape_002");
                break;
            
            case "Result_007": // 술과 감기약에 대한 설명
                RoomManager.Instance.SetEventObjectPanel(true, "Liquor");
                DialogueManager.Instance.StartDialogue("RoomEscape_003");
                break;
            
            case "Result_008": // 책상 위 스탠드에 대한 설명
                DialogueManager.Instance.StartDialogue("RoomEscape_004");
                break;
            
            case "Result_009": // 인형 수납장에 대한 설명
                DialogueManager.Instance.StartDialogue("RoomEscape_005");
                break;
            
            case "Result_010": // 베개에 대한 설명
                DialogueManager.Instance.StartDialogue("RoomEscape_006");
                break;
            
            case "Result_011": // 베개 안에 부적을 발견
                executableObjects["Pillow"].ExecuteAction();
                DialogueManager.Instance.StartDialogue("RoomEscape_007");
                //RoomManager.Instance.SetExitButton(true);
                break;
            
            case "Result_012": // 부적에 대한 메모 작성 - ### 추후 구현 필요 ###
                break;
            
            case "Result_013": // 다이어리에 대한 설명
                DialogueManager.Instance.StartDialogue("RoomEscape_009");
                break;
            
            case "Result_014": // 다이어리 잠금 장치 활성화됨
                executableObjects["Diary"].ExecuteAction();
                RoomManager.Instance.SetExitButton(true);
                break;
            
            case "Result_015": // 다이어리가 열림
                executableObjects["DiaryLock"].ExecuteAction();
                break;
            
            case "Result_016": // 다이어리에 대한 메모가 작성됨 - ### 추후 구현 필요 ###
                break;
            
            case "Result_017": // 다이어리에 대한 스크립트
                DialogueManager.Instance.StartDialogue("RoomEscape_010");
                break;
            
            case "Result_018": // 다이어리 비밀번호를 틀렸다는 스크립트
                DialogueManager.Instance.StartDialogue("RoomEscape_011");
                break;
            
            case "Result_019": // 시계에 대한 설명
                //RoomManager.Instance.SetEventObjectPanel(true, "clock1");
                DialogueManager.Instance.StartDialogue("RoomEscape_012");
                break;
            
            case "Result_020": // 시계 시스템 활성화
                executableObjects["Clock"].ExecuteAction();
                RoomManager.Instance.SetExitButton(true);
                break;

            case "Result_021": // 열쇠를 획득
                executableObjects["ClockPuzzle"].ExecuteAction();
                break;

            case "Result_022": // 열쇠를 획득 후 스크립트
                DialogueManager.Instance.StartDialogue("RoomEscape_013");
                break;

            case "Result_023": // 열쇠 획득 후 메모 - ### 추후 구현 필요 ###
                break;

            case "Result_024": // 의자가 옆으로 이동함
                executableObjects["Chair1"].ExecuteAction();
                executableObjects["Chair2"].ExecuteAction();
                break;

            case "Result_025": // 노트북에 대한 설명
                DialogueManager.Instance.StartDialogue("RoomEscape_014");
                break;

            case "Result_026": // 노트북 비밀번호 화면 활성화
                executableObjects["Laptop"].ExecuteAction();
                RoomManager.Instance.SetExitButton(true);
                break;

            case "Result_027": // 노트북이 열림
                executableObjects["LaptopLock"].ExecuteAction();
                break;

            case "Result_028": // 노트북에 대한 설명이 메모에 기록됨 - ### 추후 구현 필요 ###
                break;

            case "Result_029": // 노트북에 대한 스크립트 출력
                DialogueManager.Instance.StartDialogue("RoomEscape_015");
                break;

            case "Result_030": // 노트북 비밀번호 틀림에 대한 설명
                DialogueManager.Instance.StartDialogue("RoomEscape_016");
                break;
            
            case "Result_031": // 카펫이 들쳐짐
                executableObjects["Carpet_Closed"].ExecuteAction();
                GameManager.Instance.SetVariable("CarpetClosed", false);
                break;
            
            case "Result_032": // 종이를 확대해주는 UI
                executableObjects["Carpet_Paper"].ExecuteAction();
                RoomManager.Instance.SetExitButton(true);
                break;
            
            case "Result_033": // 종이에 대한 스크립트
                DialogueManager.Instance.StartDialogue("RoomEscape_017");
                break;
            
            case "Result_034": // 카펫이 원래대로 돌아감
                executableObjects["Carpet_Open"].ExecuteAction();
                GameManager.Instance.SetVariable("CarpetClosed", true);
                DialogueManager.Instance.StartDialogue("RoomEscape_018");
                break;
            
            case "Result_035": // 포스터에 대한 스크립트
                RoomManager.Instance.SetEventObjectPanel(true, "Poster");
                DialogueManager.Instance.StartDialogue("RoomEscape_019");
                break;

            case "Result_036": // 커터칼에 대한 스크립트
                RoomManager.Instance.SetEventObjectPanel(true, "Knife");
                DialogueManager.Instance.StartDialogue("RoomEscape_020");
                break;

            case "Result_037": // 커터칼 사라짐
                executableObjects["Knife1"].ExecuteAction();
                executableObjects["Knife2"].ExecuteAction();
                break;

            case "Result_038": // 포스터 뒷장에 대한 설명
                DialogueManager.Instance.StartDialogue("RoomEscape_021");
                GameManager.Instance.SetVariable("PosterOpened", true);
                break;

            case "Result_039": // 포스터에 대한 메모 - ### 추후 구현 필요 ###
                break;

            case "Result_040": // 옷장 문이 열림
                executableObjects["ClosedClosetDoors"].ExecuteAction();
                break;
            
            case "Result_041": // 옷장 문이 닫힘
                executableObjects["OpenClosetDoors"].ExecuteAction();
                break;
                
            case "Result_042": // 옷장 처음 상호작용했을 때 스크립트
                DialogueManager.Instance.StartDialogue("RoomEscape_022");
                break;

            case "Result_043": // 옷장 속 가방 클릭 스크립트
                DialogueManager.Instance.StartDialogue("RoomEscape_023");
                break;

            case "Result_044": // 옷장 속 가방 내에 전단지에 대한 메모 - ### 추후 구현 필요 ###
                break;

            case "Result_045": // 옷장 위 상자에 대한 설명(열쇠X 상태 일 때)
                DialogueManager.Instance.StartDialogue("RoomEscape_024");
                break;

            case "Result_046": // 상자가 열리는 스크립트
                DialogueManager.Instance.StartDialogue("RoomEscape_025");
                RoomManager.Instance.SetExitButton(true);
                break;

            case "Result_047": // 상자 열리는 사운드 - ### 추후 구현 필요 ###
                break;

            case "Result_048": // 상자 안 사진들이 UI로 보임
                executableObjects["Box"].ExecuteAction();
                break;

            case "Result_049": // 서랍장이 열리는 스크립트
                DialogueManager.Instance.StartDialogue("RoomEscape_026");
                break;

            case "Result_050": // 서랍장이 열림
                executableObjects["ClosedCabinetDoors"].ExecuteAction();
                break;

            case "Result_051": // 서랍장이 닫힘
                executableObjects["OpenCabinetDoors"].ExecuteAction();
                break;

            case "Result_052": // 달력이 열림
                executableObjects["Calendar"].ExecuteAction();
                RoomManager.Instance.SetExitButton(true);
                break;

            case "Result_053": // 필연 생일에 대한 스크립트 - ### 추후 구현 필요 ###
                DialogueManager.Instance.StartDialogue("RoomEscape_027");
                break;

            case "Result_054": // 우연 생일에 대한 스크립트 - ### 추후 구현 필요 ###
                DialogueManager.Instance.StartDialogue("RoomEscape_028");
                break;

            case "Result_055": // 10월 31일에 대한 스크립트
                DialogueManager.Instance.StartDialogue("RoomEscape_029");
                break;

            case "Result_056": // 10월 1일에 대한 스크립트
                DialogueManager.Instance.StartDialogue("RoomEscape_030");
                break;

            case "Result_057": // 필연 생일 메모 - ### 추후 구현 필요 ###
                break;

            case "Result_058": // 우연 생일 메모 - ### 추후 구현 필요 ###
                break;

            case "Result_059": // 10월 31일 메모 - ### 추후 구현 필요 ###
                break;

            case "Result_060": // 10월 1일 메모 - ### 추후 구현 필요 ###
                break;

            case "Result_061": // 옷장 확대 화면으로 전환
                executableObjects["Closet Unzoomed 2"].ExecuteAction();
                break;
            
            case "Result_062": // 서랍장 확대 화면으로 전환
                // executableObjects["Cabinet Unzoomed 2"].ExecuteAction();
                executableObjects["Cabinet Unzoomed 3"].ExecuteAction();
                break;

            case "Result_063": // 빌라에 대한 스크립트
                DialogueManager.Instance.StartDialogue("Follow_002");
                break;

            case "Result_064": // 빵집에 대한 스크립트
                DialogueManager.Instance.StartDialogue("Follow_003");
                break;

            case "Result_065": // 편의점에 대한 스크립트
                DialogueManager.Instance.StartDialogue("Follow_004");
                break;

            case "Result_066": // 바에 대한 스크립트
                DialogueManager.Instance.StartDialogue("Follow_005");
                break;

            case "Result_067": // 이자카야에 대한 스크립트
                DialogueManager.Instance.StartDialogue("Follow_006");
                break;

            case "Result_068": // 고양이에 대한 스크립트
                DialogueManager.Instance.StartDialogue("Follow_007");
                break;

            case "Result_069": // 카페에 대한 스크립트
                DialogueManager.Instance.StartDialogue("Follow_008");
                break;

            case "Result_070": // 카페에 대한 메모 1
                break;

            case "Result_071": // 카페에 대한 메모 2
                break;

            case "Result_072": // 영수증에 대한 스크립트
                DialogueManager.Instance.StartDialogue("Follow_009");
                break;

            case "Result_073": // 영수증에 대한 메모
                break;

            case "Result_074": // 신호등에 대한 스크립트
                DialogueManager.Instance.StartDialogue("Follow_010");
                break;

            case "Result_075": // 신호등에 대한 메모
                break;
        }
    }
}
