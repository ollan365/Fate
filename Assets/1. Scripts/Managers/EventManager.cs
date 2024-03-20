using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    // EventManager를 싱글턴으로 생성
    public static EventManager Instance { get; private set; }
    
    public TextAsset eventsCSV;

    // events: dictionary of "Event"s indexed by string "Event ID"
    public Dictionary<string, Event> events = new Dictionary<string, Event>();

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
            
            if (!string.IsNullOrWhiteSpace(fields[4].Trim()))  // 조건이 존재할 때만 수행
            {
                string[] conditionIDs = fields[4].Trim().Split('/');
                foreach (string conditionID in conditionIDs)
                {
                    if (!conditionManager.conditions.ContainsKey(conditionID.Trim()))
                    {
                        Debug.Log($"Condition ID \"{conditionID.Trim()}\" not found!");
                        continue;
                    }
                    conditions.Add(conditionManager.conditions[conditionID.Trim()]);
                }
            }
            
            string[] resultIDs = fields[5].Trim().Split('/');
            foreach (string resultID in resultIDs)
            {
                results.Add(resultManager.results[resultID.Trim()]);
            }

            if (events.ContainsKey(eventID)) // 이미 존재하는 event ID인 경우: EventLine을 추가
            {
                events[eventID].AddEventLine(eventLogic, conditions, results);
            }
            else // 새로운 event ID인 경우: events에 새로 추가
            {
                // 예약어 event를 피하기 위해 event_라고 이름 지음
                Event event_ = new Event(
                    eventID,
                    eventName,
                    eventDescription
                );
                event_.AddEventLine(eventLogic, conditions, results);
                events[event_.EventID] = event_;
            }

        }
    }
    
    // Event ID를 받아서 전체 조건의 true/false 판단하여 true인 경우 결과 수행
    public void CallEvent(string eventID)
    {
        List<EventLine> eventLines = events[eventID].EventLine;
        foreach (EventLine eventLine in eventLines)
        {
            string logic = eventLine.Logic;
            List<Condition> conditions = eventLine.Conditions;
            List<Result> results = eventLine.Results;
            
            if (conditions.Count == 0) { // 조건이 존재하지 않는 경우 무조건 실행
                ExecuteResults(results);
                continue;
            }
            
            if (logic == "AND") // logic이 AND인 경우
            {
                if (CheckConditions_AND(conditions)) ExecuteResults(results);
            }
            else if (logic == "OR") // logic이 OR인 경우
            {
                if (CheckConditions_OR(conditions)) ExecuteResults(results);
            }
            else // logic이 빈칸인 경우
            {
                string conditionID = conditions[0].ConditionID;
                bool isCondition = conditionManager.IsCondition(conditionID);
                if (isCondition)
                {
                    ExecuteResults(results);
                }
            }
        }
    }

    private bool CheckConditions_OR(List<Condition> conditions)
    {
        foreach (Condition condition in conditions)
        {
            string conditionID = condition.ConditionID;
            bool isCondition = conditionManager.IsCondition(conditionID);
            if (isCondition)
            {
                return true;
            }
        }

        return false;
    }

    private bool CheckConditions_AND(List<Condition> conditions)
    {
        foreach (Condition condition in conditions)
        {
            string conditionID = condition.ConditionID;
            bool isCondition = conditionManager.IsCondition(conditionID);
            if (!isCondition)
            {
                return false;
            }
        }

        return true;
    }
    
    private void ExecuteResults(List<Result> results)
    {
        foreach (Result result in results)
        {
            string resultID = result.ResultID;
            Debug.Log("리절트id "+resultID);
            resultManager.ExecuteResult(resultID);
        }
    }

    // ############### 디버깅용 메서드들 #################
    // Condition 정보를 로그로 출력하는 메서드
    // private void DebugLogConditions()
    // {
    //     Debug.Log("##### conditions #####");
    //     foreach (var item in conditionManager.conditions)
    //     {
    //         Debug.Log($"Condition ID: {item.Key}, Description: {item.Value.Description}, Variable: {item.Value.VariableName}, Logic: {item.Value.Logic}, Value: {item.Value.Value}");
    //     }
    //     Debug.Log('\n');
    // }
    //
    // // Result 정보를 로그로 출력하는 메서드
    // private void DebugLogResults()
    // {
    //     Debug.Log("##### results #####");
    //     foreach (var item in resultManager.results)
    //     {
    //         Debug.Log($"Result ID: {item.Key}, Description: {item.Value.Description}, Action: {item.Value.Action}");
    //     }
    //     Debug.Log('\n');
    // }
    //
    // // Event 정보를 로그로 출력하는 메서드
    // private void DebugLogEvents()
    // {
    //     Debug.Log("##### events #####");
    //     foreach (var evt in events)
    //     {
    //         Debug.Log($"Event ID: {evt.Value.EventID}, Name: {evt.Value.EventName}, Description: {evt.Value.EventDescription}");
    //         foreach (var lcr in evt.Value.LogicConditionsResults)
    //         {
    //             Debug.Log($"    Logic: {lcr.Logic}");
    //             Debug.Log("    Conditions:");
    //             foreach (var condition in lcr.Conditions)
    //             {
    //                 Debug.Log($"        {condition.ConditionID}");
    //             }
    //             Debug.Log("    Results:");
    //             foreach (var result in lcr.Results)
    //             {
    //                 Debug.Log($"        {result.ResultID}");
    //             }
    //         }
    //     }
    // }
}
