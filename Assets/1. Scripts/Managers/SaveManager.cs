using Newtonsoft.Json;
using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using static Constants;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    // --- 게임 데이터 파일이름 설정 ("원하는 이름(영문).json") --- //
    private string SAVE_DATA_FILE_PATH = "/GameData.json";
    private SaveData InitData { set; get; }
    private List<string> endingVariableNames;

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

        SAVE_DATA_FILE_PATH = Application.persistentDataPath + SAVE_DATA_FILE_PATH;

        // 엔딩 후에도 초기화되면 안되는 변수들
        endingVariableNames = new List<string>
        {
            "EndingCollect", "LastEnding","BadEndingCollect", "SkipLobby",
            "BadACollect", "BadBCollect", "TrueCollect", "HiddenCollect",
            "FateName","Language","AccidyGender","FateBirthday"
        };
    }

    // 게임 데이터를 초기화 시킬 값 저장
    public void SaveInitGameData()
    {
        if (InitData != null) return;

        InitData = new SaveData(
            GameManager.Instance.Variables,
            GameManager.Instance.eventObjectsStatusDict,
            MemoManager.Instance.SavedMemoList,
            MemoManager.Instance.RevealedMemoList);
    }
    
    // 엔딩을 저장하고 게임 데이터 초기화 (엔딩 데이터는 초기화하지 않음)
    public void SaveEndingDataAndInitGameDataExceptEndingData(EndingType ending)
    {
        switch (ending)
        {
            case EndingType.BAD_A:
                GameManager.Instance.SetVariable("SkipLobby", true);
                GameManager.Instance.IncrementVariable("BadEndingCollect");
                GameManager.Instance.IncrementVariable("BadACollect");
                break;

            case EndingType.BAD_B:
                GameManager.Instance.SetVariable("SkipLobby", true);
                GameManager.Instance.IncrementVariable("BadEndingCollect");
                GameManager.Instance.IncrementVariable("BadBCollect");
                break;

            case EndingType.TRUE:
                GameManager.Instance.SetVariable("SkipLobby", true);
                GameManager.Instance.IncrementVariable("TrueCollect");
                break;

            case EndingType.HIDDEN:
                GameManager.Instance.SetVariable("SkipLobby", false);
                GameManager.Instance.IncrementVariable("HiddenCollect");
                break;
        }
        GameManager.Instance.IncrementVariable("EndingCollect");
        GameManager.Instance.SetVariable("LastEnding", ending.ToString());

        // 초기화용 데이터에서 엔딩 데이터(회차가 넘어가도 유지되어야하는 데이터)를 변경
        Dictionary<string, object> tmpDictionary = InitData.Variables;
        for (int i = 0; i < endingVariableNames.Count; i++)
        {
            string dataName = endingVariableNames[i];
            tmpDictionary[dataName] = GameManager.Instance.GetVariable(dataName);
        }

        // 초기화용 변수로 저장 후 로드
        SaveGameData(new SaveData(tmpDictionary, InitData.EventObjectStatusDictionary, InitData.SavedMemoList, InitData.RevealedMemoList));
        ApplySavedGameData();
    }

    // 게임을 새로 시작할 때, 기존 데이터가 있는지 확인하는 함수
    public bool CheckGameData()
    {
        string FromJsonData = File.ReadAllText(SAVE_DATA_FILE_PATH);
        SaveData saveData = JsonUtility.FromJson<SaveData>(FromJsonData);

        // n회차이거나 Room1의 튜토리얼을 끝냈으면 게임 데이터가 있는 것으로 판단
        return (int)saveData.Variables["EndingCollect"] != 0 || (bool)saveData.Variables["EndTutorial_ROOM_1"];
    }
    // 불러오기
    public void ApplySavedGameData()
    {
        if (!File.Exists(SAVE_DATA_FILE_PATH)) return; // no save game data

        // 저장된 파일 읽어오고 Json을 클래스 형식으로 전환해서 할당
        string FromJsonData = File.ReadAllText(SAVE_DATA_FILE_PATH);
        SaveData saveData = JsonUtility.FromJson<SaveData>(FromJsonData);

        // 저장된 내용 로드
        GameManager.Instance.Variables = saveData.Variables;
        GameManager.Instance.eventObjectsStatusDict = saveData.EventObjectStatusDictionary;
        MemoManager.Instance.SavedMemoList = saveData.SavedMemoList;
        MemoManager.Instance.RevealedMemoList = saveData.RevealedMemoList;
    }
    // 저장하기
    public void SaveGameData(SaveData newSaveData = null)
    {
        // 일반적인 저장의 경우, 현재 게임의 상태를 저장
        if (newSaveData == null)
            newSaveData = new SaveData(
                GameManager.Instance.Variables,
                GameManager.Instance.eventObjectsStatusDict,
                MemoManager.Instance.SavedMemoList,
                MemoManager.Instance.RevealedMemoList);
        
        // 클래스를 Json 형식으로 전환하여 저장
        File.WriteAllText(SAVE_DATA_FILE_PATH, JsonUtility.ToJson(newSaveData, true));
    }

    // 저장된 게임 데이터 삭제
    public void CreateNewGameData()
    {
        if (File.Exists(SAVE_DATA_FILE_PATH)) File.Delete(SAVE_DATA_FILE_PATH);
        File.WriteAllText(SAVE_DATA_FILE_PATH, JsonUtility.ToJson(InitData, true));
        ApplySavedGameData();
    }
}

