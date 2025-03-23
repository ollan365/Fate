using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static Constants;

public enum eUIGameObjectName
{
    NormalVignette,
    WarningVignette,
    ActionPoints,
    HeartParent,
    DayText,
    ExitButton,
    LeftButton,
    RightButton,
    MemoButton,
    MemoContents,
    MemoGauge,
    MenuUI,
    WhiteMenu,
    BlackMenu,
    OptionUI,
    BGMSlider,
    SoundEffectSlider,
    BGMValue,
    SoundEffectValue,
    FollowMemoGauge,
    FollowUIBackground,
    DoubtGaugeSlider,
    FatePositionSlider,
    AccidyPositionSlider
}

public enum FloatDirection
{
    None,
    Left,
    Right,
    Up,
    Down
}

public class UIManager : MonoBehaviour
{
    [Header("UI Game Objects")]
    public GameObject normalVignette;
    public GameObject warningVignette;
    public GameObject actionPoints;
    public GameObject heartParent;
    public GameObject dayText;
    public GameObject exitButton;
    public GameObject leftButton;
    public GameObject rightButton;
    public GameObject memoButton;
    public GameObject memoContents;
    public GameObject memoGauge;

    [Header("UI Game Objects - Menu")]
    public GameObject menuUI;
    public GameObject whiteMenu;
    public GameObject blackMenu;
    public GameObject optionUI;
    public GameObject BGMSlider;
    public GameObject SoundEffectSlider;
    public GameObject BGMValueText;
    public GameObject SoundEffectValueText;

    [Header("UI Game Objects - Follow")]
    public GameObject followMemoGauge;
    public GameObject followUIBackground;
    public GameObject doubtGaugeSlider;
    public GameObject fatePositionSlider;
    public GameObject accidyPositionSlider;

    private readonly Dictionary<eUIGameObjectName, GameObject> uiGameObjects = new();
    private Q_Vignette_Single warningVignetteQVignetteSingle;
    [HideInInspector] public TextMeshProUGUI dayTextTextMeshProUGUI;
    
    [Header("Warning Vignette Settings")]
    [SerializeField] protected float warningTime;
    
    [Header("Animation Settings")]
    public float fadeAnimationDuration = 0.3f;
    [SerializeField] private float floatAnimationDuration = 0.3f;
    [SerializeField] private float floatDistance = 50f;
    
    public static UIManager Instance { get; private set; }
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
        
