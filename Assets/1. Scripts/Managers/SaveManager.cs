using Newtonsoft.Json;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using static Constants;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    // --- 게임 데이터 파일이름 설정 ("원하는 이름(영문).json") --- //
    private string GameDataFileName = "GameData.json";
    private string EndingDataFileName = "EndingData.json";

    // --- 저장용 클래스 변수 --- //
    private SaveData initData;
    [SerializeField] private SaveData data;
    public EndingData EndingData { get; private set; }

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

        string filePath = Application.persistentDataPath + "/" + EndingDataFileName;
        if (File.Exists(filePath)) EndingData = JsonUtility.FromJson<EndingData>(File.ReadAllText(filePath));
        else EndingData = new EndingData();
    }
    public void SaveInitGameData()
    {
        if (initData == null)
            initData = new SaveData(SceneType.START,
                1,
                GameManager.Instance.Variables,
                GameManager.Instance.eventObjectsStatusDict,
                MemoManager.Instance.SavedMemoList,
                MemoManager.Instance.RevealedMemoList);

        if (EndingData == null) return;

        GameManager.Instance.SetVariable("EndingCollect", EndingData.allEndingCollectCount);
        GameManager.Instance.SetVariable("LastEnding", EndingData.lastEnding.ToString());
        GameManager.Instance.SetVariable("BadACollect", EndingData.endingCollectCount[0]);
        GameManager.Instance.SetVariable("BadBCollect", EndingData.endingCollectCount[1]);
        GameManager.Instance.SetVariable("TrueCollect", EndingData.endingCollectCount[2]);
        GameManager.Instance.SetVariable("HiddenCollect", EndingData.endingCollectCount[3]);
        GameManager.Instance.SetVariable("BadEndingCollect", EndingData.badEndingColloectCount);

        GameManager.Instance.SetVariable("FateName", EndingData.fateName);
        GameManager.Instance.SetVariable("Language", EndingData.language);
        GameManager.Instance.SetVariable("AccidyGender", EndingData.accidyGender);
        GameManager.Instance.SetVariable("FateBirthday", EndingData.fateBirthDay);
}
    public void InitGameData()
    {
        string ToJsonData = JsonUtility.ToJson(initData, true);
        string filePath = Application.persistentDataPath + "/" + GameDataFileName;

        File.WriteAllText(filePath, ToJsonData);
    }
    public void SaveEndingData(EndingType ending)
    {
        if(ending != EndingType.NONE) EndingData.AddEnding(ending);

        string ToJsonData = JsonUtility.ToJson(EndingData, true);
        string filePath = Application.persistentDataPath + "/" + EndingDataFileName;

        File.WriteAllText(filePath, ToJsonData);
    }
    public bool CheckGameData()
    {
        return File.Exists(Application.persistentDataPath + "/" + GameDataFileName);
    }
    // 불러오기
    public SceneType LoadGameData()
    {
        string filePath = Application.persistentDataPath + "/" + GameDataFileName;

        if (!File.Exists(filePath)) return SceneType.START; // no save game data

        // 저장된 파일 읽어오고 Json을 클래스 형식으로 전환해서 할당
        string FromJsonData = File.ReadAllText(filePath);
        data = JsonUtility.FromJson<SaveData>(FromJsonData);

        // 저장된 내용 로드
        SceneManager.Instance.roomSideIndex = data.lastSideIndex;
        GameManager.Instance.Variables = data.Variables;
        GameManager.Instance.eventObjectsStatusDict = data.EventObjectStatusDictionary;
        MemoManager.Instance.SavedMemoList = data.SavedMemoList;
        MemoManager.Instance.RevealedMemoList = data.RevealedMemoList;

        return data.sceneType;
    }

    // 저장하기
    public void SaveGameData()
    {
        // 저장할 데이터 생성 (방이면 RoomManager에 접근하여 현재 화면의 인덱스도 저장)
        var currentSideIndex = SceneManager.Instance.CurrentScene == SceneType.ROOM_1 || SceneManager.Instance.CurrentScene == SceneType.ROOM_2
        ? RoomManager.Instance.currentSideIndex : 1;

        data = new(SceneManager.Instance.CurrentScene,
            currentSideIndex,
            GameManager.Instance.Variables,
            GameManager.Instance.eventObjectsStatusDict,
            MemoManager.Instance.SavedMemoList,
            MemoManager.Instance.RevealedMemoList);

        // 클래스를 Json 형식으로 전환 (true : 가독성 좋게 작성)
        string ToJsonData = JsonUtility.ToJson(data, true);
        string filePath = Application.persistentDataPath + "/" + GameDataFileName;

        // 이미 저장된 파일이 있다면 덮어쓰고, 없다면 새로 만들어서 저장
        File.WriteAllText(filePath, ToJsonData);
    }

    // 저장된 게임 데이터 삭제
    public void DeleteGameData()
    {
        string filePath = Application.persistentDataPath + "/" + GameDataFileName;
        if (File.Exists(filePath)) File.Delete(filePath);

        EndingData.Init();
        SaveEndingData(EndingType.NONE);
    }
}

