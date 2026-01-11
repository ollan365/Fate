using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Constants;

public class GameSceneManager : MonoBehaviour
{
    public static GameSceneManager Instance { get; private set; }
    
    public bool IsSceneChanging { get; private set; }
    
    private bool shouldInitializeLobbyOnTitleLoad;
    
    void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else
            Destroy(gameObject);
        
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    
    void OnDestroy() {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    
    private void OnSceneLoaded(Scene s, LoadSceneMode m) {
        IsSceneChanging = false;
        if (UIManager.Instance && UIManager.Instance.progressBar)
            UIManager.Instance.progressBar.fillAmount = 1f;

        ChangeSceneEffect();
    }
    
    private void PrepareCurrentSceneForTitle()
    {
        if (DialogueManager.Instance && DialogueManager.Instance.isDialogueActive)
            DialogueManager.Instance.ForceHideDialogueForSceneChange();

        if (MemoManager.Instance && MemoManager.Instance.isMemoOpen)
            MemoManager.Instance.OnExit();

        SceneType currentScene = GetActiveScene();
        if (currentScene is SceneType.ROOM_1 or SceneType.ROOM_2)
        {
            if (RoomManager.Instance)
                RoomManager.Instance.ExitToRoot();

            if (UIManager.Instance)
            {
                UIManager.Instance.SetUI(eUIGameObjectName.ActionPoints, false);
                UIManager.Instance.SetUI(eUIGameObjectName.MemoGauge, false);
                UIManager.Instance.SetUI(eUIGameObjectName.TutorialBlockingPanel, false);
                UIManager.Instance.SetCursorAuto();
            }
        }
        else if (currentScene is SceneType.FOLLOW_1 or SceneType.FOLLOW_2)
        {
            if (UIManager.Instance)
                UIManager.Instance.SetUI(eUIGameObjectName.FollowUI, false);
        }
    }
    
    public void GoTitle() {
        UIManager.Instance.SetTimeScale();
        PrepareCurrentSceneForTitle();
        
        shouldInitializeLobbyOnTitleLoad = true;
        LoadScene(SceneType.START);
    }
    
    public void LoadScene(SceneType loadSceneType) {
        InputManager.Instance.IgnoreInput = true;
        InputManager.Instance.IgnoreEscape = true;

        StartCoroutine(loadSceneType == SceneType.ENDING ? LoadEnding() : ChangeScene(loadSceneType));
    }
    
    private IEnumerator LoadEnding()
    {
        // 대사 출력 중이면 기다리기
        while (DialogueManager.Instance.isDialogueActive)
            yield return null;

        MemoManager.Instance.SetMemoButtons(false);
        SoundPlayer.Instance.ChangeBGM(BGM_STOP);
        StartCoroutine(UIManager.Instance.OnFade(null, 0, 1, 1, false, 0, 0));

        yield return new WaitForSeconds(1f);

        if((int)GameManager.Instance.GetVariable("SavedCurrentSceneIndex") != SceneType.ENDING.ToInt()
            && (int)GameManager.Instance.GetVariable("SavedCurrentSceneIndex") != SceneType.START.ToInt())
            GameManager.Instance.SetVariable("EndingLoadScene", GameManager.Instance.GetVariable("SavedCurrentSceneIndex"));
        
        GameManager.Instance.SetVariable("SavedCurrentSceneIndex", SceneType.ENDING.ToInt());

        InputManager.Instance.IgnoreInput = false;
        InputManager.Instance.IgnoreEscape = false;

        SceneManager.LoadScene(SceneType.ENDING.ToInt());
    }
    
    public SceneType GetActiveScene() {
        Scene activeScene = SceneManager.GetActiveScene();
        switch (activeScene.name) {
            case "Start":
                return SceneType.START;
            case "Room1":
                return SceneType.ROOM_1;
            case "Follow1":
                return SceneType.FOLLOW_1;
            case "Room2":
                return SceneType.ROOM_2;
            case "Follow2":
                return SceneType.FOLLOW_2;
            case "Ending":
                return SceneType.ENDING;
        }
        
        Debug.LogError($"Unknown scene: {activeScene.name}");
        return SceneType.START;
    }

    private IEnumerator ChangeScene(SceneType loadSceneType)
    {
        while (DialogueManager.Instance.isDialogueActive) // 대사 출력 중이면 기다리기
            yield return null;

        // Reset cursor to default when exiting room scenes
        SceneType currentScene = GetActiveScene();
        if (currentScene is SceneType.ROOM_1 or SceneType.ROOM_2)
        {
            if (UIManager.Instance)
                UIManager.Instance.ChangeCursor(true);
        }

        if (currentScene == SceneType.START)
            UIManager.Instance.SetUI(eUIGameObjectName.AlbumButton, false);
        UIManager.Instance.ResetLoadingUI();
        MemoManager.Instance.SetMemoButtons(false, false, false);
        SoundPlayer.Instance.ChangeBGM(BGM_STOP);

        InputManager.Instance.IgnoreInput = true;
        yield return StartCoroutine(UIManager.Instance.OnFade(null, 0, 1, 1, false, 0, 0));

        StartCoroutine(UIManager.Instance.SetLoadingAnimation(0, 1, 1));
        UIManager.Instance.progressBar.fillAmount = 0f;
        string[] textOnFade = { "Prologue", "Chapter I", "Chapter II", "Chapter III", "Chapter IV" };
        int sceneTypeToInt = loadSceneType.ToInt();
        string loadingText = textOnFade[sceneTypeToInt];
        if (loadSceneType == SceneType.START && (bool)GameManager.Instance.GetVariable("SkipLobby") == false)
            loadingText = "";
        UIManager.Instance.TextOnFade(loadingText);
        
        GameManager.Instance.SetVariable("SavedCurrentSceneIndex", sceneTypeToInt);
        yield return new WaitForSeconds(1f);

        // 씬 로드
        StartCoroutine(Load(sceneTypeToInt));
    }

    private IEnumerator Load(int sceneName) { // 씬 비동기 로드 및 진행률 표시
        IsSceneChanging = true;
        UIManager.Instance.progressBar.fillAmount = 0f;

        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        op.allowSceneActivation = false;

        float timer = 0f;
        while (!op.isDone) {
            yield return null;
            timer += Time.unscaledDeltaTime;

            if (op.progress < 0.9f) {
                UIManager.Instance.progressBar.fillAmount =
                    Mathf.Lerp(UIManager.Instance.progressBar.fillAmount, op.progress, timer);
                if (UIManager.Instance.progressBar.fillAmount >= op.progress) timer = 0f;
            }
            else {
                UIManager.Instance.progressBar.fillAmount =
                    Mathf.Lerp(UIManager.Instance.progressBar.fillAmount, 1f, timer);
                if (UIManager.Instance.progressBar.fillAmount >= 1.0f) {
                    op.allowSceneActivation = true;

                    InputManager.Instance.IgnoreInput = false;
                    InputManager.Instance.IgnoreEscape = false;

                    yield break;
                }
            }
        }

        InputManager.Instance.IgnoreInput = false;
        InputManager.Instance.IgnoreEscape = false;
    }
    
    private void ChangeSceneEffect() { // 방탈출 씬인지 미행 씬인지에 따라 메모 버튼 변경, 대화창의 종류 변경, 방이면 방의 화면 변경
        switch (GetActiveScene()) {
            case SceneType.START:
            case SceneType.ROOM_1:
            case SceneType.ROOM_2:
                MemoManager.Instance.isFollow = false;
                DialogueManager.Instance.dialogueType = DialogueType.ROOM_ACCIDY;
                break;
            case SceneType.FOLLOW_1:
            case SceneType.FOLLOW_2:
                MemoManager.Instance.isFollow = true;
                DialogueManager.Instance.dialogueType = DialogueType.FOLLOW;
                break;
            
            default:
                return;
        }

        int bgmIndex = -1;
        switch (GetActiveScene()) {
            case SceneType.START:
                if (shouldInitializeLobbyOnTitleLoad && LobbyManager.Instance) {
                    shouldInitializeLobbyOnTitleLoad = false;
                    LobbyManager.Instance.InitializeLobbyScene();
                }
                SoundPlayer.Instance.ChangeBGM(BGM_OPENING);
                return;
            
            case SceneType.ROOM_1:
                bgmIndex = BGM_ROOM1;
                break;
            case SceneType.ROOM_2:
                bgmIndex = BGM_ROOM2;
                break;
            case SceneType.FOLLOW_1:
            case SceneType.FOLLOW_2:
                bgmIndex = BGM_FOLLOW1;
                break;
        }
        SoundPlayer.Instance.ChangeBGM(bgmIndex);

        MemoManager.Instance.SetMemoCurrentPageAndFlags();
        MemoManager.Instance.SetShouldHideMemoButton(false);
        MemoManager.Instance.SetMemoButtons(true);

        if (GetActiveScene() == SceneType.ROOM_1 || GetActiveScene() == SceneType.FOLLOW_1)
            StartCoroutine(DelayedMemoGaugeRefresh());

        // 씬 바뀐 후 저장
        SaveManager.Instance.SaveGameData();

        StartCoroutine(UIManager.Instance.OnFade(null, 1, 0, 1, false, 0, 0));
    }

    private IEnumerator DelayedMemoGaugeRefresh() {
        yield return new WaitForSeconds(0.5f);
        
        MemoManager.Instance.RebuildUnseenMemoPages();
        MemoManager.Instance.ForceRefreshMemoGauge();
    }

    public void NotClearThisScene()
    {
        string currentScene = GetActiveScene().ToString();
        GameManager.Instance.SetVariable($"MemoCount_{currentScene}", 0);

        LoadScene(SceneType.ENDING);
    }
    
    public void ClearThisScene()
    {
        string currentScene = GetActiveScene().ToString();
        GameManager.Instance.SetVariable($"MemoCount_{currentScene}", 9);

        LoadScene(SceneType.ENDING);
    }
}

