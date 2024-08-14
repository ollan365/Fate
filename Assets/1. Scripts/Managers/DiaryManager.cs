using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DiaryManager : PageContentsManager
{
    private Dictionary<string, string> diary1Pages = new Dictionary<string, string>();
    private Dictionary<string, string> diary2Pages = new Dictionary<string, string>();
    private Dictionary<string, string> room2BookPages = new Dictionary<string, string>();

    public int totalPageCount = 0;
    
    [SerializeField] private PageFlip diaryPages;
    [SerializeField] private string diaryType;

    private int presentPageNum;

    private void Awake()
    {
        ParsePageContents();
    }

    public void SetTotalPages()
    {
        switch (diaryType)
        {
            case "Diary1":
                diaryPages.totalPageCount = diary1Pages.Count;
                break;
            
            case "Diary2":
                diaryPages.totalPageCount = diary2Pages.Count;
                break;

            case "Room2Book":
                diaryPages.totalPageCount = room2BookPages.Count;
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

        string diaryID = GetDiaryID(pageNum);

        if (diaryID == null || !currentPages.ContainsKey(diaryID))
        {
            SetPageText(pageType, "");
            Debug.LogWarning($"Diary ID '{diaryID}' not found in current pages dictionary");
            return;
        }

        string pageText = currentPages[diaryID];

        SetPageText(pageType, pageText);

        presentPageNum = pageNum;
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

    private string GetDiaryID(int pageNum)
    {
        return diaryType + "_" + pageNum.ToString().PadLeft(3, '0');
    }

    private Dictionary<string, string> GetCurrentPagesDictionary()
    {
        switch (diaryType)
        {
            case "Diary1":
                return diary1Pages;
            case "Diary2":
                return diary2Pages;
            case "Room2Book":
                return room2BookPages;
            default:
                return diary1Pages;
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

        // 방탈출2 다이어리 관련
        if ((bool)GameManager.Instance.GetVariable("Diary2PasswordCorrect")
            && presentPageNum == 2 && diaryType == "Diary2")
        {
            // 다이어리 내용 끝까지인 2페이지 확인하면 다이어리 내용 확인 스크립트 출력됨
            DialogueManager.Instance.StartDialogue("RoomEscape2_016");
        }
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

            if (diaryPageID.StartsWith("Diary1_")) targetDictionary = diary1Pages;
            else if (diaryPageID.StartsWith("Diary2_")) targetDictionary = diary2Pages;
            else if (diaryPageID.StartsWith("Room2Book_")) targetDictionary = room2BookPages;

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
}