        AddUIGameObjects();
        SetAllUI(false);
        SetOptionUI();
    }

    private void AddUIGameObjects()
    {
        uiGameObjects.Add(eUIGameObjectName.NormalVignette, normalVignette);
        uiGameObjects.Add(eUIGameObjectName.WarningVignette, warningVignette);

        uiGameObjects.Add(eUIGameObjectName.ActionPoints, actionPoints);
        uiGameObjects.Add(eUIGameObjectName.HeartParent, heartParent);
        uiGameObjects.Add(eUIGameObjectName.DayText, dayText);

        uiGameObjects.Add(eUIGameObjectName.ExitButton, exitButton);
        uiGameObjects.Add(eUIGameObjectName.LeftButton, leftButton);
        uiGameObjects.Add(eUIGameObjectName.RightButton, rightButton);

        uiGameObjects.Add(eUIGameObjectName.MemoContents, memoContents);
        uiGameObjects.Add(eUIGameObjectName.MemoGauge, memoGauge);

        uiGameObjects.Add(eUIGameObjectName.MenuUI, menuUI);
        uiGameObjects.Add(eUIGameObjectName.WhiteMenu, whiteMenu);
        uiGameObjects.Add(eUIGameObjectName.BlackMenu, blackMenu);

        uiGameObjects.Add(eUIGameObjectName.OptionUI, optionUI);
        uiGameObjects.Add(eUIGameObjectName.BGMSlider, BGMSlider);
        uiGameObjects.Add(eUIGameObjectName.SoundEffectSlider, SoundEffectSlider);
        uiGameObjects.Add(eUIGameObjectName.BGMValue, BGMValueText);
        uiGameObjects.Add(eUIGameObjectName.SoundEffectValue, SoundEffectValueText);

        uiGameObjects.Add(eUIGameObjectName.FollowMemoGauge, followMemoGauge);

        uiGameObjects.Add(eUIGameObjectName.FollowUIBackground, followUIBackground);

        uiGameObjects.Add(eUIGameObjectName.DoubtGaugeSlider, doubtGaugeSlider);

        uiGameObjects.Add(eUIGameObjectName.FatePositionSlider, fatePositionSlider);
        uiGameObjects.Add(eUIGameObjectName.AccidyPositionSlider, accidyPositionSlider);

        warningVignetteQVignetteSingle = warningVignette.GetComponent<Q_Vignette_Single>();
        dayTextTextMeshProUGUI = dayText.GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) 
            SetMenuUI();
    }

    private void SetAllUI(bool isActive)
    {
        foreach (var ui in uiGameObjects)
            SetUI(ui.Key, isActive);
    }

    public void SetUI(eUIGameObjectName uiName,
        bool isActive,
        bool fade = false,
        FloatDirection floatDir = FloatDirection.None)
    {
        GameObject targetUI = uiGameObjects[uiName];
        
        if (fade || floatDir != FloatDirection.None)
        {
            CanvasGroup canvasGroup = targetUI.GetComponent<CanvasGroup>(); // need to memoize this
            if (!canvasGroup)
            {
                Debug.LogWarning("CanvasGroup is not found in the target UI.");
                targetUI.SetActive(isActive);
                return;
            }
            
            if (isActive)
                targetUI.SetActive(true);
            
            string coroutineName = $"Animate_{uiName}";
            if (IsInvoking(coroutineName))
                StopCoroutine(coroutineName);
            StartCoroutine(AnimateUI(uiName, canvasGroup, isActive, floatDir));
        }
        else
            targetUI.SetActive(isActive);
    }
    
    private IEnumerator AnimateUI(eUIGameObjectName uiName, CanvasGroup canvasGroup, bool show, FloatDirection floatDir)
    {
        float targetAlpha = show ? 1f : 0f;
        float startAlpha = show ? 0f : 1f;
        float fadeTime = fadeAnimationDuration;
        float elapsedTime = 0f;
        
        canvasGroup.alpha = startAlpha;
        
        RectTransform rectTransform = null;
        Vector2 startPos = Vector2.zero;
        Vector2 targetPos = Vector2.zero;
        Vector2 basePosition = Vector2.zero;
        
        if (floatDir != FloatDirection.None)
        {
            rectTransform = uiGameObjects[uiName].GetComponent<RectTransform>();
            basePosition = rectTransform.anchoredPosition;
            
            // Calculate start and target positions with original direction on entry, reversed on exit
            FloatDirection effectiveDirection = show ? floatDir : GetReverseDirection(floatDir);
            GetAnimationPositions(effectiveDirection, show, basePosition, out startPos, out targetPos);
            
            rectTransform.anchoredPosition = startPos;
        }
        
        while (elapsedTime < fadeTime)
        {
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / fadeTime);
            
            if (floatDir != FloatDirection.None && rectTransform)
                rectTransform.anchoredPosition = Vector2.Lerp(startPos, targetPos, elapsedTime / floatAnimationDuration);
            
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        canvasGroup.alpha = targetAlpha;
        
        if (floatDir != FloatDirection.None && rectTransform)
        {
            // If hiding, always return to original base position after animation
            rectTransform.anchoredPosition = show ? targetPos : basePosition;
        }
        
        // If hiding, deactivate the object after animation is complete
        if (!show)
            uiGameObjects[uiName].SetActive(false);
    }

    private FloatDirection GetReverseDirection(FloatDirection direction)
    {
        switch (direction)
        {
            case FloatDirection.Left: return FloatDirection.Right;
            case FloatDirection.Right: return FloatDirection.Left;
            case FloatDirection.Up: return FloatDirection.Down;
            case FloatDirection.Down: return FloatDirection.Up;
            default: return FloatDirection.None;
        }
    }

    private void GetAnimationPositions(FloatDirection floatDir, bool show, Vector2 basePosition, out Vector2 startPos, out Vector2 targetPos)
    {
        startPos = basePosition;
        targetPos = basePosition;

        switch (floatDir)
        {
            case FloatDirection.Right:
                startPos.x = show ? basePosition.x - floatDistance : basePosition.x;
                targetPos.x = show ? basePosition.x : basePosition.x + floatDistance;
                break;
            case FloatDirection.Left:
                startPos.x = show ? basePosition.x + floatDistance : basePosition.x;
                targetPos.x = show ? basePosition.x : basePosition.x - floatDistance;
                break;
            case FloatDirection.Down:
                startPos.y = show ? basePosition.y + floatDistance : basePosition.y;
                targetPos.y = show ? basePosition.y : basePosition.y - floatDistance;
                break;
            case FloatDirection.Up:
                startPos.y = show ? basePosition.y - floatDistance : basePosition.y;
                targetPos.y = show ? basePosition.y : basePosition.y + floatDistance;
                break;
        }
    }

    public GameObject GetUI(eUIGameObjectName uiName)
    {
        return uiGameObjects[uiName];
    }

    private void SetMenuUI()
    {
        if (GetUI(eUIGameObjectName.MenuUI).activeSelf)
        {
            SetUI(eUIGameObjectName.MenuUI, false);
            SetUI(eUIGameObjectName.WhiteMenu, false);
            SetUI(eUIGameObjectName.BlackMenu, false);
            Time.timeScale = 1f;
        }
        else
        {
            SetUI(eUIGameObjectName.MenuUI, true);
            SetUI(Random.Range(0, 2) == 0 
                ? eUIGameObjectName.WhiteMenu 
                : eUIGameObjectName.BlackMenu, true);
            Time.timeScale = 0f;
        }
    }

    public void SetTimeScale()
    {
        Time.timeScale = 1f;
    }

    private void SetOptionUI()
    {
        SetUI(eUIGameObjectName.BGMSlider, true);
        SetUI(eUIGameObjectName.SoundEffectSlider, true);
        SetUI(eUIGameObjectName.BGMValue, true);
        SetUI(eUIGameObjectName.SoundEffectValue, true);
    }
    
    public void OnLeftButtonClick()
    {
        switch (GetCurrentSceneIndex())
        {
            case (int)SceneType.ROOM_1:
            case (int)SceneType.ROOM_2:
                RoomManager.Instance.MoveSides(-1);
                break;
        }
    }
    
    public void OnRightButtonClick()
    {
        switch (GetCurrentSceneIndex())
        {
            case (int)SceneType.ROOM_1:
            case (int)SceneType.ROOM_2:
                RoomManager.Instance.MoveSides(1);
                break;
        }
    }
    
    public void OnExitButtonClick()
    {
        if (MemoManager.Instance && MemoManager.Instance.isMemoOpen)
        {
            MemoManager.Instance.OnExit();
            return;
        }
        
        switch (GetCurrentSceneIndex())
        {
            case (int)SceneType.START:
                Debug.Log("Exit button is not implemented in this scene.");
                break;
            
            case (int)SceneType.ROOM_1:
            case (int)SceneType.ROOM_2:
                RoomManager.Instance.OnExitButtonClick();
                break;
            
            default:
                Debug.Log("Exit button is not implemented in this scene.");
                break;
        }
    }
    
    public void ChangeSoundValue(string uiName)
    {
        TMP_Text text;
        Slider slider;

        if (uiName == eUIGameObjectName.BGMSlider.ToString())
        {
            text = uiGameObjects[eUIGameObjectName.BGMValue].GetComponent<TextMeshProUGUI>();
            slider = uiGameObjects[eUIGameObjectName.BGMSlider].GetComponent<Slider>();
            SoundPlayer.Instance.ChangeVolume(slider.value, -1);
        }
        else
        {
            text = uiGameObjects[eUIGameObjectName.SoundEffectValue].GetComponent<TextMeshProUGUI>();
            slider = uiGameObjects[eUIGameObjectName.SoundEffectSlider].GetComponent<Slider>();
            SoundPlayer.Instance.ChangeVolume(-1, slider.value);
        }

        text.text = (slider.value * 100).ToString("F0");
    }

    public void ChangeSliderValue(eUIGameObjectName uiName, float absoluteValue, float addValue)
    {
        Slider slider = uiGameObjects[uiName].GetComponent<Slider>();
        if (addValue != 0) 
            slider.value += addValue;
        else 
            slider.value = absoluteValue;
    }

    /*
     * startAlpha: 경고 표시 시작 시 투명도
     * endAlpha: 경고 표시 종료 시 투명도
     * fadeInDuration: 경고 표시 페이드 인 소요 시간
     * fadeOutDuration: 경고 표시 페이드 아웃 소요 시간
     */
    public IEnumerator WarningCoroutine(float startAlpha = 0f,
        float endAlpha = 1f,
        float fadeInDuration = 0.5f,
        float fadeOutDuration = 0.5f)
    {
        SetUI(eUIGameObjectName.WarningVignette, true); // 경고 표시 활성화
        
        float timeAccumulated = 0;
        while (timeAccumulated < fadeInDuration)
        {
            timeAccumulated += Time.deltaTime;
            warningVignetteQVignetteSingle.mainColor.a = Mathf.Lerp(startAlpha,
                endAlpha,
                timeAccumulated / fadeInDuration); // WarningVignette 투명도를 0에서 1로 선형 보간(Lerp)

            yield return null;
        }

        yield return new WaitForSeconds(warningTime); // warningTime 동안 경고 상태 유지

        timeAccumulated = 0; // 경고 종료: WarningVignette.mainColor.a를 다시 0으로 페이드 아웃
        while (timeAccumulated < fadeOutDuration)
        {
            timeAccumulated += Time.deltaTime * 2;  // 페이드 아웃 속도를 더 빠르게 설정
            warningVignetteQVignetteSingle.mainColor.a = Mathf.Lerp(endAlpha,
                startAlpha,
                timeAccumulated / fadeOutDuration); // WarningVignette 투명도를 1에서 0으로 선형 보간(Lerp)

            yield return null;
        }

        SetUI(eUIGameObjectName.WarningVignette, false); // 경고 표시 비활성화
    }
    
    private int GetCurrentSceneIndex()
    {
        return (int)GameManager.Instance.GetVariable("CurrentScene");
    }
}