[System.Serializable]
public class SaveData
{
    // 게임 데이터와 그 타입 (클릭 횟수 등)
    public string variablesToJson;
    public string variablesTypeToJson;

    //
    public string eventObjectStatusToJson;

    // 메모 관련 데이터들
    public string savedMemo;
    public string revealedMemo;

    public Dictionary<string, object> Variables => ToDictionary(variablesToJson, variablesTypeToJson); // 게임 매니저의 변수들
    public Dictionary<string, bool> EventObjectStatusDictionary => ToBoolDictionary(eventObjectStatusToJson); // 게임 매니저의 변수들
    public List<List<string[]>> SavedMemoList => ToArrayList(savedMemo);
    public List<List<string>> RevealedMemoList => ToList(revealedMemo);
    
    public SaveData (Dictionary<string, object> variables, Dictionary<string, bool>  eventObejctStatusDictionary, List<List<string[]>> savedMemoList, List<List<string>> revealedMemoList)
    {
        variablesToJson = ToJson(variables);
        variablesTypeToJson = TypeToJson(variables);

        eventObjectStatusToJson = ToJson(eventObejctStatusDictionary);

        savedMemo = ToJson(savedMemoList);
        revealedMemo = ToJson(revealedMemoList);
    }

    private string ToJson(Dictionary<string, object> dictionary)
    {
        // string으로 변환된 값을 담을 Dictionary<string, string>
        Dictionary<string, string> stringVariables = new();

        // 각각의 object를 string으로 변환하여 stringVariables에 추가
        foreach (var variable in dictionary)
        {
            string key = variable.Key;
            object value = variable.Value;
            string stringValue = value.ToString();
            stringVariables.Add(key, stringValue);
        }
        return JsonConvert.SerializeObject(stringVariables, Formatting.Indented);
    }
    private string ToJson(Dictionary<string, bool> dictionary)
    {
        return JsonConvert.SerializeObject(dictionary, Formatting.Indented);
    }
    private string TypeToJson(Dictionary<string, object> dictionary)
    {
        Dictionary<string, string> typeVariables = new();

        foreach (var variable in dictionary)
        {
            string key = variable.Key;
            object value = variable.Value;

            typeVariables.Add(key, value.GetType().ToString());
        }
        return JsonConvert.SerializeObject(typeVariables, Formatting.Indented);
    }
    private string ToJson(List<List<string[]>> array)
    {
        return JsonConvert.SerializeObject(array, Formatting.Indented);
    }
    private string ToJson(List<List<string>> array)
    {
        return JsonConvert.SerializeObject(array, Formatting.Indented);
    }
    private List<List<string[]>> ToArrayList(string json)
    {
        return JsonConvert.DeserializeObject<List<List<string[]>>>(json);
    }
    private List<List<string>> ToList(string json)
    {
        return JsonConvert.DeserializeObject<List<List<string>>>(json);
    }
    private Dictionary<string, object> ToDictionary(string json, string typeJson)
    {
        // JSON 문자열을 Dictionary<string, string>으로 역직렬화
        Dictionary<string, string> stringVariables = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
        Dictionary<string, string> typeVariables = JsonConvert.DeserializeObject<Dictionary<string, string>>(typeJson);

        // Dictionary<string, object>을 생성하여 변환된 값들을 추가
        Dictionary<string, object> objectVariables = new();
        foreach (var variable in stringVariables)
        {
            object value;
            Type type = Type.GetType(typeVariables[variable.Key]);

            if (type == typeof(int))
            {
                value = Convert.ToInt32(variable.Value);
            }
            else if (bool.TryParse(variable.Value, out bool boolValue))
            {
                value = Convert.ToBoolean(variable.Value);
            }
            else
            {
                value = variable.Value;
            }

            objectVariables.Add(variable.Key, value);
        }

        return objectVariables;
    }
    private Dictionary<string, bool> ToBoolDictionary(string json)
    {
        return JsonConvert.DeserializeObject<Dictionary<string, bool>>(json); ;
    }
}