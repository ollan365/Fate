using System.Runtime.InteropServices;
using UnityEngine;

public class HapticManager : MonoBehaviour
{
    public static HapticManager Instance { get; private set; }

    public enum ImpactStyle { Light, Medium, Heavy }
    public enum NotificationType { Success, Warning, Error }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Bootstrap()
    {
        if (Instance != null)
            return;

        GameObject go = new ("HapticManager");
        go.AddComponent<HapticManager>();
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

#if UNITY_IOS && !UNITY_EDITOR
    [DllImport("__Internal")] private static extern void _PlayImpactHaptic(int style);
    [DllImport("__Internal")] private static extern void _PlayNotificationHaptic(int type);
    [DllImport("__Internal")] private static extern void _PlaySelectionHaptic();
#endif

    public void PlayImpact(ImpactStyle style = ImpactStyle.Light)
    {
        if (!InputManager.IsiOSEnvironment())
            return;

#if UNITY_IOS && !UNITY_EDITOR
        _PlayImpactHaptic((int)style);
#endif
    }

    public void PlayNotification(NotificationType type)
    {
        if (!InputManager.IsiOSEnvironment())
            return;

#if UNITY_IOS && !UNITY_EDITOR
        _PlayNotificationHaptic((int)type);
#endif
    }

    public void PlaySelection()
    {
        if (!InputManager.IsiOSEnvironment())
            return;

#if UNITY_IOS && !UNITY_EDITOR
        _PlaySelectionHaptic();
#endif
    }
}
