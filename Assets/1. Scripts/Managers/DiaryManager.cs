using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DiaryManager : PageContentsManager
{
    private Dictionary<string, (string, string)> diary1Pages = new Dictionary<string, (string, string)>();
    private Dictionary<string, (string, string)> diary2Pages = new Dictionary<string, (string, string)>();
    private Dictionary<string, (string, string)> room2BookPages = new Dictionary<string, (string, string)>();
    private Dictionary<string, (string, string)> dreamDiaryPages = new Dictionary<string, (string, string)>();
    private Dictionary<string, (string, string)> albumPages = new Dictionary<string, (string, string)>();

    public int totalPageCount = 0;
    
    [SerializeField] private PageFlip diaryPages;
    [SerializeField] private string diaryType;

    public Image leftPageImage;
    public Image rightPageImage;
    public Image backPageImage;
    public Image frontPageImage;

    [SerializeField] private GameObject leftPageEndingFrames,
        rightPageEndingFrames,
        backPageEndingFrames,
        frontPageEndingFrames;
    
    [SerializeField] private Image leftPageEndingFrameTop, leftPageEndingFrameBottom, 
        rightPageEndingFrameTop, rightPageEndingFrameBottom,
        backPageEndingFrameTop, backPageEndingFrameBottom,
        frontPageEndingFrameTop, frontPageEndingFrameBottom;
    
    [SerializeField] private Button leftPageEndingButtonTop, leftPageEndingButtonBottom,
        rightPageEndingButtonTop, rightPageEndingButtonBottom,
        backPageEndingButtonTop, backPageEndingButtonBottom,
        frontPageEndingButtonTop, frontPageEndingButtonBottom;
    
    private Sprite[] endingSprites;

    private int presentPageNum;

    private string doodlesOrder = "";
    private int replayCount = 1; 
    private const int AlbumFramePageNumMinimum = 2, AlbumFramePageNumMaximum = 3;

    private void Awake()
    {
        // SetDoodlesOrder();
        ParsePageContents();
        endingSprites = UIManager.Instance.endingSprites;
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

            case "DreamDiary":
                diaryPages.totalPageCount = dreamDiaryPages.Count;
                break;
            
            case "Album":
                diaryPages.totalPageCount = albumPages.Count;
                break;
        }
    }
    
    public override void DisplayPage(PageType pageType, int pageNum)
    {
        Dictionary<string, (string, string)> currentPages = GetCurrentPagesDictionary();
        if (currentPages == null)
        {
            SetPageText(pageType, "", pageNum);
            return;
        }

        if (pageNum < 1 || pageNum > totalPageCount)
        {
            SetPageText(pageType, "", pageNum);
            SetPageImage(pageType, "");
            if (diaryType == "Album") // Hide album frames
                SetPageFrame(pageType, pageNum);
            return;
        }

        string diaryID = GetDiaryID(pageNum);
        if (diaryID == null || !currentPages.ContainsKey(diaryID))
        {
            SetPageText(pageType, "", pageNum);
            Debug.LogWarning($"Diary ID '{diaryID}' not found in current pages dictionary");
            return;
        }

        string pageText = currentPages[diaryID].Item1;
        string doodleID = currentPages[diaryID].Item2;

        SetPageText(pageType, pageText, pageNum);
        SetPageImage(pageType, doodleID);
        if (diaryType == "Album") // Show/hide album frames
            SetPageFrame(pageType, pageNum);

        presentPageNum = pageNum;
    }

    private void SetPageFrame(PageType pageType, int pageNum) {
        switch (pageType) {
            case PageType.Left:
                if (pageNum is >= AlbumFramePageNumMinimum and <= AlbumFramePageNumMaximum) {
                    leftPageEndingFrames.SetActive(true);
                    SetAlbumFrame(leftPageEndingFrameTop,
                        leftPageEndingFrameBottom,
                        leftPageEndingButtonTop,
                        leftPageEndingButtonBottom,
                        pageNum);
                } else 
                    leftPageEndingFrames.SetActive(false);
                break;
            
            case PageType.Right:
                if (pageNum is >= AlbumFramePageNumMinimum and <= AlbumFramePageNumMaximum) {
                    rightPageEndingFrames.SetActive(true);
                    SetAlbumFrame(rightPageEndingFrameTop,
                        rightPageEndingFrameBottom,
                        rightPageEndingButtonTop,
                        rightPageEndingButtonBottom,
                        pageNum);
                } else 
                    rightPageEndingFrames.SetActive(false);
                break;
            
            case PageType.Back:
                if (pageNum is >= AlbumFramePageNumMinimum and <= AlbumFramePageNumMaximum) {
                    backPageEndingFrames.SetActive(true);
                    SetAlbumFrame(backPageEndingFrameTop,
                        backPageEndingFrameBottom,
                        backPageEndingButtonTop,
                        backPageEndingButtonBottom,
                        pageNum);
                } else 
                    backPageEndingFrames.SetActive(false);
                break;
            
            case PageType.Front:
                if (pageNum is >= AlbumFramePageNumMinimum and <= AlbumFramePageNumMaximum) {
                    frontPageEndingFrames.SetActive(true);
                    SetAlbumFrame(frontPageEndingFrameTop,
                        frontPageEndingFrameBottom,
                        frontPageEndingButtonTop,
                        frontPageEndingButtonBottom,
                        pageNum);
                } else 
                    frontPageEndingFrames.SetActive(false);
                break;
        }
    }

    private void SetAlbumFrame(
        Image topFrame,
        Image bottomFrame,
        Button topButton,
        Button bottomButton,
        int pageNum)
    {
        topButton.onClick.RemoveAllListeners();
        bottomButton.onClick.RemoveAllListeners();

        int accidyGender = (int)GameManager.Instance.GetVariable("AccidyGender");
        int pageIndex = pageNum - 2;
        int spriteBase = pageIndex * 4;
        int albumOffset = pageIndex * 2;

        bool isTopFrameCollected, isBottomFrameCollected;
        if (pageNum == 2) {
            isTopFrameCollected = (int)GameManager.Instance.GetVariable("BadACollect") > 0;
            isBottomFrameCollected = (int)GameManager.Instance.GetVariable("BadBCollect") > 0;
        } else {
            isTopFrameCollected = (int)GameManager.Instance.GetVariable("TrueCollect") > 0;
            isBottomFrameCollected = (int)GameManager.Instance.GetVariable("HiddenCollect") > 0;
        }

        // top
        topFrame.gameObject.SetActive(isTopFrameCollected);
        topFrame.sprite = endingSprites[spriteBase + accidyGender];
        topButton.onClick.AddListener(() =>
            UIManager.Instance.OpenAlbumPage(albumOffset)
        );

        // bottom
        bottomFrame.gameObject.SetActive(isBottomFrameCollected);
        bottomFrame.sprite = endingSprites[spriteBase + 2 + accidyGender];
        bottomButton.onClick.AddListener(() =>
            UIManager.Instance.OpenAlbumPage(albumOffset + 1)
        );
    }

    private void SetPageText(PageType pageType, string text, int pageNum)
    {
        switch (pageType)
        {
            case PageType.Left:
                leftPage.text = text;
                leftPageNum.text = pageNum == 0 ? "" : pageNum.ToString();
                break;

            case PageType.Right:
                rightPage.text = text;
                rightPageNum.text = pageNum.ToString();
                break;

            case PageType.Back:
                backPage.text = text;
                backPageNum.text = pageNum.ToString();
                break;

            case PageType.Front:
                frontPage.text = text;
                frontPageNum.text = pageNum.ToString();
                break;
        }
    }
    
    private void SetPageImage(PageType pageType, string imageID) {
        string path = "Room/Diary/doodles/" + imageID;
        var sprite = Resources.Load<Sprite>(path);
        var imageAlpha = imageID == "" ? 0 : 1;
        
        switch (pageType)
        {
            case PageType.Left:
                leftPageImage.sprite = sprite ? sprite : null;
                leftPageImage.color = new Color(1, 1, 1, imageAlpha);
                break;

            case PageType.Right:
                rightPageImage.sprite = sprite ? sprite : null;
                rightPageImage.color = new Color(1, 1, 1, imageAlpha);
                break;

            case PageType.Back:
                backPageImage.sprite = sprite ? sprite : null;
                backPageImage.color = new Color(1, 1, 1, imageAlpha);
                break;

            case PageType.Front:
                frontPageImage.sprite = sprite ? sprite : null;
                frontPageImage.color = new Color(1, 1, 1, imageAlpha);
                break;
        }
    }

    private string GetDiaryID(int pageNum)
    {
        return diaryType + "_" + pageNum.ToString().PadLeft(3, '0');
    }

    public string GetDiaryType()
    {
        return diaryType;
    }

    private Dictionary<string, (string, string)> GetCurrentPagesDictionary()
    {
        switch (diaryType)
        {
            case "Diary1":
                return diary1Pages;
            case "Diary2":
                return diary2Pages;
            case "Room2Book":
                return room2BookPages;
            case "DreamDiary":
                return dreamDiaryPages;
            case "Album":
                return albumPages;
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

        if ((bool)GameManager.Instance.GetVariable("Diary2PasswordCorrect") && GetDiaryType() == "Diary2")
        {
            GameManager.Instance.SetVariable("Diary2PresentPageNumber", presentPageNum);
            EventManager.Instance.CallEvent("EventDiary2Content");
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
        doodlesOrder = GameManager.Instance.GetVariable("DoodlesOrder") as string;

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

            var script = DialogueManager.Instance.scripts[scriptID].GetProcessedScript().ProcessedText;
            Dictionary<string, (string, string)> targetDictionary = null;

            if (diaryPageID.StartsWith("Diary1_")) targetDictionary = diary1Pages;
            else if (diaryPageID.StartsWith("Diary2_")) targetDictionary = diary2Pages;
            else if (diaryPageID.StartsWith("Room2Book_")) targetDictionary = room2BookPages;
            else if (diaryPageID.StartsWith("DreamDiary_")) targetDictionary = dreamDiaryPages;
            else if (diaryPageID.StartsWith("Album_")) targetDictionary = albumPages;
            else
            {
                Debug.LogWarning($"Unknown diary page ID format: {diaryPageID}");
                continue;
            }

            // add doodles
            bool isDoodle = fields[3].Trim() == "TRUE";
            string doodleID = "";
            if (isDoodle)
                doodleID = $"doodling{doodlesOrder[replayCount - 1]}_2";

            if (targetDictionary.ContainsKey(diaryPageID))
            {
                var currentString = targetDictionary[diaryPageID].Item1;
                currentString += "\n\n" + script;
                targetDictionary[diaryPageID] = (currentString, doodleID);
            }
            else targetDictionary.Add(diaryPageID, (script, doodleID));
        }

        // Set totalPageCount based on the current scene's dictionary size
        totalPageCount = GetCurrentPagesDictionary()?.Count ?? 0;
    }
    
    public void SetDoodlesOrder(int doodlesCount=8)
    {
        replayCount = (int)GameManager.Instance.GetVariable("ReplayCount");
        if (replayCount != 1)
        {
            doodlesOrder = (string)GameManager.Instance.GetVariable("DoodlesOrder");
            return;
        }
        
        bool[] visited = new bool[doodlesCount + 1];
        for (int i = 1; i <= doodlesCount; i++)
            visited[i] = false;
        
        for (int i = 1; i <= doodlesCount; i++)
        {
            int randomIndex = UnityEngine.Random.Range(1, doodlesCount + 1);
            while (visited[randomIndex])
                randomIndex = UnityEngine.Random.Range(1, doodlesCount + 1);
            
            visited[randomIndex] = true;
            doodlesOrder += randomIndex.ToString();
        }
        
        GameManager.Instance.SetVariable("DoodlesOrder", doodlesOrder);
        
        Debug.Log(doodlesOrder);
    }
}
