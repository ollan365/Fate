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
      
    public SaveData SavedData { private set; get; }
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

        if (File.Exists(SAVE_DATA_FILE_PATH))
            SavedData = JsonUtility.FromJson<SaveData>(File.ReadAllText(SAVE_DATA_FILE_PATH));
        else
        {
            SavedData = new SaveData();

            for (int i = 0; i < endingVariableNames.Count; i++)
                SavedData.EndingVariabls[endingVariableNames[i]] = -1;
        }
        
    }

    // 게임 데이터를 초기화 시킬 값 저장
    public void SaveInitGameData()
    {
        if (InitData != null) return;

        InitData = new SaveData();
        InitData.ChangeSavedGameData(
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

        for (int i = 0; i < endingVariableNames.Count; i++)
        {
            string dataName = endingVariableNames[i];
            SavedData.EndingVariabls[dataName] = GameManager.Instance.GetVariable(dataName);
        }
        SavedData.ChangeSavedEndingData(); // 엔딩 데이터 저장

        // 초기화용 데이터에서, 엔딩 데이터(회차가 넘어가도 유지되어야하는 데이터)를 변경
        for (int i = 0; i < endingVariableNames.Count; i++)
        {
            string dataName = endingVariableNames[i];
            InitData.Variables[dataName] = SavedData.EndingVariabls[dataName];
        }

        // 초기화용 변수로 저장 후 로드
        SaveGameData(InitData);
        LoadGameData();

    }

    public bool CheckGameData()
    {
        return File.Exists(SAVE_DATA_FILE_PATH);
    }
    // 불러오기
    public void LoadGameData()
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
            SavedData.ChangeSavedGameData(
                GameManager.Instance.Variables,
                GameManager.Instance.eventObjectsStatusDict,
                MemoManager.Instance.SavedMemoList,
                MemoManager.Instance.RevealedMemoList);
        // 저장된 데이터를 덮어씌울 때 (엔딩 후)
        else
            SavedData.ChangeSavedGameData(
                newSaveData.Variables,
                newSaveData.EventObjectStatusDictionary,
                newSaveData.SavedMemoList,
                newSaveData.RevealedMemoList);

        // 클래스를 Json 형식으로 전환하여 저장
        File.WriteAllText(SAVE_DATA_FILE_PATH, JsonUtility.ToJson(SavedData, true));
    }

    // 저장된 게임 데이터 삭제
    public void DeleteGameData()
    {
        if (File.Exists(SAVE_DATA_FILE_PATH)) File.Delete(SAVE_DATA_FILE_PATH);
    }
}

[System.Serializable]
public class SaveData
{
    // 엔딩과 관련된 데이터들
    public string endingVariablesToJson;
    public string endingVariablesTypeToJson;

    // 게임 데이터와 그 타입 (클릭 횟수 등)
    public string variablesToJson;
    public string variablesTypeToJson;

    //
    public string eventObjectStatusToJson;

    // 메모 관련 데이터들
    public string savedMemo;
    public string revealedMemo;

    public Dictionary<string, object> EndingVariabls { set; get; }
    public Dictionary<string, object> Variables => ToDictionary(variablesToJson, variablesTypeToJson); // 게임 매니저의 변수들
    public Dictionary<string, bool> EventObjectStatusDictionary => ToBoolDictionary(eventObjectStatusToJson); // 게임 매니저의 변수들
    public List<List<string[]>> SavedMemoList => ToArrayList(savedMemo);
    public List<List<string>> RevealedMemoList => ToList(revealedMemo);
    public SaveData()
    {
        EndingVariabls = new Dictionary<string, object>();
    }
    public void ChangeSavedGameData (Dictionary<string, object> variables, Dictionary<string, bool>  eventObejctStatusDictionary, List<List<string[]>> savedMemoList, List<List<string>> revealedMemoList)
    {
        variablesToJson = ToJson(variables);
        variablesTypeToJson = TypeToJson(variables);

        eventObjectStatusToJson = ToJson(eventObejctStatusDictionary);

        savedMemo = ToJson(savedMemoList);
        revealedMemo = ToJson(revealedMemoList);
    }
    public void ChangeSavedEndingData()
    {
        endingVariablesToJson = ToJson(EndingVariabls);
        endingVariablesTypeToJson = TypeToJson(EndingVariabls);
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