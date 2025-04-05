using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class GameManager : MonoBehaviour
{
    // GameManager를 싱글턴으로 생성
    public static GameManager Instance { get; private set; }

    private TextAsset variablesCSV;
    
    // 이벤트의 실행 조건을 확인하기 위한 변수를 모두 이곳에서 관리
    // 변수의 타입은 int 또는 bool
    private Dictionary<string, object> variables = new Dictionary<string, object>();
    public Dictionary<string, object> Variables // 데이터 저장을 위해 작성
    {
        get => variables;
        set => variables = value;
    }

    // 디버깅용
    [SerializeField] private TextMeshProUGUI variablesText;
    public bool isDebug = false;
    public bool skipTutorial = false;
    public bool skipInquiry = false;
    public int accidyGender = 0;  // 우연 성별 {0: 여자, 1: 남자}

    // 조사 시스템에서 현재 조사하고 있는 오브젝트의 evnetId를 저장함
    private string currentInquiryObjectId = "";

    // 중복조사 관련 딕셔너리
    public Dictionary<string, bool> eventObjectsStatusDict = new Dictionary<string, bool>();

    public void SetCurrentInquiryObjectId(string objectId)
    {
        currentInquiryObjectId = objectId;
    }

    public string GetCurrentInquiryObjectId()
    {
        if (currentInquiryObjectId == null)
        {
            Debug.Log("currentInquiryObjectId is NULL!");
            return null;
        }
        else return currentInquiryObjectId;
    }

    private void Awake()
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
    
    private void Start()
    {
        variablesCSV = Resources.Load<TextAsset>("Datas/variables");
        CreateVariables();

        if (isDebug)
            ShowVariables();
    }

    private void CreateVariables()
    {
        string[] variableLines = variablesCSV.text.Split('\n');
        
        for (int i = 1; i < variableLines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(variableLines[i])) continue;

            string[] fields = variableLines[i].Split(',');

            string variableName = fields[0].Trim();
            string variableValue = fields[1].Trim();
            string variableType = fields[2].Trim();

            switch (variableType)
            {
                case "int":
                    variables.Add(variableName, int.Parse(variableValue));
                    break;
                case "bool":
                    variables.Add(variableName, bool.Parse(variableValue));
                    break;
                case "string":
                    variables.Add(variableName, variableValue);
                    break;
                default:
                    Debug.Log("Unknown variable type: " + variableType);
                    break;
            } 
        }
    }

    public void SetVariable(string variableName, object value)
    {
        if (variables.ContainsKey(variableName))
        {
            variables[variableName] = value;
        }
        else
        {
            Debug.Log($"variable \"{variableName}\" does not exist!");
        }
    }

    public object GetVariable(string variableName)
    {
        if (variables.ContainsKey(variableName))
        {
            return variables[variableName];
        }
        else
        {
            Debug.Log($"variable: \"{variableName}\" does not exist!");
            return null;
        }
    }

    public void IncrementVariable(string variableName)
    {
        int cnt = (int)GetVariable(variableName);
        cnt++;
        SetVariable(variableName, cnt);
    }

    public void IncrementVariable(string variableName, int count)
    {
        int cnt = (int)GetVariable(variableName);
        cnt += count;
        SetVariable(variableName, cnt);
    }

    public void DecrementVariable(string variableName)
    {
        int cnt = (int)GetVariable(variableName);
        cnt--;
        SetVariable(variableName, cnt);
    }

    public void DecrementVariable(string variableName, int count)
    {
        int cnt = (int)GetVariable(variableName);
        cnt -= count;
        SetVariable(variableName, cnt);
    }

    public void InverseVariable(string variableName)
    {
        bool variableValue = (bool)GetVariable(variableName);
        variableValue = !variableValue;
        SetVariable(variableName, variableValue);
    }

    // 디버깅 용
    private void Update()
    {
        if (isDebug) ShowVariables();
    }

    private void ShowVariables()
    {
        variablesText.text = "";  // 텍스트 초기화

        // 화면에 표시하고 싶은 변수명 추가
        List<string> keysToShow = new List<string>(new string[]
        {
            "CurrentScene",
        });

        foreach (var item in variables)
        {
            if (keysToShow.Contains(item.Key)) variablesText.text += $"{item.Key}: {item.Value}\n";
        }
    }

    public bool GetIsBusy()  // 클릭을 막아야 하는 상황들
    {
        bool isDialogueActive = DialogueManager.Instance.isDialogueActive;
        bool isInvestigating = RoomManager.Instance && RoomManager.Instance.isInvestigating;
        bool isTutorialPhase1 = (int)variables["TutorialPhase"] == 1;
        bool isMemoOpen = MemoManager.Instance.isMemoOpen;

        bool isBusy = isDialogueActive || isInvestigating || isTutorialPhase1 || isMemoOpen;

        return isBusy;
    }

    // 원래 EventObjectManager의 별로 없었던 기능들 GameManager에 옮김

    public void AddEventObject(EventObject eventObject)
    {
        if (!eventObjectsStatusDict.ContainsKey(eventObject.GetEventId()))
        {
            eventObjectsStatusDict.Add(eventObject.GetEventId(), false);
        }
    }

    public void AddEventObject(string eventObjectId)
    {
        if (!eventObjectsStatusDict.ContainsKey(eventObjectId))
        {
            eventObjectsStatusDict.Add(eventObjectId, false);
        }
    }

    public void SetEventFinished(string eventId)
    {
        if (eventObjectsStatusDict.ContainsKey(eventId))
        {
            eventObjectsStatusDict[eventId] = true;
        }
        else
        {
            Debug.Log(eventId+ " is not existed!");
        }
    }

    public void SetEventUnFinished(string eventId)
    {
        if (eventObjectsStatusDict.ContainsKey(eventId))
        {
            eventObjectsStatusDict[eventId] = false;
        }
    }

    public bool GetEventStatus(string eventId)
    {
        return eventObjectsStatusDict.ContainsKey(eventId) && eventObjectsStatusDict[eventId];
    }
}