using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static Constants;

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
    private List<string>[] savedMemoList = new List<string>[4]; // 저장된 메모 전체
    public List<string>[] SavedMemoList
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

        for (int i = 0; i < 4; i++) savedMemoList[i] = new List<string>();

        memoButton = memoButtons[0];
    }

    // 현재 씬에 따라 메모의 개수를 파악 -> 현재 씬에 해당하는 메모의 개수에 따라 엔딩 선택지 해금 여부 결정
    public bool UnlockNextScene()
    {
        switch (SceneManager.Instance.CurrentScene)
        {
            case SceneType.ROOM_1:
                if (savedMemoList[0].Count > 8) return true;
                else return false;
            case SceneType.FOLLOW_1:
                if (savedMemoList[1].Count >= 8) return true;
                else return false;
            case SceneType.FOLLOW_2:
                if (savedMemoList[2].Count >= 8 && savedMemoList[3].Count >= 8) return true;
                else return false;
            default: return false;
        }
    }

    // 메모 추가하기
    public void AddMemo(string memoID)
    {
        string scriptID = allMemo[memoID];

        // 현재 씬에 따라 메모가 저장되는 곳이 달라짐
        int index = 0;
        switch(SceneManager.Instance.CurrentScene)
        {
            case SceneType.ROOM_1: index = 0; break;
            case SceneType.FOLLOW_1: index = 1; break;
            case SceneType.ROOM_2: index = 2; break;
            case SceneType.FOLLOW_2: index = 3; break;
        }
        // 이미 저장된 메모면 return
        foreach(string savedMemo in savedMemoList[index])
        {
            if (scriptID == savedMemo) return;
        }
        savedMemoList[index].Add(DialogueManager.Instance.scripts[scriptID].GetScript());
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

            for (int i = 0; i < 4; i++)
            {
                foreach (string memo in savedMemoList[i])
                {
                    GameObject memoTextObject = Instantiate(memoTextPrefab, scrollViewContent);
                    memoTextObject.GetComponent<TextMeshProUGUI>().text = memo;
                }
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
