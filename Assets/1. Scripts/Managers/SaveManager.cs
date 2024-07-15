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

    // --- 저장용 클래스 변수 --- //
    private SaveData initData;
    public SaveData data;

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
    public void SaveInitGameData()
    {
        initData = new SaveData(SceneType.START, 1, GameManager.Instance.Variables, MemoManager.Instance.SavedMemoList);
    }
    public void InitGameData()
    {
        // 엔딩을 본 개수를 제외하고 저장된 데이터 초기화하여 저장
        initData.endingCollectCount = (int)GameManager.Instance.GetVariable("EndingCollect");

        string ToJsonData = JsonUtility.ToJson(initData, true);
        string filePath = Application.persistentDataPath + "/" + GameDataFileName;

        File.WriteAllText(filePath, ToJsonData);
    }
    public int CheckGameData()
    {
        string filePath = Application.persistentDataPath + "/" + GameDataFileName;

        if (!File.Exists(filePath)) return -1; // 저장된 게임 데이터 없음

        string FromJsonData = File.ReadAllText(filePath);
        data = JsonUtility.FromJson<SaveData>(FromJsonData);

        if (data.sceneType == SceneType.START && data.lastEnding != EndingType.NONE) return 0; // 저장된 게임 데이터가 있고, 시작 씬에서 종료된 경우 (엔딩 직후)
        else return 1;
    }
    // 불러오기
    public void LoadGameData()
    {
        string filePath = Application.persistentDataPath + "/" + GameDataFileName;

        if (!File.Exists(filePath)) return; // no save game data

        // 저장된 파일 읽어오고 Json을 클래스 형식으로 전환해서 할당
        string FromJsonData = File.ReadAllText(filePath);
        data = JsonUtility.FromJson<SaveData>(FromJsonData);

        // 저장된 내용 로드
        SceneManager.Instance.roomSideIndex = data.lastSideIndex;
        GameManager.Instance.Variables = data.variables;
        GameManager.Instance.SetVariable("EndingCollect", data.endingCollectCount);
        MemoManager.Instance.SavedMemoList = data.savedMemoList;

        // 게임 데이터에 따른 씬으로 이동
        SceneManager.Instance.LoadScene(data.sceneType);
    }

    // 저장하기
    public void SaveGameData()
    {
        // 저장할 데이터 생성 (방이면 RoomManager에 접근하여 현재 화면의 인덱스도 저장)
        if (SceneManager.Instance.CurrentScene == SceneType.ROOM_1 || SceneManager.Instance.CurrentScene == SceneType.ROOM_2)
            data = new(SceneManager.Instance.CurrentScene, RoomManager.Instance.currentSideIndex, GameManager.Instance.Variables, MemoManager.Instance.SavedMemoList);
        else data = new(SceneManager.Instance.CurrentScene, 1, GameManager.Instance.Variables, MemoManager.Instance.SavedMemoList);

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

        // 파일이 존재하는지 확인 후 삭제
        if (File.Exists(filePath)) File.Delete(filePath);
    }
}

[System.Serializable]
public class SaveData
{
    public int endingCollectCount;
    public EndingType lastEnding; // 마지막으로 본 엔딩

    public SceneType sceneType; // 어느 씬에서 끝났는지
    public int lastSideIndex;
    public string variablesToJson; //  Dictionary<string, string> 을 json 형태로 저장
    public Dictionary<string, object> variables // GameManager의 변수들
    {
        get => ToDictionary(variablesToJson);
    }
    public string memoToJson;
    public List<string>[] savedMemoList // Memo 저장
    {
        get => ToListArray(memoToJson);
    }

    public SaveData(SceneType type, int currentSideIndex, Dictionary<string, object> variables, List<string>[] memo)
    {
        endingCollectCount = (int)GameManager.Instance.GetVariable("EndingCollect");

        sceneType = type;
        lastSideIndex = currentSideIndex;
        variablesToJson = ToJson(variables);
        memoToJson = ToJson(memo);
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
    private string ToJson(List<string>[] array)
    {
        return JsonConvert.SerializeObject(array, Formatting.Indented);
    }
    private List<string>[] ToListArray(string json)
    {
        return JsonConvert.DeserializeObject<List<string>[]>(json);
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
}