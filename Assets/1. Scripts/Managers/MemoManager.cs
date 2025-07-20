using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Constants;

public class MemoManager : PageContentsManager
{
    [SerializeField] private GameObject memoContents;
    [SerializeField] private PageFlip memoPages;
    [SerializeField] private GameObject memoButton;
    [SerializeField] private List<Button> flags;
    [SerializeField] private GameObject notificationImage;
    [SerializeField] private GameObject leftButtonNotificationImage;
    [SerializeField] private GameObject rightButtonNotificationImage;
    public bool fade = true;
    public FloatDirection floatDirection = FloatDirection.Up;

    public static MemoManager Instance { get; private set; }
    public bool isMemoOpen;
    public bool isFollow;
    private bool shouldHideMemoButton;
    public void SetShouldHideMemoButton(bool value)
    {
        shouldHideMemoButton = value;
    }

    // 모든 메모
    private readonly Dictionary<string, string> memoScripts = new Dictionary<string, string>();  // memoScripts[memoID] = scriptID

    // 저장된 메모
    // create a list of lists to store the memos
    public List<List<string[]>> SavedMemoList { get; set; } = new List<List<string[]>>(); // SavedMemoList[sceneIndex][memoIndex] = [memoID, memoContent]
    public List<List<string>> RevealedMemoList { get; set; } = new List<List<string>>();  // RevealedMemoList[sceneIndex][memoIndex] = memoID
    private readonly List<int> unseenMemoPages = new List<int>(); // List to track page numbers of unseen memos

    // 메모 게이지
    private GameObject memoGauge;
    private Image gaugeImage;
    private Slider clearFlagSlider;
    private Image clearFlagImage;
    private readonly Color unclearColor = new Color(0.5f, 0.5f, 0.5f);
    private readonly Color clearColor = new Color(1, 0.792f, 0.259f);

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
        int currentSceneIndex = SceneManager.Instance.GetActiveScene().ToInt();
        for (int i = 0; i < currentSceneIndex; i++)
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

