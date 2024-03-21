using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultManager : MonoBehaviour
{
    public static ResultManager Instance { get; private set; }

    public TextAsset resultsCSV;
    
    // results: dictionary of "Results"s indexed by string "Result ID"
    public Dictionary<string, Result> results = new Dictionary<string, Result>();
    
    // ----------------- 스펠링 실수로 인한 에러를 피하기 위해 스트링 변수들을 선언 -----------------
    // System variables
    private string ActionPoint = "ActionPoint";
    
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
            case "Result_001": // 눈 깜박거리는 효과 - ### 추후 구현 필요 ###
                break;
            
            case "Result_002": // 설명창 띄우기 - ### 추후 구현 필요 ###
                break;
            
            case "Result_003": // 행동력 감소
                GameManager.Instance.DecrementVariable(ActionPoint);
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
                executableObjects["Chair"].ExecuteAction();
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
            
            case "Result_031": // 카펫이 들쳐짐- ### 추후 구현 필요 ###
                break;
            
            case "Result_032": // 종이를 확대해주는 UI- ### 추후 구현 필요 ###
                break;
            
            case "Result_033": // 종이에 대한 스크립트
                DialogueManager.Instance.StartDialogue("RoomEscape_017");
                break;
            
            case "Result_034": // 카펫이 원래대로 돌아감
                DialogueManager.Instance.StartDialogue("RoomEscape_018");
                break;
            
        }
    }
}
