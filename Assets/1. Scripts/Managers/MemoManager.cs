using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static Constants;

public class MemoManager : PageContentsManager
{
    [SerializeField] private GameObject memoContents;
    [SerializeField] private PageFlip memoPages;
    [SerializeField] private GameObject memoButton;
    [SerializeField] public GameObject exitButton;
    [SerializeField] private List<Button> flags;
    [SerializeField] private GameObject notificationImage;
    [SerializeField] private GameObject leftButtonNotificationImage;
    [SerializeField] private GameObject rightButtonNotificationImage;

    [Header("Page Num Text")]
    [SerializeField] private TextMeshProUGUI leftPageNum;
    [SerializeField] private TextMeshProUGUI rightPageNum;
    [SerializeField] private TextMeshProUGUI frontPageNum;
    [SerializeField] private TextMeshProUGUI backPageNum;
    
    public static MemoManager Instance { get; private set; }
    public bool isMemoOpen = false;
    public bool isFollow = false;
    private bool wantToHideMemoButton = false;
    public bool HideMemoButton { set => wantToHideMemoButton = value; }

    // 모든 메모
    private readonly Dictionary<string, string> memoScripts = new Dictionary<string, string>();  // memoScripts[memoID] = scriptID

    // 저장된 메모
    // create a list of lists to store the memos
    public List<List<string[]>> SavedMemoList { get; set; } = new List<List<string[]>>(); // SavedMemoList[sceneIndex][memoIndex] = [memoID, memoContent]
    public List<List<string>> RevealedMemoList { get; set; } = new List<List<string>>();  // RevealedMemoList[sceneIndex][memoIndex] = memoID
    private List<int> unseenMemoPages = new List<int>(); // List to track page numbers of unseen memos

