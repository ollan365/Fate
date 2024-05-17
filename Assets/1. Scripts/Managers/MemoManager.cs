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

    private GameObject memoButton; // ���� ��� ���� �޸��ư

    // ��� �޸�
    public Dictionary<string, string> allMemo = new();

    // ����� �޸�
    private List<string> savedMemoList = new();
    public List<string> SavedMemoList
    {
        get => savedMemoList;
        set => savedMemoList = value;
    }

    // �޸� ������
    [SerializeField] private GameObject memoTextPrefab;

    // �޸� ����� ��ũ��
    public Transform scrollViewContent;

    // ���� ���� ������ ��Ż�� ������
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

    // �޸� �߰��ϱ�
    public void AddMemo(string memoID)
    {
        string scriptID = allMemo[memoID];
        foreach(string savedMemo in savedMemoList)
        {
            if (scriptID == savedMemo) return;
        }
        savedMemoList.Add(DialogueManager.Instance.scripts[scriptID].GetScript());
    }

    // �޸��� ����
    public void OpenMemoPage()
    {
        // �޸����� �������� ���� �޸����� �ݰ�, �������� ���� ����
        if (!memoPage.activeSelf)
        {
            // �޸����� ���� ���� ������ �޸����� �����ϰ� ���̰� �Ѵ�
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
        // �޸� ��ư�� ������ �ʰ� or ���̰� �� �� ����
        memoButton.SetActive(!flag);
    }
    public void MemoButtonChange()
    {
        if (!isFollow) memoButton = memoButtons[0];
        else memoButton = memoButtons[1];
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
