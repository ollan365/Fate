using UnityEngine;
using UnityEngine.UI;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }
    public bool IgnoreInput { get; set; }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Bootstrap()
    {
        if (Instance != null)
            return;

        GameObject go = new ("InputManager");
        go.AddComponent<InputManager>();
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            IgnoreInput = false;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (!IsDesktopEnvironment() || IgnoreInput)
            return;

        // Spacebar press-and-hold skip handling for Dialogue
        if (Input.GetKeyDown(KeyCode.Space)) {
            if (DialogueManager.Instance && DialogueManager.Instance.isDialogueActive)
                spacePressedTime = Time.unscaledTime;
        }

        if (Input.GetKey(KeyCode.Space))
        {
            if (DialogueManager.Instance && DialogueManager.Instance.isDialogueActive)
            {
                if (DialogueManager.Instance.IsSkipActive() && spacePressedTime > 0f)
                {
                    float held = Time.unscaledTime - spacePressedTime;
                    float progress = Mathf.Clamp01(held / SpaceSkipHoldSeconds);
                    DialogueManager.Instance.UpdateSkipHoldProgress(progress);
                }
            }
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            if (DialogueManager.Instance && DialogueManager.Instance.isDialogueActive)
            {
                bool heldLongEnough = spacePressedTime > 0f && (Time.unscaledTime - spacePressedTime) >= SpaceSkipHoldSeconds;
                bool skipActive = DialogueManager.Instance.IsSkipActive();

                if (skipActive && heldLongEnough)
                    DialogueManager.Instance.SkipButtonClick();
                else
                    DialogueManager.Instance.OnDialoguePanelClick();

                spacePressedTime = 0f;
                DialogueManager.Instance.ResetSkipHoldProgress();
                return;
            }
        }

        if (Input.GetKeyDown(KeyCode.A))
            HandleLeftKeyClick();

        if (Input.GetKeyDown(KeyCode.D))
            HandleRightKeyClick();

        if (Input.GetKeyDown(KeyCode.S)) {
            if (IsClickable(UIManager.Instance.exitButton))
                UIManager.Instance.OnExitButtonClick();
        }
    }

    private const float SpaceSkipHoldSeconds = 1f;
    private float spacePressedTime = 0f;

    private static void HandleLeftKeyClick() {
        if (DialogueManager.Instance && DialogueManager.Instance.isDialogueActive)
            return;

        if (MemoManager.Instance is not null && MemoManager.Instance.isMemoOpen) {
            MemoManager.Instance.autoFlip.FlipLeftPage();
            return;
        }
            
        if (UIManager.Instance is not null && UIManager.Instance.GetUI(eUIGameObjectName.Album).activeInHierarchy) {
            UIManager.Instance.GetUI(eUIGameObjectName.Album).GetComponent<PageContentsManager>().autoFlip.FlipLeftPage();
            return;
        }

        if (GameSceneManager.Instance is null ||
            GameSceneManager.Instance.GetActiveScene() is not (Constants.SceneType.ROOM_1 or Constants.SceneType.ROOM_2)) 
            return;
        
        GetCurrentLockObject(out GameObject currentLockObject);
        AutoFlip autoFlip = currentLockObject?.GetComponent<PageContentsManager>()?.autoFlip;
        if (autoFlip && autoFlip.controlledBook.pageContentsManager.flipLeftButton.activeInHierarchy) {
            autoFlip.FlipLeftPage();
            return;
        }
                
        CalendarPanel calendarPanel = currentLockObject?.GetComponent<CalendarPanel>();
        if (calendarPanel && currentLockObject.activeInHierarchy && calendarPanel.prevButton.gameObject.activeInHierarchy) {
            calendarPanel.ChangeMonth(-1);
            return;
        }
                
        if (UIManager.Instance is not null && IsClickable(UIManager.Instance.leftButton))
            UIManager.Instance.OnLeftButtonClick();
    }
    
    private static void HandleRightKeyClick() {
        if (DialogueManager.Instance && DialogueManager.Instance.isDialogueActive)
            return;
            
        if (MemoManager.Instance is not null && MemoManager.Instance.isMemoOpen) {
            MemoManager.Instance.autoFlip.FlipRightPage();
            return;
        }
            
        if (UIManager.Instance is not null && UIManager.Instance.GetUI(eUIGameObjectName.Album).activeInHierarchy) {
            UIManager.Instance.GetUI(eUIGameObjectName.Album).GetComponent<PageContentsManager>().autoFlip.FlipRightPage();
            return;
        }

        if (GameSceneManager.Instance is null ||
            GameSceneManager.Instance.GetActiveScene() is not (Constants.SceneType.ROOM_1 or Constants.SceneType.ROOM_2)) 
            return;
        
        GetCurrentLockObject(out GameObject currentLockObject);
        AutoFlip autoFlip = currentLockObject?.GetComponent<PageContentsManager>()?.autoFlip;
        if (autoFlip && autoFlip.controlledBook.pageContentsManager.flipRightButton.activeInHierarchy) {
            autoFlip.FlipRightPage();
            return;
        }
                
        CalendarPanel calendarPanel = currentLockObject?.GetComponent<CalendarPanel>();
        if (calendarPanel && currentLockObject.activeInHierarchy && calendarPanel.nextButton.gameObject.activeInHierarchy) {
            calendarPanel.ChangeMonth(1);
            return;
        }
                
        if (UIManager.Instance is not null && IsClickable(UIManager.Instance.rightButton))
            UIManager.Instance.OnRightButtonClick();
    }

    private static void GetCurrentLockObject(out GameObject currentLockObject){
        ImageAndLockPanelManager imageAndLockPanelManager = RoomManager.Instance?.imageAndLockPanelManager;
        string currentLockObjectName = imageAndLockPanelManager?.currentLockObjectName;
        if (string.IsNullOrEmpty(currentLockObjectName) == false && imageAndLockPanelManager.lockObjectDictionary != null)
            currentLockObject = imageAndLockPanelManager.lockObjectDictionary[currentLockObjectName];
        else
            currentLockObject = null;
    }

    private static bool IsDesktopEnvironment()
    {
        return Application.platform 
            is RuntimePlatform.OSXEditor
            or RuntimePlatform.WindowsEditor
            or RuntimePlatform.LinuxEditor
            or RuntimePlatform.OSXPlayer
            or RuntimePlatform.WindowsPlayer
            or RuntimePlatform.LinuxPlayer;
    }

    private static bool IsClickable(GameObject target)
    {
        if (target is null)
            return false;

        if (target.activeInHierarchy == false)
            return false;

        Button button = target.GetComponent<Button>();
        if (button is not null && button.interactable == false)
            return false;

        CanvasGroup canvasGroup = target.GetComponent<CanvasGroup>();
        return canvasGroup is not null && 
               canvasGroup.interactable && 
               canvasGroup.blocksRaycasts && 
               canvasGroup.alpha > 0.001f;
    }
}


