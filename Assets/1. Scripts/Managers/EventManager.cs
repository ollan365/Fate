using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    // EventManager를 싱글턴으로 생성
    public static EventManager Instance { get; private set; }
    
    public TextAsset eventsCSV;

    // events: dictionary of "Event"s indexed by string "Event ID"
    private Dictionary<string, Event> events = new Dictionary<string, Event>();

    public ConditionManager conditionManager;
    public ResultManager resultManager;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // ParseConditions와 ParseResults가 먼저 실행되어야 함
            conditionManager.ParseConditions();
            resultManager.ParseResults();
            ParseEvents();
            
            // 디버깅용
            // DebugLogConditions();
            // DebugLogResults();
            // DebugLogEvents();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // events.csv 파일 파싱
    private void ParseEvents()
    {
        string[] lines = eventsCSV.text.Split('\n');

        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;

            string[] fields = lines[i].Split(',');
            
            string eventID = fields[0].Trim();
            string eventName = fields[1].Trim();
            string eventDescription = fields[2].Trim();
            string eventLogic = fields[3].Trim();

            // conditions와 results: '/' 기준으로 스플릿한 리스트
            List<Condition> conditions = new List<Condition>();
            List<Result> results = new List<Result>();
            
            string[] conditionIDs = fields[4].Trim().Split('/');
            foreach (string conditionID in conditionIDs)
            {
                conditions.Add(conditionManager.conditions[conditionID.Trim()]);
            }
            
            string[] resultIDs = fields[5].Trim().Split('/');
            foreach (string resultID in resultIDs)
            {
                results.Add(resultManager.results[resultID.Trim()]);
            }

            if (events.ContainsKey(eventID)) // 이미 존재하는 event ID인 경우: LogicConditionResult를 추가
            {
                events[eventID].AddLogicConditionResult(eventLogic, conditions, results);
            }
            else // 새로운 event ID인 경우: events에 새로 추가
            {
                // 예약어 event를 피하기 위해 event_라고 이름 지음
                Event event_ = new Event(
                    eventID,
                    eventName,
                    eventDescription
                );
                event_.AddLogicConditionResult(eventLogic, conditions, results);
                events[event_.EventID] = event_;
            }

        }
    }
    
    // Event ID를 받아서 전체 조건의 true/false 판단하여 true인 경우 결과 수행
    public bool callEvent(string eventID)
    {
        List<LogicConditionResult> logicConditionResults = events[eventID].LogicConditionsResults;
        foreach (LogicConditionResult logicConditionResult in logicConditionResults)
        {
            string logic = logicConditionResult.Logic;
            List<Condition> conditions = logicConditionResult.Conditions;
            List<Result> results = logicConditionResult.Results;
            if (logic == "") // logic이 빈칸인 경우
            {
                string conditionID = conditions[0].ConditionID;
                bool isCondition = conditionManager.IsCondition(conditionID);
                if (isCondition)
                {
                    executeResults(results);
                }
            } 
            else if (logic == "OR") // logic이 OR인 경우
            {
                foreach (Condition condition in conditions)
                {
                    string conditionID = condition.ConditionID;
                    bool isCondition = conditionManager.IsCondition(conditionID);
                    if (isCondition)
                    {
                        executeResults(results);
                        break;
                    }
                }
            }
            else // logic이 AND인 경우
            {
                bool isConditions = true; // 모든 조건을 확인하기 위한 flag
                foreach (Condition condition in conditions)
                {
                    string conditionID = condition.ConditionID;
                    bool isCondition = conditionManager.IsCondition(conditionID);
                    
                    // Debug.Log("ID: " + conditionID + ", | isCondition: " + isCondition);
                    
                    if (!isCondition)
                    {
                        isConditions = false;
                        break;
                    }
                }

                if (isConditions)
                {
                    executeResults(results);
                }
            }
        }
        return false;
    }

    private void executeResults(List<Result> results)
    {
        foreach (Result result in results)
        {
            string resultID = result.ResultID;
            resultManager.excuteResult(resultID);
        }
    }

    // ############### 디버깅용 메서드들 #################
    // Condition 정보를 로그로 출력하는 메서드
    private void DebugLogConditions()
    {
        Debug.Log("##### conditions #####");
        foreach (var item in conditionManager.conditions)
        {
            Debug.Log($"Condition ID: {item.Key}, Description: {item.Value.Description}, Variable: {item.Value.VariableName}, Logic: {item.Value.Logic}, Value: {item.Value.Value}");
        }
        Debug.Log('\n');
    }

    // Result 정보를 로그로 출력하는 메서드
    private void DebugLogResults()
    {
        Debug.Log("##### results #####");
        foreach (var item in resultManager.results)
        {
            Debug.Log($"Result ID: {item.Key}, Description: {item.Value.Description}, Action: {item.Value.Action}");
        }
        Debug.Log('\n');
    }

    // Event 정보를 로그로 출력하는 메서드
    private void DebugLogEvents()
    {
        Debug.Log("##### events #####");
        foreach (var evt in events)
        {
            Debug.Log($"Event ID: {evt.Value.EventID}, Name: {evt.Value.EventName}, Description: {evt.Value.EventDescription}");
            foreach (var lcr in evt.Value.LogicConditionsResults)
            {
                Debug.Log($"    Logic: {lcr.Logic}");
                Debug.Log("    Conditions:");
                foreach (var condition in lcr.Conditions)
                {
                    Debug.Log($"        {condition.ConditionID}");
                }
                Debug.Log("    Results:");
                foreach (var result in lcr.Results)
                {
                    Debug.Log($"        {result.ResultID}");
                }
            }
        }
    }
}
