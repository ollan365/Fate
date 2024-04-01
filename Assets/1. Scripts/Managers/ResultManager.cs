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
            if (string.IsNullOrWhiteSpace(lines[i])) continue;

            string[] fields = lines[i].Split(',');
            
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
            
            case "Result_005": // 곰인형에 대한 설명
                DialogueManager.Instance.StartDialogue("RoomEscape_001");
                break;

            case "Result_006": // 이불에 대한 설명
                DialogueManager.Instance.StartDialogue("RoomEscape_002");
                break;
            
            case "Result_007": // 술과 감기약에 대한 설명
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
                break;
            
            case "Result_012": // 부적에 대한 메모 작성 - ### 추후 구현 필요 ###
                break;
            
            case "Result_013": // 다이어리에 대한 설명
                DialogueManager.Instance.StartDialogue("RoomEscape_009");
                break;
            
            case "Result_014": // 다이어리 잠금 장치 활성화됨
                executableObjects["Diary"].ExecuteAction();
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
                DialogueManager.Instance.StartDialogue("RoomEscape_012");
                break;
            
            case "Result_020": // 시계 시스템 활성화
                executableObjects["Clock"].ExecuteAction();
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
                executableObjects["Carpet"].ExecuteAction();
                break;
            
            case "Result_032": // 종이를 확대해주는 UI
                executableObjects["Carpet_Paper"].ExecuteAction();
                break;
            
            case "Result_033": // 종이에 대한 스크립트
                DialogueManager.Instance.StartDialogue("RoomEscape_017");
                break;
            
            case "Result_034": // 카펫이 원래대로 돌아감
                executableObjects["Carpet"].ExecuteAction();
                DialogueManager.Instance.StartDialogue("RoomEscape_018");
                break;
            case "Result_035": // 포스터에 대한 스크립트
                DialogueManager.Instance.StartDialogue("RoomEscape_019");
                break;

            case "Result_036": // 커터칼에 대한 스크립트
                DialogueManager.Instance.StartDialogue("RoomEscape_020");
                break;

            case "Result_037": // 커터칼 사라짐
                executableObjects["Knife"].ExecuteAction();
                break;

            case "Result_038": // 포스터 뒷장에 대한 설명
                Debug.Log("포스터 뒷장에 대한 설명");
                executableObjects["Poster"].ExecuteAction();
                Debug.Log("포스터 뒷장에 대한 설명 스크립트 시작");
                // 이 부분 대사 스크립트 계속 오류남!!
                DialogueManager.Instance.StartDialogue("RoomEscape_021");
                break;

            case "Result_039": // 포스터에 대한 메모 - ### 추후 구현 필요 ###
                break;

            case "Result_040": // 옷장 문이 열림
                executableObjects["ClosetDoor_close"].ExecuteAction();
                break;
                
            case "Result_041": // 옷장 문이 닫힘
                executableObjects["ClosetDoor_L"].ExecuteAction();
                executableObjects["ClosetDoor_R"].ExecuteAction();
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
                Debug.Log("상자 열리는 스크립트");
                DialogueManager.Instance.StartDialogue("RoomEscape_025");
                break;

            case "Result_047": // 상자 열리는 사운드 - ### 추후 구현 필요 ###
                break;

            case "Result_048": // 상자 안 사진들이 UI로 보임
                Debug.Log("상자 안의 사진들이 보임");
                executableObjects["Box"].ExecuteAction();
                break;

            case "Result_049": // 서랍장이 열리는 스크립트
                DialogueManager.Instance.StartDialogue("RoomEscape_026");
                break;

            case "Result_050": // 서랍장이 열림
                executableObjects["CabinetDoor_Close"].ExecuteAction();
                break;

            case "Result_051": // 서랍장이 닫힘
                executableObjects["CabinetDoor_L"].ExecuteAction();
                executableObjects["CabinetDoor_R"].ExecuteAction();
                break;

            case "Result_052": // 달력이 열림
                executableObjects["Calendar"].ExecuteAction();
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
        }
    }
}
