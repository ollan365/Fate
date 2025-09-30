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
    public void SetShouldHideMemoButton(bool value) {
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
    private readonly Color unclearColor = new Color(0.4f, 0.4f, 0.4f);
    private readonly Color clearColor = new Color(1, 0.7f, 0);

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
        
        // Initialize unseen memo pages based on current revealed memos
        RebuildUnseenMemoPages();
        
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
    
    public void RebuildUnseenMemoPages(){
        unseenMemoPages.Clear();
        
        for (int sceneIndex = 0; sceneIndex < RevealedMemoList.Count; sceneIndex++){
            for (int memoIndex = 0; memoIndex < SavedMemoList[sceneIndex].Count; memoIndex++) {
                string memoID = SavedMemoList[sceneIndex][memoIndex][0];
                string scriptID = memoScripts[memoID];
                
                if (RevealedMemoList[sceneIndex].Contains(scriptID) && 
                    SavedMemoList[sceneIndex][memoIndex][1] != "가려진 메모") {
                    int pageNum = CalculateFirstPageNumber(sceneIndex) + memoIndex;
                    if (!unseenMemoPages.Contains(pageNum))
                        unseenMemoPages.Add(pageNum);
                }
            }
        }
        
        UpdateNotification();
    }
    
    public void RefreshGaugeAfterSaveLoad() {
        RebuildUnseenMemoPages();
        SynchronizeMemoCounts();
        StartCoroutine(DelayedChangeMemoGauge());
    }
    
    private void SynchronizeMemoCounts() {
        for (int sceneIndex = 0; sceneIndex < RevealedMemoList.Count; sceneIndex++) {
            int revealedCount = RevealedMemoList[sceneIndex].Count;
            string sceneName = ((SceneType)(sceneIndex + 1)).ToString();
            
            GameManager.Instance.SetVariable($"MemoCount_{sceneName}", revealedCount);
        }
    }
    
    public void ClearUnseenMemoPages() {
        unseenMemoPages.Clear();
        UpdateNotification();
    }
    
    public int GetUnseenMemoPagesCount() {
        return unseenMemoPages.Count;
    }
    
    public void ResetMemoSystem() {
        for (int i = 0; i < RevealedMemoList.Count; i++)
            RevealedMemoList[i].Clear();
        
        for (int i = 0; i < SavedMemoList.Count; i++)
            for (int j = 0; j < SavedMemoList[i].Count; j++)
                SavedMemoList[i][j][1] = "가려진 메모";
        
        ClearUnseenMemoPages();
        
        UpdateNotification();
    }

    public void OnExit()
    {
        SetMemoContents(false);
        SetMemoButtons(true);
        UIManager.Instance.ToggleHighlightAnimationEffect(eUIGameObjectName.MemoButton, false);

        if (FollowManager.Instance)
            FollowManager.Instance.EndScript();
    }

    // 메모 추가하기
    public void RevealMemo(string memoID)
    {
        if (string.IsNullOrEmpty(memoID) || !memoScripts.ContainsKey(memoID)) {
            Debug.LogWarning($"Invalid memo ID: {memoID}");
            return;
        }

        var scriptID = memoScripts[memoID];
        int currentSceneIndex = GameSceneManager.Instance.GetActiveScene().ToInt();
        bool memoRevealed = false;
        
        if (currentSceneIndex <= 0 || currentSceneIndex > RevealedMemoList.Count) {
            Debug.LogWarning($"Invalid current scene index: {currentSceneIndex}");
            return;
        }
        
        int memoSceneIndex = GetMemoSceneIndex(memoID);
        if (memoSceneIndex < 0 || memoSceneIndex >= RevealedMemoList.Count) {
            Debug.LogWarning($"Invalid memo scene index: {memoSceneIndex} for memo {memoID}");
            return;
        }
        
        if (RevealedMemoList[memoSceneIndex].Contains(scriptID))
            return;
        
        RevealedMemoList[memoSceneIndex].Add(scriptID);
        
        for (var j = 0; j < SavedMemoList[memoSceneIndex].Count; j++) {
            if (SavedMemoList[memoSceneIndex][j][0] != memoID) continue;
            
            string script = DialogueManager.Instance.scripts[scriptID].GetScript().ProcessedText;
            SavedMemoList[memoSceneIndex][j][1] = script;
            
            int pageNum = CalculateFirstPageNumber(memoSceneIndex) + j;
            if (!unseenMemoPages.Contains(pageNum)) {
                unseenMemoPages.Add(pageNum);

                GameManager.Instance.IncrementVariable($"MemoCount_{GameSceneManager.Instance.GetActiveScene()}");
                SoundPlayer.Instance.UISoundPlay(Sound_Memo_Clue);
                memoRevealed = true;
            }
            break;
        }
        
        if (memoRevealed)
            StartCoroutine(DelayedChangeMemoGauge());
        
        UpdateNotification();
    }
    
    private int GetMemoSceneIndex(string memoID) {
        if (string.IsNullOrEmpty(memoID) || memoID.Length < 2)
            return -1;
            
        var memoType = memoID.Substring(0, 2);
        switch (memoType) {
            case "R1": return 0;
            case "F1": return 1;
            case "R2": return 2;
            case "F2": return 3;
            default: return -1;
        }
    }

    public void SetMemoButtons(bool showMemoIcon, bool showMemoExitButton = false)
    {
        if (showMemoIcon && shouldHideMemoButton)
            return;

        UIManager.Instance.SetUI(eUIGameObjectName.MemoButton, showMemoIcon, showMemoIcon);
        UIManager.Instance.SetUI(eUIGameObjectName.ExitButton, showMemoExitButton);

        if (RoomManager.Instance && GameSceneManager.Instance.GetActiveScene() is SceneType.ROOM_1 or SceneType.ROOM_2)
            RoomManager.Instance.SetButtons();
    }

    public void SetMemoGauge(GameObject memoGaugeParent) 
    {
        if (!memoGaugeParent) {
            Debug.LogWarning("Memo gauge parent is null, cannot set up gauge");
            return;
        }

        memoGauge = memoGaugeParent;
        
        switch (GameSceneManager.Instance.GetActiveScene())
        {
            case SceneType.START:
                break;
            
            case SceneType.ROOM_1:
            case SceneType.ROOM_2:
                GameObject gaugeImageGameObject = memoGaugeParent.transform.Find("Gauge Image")?.gameObject;
                if (gaugeImageGameObject)
                    gaugeImage = gaugeImageGameObject.GetComponent<Image>();
                else {
                    Debug.LogError("Could not find Gauge Image in memo gauge parent");
                    return;
                }

                clearFlagSlider = memoGaugeParent.GetComponentInChildren<Slider>();
                if (!clearFlagSlider) {
                    Debug.LogError("Could not find Slider component in memo gauge parent");
                    return;
                }

                GameObject handleSlideAreaGameObjectRoom = clearFlagSlider.transform.Find("Handle Slide Area")?.gameObject;
                if (handleSlideAreaGameObjectRoom) {
                    GameObject handleGameObject = handleSlideAreaGameObjectRoom.transform.Find("Handle")?.gameObject;
                    if (handleGameObject)
                        clearFlagImage = handleGameObject.GetComponent<Image>();
                }
                break;
            
            case SceneType.FOLLOW_1:
            case SceneType.FOLLOW_2:
                GameObject backgroundGameObject = memoGaugeParent.transform.Find("Fill")?.gameObject;
                if (backgroundGameObject)
                    gaugeImage = backgroundGameObject.GetComponent<Image>();
                else {
                    Debug.LogError("Could not find Fill in memo gauge parent");
                    return;
                }

                clearFlagSlider = memoGaugeParent.GetComponentInChildren<Slider>();
                if (!clearFlagSlider) {
                    Debug.LogError("Could not find Slider component in memo gauge parent");
                    return;
                }
                
                GameObject handleSlideAreaGameObjectFollow = clearFlagSlider.transform.Find("Handle Slide Area")?.gameObject;
                if (handleSlideAreaGameObjectFollow) {
                    GameObject handleGameObject = handleSlideAreaGameObjectFollow.transform.Find("Handle")?.gameObject;
                    if (handleGameObject)
                        clearFlagImage = handleGameObject.GetComponent<Image>();
                }
                break;
        }
        
        StartCoroutine(DelayedChangeMemoGauge());
    }
    
    private System.Collections.IEnumerator DelayedChangeMemoGauge() {
        yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(0.1f);
        
        ChangeMemoGauge();
    }
    
    public void ShowMemoGauge(bool show) {
        if (memoGauge != null)
            memoGauge.SetActive(show);
    }
    
    public void ForceRefreshMemoGauge() {
        if (gaugeImage && clearFlagSlider && clearFlagImage)
            ChangeMemoGauge();
        else
            StartCoroutine(DelayedChangeMemoGauge());
    }
    
    private bool ValidateMemoData() {
        if (SavedMemoList == null || SavedMemoList.Count == 0) {
            Debug.LogWarning("SavedMemoList is null or empty");
            return false;
        }

        if (RevealedMemoList != null && RevealedMemoList.Count != 0) 
            return true;
        
        Debug.LogWarning("RevealedMemoList is null or empty");
        return false;
    }

    private void ChangeMemoGauge()
    {
        // Check if all required components are available
        if ((gaugeImage && clearFlagSlider && clearFlagImage) == false) {
            Debug.LogError("Memo gauge components not properly initialized, retrying...");
            StartCoroutine(DelayedChangeMemoGauge());
            return;
        }

        if (!ValidateMemoData()) {
            Debug.LogWarning("Memo data not properly loaded, retrying...");
            StartCoroutine(DelayedChangeMemoGauge());
            return;
        }

        int currentSceneIndex = GameSceneManager.Instance.GetActiveScene().ToInt();
        int previousSceneIndex = currentSceneIndex - 1;
        if (currentSceneIndex is (int)SceneType.START or (int)SceneType.ENDING) 
            return;

        if (previousSceneIndex < 0 || previousSceneIndex >= SavedMemoList.Count || SavedMemoList[previousSceneIndex].Count == 0) {
            Debug.LogWarning($"Invalid memo data for scene {currentSceneIndex}, previousSceneIndex: {previousSceneIndex}");
            return;
        }

        int cutLine = (int)GameManager.Instance.GetVariable($"CutLine_{currentSceneIndex.ToEnum()}");
        int currentMemoCount = (int)GameManager.Instance.GetVariable($"MemoCount_{currentSceneIndex.ToEnum()}");

        float gaugeTargetValue = Mathf.Clamp01((float)currentMemoCount / SavedMemoList[previousSceneIndex].Count);
        StartCoroutine(AnimateGaugeChanging(gaugeTargetValue));

        clearFlagSlider.value = (float)cutLine / SavedMemoList[previousSceneIndex].Count;
        clearFlagImage.color = currentMemoCount < cutLine ? unclearColor : clearColor;
    }

    public void SetMemoContents(bool isActive)
    {
        flipLeftButton.SetActive(false);
        flipRightButton.SetActive(false);
         
        UIManager.Instance.SetUI(eUIGameObjectName.MemoContents, isActive, fade, floatDirection);
        UIManager.Instance.SetUI(eUIGameObjectName.BlurImage,
            RoomManager.Instance.imageAndLockPanelManager.GetIsImageOrLockPanelActive() || isActive, true);
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
        var previousSceneIndex = GameSceneManager.Instance.GetActiveScene().ToInt() - 1;
        int startingPageIndex = CalculateFirstPageNumber(previousSceneIndex); // Calculate the starting page index for the current scene
        if (startingPageIndex % 2 != 0) 
            startingPageIndex -= 1; // Ensure the page number is even
        memoPages.currentPage = startingPageIndex;  // Set the current page to the starting page of the current scene

        var currentSceneIndex = GameSceneManager.Instance.GetActiveScene().ToInt();
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
        int currentSceneIndex = GameSceneManager.Instance.GetActiveScene().ToInt();
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
        }
    }

    // 게이지 증가 애니메이션 코루틴
    private System.Collections.IEnumerator AnimateGaugeChanging(float targetValue) {
        if (!gaugeImage) {
            Debug.LogWarning("Gauge image is null, cannot animate gauge");
            yield break;
        }

        float duration = 0.5f; // 애니메이션 지속 시간 (초)
        float elapsed = 0f;
        float startValue = gaugeImage.fillAmount;
        while (elapsed < duration)
        {
            if (!gaugeImage) {
                Debug.LogWarning("Gauge image became null during animation");
                yield break;
            }

            elapsed += Time.deltaTime;
            gaugeImage.fillAmount = Mathf.Lerp(startValue, targetValue, elapsed / duration);
            yield return null;
        }

        if (gaugeImage)
            gaugeImage.fillAmount = targetValue; // 마지막에 정확히 맞춰줌
    }

    private void UpdateNotification() {
        if (notificationImage == null || leftButtonNotificationImage == null || rightButtonNotificationImage == null) {
            Debug.LogWarning("Notification components not initialized");
            return;
        }
        
        if (memoPages == null) {
            Debug.LogWarning("Memo pages not initialized");
            return;
        }

        notificationImage.SetActive(unseenMemoPages.Count > 0);

        int currentPage = memoPages.currentPage;
        bool hasUnreadLeft = unseenMemoPages.Exists(pageNum => pageNum < currentPage);
        bool hasUnreadRight = unseenMemoPages.Exists(pageNum => pageNum > currentPage + 1);
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
                RevealedMemoList[GameSceneManager.Instance.GetActiveScene().ToInt() - 1].Add(i.ToString());
        }
        GameSceneManager.Instance.LoadScene(SceneType.ENDING);
    }
}
