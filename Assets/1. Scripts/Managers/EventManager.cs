using System.Collections;
using System.Collections.Generic;
using Fate.Data;
using UnityEngine;
using GameEvent = Fate.Data.Event;

namespace Fate.Managers
{
    public class EventManager : MonoBehaviour
{
    // EventManager를 싱글턴으로 생성
    public static EventManager Instance { get; private set; }

    private TextAsset eventsCSV;

    // events: dictionary of "Event"s indexed by string "Event ID"
    public Dictionary<string, GameEvent> events = new Dictionary<string, GameEvent>();


    void Awake()
    {
        if (Instance == null)
        {
            eventsCSV = Resources.Load<TextAsset>("Datas/events");
            if (eventsCSV == null)
            {
                Debug.LogError("EventManager: Failed to load events CSV file from Resources/Datas/events");
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            // ParseConditions와 ParseResults가 먼저 실행되어야 함
            if (ConditionManager.Instance != null)
                ConditionManager.Instance.ParseConditions();
            else
                Debug.LogError("EventManager: ConditionManager.Instance is null, cannot parse conditions");

            if (ResultManager.Instance != null)
                ResultManager.Instance.ParseResults();
            else
                Debug.LogError("EventManager: ResultManager.Instance is null, cannot parse results");

            ParseEvents();

            // 디버깅용
            //DebugLogConditions();
            //DebugLogResults();
            //DebugLogEvents();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // events.csv 파일 파싱
    private void ParseEvents()
    {
        if (eventsCSV == null || string.IsNullOrEmpty(eventsCSV.text))
        {
            Debug.LogError("EventManager: Cannot parse events - CSV file is null or empty");
            return;
        }

        if (ConditionManager.Instance == null)
        {
            Debug.LogError("EventManager: ConditionManager.Instance is null, cannot parse events");
            return;
        }

        if (ResultManager.Instance == null)
        {
            Debug.LogError("EventManager: ResultManager.Instance is null, cannot parse events");
            return;
        }

        string[] lines = eventsCSV.text.Split('\n');

        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i]))
                continue;

            string[] fields = lines[i].Split(',');
            
            if (fields.Length < 6)
            {
                Debug.LogWarning($"EventManager: Invalid event line {i}: expected at least 6 fields, got {fields.Length}");
                continue;
            }

            if (fields[0] == "" && fields[1] == "")
                continue;

            string eventID = fields[0].Trim();
            if (string.IsNullOrEmpty(eventID))
            {
                Debug.LogWarning($"EventManager: Event ID is empty at line {i}");
                continue;
            }

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
                    string trimmedConditionID = conditionID.Trim();
                    if (string.IsNullOrEmpty(trimmedConditionID))
                        continue;

                    if (ConditionManager.Instance.conditions == null || !ConditionManager.Instance.conditions.ContainsKey(trimmedConditionID))
                    {
                        Debug.LogWarning($"EventManager: Condition ID '{trimmedConditionID}' not found at line {i}!");
                        continue;
                    }
                    conditions.Add(ConditionManager.Instance.conditions[trimmedConditionID]);
                }
            }

            if (string.IsNullOrWhiteSpace(fields[5].Trim()))
            {
                Debug.LogWarning($"EventManager: No results specified for event '{eventID}' at line {i}");
                continue;
            }

            string[] resultIDs = fields[5].Trim().Split('/');
            foreach (string resultID in resultIDs)
            {
                string resultIDTrimmed = resultID.Trim();
                if (string.IsNullOrEmpty(resultIDTrimmed))
                    continue;

                // Function-wrapped results
                if (resultIDTrimmed.StartsWith("Result_RevealMemo") ||
                    resultIDTrimmed.StartsWith("Result_StartDialogue") ||
                    resultIDTrimmed.StartsWith("Result_Increment") ||
                    resultIDTrimmed.StartsWith("Result_Decrement") ||
                    resultIDTrimmed.StartsWith("Result_Inverse") ||
                    resultIDTrimmed.StartsWith("Result_IsFinished") ||
                    resultIDTrimmed.StartsWith("Result_IsUnFinished"))
                {
                    Result tempResult = new Result(resultIDTrimmed, "", "");
                    results.Add(tempResult);
                }
                else
                {
                    if (ResultManager.Instance.Results == null || !ResultManager.Instance.Results.ContainsKey(resultIDTrimmed))
                    {
                        Debug.LogWarning($"EventManager: Result ID '{resultIDTrimmed}' not found at line {i}!");
                        continue;
                    }
                    results.Add(ResultManager.Instance.Results[resultIDTrimmed]);
                }
            }

            if (results.Count == 0)
            {
                Debug.LogWarning($"EventManager: No valid results found for event '{eventID}' at line {i}");
                continue;
            }

