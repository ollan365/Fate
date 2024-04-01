using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MemoManager : MonoBehaviour
{
    public static MemoManager Instance { get; private set; }
    public TextAsset memosCSV;

    // ��� �޸�
    public Dictionary<string, string> allMemo = new();

    // ����� �޸�
    private List<string> savedMemoList = new();

    // �޸� ������
    [SerializeField] private GameObject memoTextPrefab;

    // �޸� ����� ��ũ��
    public Transform scrollViewContent;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            ParseMemos();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // �޸� �߰��ϱ�
    public void AddMemo(string memoID)
    {
        string scriptID = allMemo[memoID];
        savedMemoList.Add(DialogueManager.Instance.scripts[scriptID].GetScript());
    }

    // �޸��� ����
    public void ShowMemo()
    {
        foreach (string memo in savedMemoList)
        {
            GameObject memoTextObject = Instantiate(memoTextPrefab, scrollViewContent);
            memoTextObject.GetComponent<TextMeshProUGUI>().text = memo;
        }
    }

    // memos.csv ���� �Ľ�
    public void ParseMemos()
    {
        string[] lines = memosCSV.text.Split('\n');
        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;

            string[] fields = lines[i].Split(',');

            string memoID = fields[0].Trim();
            string scriptID = fields[2].Trim();
            
            allMemo[memoID] = scriptID;
        }
    }
}
