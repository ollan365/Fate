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

    // ��� �޸�
    public Dictionary<string, string> allMemo = new();

    // ����� �޸�
    private List<string> savedMemoList = new();

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
                memo.Replace("[", "<color=red>");
                memo.Replace("]", "</color>");
                memoTextObject.GetComponent<TextMeshProUGUI>().text = memo;
            }

            if (isFollow) FollowManager.Instance.ClickObject();

            memoPage.SetActive(true);
        }
        else if (memoPage.activeSelf)
        {
            MemoButtonAlphaChange(); // ���� ���� ��Ʈ�̸� �ٽ� ��ư�� �����ϰ� �����

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
        // Ʃ�丮�� ���̸� �޸� ��ư �ѵ� ������ ����
        if ((int)GameManager.Instance.GetVariable("Tutorial_Now") < 4)
        {
            memoButton.SetActive(false);
        }
        else
        {
            // �޸� ��ư�� ������ �ʰ� or ���̰� �� �� ����
            memoButton.SetActive(!flag);
        }
        
    }
    public void MemoButtonAlphaChange()
    {
        ColorBlock colors = memoButton.GetComponent<Button>().colors;

        // Normal ������ ������ ���� ���� ����
        if (isFollow) colors.normalColor = new Color(1, 1, 1, 0.5f); // ������ ��
        if (!isFollow) colors.normalColor = new Color(1, 1, 1, 1); // ������ �ƴ� ��

        // ����� ColorBlock�� �ٽ� ��ư�� �Ҵ��մϴ�.
        memoButton.GetComponent<Button>().colors = colors;
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
