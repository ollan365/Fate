using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Book2IndexManager : PageContentsManager
{
    private Dictionary<string, string> diary1Pages = new Dictionary<string, string>();
    private Dictionary<string, string> diary2Pages = new Dictionary<string, string>();
    private Dictionary<string, string> room2BookPages = new Dictionary<string, string>();

    public int totalPageCount = 0;

    [SerializeField] private PageFlip bookPages;
    [SerializeField] private string bookType;
    [SerializeField] private List<Button> RightUpFlags;
    [SerializeField] private List<Button> RightDownFlags;
    [SerializeField] private List<Button> LeftFlags;

    [SerializeField] private int presentPageNum;

    [Header("인덱스들")]
    [SerializeField] private GameObject RightUpFlagsParent;
    [SerializeField] private GameObject RightDownFlagsParent;
    [SerializeField] private GameObject LeftFlagsParent;

    [SerializeField] private GameObject RightNextGameObject;

    [Header("bookPages.leftPage's Sprites")]
    public Sprite BookL2;
    public Sprite BookRedL;
    public Sprite BookYellowL;
    public Sprite BookBlueL;

    private void Awake()
    {
        ParsePageContents();
    }

    //private void Update()
    //{
    //    switch (bookPages.currentPage)
    //    {
    //        case 5:
    //            bookPages.leftPage = BookYellowL;
    //            break;

    //        case 6:
    //            SetFlags("LeftFlags", 0, true);
    //            SetFlags("LeftFlags", 1, true);
    //            SetFlags("LeftFlags", 2, true);
    //            break;
    //    }
    //}

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
        }
    }

    public override void DisplayPage(PageType pageType, int pageNum)
    {
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

        DisplayFlags();
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
    }

    public override void DisplayPagesStatic(int currentPage)
    {
        DisplayPage(PageType.Left, currentPage);
        DisplayPage(PageType.Right, currentPage + 1);

        flipLeftButton.SetActive(currentPage > 0);

        bool flipRightButtonOn = currentPage < totalPageCount - 1;
        // Debug.Log($"flipRightButtonOn: {flipRightButtonOn}\n\tcurrentPage: {currentPage}\n\ttotalPageCount: {totalPageCount}");
        flipRightButton.SetActive(flipRightButtonOn);
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

            if (targetDictionary.ContainsKey(diaryPageID)) targetDictionary[diaryPageID] += "\n\n" + script;
            else targetDictionary.Add(diaryPageID, script);
        }

        // Set totalPageCount based on the current scene's dictionary size
        totalPageCount = GetCurrentPagesDictionary()?.Count ?? 0;

        foreach (var page in GetCurrentPagesDictionary()) // Print all pages for debugging
        {
            // Debug.Log($"diaryID: {page.Key}\n\ttext: {page.Value}");
        }
    }

    // 페이지 넘어갈 때마다 해당 인덱스 flags를 위치에 맞게 켜줌
    private void DisplayFlags()
    {
        switch (presentPageNum)
        {
            case 0:
                bookPages.leftPage = BookL2;
                break;

            case 1:
                SetFlags("RightUpFlags",0,true);
                SetFlags("RightDownFlags", 1, true);
                SetFlags("RightDownFlags", 2, true);
                bookPages.leftPage = BookRedL;
                break;

            case 3:
                SetFlags("LeftFlags", 0, true);
                SetFlags("RightUpFlags", 1, true);
                SetFlags("RightDownFlags", 2, true);
                bookPages.leftPage = BookYellowL;
                break;

            case 5:
                SetFlags("LeftFlags", 0, true);
                SetFlags("LeftFlags", 1, true);
                SetFlags("RightUpFlags", 2, true);
                bookPages.leftPage = BookBlueL;
                break;
        }
        

        LeftFlagsParent.transform.SetAsFirstSibling();
        int rightPageIndex = RightNextGameObject.transform.GetSiblingIndex();
        RightUpFlagsParent.transform.SetSiblingIndex(rightPageIndex + 1);
        RightDownFlagsParent.transform.SetSiblingIndex(rightPageIndex - 1);
    }

    private void SetFlags(string flagsListName, int index, bool isShown)
    {
        switch (flagsListName)
        {
            case "RightUpFlags":
                RightUpFlags[index].gameObject.SetActive(isShown);
                RightDownFlags[index].gameObject.SetActive(!isShown);
                LeftFlags[index].gameObject.SetActive(!isShown);
                break;

            case "RightDownFlags":
                RightUpFlags[index].gameObject.SetActive(!isShown);
                RightDownFlags[index].gameObject.SetActive(isShown);
                LeftFlags[index].gameObject.SetActive(!isShown);
                break;

            case "LeftFlags":
                RightUpFlags[index].gameObject.SetActive(!isShown);
                RightDownFlags[index].gameObject.SetActive(!isShown);
                LeftFlags[index].gameObject.SetActive(isShown);
                break;
        }
    }
}
