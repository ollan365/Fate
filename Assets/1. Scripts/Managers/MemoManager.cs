using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
                memo.Replace("[", "<color=red>");
                memo.Replace("]", "</color>");
                memoTextObject.GetComponent<TextMeshProUGUI>().text = memo;
            }

            if (isFollow) FollowManager.Instance.ClickObject();

            memoPage.SetActive(true);
        }
        else if (memoPage.activeSelf)
        {
            MemoButtonAlphaChange(); // 만약 미행 파트이면 다시 버튼을 투명하게 만든다

            if (isFollow) FollowManager.Instance.EndScript(false);

            memoPage.SetActive(false);

            foreach (Transform child in scrollViewContent)
            {
                Destroy(child.gameObject);
            }
        }
    }
    public void HideMemoButton(bool flag)
    {
        // 튜토리얼 중이면 메모 버튼 켜도 보이지 않음
        if ((int)GameManager.Instance.GetVariable("Tutorial_Now") < 4)
        {
            memoButton.SetActive(false);
        }
        else
        {
            // 메모 버튼을 보이지 않게 or 보이게 할 수 있음
            memoButton.SetActive(!flag);
        }
        
    }
    public void MemoButtonAlphaChange()
    {
        ColorBlock colors = memoButton.GetComponent<Button>().colors;

        // Normal 상태의 색상을 씬에 따라 변경
        if (isFollow) colors.normalColor = new Color(1, 1, 1, 0.5f); // 미행일 때
        if (!isFollow) colors.normalColor = new Color(1, 1, 1, 1); // 미행이 아닐 때

        // 변경된 ColorBlock을 다시 버튼에 할당합니다.
        memoButton.GetComponent<Button>().colors = colors;
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
