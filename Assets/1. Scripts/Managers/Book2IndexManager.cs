using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Book2IndexManager : PageContentsManager
{
    private Dictionary<string, string> diary1Pages = new Dictionary<string, string>();
    private Dictionary<string, string> diary2Pages = new Dictionary<string, string>();
    private Dictionary<string, string> room2BookPages = new Dictionary<string, string>();
    private Dictionary<string, string> dreamDiaryPages = new Dictionary<string, string>();

    public int totalPageCount = 0;

    [SerializeField] private PageFlip bookPages;
    [SerializeField] private string bookType;
    //[SerializeField] private List<Button> RightUpFlags;
    [SerializeField] private List<Button> RightDownFlags;
    [SerializeField] private List<Button> LeftFlags;
    [SerializeField] private List<Button> RightNextFlags;
    [SerializeField] private List<Button> LeftNextFlags;

    [SerializeField] private int presentPageNum;

    //[SerializeField] private GameObject RightUpFlagsParent;
    [SerializeField] private GameObject RightDownFlagsParent;
    [SerializeField] private GameObject LeftFlagsParent;
    [SerializeField] private GameObject RightNextGameObject;

    [SerializeField] private GameObject NextPageClipGameObject;

    [Header("bookPages.leftPage's Sprites")]
    public Sprite BookL2;
    public Sprite BookRedL;
    public Sprite BookYellowL;
    public Sprite BookBlueL;
    [Header("bookPages.rightPage's Sprites")]
    public Sprite BookR2;
    public Sprite BookRedR;
    public Sprite BookYellowR;
    public Sprite BookBlueR;
    [Header("Right Index Sprites")]
    public Sprite IndexBlueR;
    public Sprite IndexBlueR2;
    public Image RightDownFlagThird;

    private void Awake()
    {
        ParsePageContents();
    }

    public void SetTotalPages()
    {
        switch (bookType)
        {
            case "Diary1":
                bookPages.totalPageCount = diary1Pages.Count;
                break;

            case "Diary2":
                bookPages.totalPageCount = diary2Pages.Count;
                break;
            case "Room2Book":
                bookPages.totalPageCount = room2BookPages.Count;
                break;

            case "DreamDiary":
                bookPages.totalPageCount = dreamDiaryPages.Count;
                break;
        }
    }

    public override void DisplayPage(PageType pageType, int pageNum)
    {
        switch (pageType)
        {
            case PageType.Left:
                leftPageNum.text = pageNum == 0 ? "" : pageNum.ToString();
                break;

            case PageType.Right:
                rightPageNum.text = pageNum.ToString();
                break;

            case PageType.Back:
                backPageNum.text = pageNum.ToString();
                break;

            case PageType.Front:
                frontPageNum.text = pageNum.ToString();
                break;
        }

        Dictionary<string, string> currentPages = GetCurrentPagesDictionary();
        if (currentPages == null)
        {
            SetPageText(pageType, "");
            // Debug.LogWarning("Current pages dictionary is null");
            return;
        }

        if (pageNum < 1 || pageNum > totalPageCount)
        {
            SetPageText(pageType, "");
            // Debug.LogWarning($"Invalid page number {pageNum}. Total pages: {totalPageCount}");
            return;
        }

        string diaryID = GetBookID(pageNum);

        if (diaryID == null || !currentPages.ContainsKey(diaryID))
        {
            SetPageText(pageType, "");
            Debug.LogWarning($"Diary ID '{diaryID}' not found in current pages dictionary");
            return;
        }

        string pageText = currentPages[diaryID];

        SetPageText(pageType, pageText);

        presentPageNum = pageNum;

        //DisplayFlags();
    }

    private void SetPageText(PageType pageType, string text)
    {
        switch (pageType)
        {
            case PageType.Left:
                leftPage.text = text;
                break;

            case PageType.Right:
                rightPage.text = text;
                break;

            case PageType.Back:
                backPage.text = text;
                break;

            case PageType.Front:
                frontPage.text = text;
                break;
        }
    }

    private string GetBookID(int pageNum)
    {
        return bookType + "_" + pageNum.ToString().PadLeft(3, '0');
    }

    private Dictionary<string, string> GetCurrentPagesDictionary()
    {
        switch (bookType)
        {
            case "Diary1":
                return diary1Pages;
            case "Diary2":
                return diary2Pages;
            case "Room2Book":
                return room2BookPages;
            case "DreamDiary":
                return dreamDiaryPages;
            default:
                return room2BookPages;
        }
    }

    public override void DisplayPagesDynamic(int currentPage)
    {
        DisplayPage(PageType.Left, currentPage);
        DisplayPage(PageType.Right, currentPage + 3);
        DisplayPage(PageType.Back, currentPage + 1);
        DisplayPage(PageType.Front, currentPage + 2);

        DisplayFlags();
    }

    public override void DisplayPagesStatic(int currentPage)
    {
        DisplayPage(PageType.Left, currentPage);
        DisplayPage(PageType.Right, currentPage + 1);

        flipLeftButton.SetActive(currentPage > 0);

        bool flipRightButtonOn = currentPage < totalPageCount - 1;
        // Debug.Log($"flipRightButtonOn: {flipRightButtonOn}\n\tcurrentPage: {currentPage}\n\ttotalPageCount: {totalPageCount}");
        flipRightButton.SetActive(flipRightButtonOn);

        DisplayFlags();
    }

    public override void ParsePageContents()
    {
        TextAsset diaryCsv = Resources.Load<TextAsset>("Datas/diary");
        if (diaryCsv == null)
        {
            Debug.LogError("Failed to load diary CSV file");
            return;
        }

        string[] lines = diaryCsv.text.Split('\n');
        string previousDiaryPageID = "";

        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;

            string[] fields = lines[i].Split(',');

            string diaryPageID = fields[0].Trim();
            if (diaryPageID == "") diaryPageID = previousDiaryPageID;
            else previousDiaryPageID = diaryPageID;

            string scriptID = fields[2].Trim();
            if (!DialogueManager.Instance.scripts.ContainsKey(scriptID))
            {
                Debug.LogWarning($"Script ID '{scriptID}' not found in DialogueManager scripts");
                continue;
            }

            var script = DialogueManager.Instance.scripts[scriptID].GetScript();
            Dictionary<string, string> targetDictionary = null;

            if (diaryPageID.StartsWith("Room2Book_")) targetDictionary = room2BookPages;
            else if (diaryPageID.StartsWith("Diary1_")) targetDictionary = diary1Pages;
            else if (diaryPageID.StartsWith("Diary2_")) targetDictionary = diary2Pages;
            else if (diaryPageID.StartsWith("DreamDiary_")) targetDictionary = dreamDiaryPages;
            else if (diaryPageID.StartsWith("Album_")) continue;
            else
            {
                Debug.LogWarning($"Unknown diary page ID format: {diaryPageID}");
                continue;
            }

            if (targetDictionary.ContainsKey(diaryPageID)) targetDictionary[diaryPageID] += "\n\n" + script;
            else targetDictionary.Add(diaryPageID, script);
        }

        // Set totalPageCount based on the current scene's dictionary size
        totalPageCount = GetCurrentPagesDictionary()?.Count ?? 0;

        foreach (var page in GetCurrentPagesDictionary()) // Print all pages for debugging
        {
            // Debug.Log($"diaryID: {page.Key}\n\ttext: {page.Value}");
        }


        DisplayFlags();
    }

    // ������ �Ѿ ������ �ش� �ε��� flags�� ��ġ�� �°� ����
    // presentPageNum�� ���� �ش��ϴ� �ε����� �ִ� ������ ���ҽ��� ������
    private void DisplayFlags()
    {
        switch (presentPageNum)
        {
            case 1:
                //������ �ε���
                SetFlags("RightNextFlags", 0,true);
                SetFlags("RightDownFlags", 1, true);
                SetFlags("RightDownFlags", 2, true);
                bookPages.leftNext.sprite = BookL2;
                bookPages.leftPage = BookL2;
                bookPages.rightNext.sprite = BookRedR;
                bookPages.rightPage = BookRedR;

                RightDownFlagThird.sprite = IndexBlueR2;
                break;

            case 2:
                // 1������ �巡�� ��
                bookPages.left.sprite = BookRedR;
                bookPages.right.sprite = BookRedL;

                bookPages.rightNext.sprite = BookYellowR;
                bookPages.rightPage = BookYellowR;
                break;

            case 3:
                //����� �ε���
                SetFlags("RightNextFlags", 1, true);
                SetFlags("LeftNextFlags", 0, true);
                SetFlags("RightDownFlags", 2, true);
                bookPages.leftNext.sprite = BookRedL;
                bookPages.leftPage = BookRedL;
                bookPages.rightNext.sprite = BookYellowR;
                bookPages.rightPage = BookYellowR;

                RightDownFlagThird.sprite = IndexBlueR;
                break;

            case 4:
                // 3������ �巡�� ��
                bookPages.left.sprite = BookYellowR;
                bookPages.right.sprite = BookYellowL;

                bookPages.rightNext.sprite = BookBlueR;
                bookPages.rightPage = BookBlueR;

                SetFlags("LeftFlags", 0, true);
                bookPages.leftNext.sprite = BookL2;
                break;

            case 5:
                //�Ķ��� �ε���
                SetFlags("RightNextFlags", 2, true);
                SetFlags("LeftFlags", 0, true);
                SetFlags("LeftNextFlags", 1, true);
                bookPages.leftNext.sprite = BookYellowL;
                bookPages.leftPage = BookYellowL;
                bookPages.rightNext.sprite = BookBlueR;
                bookPages.rightPage = BookBlueR;
                break;

            case 6:
                // 5������ �巡�� ��
                bookPages.left.sprite = BookBlueR;
                bookPages.right.sprite = BookBlueL;

                bookPages.rightNext.sprite = BookR2;
                bookPages.rightPage = BookR2;

                SetFlags("LeftFlags", 1, true);
                bookPages.leftNext.sprite = BookL2;
                break;

            // 5�������� �������� �ڿ� ���� 2�������� ������ 5���������� ���������� �ѱ�� �ɷ� ������ ���ܼ� ������ ����
            case 7:
                SetFlags("LeftFlags", 0, true);
                SetFlags("LeftFlags", 1, true);
                SetFlags("LeftNextFlags", 2, true);
                bookPages.leftNext.sprite = BookBlueL;
                bookPages.leftPage = BookBlueL;
                bookPages.rightNext.sprite = BookR2;
                bookPages.rightPage = BookR2;
                break;
            case 8:
                SetFlags("LeftFlags", 0, true);
                SetFlags("LeftFlags", 1, true);
                SetFlags("LeftFlags", 2, true);
                bookPages.left.sprite = BookR2;
                bookPages.right.sprite = BookL2;

                bookPages.leftNext.sprite = BookL2;
                bookPages.leftPage = BookL2;
                bookPages.rightNext.sprite = BookR2;
                bookPages.rightPage = BookR2;
                break;
        }

        // �������� �ڵ����� �Ѿ �� RightDownFlagsParent�� index�� RightNextGameObject�� ��ĭ �տ� �־�� �ϴµ�
        // ������Ʈ���� ��ġ�� ����Ǿ SetSiblingIndex�� ������ ������.
        LeftFlagsParent.transform.SetAsFirstSibling();
        int rightPageIndex = NextPageClipGameObject.transform.GetSiblingIndex();
        RightNextGameObject.transform.SetSiblingIndex(rightPageIndex - 2);
        RightDownFlagsParent.transform.SetSiblingIndex(rightPageIndex - 3);
    }

    private void SetFlags(string flagsListName, int index, bool isShown)
    {
        switch (flagsListName)
        {
            case "RightDownFlags":
                RightDownFlags[index].gameObject.SetActive(isShown);
                LeftFlags[index].gameObject.SetActive(!isShown);

                RightNextFlags[index].gameObject.SetActive(!isShown);
                LeftNextFlags[index].gameObject.SetActive(!isShown);
                break;

            case "LeftFlags":
                RightDownFlags[index].gameObject.SetActive(!isShown);
                LeftFlags[index].gameObject.SetActive(isShown);

                RightNextFlags[index].gameObject.SetActive(!isShown);
                LeftNextFlags[index].gameObject.SetActive(!isShown);
                break;

            case "RightNextFlags":
                RightNextFlags[index].gameObject.SetActive(isShown);
                LeftNextFlags[index].gameObject.SetActive(!isShown);

                RightDownFlags[index].gameObject.SetActive(!isShown);
                LeftFlags[index].gameObject.SetActive(!isShown);
                break;
            case "LeftNextFlags":
                RightNextFlags[index].gameObject.SetActive(!isShown);
                LeftNextFlags[index].gameObject.SetActive(isShown);

                RightDownFlags[index].gameObject.SetActive(!isShown);
                LeftFlags[index].gameObject.SetActive(!isShown);
                break;
        }
    }
}