    // 메모 게이지
    private GameObject memoGauge;
    private Image gaugeImage;
    private Slider clearFlagSlider;
    private Image clearFlageImage;
    private Color unclearColor = new Color(0.5f, 0.5f, 0.5f);
    private Color clearColor = new Color(1, 0.792f, 0.259f);

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);

        ParsePageContents();

        // Initialize the lists with the correct size
        for (var i = 0; i < 4; i++)
        {
            SavedMemoList.Add(new List<string[]>());
            RevealedMemoList.Add(new List<string>());
        }

        LoadMemos();
        UpdateNotification();
    }
    
    // memos.csv 파일 파싱
    public override void ParsePageContents()
    {
        var memoCsv = Resources.Load<TextAsset>("Datas/memos");
        var lines = memoCsv.text.Split('\n');
        for (var i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i]))
                continue;

            var fields = lines[i].Split(',');

            var memoID = fields[0].Trim();
            var scriptID = fields[2].Trim();
            
            memoScripts[memoID] = scriptID;
        }
    }

    private void LoadMemos()
    {
        foreach (var memoID in memoScripts.Keys)
        {
            var memoType = memoID.Substring(0, 2);
            var scriptID = memoScripts[memoID];
            var sceneIndex = 0;

            switch (memoType)
            {
                case "R1":
                    sceneIndex = 0;
                    break;
                case "F1":
                    sceneIndex = 1;
                    break;
                case "R2":
                    sceneIndex = 2;
                    break;
                case "F2":
                    sceneIndex = 3;
                    break;
            }
            
            string[] memo = { memoID, "가려진 메모" };
            SavedMemoList[sceneIndex].Add(memo);
        }
    }

    public void OnExit()
    {
        SetMemoContents(false);
        SetMemoButtons(true);

        if (FollowManager.Instance)
            FollowManager.Instance.EndScript();
    }

    // 메모 추가하기
    public void RevealMemo(string memoID)
    {
        var scriptID = memoScripts[memoID];
        int currentSceneIndex = (int)GameManager.Instance.GetVariable("CurrentScene") - 1;
        
        for (int i = 0; i <= currentSceneIndex; i++)
        {
            if (RevealedMemoList[i].Contains(scriptID)) 
                continue;
            
            RevealedMemoList[i].Add(scriptID);
            for (var j = 0; j < SavedMemoList[i].Count; j++)
            {
                if (SavedMemoList[i][j][0] != memoID) continue;
                string script = DialogueManager.Instance.scripts[scriptID].GetScript();
                SavedMemoList[i][j][1] = script;
                
                // Calculate the page number and add it to the unseen memo pages
                int pageNum = CalculateFirstPageNumber(i) + j;
                if (!unseenMemoPages.Contains(pageNum))
                {
                    unseenMemoPages.Add(pageNum);
                    // Debug.Log($"Added page {pageNum} to unseen memo pages");

                    GameManager.Instance.IncrementVariable($"MemoCount_{((int)GameManager.Instance.GetVariable("CurrentScene")).ToEnum()}");
                    ChangeMemoGauge();
                }
                break;
            }
        }
        
        UpdateNotification();
    }

    public void SetMemoButtons(bool showMemoIcon, bool showMemoExitButton = false)
    {
        if (showMemoIcon && wantToHideMemoButton)
            return;

        memoButton.SetActive(showMemoIcon);
        exitButton.SetActive(showMemoExitButton);

        var currentSceneIndex = (int)GameManager.Instance.GetVariable("CurrentScene");
        if (RoomManager.Instance && currentSceneIndex is 1 or 3)
            RoomManager.Instance.SetButtons();
    }

    public void SetMemoGauge(GameObject memoGauge, Image gaugeImage, Slider clearFlagSlider, Image clearFlageImage)
    {
        this.memoGauge = memoGauge;
        this.gaugeImage = gaugeImage;
        this.clearFlagSlider = clearFlagSlider;
        this.clearFlageImage = clearFlageImage;

        ChangeMemoGauge();
    }
    
    public void ShowMemoGauge(bool show)
    {
        memoGauge.SetActive(show);
    }
    
    public void ChangeMemoGauge()
    {
        // memoGauge.SetActive(showMemoGauge);

        int currentSceneIndex = (int)GameManager.Instance.GetVariable("CurrentScene");

        if (currentSceneIndex is 0 or 5) return;

        int cutLine = (int)GameManager.Instance.GetVariable($"CutLine_{currentSceneIndex.ToEnum()}");
        int currentMemoCount = (int)GameManager.Instance.GetVariable($"MemoCount_{currentSceneIndex.ToEnum()}");

        gaugeImage.fillAmount = (float)currentMemoCount / SavedMemoList[currentSceneIndex - 1].Count;
        clearFlagSlider.value = (float)cutLine / SavedMemoList[currentSceneIndex - 1].Count;

        if (currentMemoCount < cutLine) clearFlageImage.color = unclearColor;
        else clearFlageImage.color = clearColor;
    }

    public void SetMemoContents(bool isActive)
    {
        UIManager.Instance.SetUI("MemoContents", isActive);
        isMemoOpen = isActive;

        if (isActive) {
            memoPages.totalPageCount = GetAggregatedMemos().Count;
            DisplayPagesStatic(memoPages.currentPage);
        }
    }

    public void SetMemoCurrentPageAndFlags()
    {
        SetMemoCurrentPage();
        SetFlags();
    }
    
    private void SetMemoCurrentPage()
    {
        var currentSceneIndex = (int)GameManager.Instance.GetVariable("CurrentScene") - 1;

        // Calculate the starting page index for the current scene
        int startingPageIndex = CalculateFirstPageNumber(currentSceneIndex);
        if (startingPageIndex % 2 != 0) startingPageIndex -= 1; // Ensure the page number is even
        
        memoPages.currentPage = startingPageIndex;  // Set the current page to the starting page of the current scene
    }

    private void SetFlags()
    {
        var currentSceneIndex = (int)GameManager.Instance.GetVariable("CurrentScene") - 1;

        for (var i = 0; i <= currentSceneIndex; i++) flags[i].gameObject.SetActive(true);
        for (var i = currentSceneIndex + 1; i < flags.Count; i++) flags[i].gameObject.SetActive(false);
    }
    
    private int CalculateFirstPageNumber(int sceneIndex)
    {
        int startingPageIndex = 1; // Start from page 1
        for (int i = 0; i < sceneIndex; i++) startingPageIndex += SavedMemoList[i].Count;

        // if (startingPageIndex % 2 != 0) startingPageIndex -= 1; // Ensure the page number is even

        return startingPageIndex;
    }

    private List<string[]> GetAggregatedMemos()
    {
        var currentSceneIndex = (int)GameManager.Instance.GetVariable("CurrentScene") - 1;
        var allMemos = new List<string[]>();

        for (int i = 0; i <= currentSceneIndex; i++) allMemos.AddRange(SavedMemoList[i]);

        return allMemos;
    }
    
    public override void DisplayPage(PageType pageType, int pageNum)
    {
        var allMemos = GetAggregatedMemos();
        
        switch (pageType)
        {
            case PageType.Left:
                leftPage.text = pageNum == 0 ? "" : allMemos[pageNum - 1][1];
                leftPageNum.text = pageNum == 0 ? "" : pageNum.ToString();
                break;
            
            case PageType.Right:
                rightPage.text = pageNum > allMemos.Count ? "" : allMemos[pageNum - 1][1];
                rightPageNum.text = pageNum > allMemos.Count ? "" : pageNum.ToString();
                break;
            
            case PageType.Back:
                backPage.text = allMemos[pageNum - 1][1];
                backPageNum.text = pageNum.ToString();
                break;
            
            case PageType.Front:
                frontPage.text = allMemos[pageNum - 1][1];
                frontPageNum.text = pageNum.ToString();
                break;
        }
    }
    
    public override void DisplayPagesDynamic(int currentPage)
    {
        MarkPageAsRead(currentPage);
        MarkPageAsRead(currentPage + 1);
        DisplayPage(PageType.Left, currentPage);
        DisplayPage(PageType.Right, currentPage + 3);
        DisplayPage(PageType.Back, currentPage + 1);
        DisplayPage(PageType.Front, currentPage + 2);
    }
    
    public override void DisplayPagesStatic(int currentPage)
    {
        MarkPageAsRead(currentPage);
        MarkPageAsRead(currentPage + 1);
        DisplayPage(PageType.Left, currentPage);
        DisplayPage(PageType.Right, currentPage + 1);
        
        flipLeftButton.SetActive(currentPage > 0);
        
        var allMemos = GetAggregatedMemos();
        flipRightButton.SetActive(currentPage < allMemos.Count - 1);
        UpdateNotification();
    }

    private void MarkPageAsRead(int pageNum)
    {
        if (unseenMemoPages.Contains(pageNum))
        {
            unseenMemoPages.Remove(pageNum);
            UpdateNotification();
            
            // Debug.Log($"Marked page {pageNum} as read");
        }
    }

    private void UpdateNotification()
    {
        notificationImage.SetActive(unseenMemoPages.Count > 0);

        int currentPage = memoPages.currentPage;

        // foreach (var pageNum in unseenMemoPages) Debug.Log($"Unseen Memo Page: {pageNum}");
        
        bool hasUnreadLeft = unseenMemoPages.Exists(pageNum => pageNum < currentPage);
        bool hasUnreadRight = unseenMemoPages.Exists(pageNum => pageNum > currentPage + 1);

        // Debug.Log($"Current Page: {currentPage}, Has Unread Left: {hasUnreadLeft}, Has Unread Right: {hasUnreadRight}");
        
        leftButtonNotificationImage.SetActive(hasUnreadLeft);
        rightButtonNotificationImage.SetActive(hasUnreadRight);
    }

    // 엔딩 테스트를 위한 임시 함수들
    public void TestEnding(bool unlock)
    {
        if (unlock) // 메모를 일정 개수 이상 모았을 때
        {
            for (int i = 0; i < 10; i++)
            {
                RevealedMemoList[2].Add(i.ToString());
            }

            for (int i = 0; i < 10; i++)
            {
                RevealedMemoList[(int)GameManager.Instance.GetVariable("CurrentScene") - 1].Add(i.ToString());
            }
            SceneManager.Instance.LoadScene(SceneType.ENDING);
        }
        else
        {
            SceneManager.Instance.LoadScene(SceneType.ENDING);
        }
    }
}
