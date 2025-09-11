using UnityEngine;
using UnityEngine.UI;

public class InputManager : MonoBehaviour
{
    private static InputManager Instance { get; set; }

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
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (!IsDesktopEnvironment())
            return;

        if (Input.GetKeyDown(KeyCode.A))
            HandleLeftKeyClick();

        if (Input.GetKeyDown(KeyCode.D))
            HandleRightKeyClick();

        if (Input.GetKeyDown(KeyCode.S)) {
            if (IsClickable(UIManager.Instance.exitButton))
                UIManager.Instance.OnExitButtonClick();
        }
        
        if (Input.GetKeyDown(KeyCode.Space)) {
            if (DialogueManager.Instance && DialogueManager.Instance.isDialogueActive)
                DialogueManager.Instance.OnDialoguePanelClick();
        }
    }

    private void HandleLeftKeyClick() {
        if (MemoManager.Instance is not null && MemoManager.Instance.isMemoOpen) {
            MemoManager.Instance.autoFlip.FlipLeftPage();
            return;
        }
            
        if (UIManager.Instance is not null && UIManager.Instance.GetUI(eUIGameObjectName.Album).activeInHierarchy) {
            UIManager.Instance.GetUI(eUIGameObjectName.Album).GetComponent<PageContentsManager>().autoFlip.FlipLeftPage();
            return;
        }

        if (GameSceneManager.Instance is null ||
            GameSceneManager.Instance.GetActiveScene() is not (Constants.SceneType.ROOM_1 or Constants.SceneType.ROOM_2)) return;
        
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
    
    private void HandleRightKeyClick() {
        if (MemoManager.Instance is not null && MemoManager.Instance.isMemoOpen) {
            MemoManager.Instance.autoFlip.FlipRightPage();
            return;
        }
            
        if (UIManager.Instance is not null && UIManager.Instance.GetUI(eUIGameObjectName.Album).activeInHierarchy) {
            UIManager.Instance.GetUI(eUIGameObjectName.Album).GetComponent<PageContentsManager>().autoFlip.FlipRightPage();
            return;
        }

        if (GameSceneManager.Instance is null ||
            GameSceneManager.Instance.GetActiveScene() is not (Constants.SceneType.ROOM_1 or Constants.SceneType.ROOM_2)) return;
        
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
        return Application.platform is RuntimePlatform.OSXEditor
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


