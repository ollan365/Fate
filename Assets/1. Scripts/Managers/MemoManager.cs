using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MemoManager : MonoBehaviour
{
    public static MemoManager Instance { get; private set; }
    public TextAsset memosCSV;

    [SerializeField] private GameObject memoPage;
    [SerializeField] private GameObject memoButton;

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
            DontDestroyOnLoad(gameObject);
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
    public void OpenMemoPage()
    {
        // �޸����� �������� ���� �޸����� �ݰ�, �������� ���� ����
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
        // �޸� ��ư�� ������ �ʰ� or ���̰� �� �� ����
        memoButton.SetActive(!flag);
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
