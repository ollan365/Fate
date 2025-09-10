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

        if (Input.GetKeyDown(KeyCode.A) && IsClickable(UIManager.Instance.leftButton))
            UIManager.Instance.OnLeftButtonClick();

        if (Input.GetKeyDown(KeyCode.D) && IsClickable(UIManager.Instance.rightButton))
            UIManager.Instance.OnRightButtonClick();
        
        if (Input.GetKeyDown(KeyCode.S) && IsClickable(UIManager.Instance.exitButton))
            UIManager.Instance.OnExitButtonClick();
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


