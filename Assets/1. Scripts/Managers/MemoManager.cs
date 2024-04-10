using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MemoManager : MonoBehaviour
{
    public static MemoManager Instance { get; private set; }
    public TextAsset memosCSV;

    [SerializeField] private GameObject memoPage;
    [SerializeField] private GameObject memoButton;

    // 모든 메모
    public Dictionary<string, string> allMemo = new();

    // 저장된 메모
    private List<string> savedMemoList = new();

    // 메모 프리팹
    [SerializeField] private GameObject memoTextPrefab;

    // 메모가 저장될 스크롤
    public Transform scrollViewContent;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            ParseMemos();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 메모 추가하기
    public void AddMemo(string memoID)
    {
        string scriptID = allMemo[memoID];
        savedMemoList.Add(DialogueManager.Instance.scripts[scriptID].GetScript());
    }

    // 메모장 열기
    public void OpenMemoPage()
    {
        // 메모장이 켜져있을 때는 메모장을 닫고, 켜져있을 때는 끈다
        if (!memoPage.activeSelf)
        {
            foreach (string memo in savedMemoList)
            {
                GameObject memoTextObject = Instantiate(memoTextPrefab, scrollViewContent);
                memo.Replace("[", "<color=red>");
                memo.Replace("]", "</color>");
                memoTextObject.GetComponent<TextMeshProUGUI>().text = memo;
            }

            memoPage.SetActive(true);
        }
        else if (memoPage.activeSelf)
        {
            memoPage.SetActive(false);

            foreach (Transform child in scrollViewContent)
            {
                Destroy(child.gameObject);
            }
        }
    }
    public void HideMemoButton(bool flag)
    {
        // 메모 버튼을 보이지 않게 or 보이게 할 수 있음
        memoButton.SetActive(!flag);
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
