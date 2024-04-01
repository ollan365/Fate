using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MemoManager : MonoBehaviour
{
    public static MemoManager Instance { get; private set; }
    public TextAsset memosCSV;

    // 모든 메모
    public Dictionary<string, string> allMemo = new();

    // 저장된 메모
    private List<string> savedMemoIDList = new();

    // 메모 페이지
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

    // 메모 추가하기
    public void AddMemo(string memoID)
    {
        savedMemoIDList.Add(memoID);
    }

    // 메모장 열기
    public void ShowMemo()
    {
        // 메모장 한 페이지 당 메모 4개 기록
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

    // 메모장 넘기기
    public void NextPage()
    {
        pageNum++;
        ShowMemo();
    }

    // memos.csv 파일 파싱
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
