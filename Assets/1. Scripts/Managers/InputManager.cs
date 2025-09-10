using UnityEngine;
using UnityEngine.UI;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Bootstrap()
    {
        // Ensure a single persistent InputManager exists without requiring manual scene setup
        if (Instance != null)
            return;

        var go = new GameObject("InputManager");
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

        if (UIManager.Instance is null)
            return;
        
        if (Input.GetKeyDown(KeyCode.A)) {
            if (MemoManager.Instance != null && MemoManager.Instance.isMemoOpen) {
                MemoManager.Instance.autoFlip.FlipLeftPage();
            }
            else if (UIManager.Instance.GetUI(eUIGameObjectName.Album).activeInHierarchy) {
                UIManager.Instance.GetUI(eUIGameObjectName.Album).GetComponent<PageContentsManager>().autoFlip.FlipLeftPage();
            }
            else if (GameSceneManager.Instance.GetActiveScene() is Constants.SceneType.ROOM_1 or Constants.SceneType.ROOM_2) {
                ImageAndLockPanelManager imageAndLockPanelManager = RoomManager.Instance?.imageAndLockPanelManager;
                GameObject currentLockObject = null;
                string currentLockObjectName = imageAndLockPanelManager?.currentLockObjectName;
                if (string.IsNullOrEmpty(currentLockObjectName) == false &&
                    imageAndLockPanelManager?.lockObjectDictionary != null)
                    imageAndLockPanelManager.lockObjectDictionary.TryGetValue(currentLockObjectName, out currentLockObject);
                AutoFlip autoFlip = currentLockObject?.GetComponent<PageContentsManager>()?.autoFlip;
                CalendarPanel calendarPanel = currentLockObject?.GetComponent<CalendarPanel>();
                if (autoFlip && autoFlip.controlledBook.pageContentsManager.flipLeftButton.activeInHierarchy) {
                    autoFlip.FlipLeftPage();
                }
                else if (calendarPanel && currentLockObject.activeInHierarchy && calendarPanel.prevButton.gameObject.activeInHierarchy) {
                    calendarPanel.ChangeMonth(-1);
                }
                else if (IsClickable(UIManager.Instance.leftButton)) {
                    UIManager.Instance.OnLeftButtonClick();
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.D)) {
            if (MemoManager.Instance != null && MemoManager.Instance.isMemoOpen) {
                MemoManager.Instance.autoFlip.FlipRightPage();
            }
            else if (UIManager.Instance.GetUI(eUIGameObjectName.Album).activeInHierarchy) {
                UIManager.Instance.GetUI(eUIGameObjectName.Album).GetComponent<PageContentsManager>().autoFlip.FlipRightPage();
            }
            else if (GameSceneManager.Instance.GetActiveScene() is Constants.SceneType.ROOM_1 or Constants.SceneType.ROOM_2) {
                ImageAndLockPanelManager imageAndLockPanelManager = RoomManager.Instance?.imageAndLockPanelManager;
                GameObject currentLockObject = null;
                string currentLockObjectName = imageAndLockPanelManager?.currentLockObjectName;
                if (string.IsNullOrEmpty(currentLockObjectName) == false &&
                    imageAndLockPanelManager?.lockObjectDictionary != null)
                    imageAndLockPanelManager.lockObjectDictionary.TryGetValue(currentLockObjectName, out currentLockObject);
                AutoFlip autoFlip = currentLockObject?.GetComponent<PageContentsManager>()?.autoFlip;
                CalendarPanel calendarPanel = currentLockObject?.GetComponent<CalendarPanel>();
                if (autoFlip && autoFlip.controlledBook.pageContentsManager.flipRightButton.activeInHierarchy) {
                    autoFlip.FlipRightPage();
                } 
                else if (calendarPanel && currentLockObject.activeInHierarchy && calendarPanel.nextButton.gameObject.activeInHierarchy) {
                    calendarPanel.ChangeMonth(1);
                }
                else if (IsClickable(UIManager.Instance.rightButton)) {
                    UIManager.Instance.OnRightButtonClick();
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.S)) {
            if (IsClickable(UIManager.Instance.exitButton))
                UIManager.Instance.OnExitButtonClick();
        }
    }

    private static bool IsDesktopEnvironment()
    {
        var platform = Application.platform;
        return platform == RuntimePlatform.OSXEditor
               || platform == RuntimePlatform.WindowsEditor
               || platform == RuntimePlatform.LinuxEditor
               || platform == RuntimePlatform.OSXPlayer
               || platform == RuntimePlatform.WindowsPlayer
               || platform == RuntimePlatform.LinuxPlayer;
    }

    private static bool IsClickable(GameObject target)
    {
        if (target == null)
            return false;

        if (!target.activeInHierarchy)
            return false;

        // If there is a Button, require it to be interactable
        var button = target.GetComponent<Button>();
        if (button != null && !button.interactable)
            return false;

        // If there is a CanvasGroup, require it to be interactable, blocking raycasts, and visible
        var canvasGroup = target.GetComponent<CanvasGroup>();
        if (canvasGroup != null)
        {
            if (!canvasGroup.interactable)
                return false;
            if (!canvasGroup.blocksRaycasts)
                return false;
            if (canvasGroup.alpha <= 0.001f)
                return false;
        }

        return true;
    }
}


