using System;
using System.Collections;
using System.Collections.Generic;
using Fate.Events;
using Fate.Utilities;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.PlayerLoop;
using static Fate.Utilities.Constants;

namespace Fate.Managers
{
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
        public bool isPrologueInProgress;

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

            return currentInquiryObjectId;
        }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
                Destroy(gameObject);
        }

        private void Start()
        {
            variablesCSV = Resources.Load<TextAsset>("Datas/variables");
            if (variablesCSV == null)
            {
                Debug.LogError("GameManager: Failed to load variables CSV file from Resources/Datas/variables");
                return;
            }

            CreateVariables();

            isPrologueInProgress = false;

            if (isDebug)
            {
                ShowVariables();
                Time.timeScale = 4f;
            }

            if (isDemoBuild && SaveManager.Instance != null)
                SaveManager.Instance.CreateNewGameData();
        }

        private void Update()
        {
            if (isDebug)
                ShowVariables();

            if (isDemoBuild && GameSceneManager.Instance != null && UIManager.Instance != null)
            {
                var endOfDemoPage = UIManager.Instance.GetUI(eUIGameObjectName.EndOfDemoPage);
                if (endOfDemoPage != null && 
                    GameSceneManager.Instance.GetActiveScene() == Constants.SceneType.ROOM_2 &&
                    !endOfDemoPage.activeInHierarchy)
                {
                    UIManager.Instance.SetUI(eUIGameObjectName.EndOfDemoPage, true);
                }
            }
        }

        private void CreateVariables()
        {
            if (variablesCSV == null || string.IsNullOrEmpty(variablesCSV.text))
            {
                Debug.LogError("GameManager: Cannot create variables - CSV file is null or empty");
                return;
            }

            string[] variableLines = variablesCSV.text.Split('\n');

            for (int i = 1; i < variableLines.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(variableLines[i]))
                    continue;

                string[] fields = variableLines[i].Split(',');

                if (fields.Length < 3)
                {
                    Debug.LogWarning($"GameManager: Invalid variable line {i}: expected at least 3 fields, got {fields.Length}");
                    continue;
                }

                string variableName = fields[0].Trim();
                if (string.IsNullOrEmpty(variableName))
                {
                    Debug.LogWarning($"GameManager: Variable name is empty at line {i}");
                    continue;
                }

                string variableValue = fields[1].Trim();
                string variableType = fields[2].Trim();

                if (variables.ContainsKey(variableName))
                {
                    Debug.LogWarning($"GameManager: Duplicate variable name '{variableName}' at line {i}, skipping");
                    continue;
                }

                try
                {
                    switch (variableType)
                    {
                        case "int":
                            if (int.TryParse(variableValue, out int intValue))
                                variables.Add(variableName, intValue);
                            else
                                Debug.LogWarning($"GameManager: Failed to parse int value '{variableValue}' for variable '{variableName}' at line {i}");
                            break;
                        case "bool":
                            if (bool.TryParse(variableValue, out bool boolValue))
                                variables.Add(variableName, boolValue);
                            else
                                Debug.LogWarning($"GameManager: Failed to parse bool value '{variableValue}' for variable '{variableName}' at line {i}");
                            break;
                        case "string":
                            variables.Add(variableName, variableValue);
                            break;
                        default:
                            Debug.LogWarning($"GameManager: Unknown variable type '{variableType}' for variable '{variableName}' at line {i}");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError($"GameManager: Error parsing variable '{variableName}' at line {i}: {ex.Message}");
                }
            }

            if (SaveManager.Instance != null)
                SaveManager.Instance.SaveInitGameData();
            else
                Debug.LogWarning("GameManager: SaveManager.Instance is null, cannot save init game data");
        }

        public void ResetVariables()
        {
            if (variablesCSV == null || string.IsNullOrEmpty(variablesCSV.text))
            {
                Debug.LogError("GameManager: Cannot reset variables - CSV file is null or empty");
                return;
            }

            string[] variableLines = variablesCSV.text.Split('\n');

            for (int i = 1; i < variableLines.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(variableLines[i]))
                    continue;

                string[] fields = variableLines[i].Split(',');

                if (fields.Length < 4)
                {
                    Debug.LogWarning($"GameManager: Invalid reset variable line {i}: expected at least 4 fields, got {fields.Length}");
                    continue;
                }

                string variableName = fields[0].Trim();
                string variableValue = fields[1].Trim();
                string variableType = fields[2].Trim();
                string variableReset = fields[3].Trim();

                // 엔딩 때 초기화하지 않을 변수들은 제외
                if (variableReset == "FALSE") continue;

                if (!variables.ContainsKey(variableName))
                {
                    Debug.LogWarning($"GameManager: Cannot reset variable '{variableName}' - variable does not exist");
                    continue;
                }

                try
                {
                    switch (variableType)
                    {
                        case "int":
                            if (int.TryParse(variableValue, out int intValue))
                                variables[variableName] = intValue;
                            else
                                Debug.LogWarning($"GameManager: Failed to parse int value '{variableValue}' for variable '{variableName}' at line {i}");
                            break;
                        case "bool":
                            if (bool.TryParse(variableValue, out bool boolValue))
                                variables[variableName] = boolValue;
                            else
                                Debug.LogWarning($"GameManager: Failed to parse bool value '{variableValue}' for variable '{variableName}' at line {i}");
                            break;
                        case "string":
                            variables[variableName] = variableValue;
                            break;
                        default:
                            Debug.LogWarning($"GameManager: Unknown variable type '{variableType}' for variable '{variableName}' at line {i}");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError($"GameManager: Error resetting variable '{variableName}' at line {i}: {ex.Message}");
                }
            }
        }

        public void SetVariable(string variableName, object value)
        {
            if (string.IsNullOrEmpty(variableName))
            {
                Debug.LogWarning("GameManager: SetVariable called with null or empty variable name");
                return;
            }

            if (value == null)
            {
                Debug.LogWarning($"GameManager: SetVariable called with null value for variable '{variableName}'");
                return;
            }

            if (variables.ContainsKey(variableName))
                variables[variableName] = value;
            else
                Debug.LogWarning($"GameManager: Variable '{variableName}' does not exist!");
        }

        public object GetVariable(string variableName)
        {
            if (string.IsNullOrEmpty(variableName))
            {
                Debug.LogWarning("GameManager: GetVariable called with null or empty variable name");
                return null;
            }

            if (variables.TryGetValue(variableName, out object value))
                return value;

            Debug.LogWarning($"GameManager: Variable '{variableName}' does not exist!");
            return null;
        }

        public void IncrementVariable(string variableName)
        {
            IncrementVariable(variableName, 1);
        }

        public void IncrementVariable(string variableName, int count)
        {
            if (string.IsNullOrEmpty(variableName))
            {
                Debug.LogWarning("GameManager: IncrementVariable called with null or empty variable name");
                return;
            }

            object currentValue = GetVariable(variableName);
            if (currentValue == null)
            {
                Debug.LogWarning($"GameManager: Cannot increment variable '{variableName}' - variable does not exist");
                return;
            }

            if (currentValue is int intValue)
            {
                SetVariable(variableName, intValue + count);
            }
            else
            {
                Debug.LogError($"GameManager: Cannot increment variable '{variableName}' - variable is not of type int (current type: {currentValue.GetType().Name})");
            }
        }

        public void DecrementVariable(string variableName)
        {
            DecrementVariable(variableName, 1);
        }

        public void DecrementVariable(string variableName, int count)
        {
            if (string.IsNullOrEmpty(variableName))
            {
                Debug.LogWarning("GameManager: DecrementVariable called with null or empty variable name");
                return;
            }

            object currentValue = GetVariable(variableName);
            if (currentValue == null)
            {
                Debug.LogWarning($"GameManager: Cannot decrement variable '{variableName}' - variable does not exist");
                return;
            }

            if (currentValue is int intValue)
            {
                SetVariable(variableName, intValue - count);
            }
            else
            {
                Debug.LogError($"GameManager: Cannot decrement variable '{variableName}' - variable is not of type int (current type: {currentValue.GetType().Name})");
            }
        }

        public void InverseVariable(string variableName)
        {
            if (string.IsNullOrEmpty(variableName))
            {
                Debug.LogWarning("GameManager: InverseVariable called with null or empty variable name");
                return;
            }

            object currentValue = GetVariable(variableName);
            if (currentValue == null)
            {
                Debug.LogWarning($"GameManager: Cannot inverse variable '{variableName}' - variable does not exist");
                return;
            }

            if (currentValue is bool boolValue)
            {
                SetVariable(variableName, !boolValue);
            }
            else
            {
                Debug.LogError($"GameManager: Cannot inverse variable '{variableName}' - variable is not of type bool (current type: {currentValue.GetType().Name})");
            }
        }

        private void ShowVariables()
        {
            if (variablesText == null)
            {
                Debug.LogWarning("GameManager: Cannot show variables - variablesText is null");
                return;
            }

            variablesText.text = "";  // 텍스트 초기화

            // 화면에 표시하고 싶은 변수명 추가
            List<string> keysToShow = new List<string>(new string[]
            {
                "Language",
                "FateName",
                "NowDayNum",
                "ActionPoint"
            });

            foreach (var item in variables)
                if (keysToShow.Contains(item.Key)) variablesText.text += $"{item.Key}: {item.Value}\n";

            variablesText.gameObject.SetActive(true);
        }

        public bool GetIsBusy()
        { // 클릭을 막아야 하는 상황들
            if (DialogueManager.Instance != null && DialogueManager.Instance.isDialogueActive)
                return true;

            if (RoomManager.Instance != null && RoomManager.Instance.isInvestigating)
                return true;

            if (MemoManager.Instance != null && MemoManager.Instance.isMemoOpen)
                return true;

            if (UIManager.Instance != null)
            {
                var menuUI = UIManager.Instance.GetUI(eUIGameObjectName.MenuUI);
                if (menuUI != null && menuUI.activeInHierarchy)
                    return true;
            }

            return false;
        }

        // 원래 EventObjectManager 기능들 GameManager에 옮김

        public void AddEventObject(EventObject eventObject)
        {
            if (eventObject == null)
            {
                Debug.LogWarning("GameManager: AddEventObject called with null EventObject");
                return;
            }

            string eventId = eventObject.GetEventId();
            if (string.IsNullOrEmpty(eventId))
            {
                Debug.LogWarning("GameManager: AddEventObject called with EventObject that has null or empty eventId");
                return;
            }

            if (!eventObjectsStatusDict.ContainsKey(eventId))
                eventObjectsStatusDict.Add(eventId, false);
        }

        public void AddEventObject(string eventObjectId)
        {
            if (string.IsNullOrEmpty(eventObjectId))
            {
                Debug.LogWarning("GameManager: AddEventObject called with null or empty eventObjectId");
                return;
            }

            if (!eventObjectsStatusDict.ContainsKey(eventObjectId))
                eventObjectsStatusDict.Add(eventObjectId, false);
        }

        public void SetEventFinished(string eventId)
        {
            if (string.IsNullOrEmpty(eventId))
            {
                Debug.LogWarning("GameManager: SetEventFinished called with null or empty eventId");
                return;
            }

            if (eventObjectsStatusDict.ContainsKey(eventId))
                eventObjectsStatusDict[eventId] = true;
            else
                Debug.LogWarning($"GameManager: Event '{eventId}' does not exist in eventObjectsStatusDict");
        }

        public void SetEventUnFinished(string eventId)
        {
            if (string.IsNullOrEmpty(eventId))
            {
                Debug.LogWarning("GameManager: SetEventUnFinished called with null or empty eventId");
                return;
            }

            if (eventObjectsStatusDict.ContainsKey(eventId))
                eventObjectsStatusDict[eventId] = false;
            else
                Debug.LogWarning($"GameManager: Event '{eventId}' does not exist in eventObjectsStatusDict");
        }

        public bool GetEventStatus(string eventId)
        {
            if (string.IsNullOrEmpty(eventId))
            {
                Debug.LogWarning("GameManager: GetEventStatus called with null or empty eventId");
                return false;
            }

            return eventObjectsStatusDict.ContainsKey(eventId) && eventObjectsStatusDict[eventId];
        }

        public void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        // 치트: 날짜를 5일로, 행동력을 0으로 설정하여 씬을 즉시 종료
        public void CheatEndSceneImmediately(int todayDayNum, int actionPoint, int presentHeartIndex)
        {
            SetVariable("NowDayNum", todayDayNum);
            SetVariable("ActionPoint", actionPoint);
            SetVariable("PresentHeartIndex", presentHeartIndex);

            if (GameSceneManager.Instance != null)
            {
                var currentScene = GameSceneManager.Instance.GetActiveScene();
                if (currentScene == SceneType.ROOM_1 || currentScene == SceneType.ROOM_2)
                {
                    if (RoomManager.Instance != null && RoomManager.Instance.actionPointManager != null)
                        RoomManager.Instance.actionPointManager.RefillHeartsOrEndDay();
                    else
                        GameSceneManager.Instance.LoadScene(SceneType.ENDING);
                }
            }

            Debug.Log("Cheat: Set day to 5, action points to 0");
        }
    }
}