                    GameManager.Instance.IncrementVariable($"MemoCount_{SceneManager.Instance.GetActiveScene()}");
                    ChangeMemoGauge();
                }
                break;
            }
        }
        
        UpdateNotification();
    }

    public void SetMemoButtons(bool showMemoIcon, bool showMemoExitButton = false)
    {
        if (showMemoIcon && shouldHideMemoButton)
            return;

        UIManager.Instance.SetUI(eUIGameObjectName.MemoButton, showMemoIcon);
        UIManager.Instance.SetUI(eUIGameObjectName.ExitButton, showMemoExitButton);

        if (RoomManager.Instance && SceneManager.Instance.GetActiveScene() is SceneType.ROOM_1 or SceneType.ROOM_2)
            RoomManager.Instance.SetButtons();
    }

    public void SetMemoGauge(GameObject memoGaugeParent) 
    {
        switch (SceneManager.Instance.GetActiveScene())
        {
            case SceneType.START:
                break;
            
            case SceneType.ROOM_1:
            case SceneType.ROOM_2:
                GameObject gaugeImageGameObject = memoGaugeParent.transform.Find("Gauge Image").gameObject; 
                gaugeImage = gaugeImageGameObject.GetComponent<Image>();
                
                GameObject flagSliderGameObject = gaugeImageGameObject.transform.Find("Flag Slider").gameObject;
                clearFlagSlider = flagSliderGameObject.GetComponent<Slider>();
                
                GameObject handleSlideAreaGameObjectRoom = flagSliderGameObject.transform.Find("Handle Slide Area")
                    .gameObject;
                GameObject clearFlagBackgroundGameObject = handleSlideAreaGameObjectRoom.transform
                    .Find("Clear Flag Background").gameObject;
                clearFlagImage = clearFlagBackgroundGameObject.GetComponentInChildren<Image>();
                break;
            
            case SceneType.FOLLOW_1:
            case SceneType.FOLLOW_2:
                GameObject backgroundGameObject = memoGaugeParent.transform.Find("Background").gameObject;
                gaugeImage = backgroundGameObject.GetComponent<Image>();
                
                GameObject fillAreaGameObject = memoGaugeParent.transform.Find("Fill Area").gameObject;
                GameObject clearSliderGameObject = fillAreaGameObject.transform.Find("Clear Slider").gameObject;
                clearFlagSlider = clearSliderGameObject.GetComponent<Slider>();
                
                GameObject handleSlideAreaGameObjectFollow = clearSliderGameObject.transform.Find("Handle Slide Area")
                    .gameObject;
                GameObject handleGameObject = handleSlideAreaGameObjectFollow.transform.Find("Handle").gameObject;
                clearFlagImage = handleGameObject.GetComponent<Image>();
                break;
        }
        ChangeMemoGauge();
    }
    
    public void ShowMemoGauge(bool show)
    {
        memoGauge.SetActive(show);
    }

    private void ChangeMemoGauge()
    {
        int currentSceneIndex = SceneManager.Instance.GetActiveScene().ToInt();
        int previousSceneIndex = currentSceneIndex - 1;
        if (currentSceneIndex is (int)SceneType.START or (int)SceneType.ENDING) 
            return;

        int cutLine = (int)GameManager.Instance.GetVariable($"CutLine_{currentSceneIndex.ToEnum()}");
        int currentMemoCount = (int)GameManager.Instance.GetVariable($"MemoCount_{currentSceneIndex.ToEnum()}");

        gaugeImage.fillAmount = (float)currentMemoCount / SavedMemoList[previousSceneIndex].Count;
        clearFlagSlider.value = (float)cutLine / SavedMemoList[previousSceneIndex].Count;

        clearFlagImage.color = currentMemoCount < cutLine ? unclearColor : clearColor;
    }

    public void SetMemoContents(bool isActive)
    {
        flipLeftButton.SetActive(false);
        flipRightButton.SetActive(false);
         
        UIManager.Instance.SetUI(eUIGameObjectName.MemoContents, isActive, fade, floatDirection);
        isMemoOpen = isActive;

        if (isActive) {
            memoPages.totalPageCount = GetAggregatedMemos().Count;
            float delay = UIManager.Instance.fadeAnimationDuration;
            StartCoroutine(DisplayPagesAfterDelay(memoPages.currentPage, delay));
        }
    }
    
    private System.Collections.IEnumerator DisplayPagesAfterDelay(int page, float delay)
    {
        yield return new WaitForSeconds(delay);
         
        DisplayPagesStatic(page);
    }

    public void SetMemoCurrentPageAndFlags()
    {
        SetMemoCurrentPage();
        SetFlags();
    }
    
    private void SetMemoCurrentPage()
    {
        var previousSceneIndex = SceneManager.Instance.GetActiveScene().ToInt() - 1;
        int startingPageIndex = CalculateFirstPageNumber(previousSceneIndex); // Calculate the starting page index for the current scene
        if (startingPageIndex % 2 != 0) 
            startingPageIndex -= 1; // Ensure the page number is even
        
        memoPages.currentPage = startingPageIndex;  // Set the current page to the starting page of the current scene
    }

    private void SetFlags()
    {
        var currentSceneIndex = SceneManager.Instance.GetActiveScene().ToInt();
        for (var i = 0; i < currentSceneIndex; i++) 
            flags[i].gameObject.SetActive(true);
        for (var i = currentSceneIndex; i < flags.Count; i++) 
            flags[i].gameObject.SetActive(false);
    }
    
    private int CalculateFirstPageNumber(int sceneIndex)
    {
        int startingPageIndex = 1; // Start from page 1
        for (int i = 0; i < sceneIndex; i++) 
            startingPageIndex += SavedMemoList[i].Count;

        return startingPageIndex;
    }

    private List<string[]> GetAggregatedMemos()
    {
        var allMemos = new List<string[]>();
        int currentSceneIndex = SceneManager.Instance.GetActiveScene().ToInt();
        for (int i = 0; i < currentSceneIndex; i++) 
            allMemos.AddRange(SavedMemoList[i]);

        return allMemos;
    }
    
    public override void DisplayPage(PageType pageType, int pageNum)
    {
        var allMemos = GetAggregatedMemos();
        string memoText = "";
        if (pageNum > 0 && pageNum <= allMemos.Count)
            memoText = allMemos[pageNum - 1][1];
        switch (pageType)
        {
            case PageType.Left:
                leftPage.text = memoText;
                leftPageNum.text = pageNum == 0 ? "" : pageNum.ToString();
                break;
            
            case PageType.Right:
                rightPage.text = memoText;
                rightPageNum.text = pageNum > allMemos.Count ? "" : pageNum.ToString();
                break;
            
            case PageType.Back:
                backPage.text = memoText;
                backPageNum.text = pageNum.ToString();
                break;
            
            case PageType.Front:
                frontPage.text = memoText;
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
                RevealedMemoList[2].Add(i.ToString());

            for (int i = 0; i < 10; i++)
                RevealedMemoList[SceneManager.Instance.GetActiveScene().ToInt() - 1].Add(i.ToString());
        }
        SceneManager.Instance.LoadScene(SceneType.ENDING);
    }
}