[System.Serializable]
public class SaveData
{
    public SceneType sceneType; // 어느 씬에서 끝났는지
    public int lastSideIndex;

    public string variablesToJson;
    public string eventObjectStatusToJson;
    public string savedMemo;
    public string revealedMemo;

    public Dictionary<string, object> Variables => ToDictionary(variablesToJson); // 게임 매니저의 변수들
    public Dictionary<string, bool> EventObjectStatusDictionary => ToBoolDictionary(eventObjectStatusToJson); // 게임 매니저의 변수들
    public List<List<string[]>> SavedMemoList => ToArrayList(savedMemo);
    public List<List<string>> RevealedMemoList => ToList(revealedMemo);

    public SaveData(SceneType type, int currentSideIndex, Dictionary<string, object> variables, Dictionary<string, bool>  eventObejctStatusDictionary, List<List<string[]>> savedMemoList, List<List<string>> revealedMemoList)
    {
        sceneType = type;
        lastSideIndex = currentSideIndex;
        variablesToJson = ToJson(variables);
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
            string stringValue;

            // 값의 유형에 따라 적절한 처리를 수행
            if (value is int intValue)
            {
                stringValue = intValue.ToString();
            }
            else if (value is bool boolValue)
            {
                stringValue = boolValue.ToString();
            }
            else
            {
                stringValue = value.ToString();
            }

            stringVariables.Add(key, stringValue);
        }
        return JsonConvert.SerializeObject(stringVariables, Formatting.Indented);
    }
    private string ToJson(Dictionary<string, bool> dictionary)
    {
        return JsonConvert.SerializeObject(dictionary, Formatting.Indented);
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
    private Dictionary<string, object> ToDictionary(string json)
    {
        // JSON 문자열을 Dictionary<string, string>으로 역직렬화
        Dictionary<string, string> stringVariables = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);

        // Dictionary<string, object>을 생성하여 변환된 값들을 추가
        Dictionary<string, object> objectVariables = new();
        foreach (var variable in stringVariables)
        {
            object value;
            // string을 적절한 형식으로 변환하여 object 변수에 할당
            if (int.TryParse(variable.Value, out int intValue))
            {
                value = intValue;
            }
            else if (bool.TryParse(variable.Value, out bool boolValue))
            {
                value = boolValue;
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
public class EndingData
{
    public bool isEndingLogicEnd = true; // 엔딩 로직이 끝났는지 (자동 프롤로그 시작 후 프롤로그 완료까지 했는지)
    public int allEndingCollectCount = 0; // 엔딩을 본 횟수
    public int badEndingColloectCount = 0; // 배드 엔딩을 본 횟수
    public int[] endingCollectCount = new int[4] { 0, 0, 0, 0 }; // 각 엔딩을 본 횟수
    public EndingType lastEnding = EndingType.NONE; // 마지막으로 본 엔딩

    public int accidyGender = 0;
    public string fateName = "필연";
    public int language = 1;
    public string fateBirthDay = "0516";

    public void AddEnding(EndingType ending)
    {
        allEndingCollectCount++;
        lastEnding = ending;

        switch (ending)
        {
            case EndingType.BAD_A:
                isEndingLogicEnd = false;
                badEndingColloectCount++;
                endingCollectCount[0]++;
                break;

            case EndingType.BAD_B:
                isEndingLogicEnd = false;
                badEndingColloectCount++;
                endingCollectCount[1]++;
                break;

            case EndingType.TRUE:
                isEndingLogicEnd = false;
                endingCollectCount[2]++;
                break;

            case EndingType.HIDDEN:
                isEndingLogicEnd = true;
                endingCollectCount[3]++;
                break;
        }
    }
    public void Init()
    {
        isEndingLogicEnd = true;
        allEndingCollectCount = 0;
        badEndingColloectCount = 0;
        endingCollectCount = new int[4] { 0, 0, 0, 0 };
        lastEnding = EndingType.NONE;
    }
}