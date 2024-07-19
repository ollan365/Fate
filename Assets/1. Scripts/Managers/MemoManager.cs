using System;
using System.Collections.Generic;
using UnityEngine;
using static Constants;

public class MemoManager : PageContentsManager
{
    [SerializeField] private GameObject memoContents;
    [SerializeField] private PageFlip memoPages;
    [SerializeField] private GameObject memoButton;
    [SerializeField] private GameObject exitButton;
    
    public static MemoManager Instance { get; private set; }
    public bool isMemoOpen = false;
    public bool isFollow;  // 현재 미행 씬인지 방탈출 씬인지

    // 모든 메모
    private readonly Dictionary<string, string> memoScripts = new Dictionary<string, string>();  // memoScripts[memoID] = scriptID

    // 저장된 메모
    // create a list of lists to store the memos
    public List<List<string[]>> SavedMemoList { get; set; } = new List<List<string[]>>(); // SavedMemoList[sceneIndex][memoIndex] = [memoID, memoContent]

    public List<List<string>> RevealedMemoList { get; set; } = new List<List<string>>();  // RevealedMemoList[sceneIndex][memoIndex] = memoID

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            ParsePageContents();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        // Initialize the lists with the correct size
        for (var i = 0; i < 4; i++)
        {
            SavedMemoList.Add(new List<string[]>());
            RevealedMemoList.Add(new List<string>());
        }

        var currentSceneIndex = GetCurrentSceneIndex();
        
        LoadMemos();
    }
    
    // memos.csv 파일 파싱
    public override void ParsePageContents()
    {
        var memoCsv = Resources.Load<TextAsset>("Datas/memos");
        var lines = memoCsv.text.Split('\n');
        for (var i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;

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
            
            string[] memo = {memoID, "가려진 메모"};
            SavedMemoList[sceneIndex].Add(memo);
        }
    }

    public void OnExit()
    {
        SetMemoContents(false);

        if (FollowManager.Instance) FollowManager.Instance.EndScript(false);
    }

    // 현재 씬에 따라 메모의 개수를 파악 -> 현재 씬에 해당하는 메모의 개수에 따라 엔딩 선택지 해금 여부 결정
    public bool UnlockNextScene(bool room2 = false)
    {
        if (room2) return RevealedMemoList[2].Count >= 8;

        switch (SceneManager.Instance.CurrentScene)
        {
            case SceneType.ROOM_1:
                return RevealedMemoList[0].Count > 8;
            case SceneType.FOLLOW_1:
                return RevealedMemoList[1].Count >= 8;
            case SceneType.FOLLOW_2:
                return RevealedMemoList[3].Count >= 8;
            default: return false;
        }
    }

    // 메모 추가하기
    public void RevealMemo(string memoID)
    {
        // Debug.Log(memoID);
        
        var scriptID = memoScripts[memoID];

        // 현재 씬에 따라 메모가 저장되는 곳이 달라짐
        int currentSceneIndex = GetCurrentSceneIndex();
        
        // 이미 저장된 메모면 return
        if (RevealedMemoList[currentSceneIndex].Contains(scriptID)) return;
        
        RevealedMemoList[currentSceneIndex].Add(scriptID);
        
        for (var i = 0; i < SavedMemoList[currentSceneIndex].Count; i++)
        {
            if (SavedMemoList[currentSceneIndex][i][0] != memoID) continue;
            string script = DialogueManager.Instance.scripts[scriptID].GetScript();
            SavedMemoList[currentSceneIndex][i][1] = script;
            break;
        }
        
        // Debug.Log($"Revealed {scriptID}");
    }

    private static int GetCurrentSceneIndex()
    {
        var index = 0;
        switch (SceneManager.Instance.CurrentScene)
        {
            case SceneType.ROOM_1: index = 0; break;
            case SceneType.FOLLOW_1: index = 1; break;
            case SceneType.ROOM_2: index = 2; break;
            case SceneType.FOLLOW_2: index = 3; break;
        }

        return index;
    }
    
    public void SetMemoButtons(bool showMemoIcon, bool showMemoExitButton=false)
    {
        // Debug.Log($"showMemoIcon: {showMemoIcon}, showMemoExitButton: {showMemoExitButton}");
        memoButton.SetActive(showMemoIcon);
        exitButton.SetActive(showMemoExitButton);

        var currentSceneIndex = GetCurrentSceneIndex();
        if (RoomManager.Instance && currentSceneIndex is 0 or 2) RoomManager.Instance.SetButtons();
    }
    
    public void SetMemoContents(bool isTrue)
    {
        memoContents.SetActive(isTrue);
        isMemoOpen = isTrue;
        SetMemoButtons(!isTrue, isTrue);

        if (!isTrue) return;
        
        var currentSceneIndex = GetCurrentSceneIndex();
        memoPages.totalPageCount = SavedMemoList[currentSceneIndex].Count;
        var currentPage = memoPages.currentPage;
        DisplayPagesStatic(currentPage);

        if (currentSceneIndex is 0 or 2) RoomManager.Instance.SetButtons();  // 방탈출 씬인 경우 버튼 설정 필요
    }
    
    public override void DisplayPage(PageType pageType, int pageNum)
    {
        var currentSceneIndex = GetCurrentSceneIndex();
        
        switch (pageType)
        {
            case PageType.Left:
                leftPage.text = pageNum == 0 ? "" : SavedMemoList[currentSceneIndex][pageNum - 1][1];
                break;
            
            case PageType.Right:
                rightPage.text = pageNum > SavedMemoList[currentSceneIndex].Count ? "" : SavedMemoList[currentSceneIndex][pageNum - 1][1];
                break;
            
            case PageType.Back:
                backPage.text = SavedMemoList[currentSceneIndex][pageNum - 1][1];
                break;
            
            case PageType.Front:
                frontPage.text = SavedMemoList[currentSceneIndex][pageNum - 1][1];
                break;
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
        
        int currentSceneIndex = GetCurrentSceneIndex();
        flipRightButton.SetActive(currentPage < SavedMemoList[currentSceneIndex].Count - 1);
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
                RevealedMemoList[GetCurrentSceneIndex()].Add(i.ToString());
            }
            SceneManager.Instance.LoadScene(SceneType.ENDING);
        }
        else
        {
            SceneManager.Instance.LoadScene(SceneType.ENDING);
        }
    }
}