            if (events.ContainsKey(eventID)) // 이미 존재하는 event ID인 경우: EventLine을 추가
            {
                events[eventID].AddEventLine(eventLogic, conditions, results);
            }
            else // 새로운 event ID인 경우: events에 새로 추가
            {
                // 예약어 event를 피하기 위해 event_라고 이름 지음
                GameEvent event_ = new GameEvent(
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
        if (string.IsNullOrEmpty(eventID))
        {
            Debug.LogWarning("EventManager: CallEvent called with null or empty eventID");
            return;
        }

        if (!events.ContainsKey(eventID))
        {
            Debug.LogWarning($"EventManager: Event '{eventID}' does not exist in events dictionary");
            return;
        }

        if (GameManager.Instance != null && GameManager.Instance.isDebug)
            Debug.Log($"-#-#-#-#-#-#-#-#- event: \"{eventID}\" -#-#-#-#-#-#-#-#-");

        GameEvent gameEvent = events[eventID];
        if (gameEvent == null || gameEvent.EventLine == null)
        {
            Debug.LogWarning($"EventManager: Event '{eventID}' has null EventLine");
            return;
        }

        List<EventLine> eventLines = gameEvent.EventLine;

        int eventCount = 0;
        
        foreach (EventLine eventLine in eventLines)
        {
            if (eventLine == null)
            {
                Debug.LogWarning($"EventManager: Null EventLine found in event '{eventID}' at index {eventCount}");
                continue;
            }

            eventCount++;
            if (GameManager.Instance != null && GameManager.Instance.isDebug)
                Debug.Log($"--------- #{eventCount} ---------");
            
            string logic = eventLine.Logic;
            List<Condition> conditions = eventLine.Conditions;
            List<Result> results = eventLine.Results;

            if (conditions == null || conditions.Count == 0)
            { // 조건이 존재하지 않는 경우 무조건 실행
                ExecuteResults(results);
                continue;
            }

            if (logic == "AND") // logic이 AND인 경우
            {
                if (CheckConditions_AND(conditions))
                {
                    ExecuteResults(results);
                    return;
                }
            }
            else if (logic == "OR") // logic이 OR인 경우
            {
                if (CheckConditions_OR(conditions))
                {
                    ExecuteResults(results); 
                    return;
                }
            }
            else // logic이 빈칸인 경우
            {
                if (conditions.Count > 0 && conditions[0] != null)
                {
                    string conditionID = conditions[0].ConditionID;
                    if (ConditionManager.Instance != null)
                    {
                        bool isCondition = ConditionManager.Instance.IsCondition(conditionID);
                        //Debug.Log(conditionID+" : "+isCondition);
                        if (isCondition)
                        {
                            ExecuteResults(results);
                            // 밑에 return 안 넣어주면 계속 다음에 있는 Conditions에 맞는 Results까지 불러오게 됨
                            return;
                        }
                    }
                    else
                    {
                        Debug.LogWarning("EventManager: ConditionManager.Instance is null, cannot check condition");
                    }
                }
            }
        }
    }

    private bool CheckConditions_OR(List<Condition> conditions)
    {
        if (conditions == null || conditions.Count == 0)
            return false;

        if (ConditionManager.Instance == null)
        {
            Debug.LogWarning("EventManager: ConditionManager.Instance is null in CheckConditions_OR");
            return false;
        }

        foreach (Condition condition in conditions)
        {
            if (condition == null)
                continue;

            string conditionID = condition.ConditionID;
            if (string.IsNullOrEmpty(conditionID))
                continue;

            bool isCondition = ConditionManager.Instance.IsCondition(conditionID);
            if (isCondition)
            {
                return true;
            }
        }

        return false;
    }

    private bool CheckConditions_AND(List<Condition> conditions)
    {
        if (conditions == null || conditions.Count == 0)
            return true;

        if (ConditionManager.Instance == null)
        {
            Debug.LogWarning("EventManager: ConditionManager.Instance is null in CheckConditions_AND");
            return false;
        }

        foreach (Condition condition in conditions)
        {
            if (condition == null)
                continue;

            string conditionID = condition.ConditionID;
            if (string.IsNullOrEmpty(conditionID))
                continue;

            bool isCondition = ConditionManager.Instance.IsCondition(conditionID);
            if (!isCondition)
            {
                return false;
            }
        }

        return true;
    }

    private void ExecuteResults(List<Result> results)
    {
        if (results == null || results.Count == 0)
            return;

        if (ResultManager.Instance == null)
        {
            Debug.LogError("EventManager: ResultManager.Instance is null, cannot execute results");
            return;
        }

        foreach (Result result in results)
        {
            if (result == null)
            {
                Debug.LogWarning("EventManager: Null result found in ExecuteResults");
                continue;
            }

            string resultID = result.ResultID;
            if (string.IsNullOrEmpty(resultID))
            {
                Debug.LogWarning("EventManager: Result has null or empty ResultID");
                continue;
            }

            ResultManager.Instance.ExecuteResult(resultID);
        }
    }

    // ############### 디버깅용 메서드들 #################
    //Condition 정보를 로그로 출력하는 메서드
    //private void DebugLogConditions()
    //{
    //    Debug.Log("##### conditions #####");
    //    foreach (var item in ConditionManager.Instance.conditions)
    //    {
    //        Debug.Log($"Condition ID: {item.Key}, Description: {item.Value.Description}, Variable: {item.Value.VariableName}, Logic: {item.Value.Logic}, Value: {item.Value.Value}");
    //    }
    //    Debug.Log('\n');
    //}

    //Result 정보를 로그로 출력하는 메서드
    //private void DebugLogResults()
    //{
    //    Debug.Log("##### results #####");
    //    foreach (var item in ResultManager.Instance.results)
    //    {
    //        Debug.Log($"Result ID: {item.Key}, Description: {item.Value.Description}, Action: {item.Value.Action}");
    //    }
    //    Debug.Log('\n');
    //}

    //Event 정보를 로그로 출력하는 메서드
    private void DebugLogEvents()
    {
        Debug.Log("##### events #####");
        foreach (var evt in events)
        {
            Debug.Log($"Event ID: {evt.Value.EventID}, Name: {evt.Value.EventName}, Description: {evt.Value.EventDescription}");
            foreach (var lcr in evt.Value.EventLine)
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
}