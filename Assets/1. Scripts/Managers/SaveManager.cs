using Newtonsoft.Json;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Constants;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    // --- ���� ������ �����̸� ���� ("���ϴ� �̸�(����).json") --- //
    private string GameDataFileName = "GameData.json";

    // --- ����� Ŭ���� ���� --- //
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

    // �ҷ�����
    public void LoadGameData()
    {
        string filePath = Application.persistentDataPath + "/" + GameDataFileName;

        // ����� ������ �ִٸ�
        if (File.Exists(filePath))
        {
            // ����� ���� �о���� Json�� Ŭ���� �������� ��ȯ�ؼ� �Ҵ�
            string FromJsonData = File.ReadAllText(filePath);
            data = JsonUtility.FromJson<SaveData>(FromJsonData);

            // ����� ���� �ε�
            DialogueManager.Instance.dialogueType = data.dialogueType;
            GameManager.Instance.Variables = data.variables;
            MemoManager.Instance.SavedMemoList = data.savedMemoList;

            GoScene(data.dialogueType);
        }
    }

    // ����� ���� �����Ͱ� �ִٸ�, �׿� ���� ������ �̵�
    private void GoScene(DialogueType dialogueType)
    {
        if (dialogueType == DialogueType.ROOM || dialogueType == DialogueType.ROOM_THINKING)
            SceneManager.LoadScene(1);
        else if(dialogueType == DialogueType.FOLLOW || dialogueType == DialogueType.FOLLOW_THINKING || dialogueType == DialogueType.FOLLOW_ANGRY)
            SceneManager.LoadScene(2);
    }

    // �����ϱ�
    public void SaveGameData()
    {
        // ������ ������ ����
        data = new(DialogueManager.Instance.dialogueType, GameManager.Instance.Variables, MemoManager.Instance.SavedMemoList);

        // Ŭ������ Json �������� ��ȯ (true : ������ ���� �ۼ�)
        string ToJsonData = JsonUtility.ToJson(data, true);
        string filePath = Application.persistentDataPath + "/" + GameDataFileName;

        // �̹� ����� ������ �ִٸ� �����, ���ٸ� ���� ���� ����
        File.WriteAllText(filePath, ToJsonData);
    }
}

[System.Serializable]
public class SaveData
{
    public DialogueType dialogueType; // ��� ������ ��������

    public string variablesToJson; //  Dictionary<string, string> �� json ���·� ����
    public Dictionary<string, object> variables // GameManager�� ������
    {
        get => ToDictionary(variablesToJson);
    }

    public List<string> savedMemoList; // Memo ����

    public SaveData(DialogueType type, Dictionary<string, object> variables, List<string> memo)
    {
        dialogueType = type;
        variablesToJson = ToJson(variables);
        savedMemoList = memo;
    }

    private string ToJson(Dictionary<string, object> dictionary)
    {
        // string���� ��ȯ�� ���� ���� Dictionary<string, string>
        Dictionary<string, string> stringVariables = new();

        // ������ object�� string���� ��ȯ�Ͽ� stringVariables�� �߰�
        foreach (var variable in dictionary)
        {
            string key = variable.Key;
            object value = variable.Value;
            string stringValue;

            // ���� ������ ���� ������ ó���� ����
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
    private Dictionary<string, object> ToDictionary(string json)
    {
        // JSON ���ڿ��� Dictionary<string, string>���� ������ȭ
        Dictionary<string, string> stringVariables = JsonConvert.DeserializeObject<Dictionary<string, string>>(variablesToJson);

        // Dictionary<string, object>�� �����Ͽ� ��ȯ�� ������ �߰�
        Dictionary<string, object> objectVariables = new();
        foreach (var variable in stringVariables)
        {
            object value;
            // string�� ������ �������� ��ȯ�Ͽ� object ������ �Ҵ�
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