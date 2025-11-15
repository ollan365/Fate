using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.PlayerLoop;
using static Constants;

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
    public bool skipTutorial;
    public bool skipInquiry;
    public bool isDebug;
    public bool isReleaseBuild;
    public bool isDemoBuild;

    // 조사 시스템에서 현재 조사하고 있는 오브젝트의 evnetId를 저장함
    private string currentInquiryObjectId = "";

    // 중복조사 관련 딕셔너리
    public Dictionary<string, bool> eventObjectsStatusDict = new Dictionary<string, bool>();

    public void SetCurrentInquiryObjectId(string objectId) {
        currentInquiryObjectId = objectId;
    }

    public string GetCurrentInquiryObjectId() {
        if (currentInquiryObjectId == null) {
            Debug.Log("currentInquiryObjectId is NULL!");
            return null;
        }

        return currentInquiryObjectId;
    }

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else
            Destroy(gameObject);
    }
    
    private void Start() {
        variablesCSV = Resources.Load<TextAsset>("Datas/variables");
        CreateVariables();
        
        if (isDebug) {
            ShowVariables();
            Time.timeScale = 4f;
        }

        if (isDemoBuild)
            SaveManager.Instance.CreateNewGameData();
    }
    
    private void Update() {
        if (isDebug)
            ShowVariables();

        if (isDemoBuild || isReleaseBuild) {
            if (GameSceneManager.Instance.GetActiveScene() == Constants.SceneType.ROOM_2 &&
                UIManager.Instance.GetUI(eUIGameObjectName.EndOfDemoPage).activeInHierarchy == false)
                UIManager.Instance.SetUI(eUIGameObjectName.EndOfDemoPage, true);
        }
    }

    private void CreateVariables() {
        string[] variableLines = variablesCSV.text.Split('\n');
        
        for (int i = 1; i < variableLines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(variableLines[i])) 
                continue;

            string[] fields = variableLines[i].Split(',');

            string variableName = fields[0].Trim();
            string variableValue = fields[1].Trim();
            string variableType = fields[2].Trim();

            switch (variableType) {
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

        SaveManager.Instance.SaveInitGameData();
    }

    public void ResetVariables()
    {
        string[] variableLines = variablesCSV.text.Split('\n');

        for (int i = 1; i < variableLines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(variableLines[i]))
                continue;

            string[] fields = variableLines[i].Split(',');

            string variableName = fields[0].Trim();
            string variableValue = fields[1].Trim();
            string variableType = fields[2].Trim();
            string variableReset = fields[3].Trim();

            // 엔딩 때 초기화하지 않을 변수들은 제외
            if (variableReset == "FALSE") continue;

            switch (variableType)
            {
                case "int":
                    variables[variableName] = int.Parse(variableValue);
                    break;
                case "bool":
                    variables[variableName] = bool.Parse(variableValue);
                    break;
                case "string":
                    variables[variableName] = variableValue;
                    break;
                default:
                    Debug.Log("Unknown variable type: " + variableType);
                    break;
            }
        }
    }

    public void SetVariable(string variableName, object value) {
        if (variables.ContainsKey(variableName))
            variables[variableName] = value;
        else
            Debug.Log($"variable \"{variableName}\" does not exist!");
    }

    public object GetVariable(string variableName) {
        if (variables.ContainsKey(variableName))
            return variables[variableName];

        Debug.Log($"variable: \"{variableName}\" does not exist!");
        return null;
    }

    public void IncrementVariable(string variableName) {
        SetVariable(variableName, (int)GetVariable(variableName) + 1);
    }

    public void IncrementVariable(string variableName, int count) {
        SetVariable(variableName, (int)GetVariable(variableName) + count);
    }

    public void DecrementVariable(string variableName) {
        SetVariable(variableName, (int)GetVariable(variableName) - 1);
    }

    public void DecrementVariable(string variableName, int count) {
        SetVariable(variableName, (int)GetVariable(variableName) - count);
    }

    public void InverseVariable(string variableName) {
        SetVariable(variableName, !(bool)GetVariable(variableName));
    }

    private void ShowVariables() {
        variablesText.text = "";  // 텍스트 초기화

        // 화면에 표시하고 싶은 변수명 추가
        List<string> keysToShow = new List<string>(new string[] {
            "Language",
            "FateName",
            "NowDayNum",
            "ActionPoint"
        });

        foreach (var item in variables)
            if (keysToShow.Contains(item.Key)) variablesText.text += $"{item.Key}: {item.Value}\n";
        
        variablesText.gameObject.SetActive(true);
    }

    public bool GetIsBusy() { // 클릭을 막아야 하는 상황들
        return DialogueManager.Instance.isDialogueActive ||
               (RoomManager.Instance && RoomManager.Instance.isInvestigating) ||
               MemoManager.Instance.isMemoOpen;
    }

    // 원래 EventObjectManager 기능들 GameManager에 옮김

    public void AddEventObject(EventObject eventObject) {
        if (!eventObjectsStatusDict.ContainsKey(eventObject.GetEventId()))
            eventObjectsStatusDict.Add(eventObject.GetEventId(), false);
    }

    public void AddEventObject(string eventObjectId) {
        if (!eventObjectsStatusDict.ContainsKey(eventObjectId))
            eventObjectsStatusDict.Add(eventObjectId, false);
    }

    public void SetEventFinished(string eventId) {
        if (eventObjectsStatusDict.ContainsKey(eventId))
            eventObjectsStatusDict[eventId] = true;
        else
            Debug.Log(eventId+ " is not existed!");
    }

    public void SetEventUnFinished(string eventId) {
        if (eventObjectsStatusDict.ContainsKey(eventId))
            eventObjectsStatusDict[eventId] = false;
    }

    public bool GetEventStatus(string eventId) {
        return eventObjectsStatusDict.ContainsKey(eventId) && eventObjectsStatusDict[eventId];
    }
    
    public void QuitGame() {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
        }

    // 치트: 날짜를 5일로, 행동력을 0으로 설정하여 씬을 즉시 종료
    public void CheatEndSceneImmediately(int todayDayNum, int actionPoint, int presentHeartIndex) {
        SetVariable("NowDayNum", todayDayNum);
        SetVariable("ActionPoint", actionPoint);
        SetVariable("PresentHeartIndex", presentHeartIndex);
        
        if (GameSceneManager.Instance != null) {
            var currentScene = GameSceneManager.Instance.GetActiveScene();
            if (currentScene == SceneType.ROOM_1 || currentScene == SceneType.ROOM_2) {
                if (RoomManager.Instance != null && RoomManager.Instance.actionPointManager != null)
                    RoomManager.Instance.actionPointManager.RefillHeartsOrEndDay();
                else
                    GameSceneManager.Instance.LoadScene(SceneType.ENDING);
            }
        }
        
        Debug.Log("Cheat: Set day to 5, action points to 0");
    }
}