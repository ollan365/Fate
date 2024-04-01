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
    private List<string> savedMemoIDList = new();

    // �޸� ������
    private int pageNum = 0;
    [SerializeField] private TextMeshProUGUI[] memoTexts;

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
        savedMemoIDList.Add(memoID);
    }

    // �޸��� ����
    public void ShowMemo()
    {
        // �޸��� �� ������ �� �޸� 4�� ���
        for (int i = pageNum * 4; i < pageNum * 4 + 4; i++)
        {
            if (i >= savedMemoIDList.Count)
            {
                memoTexts[i - pageNum * 4].text = "";
                break;
            }
            string scriptID = allMemo[savedMemoIDList[i]];
            memoTexts[i - pageNum * 4].text = DialogueManager.Instance.scripts[scriptID].GetScript();
        }
    }

    // �޸��� �ѱ��
    public void NextPage()
    {
        pageNum++;
        ShowMemo();
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
