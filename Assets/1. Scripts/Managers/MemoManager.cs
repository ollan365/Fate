using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MemoManager : MonoBehaviour
{
    public static MemoManager Instance { get; private set; }
    public TextAsset memosCSV;

    [SerializeField] private GameObject memoPage;
    [SerializeField] private GameObject[] memoButtons;
    [SerializeField] private GameObject closeButton;

    private GameObject memoButton; // 현재 사용 중인 메모버튼

    // 모든 메모
    public Dictionary<string, string> allMemo = new();

    // 저장된 메모
    private List<string> savedMemoList = new();
    public List<string> SavedMemoList
    {
        get => savedMemoList;
        set => savedMemoList = value;
    }

    // 메모 프리팹
    [SerializeField] private GameObject memoTextPrefab;

    // 메모가 저장될 스크롤
    public Transform scrollViewContent;

    // 현재 미행 씬인지 방탈출 씬인지
    public bool isFollow;

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

        memoButton = memoButtons[0];
    }

    // 메모 추가하기
    public void AddMemo(string memoID)
    {
        string scriptID = allMemo[memoID];
        foreach(string savedMemo in savedMemoList)
        {
            if (scriptID == savedMemo) return;
        }
        savedMemoList.Add(DialogueManager.Instance.scripts[scriptID].GetScript());
    }

    // 메모장 열기
    public void OpenMemoPage()
    {
        // 메모장이 켜져있을 때는 메모장을 닫고, 켜져있을 때는 끈다
        if (!memoPage.activeSelf)
        {
            // 메모장을 켰을 때는 무조건 메모장이 선명하게 보이게 한다
            ColorBlock colors = memoButton.GetComponent<Button>().colors;
            colors.normalColor = new Color(1, 1, 1, 1);
            memoButton.GetComponent<Button>().colors = colors;

            foreach (string memo in savedMemoList)
            {
                GameObject memoTextObject = Instantiate(memoTextPrefab, scrollViewContent);
                memoTextObject.GetComponent<TextMeshProUGUI>().text = memo;
            }

            if (isFollow) FollowManager.Instance.ClickObject();

            memoPage.SetActive(true);
            closeButton.SetActive(true);
        }
        else if (memoPage.activeSelf)
        {
            if (isFollow)
            {
                FollowManager.Instance.EndScript(false);

                ColorBlock colors = memoButton.GetComponent<Button>().colors;
                colors.normalColor = new Color(1, 1, 1, 0.5f);
                memoButton.GetComponent<Button>().colors = colors;
            }

            memoPage.SetActive(false);
            closeButton.SetActive(false);

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
    public void MemoButtonChange()
    {
        if (!isFollow) memoButton = memoButtons[0];
        else memoButton = memoButtons[1];
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
