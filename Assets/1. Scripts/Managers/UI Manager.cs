using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using static Constants;

public enum FloatDirection {
    None,
    Left,
    Right,
    Up,
    Down
}

public enum eUIGameObjectName {
    BlurImage,
    StartupImage,
    ObjectImageParentRoom, // object image panel parent in room scene
    ObjectImageRoom, // object image panel in room scene
    LoadingScreen,
    NormalVignette,
    WarningVignette,
    ActionPoints,
    ActionPointsBackgroundImage,
    HeartParent,
    DayText,
    ExitButton,
    LeftButton,
    RightButton,
    SaveImage,
    SaveText,
    MemoButton,
    MemoContents,
    MemoGauge,
    LobbyPanels,
    NewGamePanel,
    NoGameDataPanel,
    NamePanel,
    NameConfirmPanel,
    BirthdayPanel,
    MenuUI,
    Menu,
    GoToTitleButton,
    OptionUI,
    BGMSlider,
    SoundEffectSlider,
    BGMValue,
    SoundEffectValue,
    FollowMemoGauge,
    FollowUI,
    FollowUIBackground,
    DoubtGaugeSlider,
    FatePositionSlider,
    AccidyPositionSlider,
    FollowMemoGauge_Night,
    FollowUI_Night,
    FollowUIBackground_Night,
    DoubtGaugeSlider_Night,
    FatePositionSlider_Night,
    AccidyPositionSlider_Night,
    FollowEventButtonImage,
    FollowEventButton,
    followEventButtonNextButton,
    DayChangingGameObject,
    YesterdayNumText,
    TodayNumText,
    Yesterday,
    MainGear,
    SubGear,
    GearHourHand,
    GearMinuteHand,
    Album,
    AlbumButton,
    AlbumPage,
    AlbumImageGameObject,
    AlbumEndingImage,
    EndingNameGameObject,
    TutorialBlockingPanel,
    EndOfDemoPage
}

public class UIManager : MonoBehaviour {
    [Header("Screen Effect")] public Image coverPanel;
    [SerializeField] private TextMeshProUGUI coverText;
    public GameObject loadingScreen;
    public Image progressBar;
    public CanvasGroup progressBarGroup;
    public CanvasGroup Loading_AnimationGroup;
    public List<GameObject> loading_characters;
    
    [Header("Blur Image")]
    public GameObject blurImage;
    
    [Header("Startup Image")]
    public GameObject startupImage;

    [Header("Object Image")]
    public GameObject objectImageParentRoom;
    public GameObject objectImageRoom;

    [Header("UI Game Objects")] 
    public GameObject normalVignette;
    public GameObject warningVignette;
    public GameObject actionPoints;
    public GameObject actionPointsBackgroundImage;
    public GameObject heartParent;
    public GameObject dayText;
    public GameObject exitButton;
    public GameObject leftButton;
    public GameObject rightButton;
    public GameObject memoButton;
    public GameObject memoContents;
    public GameObject memoGauge;
    public GameObject tutorialBlockingPanel;

    [Header("UI Game Objects - Album")] 
    public GameObject album;
    public GameObject albumButton;
    public GameObject albumPage;
    public GameObject albumImageGameObject;
    public GameObject endingTypeGameObject;
    public GameObject endingNameGameObject;
    public Sprite[] endingSprites;
    public Image[] targetAlbumImages;
    private Vector3 albumImageDestination;
    public GameObject albumEndingImage;

    [Header("UI Game Objects - Day Animation")]
    public GameObject dayChangingGameObject;
    public GameObject yesterdayNumText;
    public GameObject todayNumText;
    public GameObject yesterday;
    public GameObject mainGear;
    public GameObject subGear;
    public GameObject gearHourHand;
    public GameObject gearMinuteHand;

    [HideInInspector] public TextMeshProUGUI yesterdayNumTextTextMeshProUGUI;
    [HideInInspector] public TextMeshProUGUI todayNumTextTextMeshProUGUI;
    [HideInInspector] public RectTransform yesterdayRectTransform;
    [HideInInspector] public RectTransform dayChangingGroupRectTransform;
    private TextMeshProUGUI endingNameText;
    private Image albumImage;

    [Header("UI Game Objects - Lobby Panels")]
    public GameObject lobbyPanels;
    public GameObject newGamePanel;
    public GameObject noGameDataPanel;
    public GameObject namePanel;
    public GameObject nameConfirmPanel;
    public GameObject birthdayPanel;
    
    [Header("UI Game Objects - Menu")] 
    public GameObject menuUI;
    public GameObject menu;
    public GameObject goToTitleButton;
    public Sprite[] optionButtonImages;
    public GameObject optionUI;
    public GameObject BGMSlider;
    public GameObject SoundEffectSlider;
    public GameObject BGMValueText;
    public GameObject SoundEffectValueText;
    private bool menuOpenByStartSceneButton = false;

    [Header("UI Game Objects - Follow")] 
    public GameObject followUIParent;
    public GameObject followMemoGauge;
    public GameObject followUIBackground;
    public GameObject doubtGaugeSlider;
    public GameObject fatePositionSlider;
    public GameObject accidyPositionSlider;
    public GameObject followUIParent_Night;
    public GameObject followMemoGauge_Night;
    public GameObject followUIBackground_Night;
    public GameObject doubtGaugeSlider_Night;
    public GameObject fatePositionSlider_Night;
    public GameObject accidyPositionSlider_Night;
    public GameObject followEventButton;
    public GameObject followEventButtonImage;
    public GameObject followEventButtonNextButton;

    [Header("UI Game Objects - Save")]
    public GameObject saveImage;
    public GameObject saveText;

    [Header("UI Game Objects - End of Demo Page")] 
    public GameObject endOfDemoPage;

    private readonly Dictionary<eUIGameObjectName, GameObject> uiGameObjects = new();
    private Q_Vignette_Single warningVignetteQVignetteSingle;
    [HideInInspector] public TextMeshProUGUI dayTextTextMeshProUGUI;

    [Header("Warning Vignette Settings")] [SerializeField]
    protected float warningTime;

    [Header("Cursor Settings")] public Texture2D defaultCursorTexture;
    public Texture2D investigateCursorTexture;

    [Header("UI Camera")] public Camera uiCamera;

    private bool isCursorTouchingUI;

    // UI GameObjects to explicitly check for cursor hover
    private List<RectTransform> uiToCheck;

    [Header("Animation Settings")]
    public float fadeAnimationDuration = 0.3f;
    public float floatAnimationDuration = 0.3f;
    [SerializeField] private float floatDistance = 50f;
    
    private readonly Dictionary<GameObject, Coroutine> activeUiAnimations = new();    // Track currently running UI animations so newer calls can cancel older ones
    private readonly Dictionary<GameObject, bool> originalButtonInteractable = new(); // Track original Button.interactable per target so we can restore on cancel/finish

    public static UIManager Instance { get; private set; }
    private bool isQuitting;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }

        AddUIGameObjects();
        SetAllUI(false);
        SetOptionUI();
        InitializeUIToCheck();

        // UI Objects that should be active by default
        SetUI(eUIGameObjectName.ObjectImageRoom, true);
        SetUI(eUIGameObjectName.AlbumButton, true);
    }

    private void Start() {
        ChangeBgmOrSoundEffectValue(true);
        ChangeBgmOrSoundEffectValue(false);
    }

    private void OnApplicationQuit() {
        isQuitting = true;
    }

    private void AddUIGameObjects() {
        uiGameObjects.Add(eUIGameObjectName.BlurImage, blurImage);
        
        uiGameObjects.Add(eUIGameObjectName.StartupImage, startupImage);

        uiGameObjects.Add(eUIGameObjectName.ObjectImageParentRoom, objectImageParentRoom);
        uiGameObjects.Add(eUIGameObjectName.ObjectImageRoom, objectImageRoom);

        uiGameObjects.Add(eUIGameObjectName.LoadingScreen, loadingScreen);

        uiGameObjects.Add(eUIGameObjectName.NormalVignette, normalVignette);
        uiGameObjects.Add(eUIGameObjectName.WarningVignette, warningVignette);

        uiGameObjects.Add(eUIGameObjectName.ActionPoints, actionPoints);
        uiGameObjects.Add(eUIGameObjectName.ActionPointsBackgroundImage, actionPointsBackgroundImage);
        uiGameObjects.Add(eUIGameObjectName.HeartParent, heartParent);
        uiGameObjects.Add(eUIGameObjectName.DayText, dayText);

        uiGameObjects.Add(eUIGameObjectName.ExitButton, exitButton);
        uiGameObjects.Add(eUIGameObjectName.LeftButton, leftButton);
        uiGameObjects.Add(eUIGameObjectName.RightButton, rightButton);

        uiGameObjects.Add(eUIGameObjectName.MemoButton, memoButton);
        uiGameObjects.Add(eUIGameObjectName.MemoContents, memoContents);
        uiGameObjects.Add(eUIGameObjectName.MemoGauge, memoGauge);

        uiGameObjects.Add(eUIGameObjectName.SaveImage, saveImage);
        uiGameObjects.Add(eUIGameObjectName.SaveText, saveText);

        uiGameObjects.Add(eUIGameObjectName.TutorialBlockingPanel, tutorialBlockingPanel);
        
        uiGameObjects.Add(eUIGameObjectName.LobbyPanels, lobbyPanels);
        uiGameObjects.Add(eUIGameObjectName.NewGamePanel, newGamePanel);
        uiGameObjects.Add(eUIGameObjectName.NoGameDataPanel, noGameDataPanel);
        uiGameObjects.Add(eUIGameObjectName.NamePanel, namePanel);
        uiGameObjects.Add(eUIGameObjectName.NameConfirmPanel, nameConfirmPanel);
        uiGameObjects.Add(eUIGameObjectName.BirthdayPanel, birthdayPanel);

        uiGameObjects.Add(eUIGameObjectName.MenuUI, menuUI);
        uiGameObjects.Add(eUIGameObjectName.Menu, menu);
        uiGameObjects.Add(eUIGameObjectName.GoToTitleButton, goToTitleButton);

        uiGameObjects.Add(eUIGameObjectName.OptionUI, optionUI);
        uiGameObjects.Add(eUIGameObjectName.BGMSlider, BGMSlider);
        uiGameObjects.Add(eUIGameObjectName.SoundEffectSlider, SoundEffectSlider);
        uiGameObjects.Add(eUIGameObjectName.BGMValue, BGMValueText);
        uiGameObjects.Add(eUIGameObjectName.SoundEffectValue, SoundEffectValueText);

        uiGameObjects.Add(eUIGameObjectName.FollowUI, followUIParent);
        uiGameObjects.Add(eUIGameObjectName.FollowMemoGauge, followMemoGauge);
        uiGameObjects.Add(eUIGameObjectName.FollowUIBackground, followUIBackground);
        uiGameObjects.Add(eUIGameObjectName.DoubtGaugeSlider, doubtGaugeSlider);
        uiGameObjects.Add(eUIGameObjectName.FatePositionSlider, fatePositionSlider);
        uiGameObjects.Add(eUIGameObjectName.AccidyPositionSlider, accidyPositionSlider);

        uiGameObjects.Add(eUIGameObjectName.FollowUI_Night, followUIParent_Night);
        uiGameObjects.Add(eUIGameObjectName.FollowMemoGauge_Night, followMemoGauge_Night);
        uiGameObjects.Add(eUIGameObjectName.FollowUIBackground_Night, followUIBackground_Night);
        uiGameObjects.Add(eUIGameObjectName.DoubtGaugeSlider_Night, doubtGaugeSlider_Night);
        uiGameObjects.Add(eUIGameObjectName.FatePositionSlider_Night, fatePositionSlider_Night);
        uiGameObjects.Add(eUIGameObjectName.AccidyPositionSlider_Night, accidyPositionSlider_Night);

        uiGameObjects.Add(eUIGameObjectName.FollowEventButton, followEventButton);
        uiGameObjects.Add(eUIGameObjectName.FollowEventButtonImage, followEventButtonImage);
        uiGameObjects.Add(eUIGameObjectName.followEventButtonNextButton, followEventButtonNextButton);

        uiGameObjects.Add(eUIGameObjectName.MainGear, mainGear);
        uiGameObjects.Add(eUIGameObjectName.SubGear, subGear);
        uiGameObjects.Add(eUIGameObjectName.GearHourHand, gearHourHand);
        uiGameObjects.Add(eUIGameObjectName.GearMinuteHand, gearMinuteHand);
        uiGameObjects.Add(eUIGameObjectName.DayChangingGameObject, dayChangingGameObject);
        uiGameObjects.Add(eUIGameObjectName.YesterdayNumText, yesterdayNumText);
        uiGameObjects.Add(eUIGameObjectName.TodayNumText, todayNumText);
        uiGameObjects.Add(eUIGameObjectName.Yesterday, yesterday);

        uiGameObjects.Add(eUIGameObjectName.Album, album);
        uiGameObjects.Add(eUIGameObjectName.AlbumButton, albumButton);
        uiGameObjects.Add(eUIGameObjectName.AlbumPage, albumPage);
        uiGameObjects.Add(eUIGameObjectName.AlbumImageGameObject, albumImageGameObject);
        uiGameObjects.Add(eUIGameObjectName.EndingNameGameObject, endingNameGameObject);
        uiGameObjects.Add(eUIGameObjectName.AlbumEndingImage, albumEndingImage);
        
        uiGameObjects.Add(eUIGameObjectName.EndOfDemoPage, endOfDemoPage);

        yesterdayNumTextTextMeshProUGUI = yesterdayNumText.GetComponent<TextMeshProUGUI>();
        todayNumTextTextMeshProUGUI = todayNumText.GetComponent<TextMeshProUGUI>();
        yesterdayRectTransform = yesterday.GetComponent<RectTransform>();
        dayChangingGroupRectTransform = dayChangingGameObject.GetComponent<RectTransform>();

        warningVignetteQVignetteSingle = warningVignette.GetComponent<Q_Vignette_Single>();
        dayTextTextMeshProUGUI = dayText.GetComponent<TextMeshProUGUI>();

        endingNameText = endingNameGameObject.GetComponent<TextMeshProUGUI>();
        albumImage = albumImageGameObject.GetComponent<Image>();
    }

    private void Update() {
        CheckCursorTouchingUIs();
    }
    
    public void SetAllUI(bool isActive) {
        foreach (var ui in uiGameObjects)
            SetUI(ui.Key, isActive);
    }

    public void SetUI(eUIGameObjectName uiName, bool isActive, bool fade = false, FloatDirection floatDir = FloatDirection.None) {
        if (fade || floatDir != FloatDirection.None)
            AnimateUI(uiGameObjects[uiName], isActive, fade, floatDir);
        else
            uiGameObjects[uiName].SetActive(isActive);
    }

    public GameObject GetUI(eUIGameObjectName uiName) {
        return uiGameObjects[uiName];
    }

    public void AnimateUI(GameObject targetUI, bool isActive, bool fade = false, FloatDirection floatDir = FloatDirection.None) {
        if (!targetUI) {
            Debug.LogWarning("Target UI is null!");
            return;
        }

        if (!fade && floatDir == FloatDirection.None) {
            targetUI.SetActive(isActive);
            return;
        }

        // If the application is quitting or this component is inactive, skip animations and set state immediately
        if (!Application.isPlaying || isQuitting || !isActiveAndEnabled || !gameObject.activeInHierarchy) {
            if (isActive) {
                targetUI.SetActive(true);
                var cg = targetUI.GetComponent<CanvasGroup>();
                if (cg)
                    cg.alpha = 1f;
            } else {
                var cg = targetUI.GetComponent<CanvasGroup>();
                if (cg)
                    cg.alpha = 0f;
                targetUI.SetActive(false);
            }
            return;
        }

        CanvasGroup canvasGroup = targetUI.GetComponent<CanvasGroup>();
        if (!canvasGroup) {
            Debug.LogWarning($"CanvasGroup is not found in the target object: {targetUI.name}");
            targetUI.SetActive(isActive);
            return;
        }

        if (isActive)
            targetUI.SetActive(true);

        // Cancel any previous animation for this target and start a new one
        if (activeUiAnimations.TryGetValue(targetUI, out Coroutine running)) {
            // Restore any button state that might have been disabled by the previous animation
            RestoreTargetUIButtonState(targetUI);
            if (running != null)
                StopCoroutine(running);
            activeUiAnimations.Remove(targetUI);
        }

        Coroutine coroutine = StartCoroutine(AnimateUICoroutine(targetUI, canvasGroup, isActive, fade, floatDir));
        activeUiAnimations[targetUI] = coroutine;
    }

    private IEnumerator AnimateUICoroutine(GameObject targetUI,
                                            CanvasGroup canvasGroup,
                                            bool show,
                                            bool fade,
                                            FloatDirection floatDir) 
    {
        float targetAlpha = show ? 1f : 0f;
        float startAlpha = canvasGroup.alpha; // start from current alpha for smooth interruption handling
        float elapsedTime = 0f;

        Button button = targetUI.GetComponent<Button>();
        bool buttonInteractableOriginalValue = false;
        if (button) {
            // Store original interactable state if not stored yet
            if (!originalButtonInteractable.ContainsKey(targetUI))
                originalButtonInteractable[targetUI] = button.interactable;
            buttonInteractableOriginalValue = originalButtonInteractable[targetUI];
            button.interactable = false;
        }

        if (fade)
            canvasGroup.alpha = startAlpha;

        RectTransform rectTransform = null;
        Vector2 startPos = Vector2.zero;
        Vector2 targetPos = Vector2.zero;
        Vector2 basePosition = Vector2.zero;

        if (floatDir != FloatDirection.None) {
            rectTransform = targetUI.GetComponent<RectTransform>();
            basePosition = rectTransform.anchoredPosition;

            // Calculate start and target positions with original direction on entry, reversed on exit
            FloatDirection effectiveDirection = show ? floatDir : GetReverseDirection(floatDir);
            GetAnimationPositions(effectiveDirection, show, basePosition, out startPos, out targetPos);

            rectTransform.anchoredPosition = startPos;
        }

        while (elapsedTime < fadeAnimationDuration) {
            if (fade)
                canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / fadeAnimationDuration);

            if (floatDir != FloatDirection.None && rectTransform)
                rectTransform.anchoredPosition =
                    Vector2.Lerp(startPos, targetPos, elapsedTime / floatAnimationDuration);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (fade)
            canvasGroup.alpha = targetAlpha;

        if (floatDir != FloatDirection.None && rectTransform)
            rectTransform.anchoredPosition = show ? targetPos : basePosition;

        if (!show)
            targetUI.SetActive(false);

        if (button)
            button.interactable = buttonInteractableOriginalValue;

        // Mark this target as no longer animating
        if (activeUiAnimations.ContainsKey(targetUI))
            activeUiAnimations.Remove(targetUI);
        // Clear stored original button state after completion
        if (originalButtonInteractable.ContainsKey(targetUI))
            originalButtonInteractable.Remove(targetUI);
    }

    private void RestoreTargetUIButtonState(GameObject targetUI) {
        Button btn = targetUI ? targetUI.GetComponent<Button>() : null;
        if (!btn)
            return;
        if (originalButtonInteractable.TryGetValue(targetUI, out bool original)) {
            btn.interactable = original;
            originalButtonInteractable.Remove(targetUI);
        } else {
            // If we don't have a stored value, ensure it's at least enabled
            btn.interactable = true;
        }
    }

    private FloatDirection GetReverseDirection(FloatDirection direction) {
        switch (direction) {
            case FloatDirection.Left:
                return FloatDirection.Right;
            case FloatDirection.Right:
                return FloatDirection.Left;
            case FloatDirection.Up:
                return FloatDirection.Down;
            case FloatDirection.Down:
                return FloatDirection.Up;
            default:
                return FloatDirection.None;
        }
    }

    private void GetAnimationPositions(FloatDirection floatDir, bool show, Vector2 basePosition, out Vector2 startPos,
        out Vector2 targetPos) {
        startPos = basePosition;
        targetPos = basePosition;

        switch (floatDir) {
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
    public void PlaySaveAnimation()
    {
        saveImage.GetComponent<Animator>().SetTrigger("Play");
        saveText.GetComponent<Animator>().SetTrigger("Play");
    }

    public void SetMenuUI(bool startSceneButtonClick = false) {
        if (startSceneButtonClick)
            menuOpenByStartSceneButton = true;

        bool isMenuUIOpen = GetUI(eUIGameObjectName.MenuUI).activeInHierarchy;
        bool isOptionUIOpen = GetUI(eUIGameObjectName.OptionUI).activeInHierarchy;
        if (isMenuUIOpen || isOptionUIOpen) {
            if (isMenuUIOpen) {
                SetUI(eUIGameObjectName.MenuUI, false);
                SetUI(eUIGameObjectName.Menu, false);
            } else {
                SetUI(eUIGameObjectName.OptionUI, false);
            }
            
            if (menuOpenByStartSceneButton) {
                LobbyManager.Instance.lobbyButtons.SetActive(true);
                SetUI(eUIGameObjectName.AlbumButton, true);
                menuOpenByStartSceneButton = false;
            }
            SetTimeScale();
            InputManager.Instance.IgnoreInput = false;
        }
        else {
            SetUI(eUIGameObjectName.MenuUI, true);
            SetUI(eUIGameObjectName.Menu, true);

            SceneType currentScene = GameSceneManager.Instance.GetActiveScene();
            SetUI(eUIGameObjectName.GoToTitleButton, currentScene is not SceneType.START);
            SetMenuColor(currentScene is not (SceneType.ROOM_1 or SceneType.FOLLOW_1));
            Time.timeScale = 0f;
            InputManager.Instance.IgnoreInput = true;
        }
    }

    private void SetMenuColor(bool isWhite)
    {
        for (int i = 0; i < 3; i++) {
            Transform child = GetUI(eUIGameObjectName.Menu).transform.GetChild(i);
            child.GetComponent<Image>().sprite = optionButtonImages[i % 2 + (isWhite ? 0 : 2)];
            child.GetChild(0).GetComponent<TextMeshProUGUI>().color = isWhite ? Color.black : Color.white; // font color is opposite
        }
    }

    public void SetTimeScale() {
        Time.timeScale = GameManager.Instance.isDebug ? 4f : 1f;
    }

    public void SetMenuOpenByStartSceneButton() {
        menuOpenByStartSceneButton = true;
    }

    public void SetOptionUI() {
        SetUI(eUIGameObjectName.BGMSlider, true);
        SetUI(eUIGameObjectName.SoundEffectSlider, true);
        SetUI(eUIGameObjectName.BGMValue, true);
        SetUI(eUIGameObjectName.SoundEffectValue, true);
    }

    public void OnLeftButtonClick() {
        if (DialogueManager.Instance && DialogueManager.Instance.isDialogueActive)
            return;
        
        switch (GameSceneManager.Instance.GetActiveScene()) {
            case SceneType.ROOM_1:
            case SceneType.ROOM_2:
                RoomManager.Instance.MoveSides(-1);
                break;
        }
    }

    public void OnRightButtonClick() {
        if (DialogueManager.Instance && DialogueManager.Instance.isDialogueActive)
            return;
        
        switch (GameSceneManager.Instance.GetActiveScene()) {
            case SceneType.ROOM_1:
            case SceneType.ROOM_2:
                RoomManager.Instance.MoveSides(1);
                break;
        }
    }

    public void OnExitButtonClick() {
        if (MemoManager.Instance && MemoManager.Instance.isMemoOpen) {
            MemoManager.Instance.OnExit();
        }
        else {
            switch (GameSceneManager.Instance.GetActiveScene()) {
                case SceneType.START:
                    SetUI(eUIGameObjectName.Album, false, true, FloatDirection.Down);
                    SetUI(eUIGameObjectName.AlbumButton, true);
                    SetUI(eUIGameObjectName.ExitButton, false);
                    SetUI(eUIGameObjectName.BlurImage, false, true);
                    break;
                case SceneType.ENDING:
                    SetUI(eUIGameObjectName.Album, false, true, FloatDirection.Down);
                    SetUI(eUIGameObjectName.AlbumButton, false);
                    SetUI(eUIGameObjectName.ExitButton, false);
                    SetUI(eUIGameObjectName.BlurImage, false, true);
                    break;
                case SceneType.ROOM_1:
                case SceneType.ROOM_2:
                    RoomManager.Instance.OnExitButtonClick();
                    break;
                default:
                    Debug.Log("Exit button is not implemented in this scene.");
                    break;
            }
        }

        SetCursorAuto();
    }

    // 방에서 이동 버튼 눌렀을 때
    public void MoveSideEffect(GameObject screen, Vector3 direction) {
        StartCoroutine(OnMoveUI(screen, direction, 100, 0.5f));
        StartCoroutine(OnFade(null, 0, 1, 0, true, 0.25f, 0.25f));
    }

    public void FollowEventButtonSet(FollowObject followObject)
    {
        AnimateUI(followEventButton, true, true);
        followEventButtonImage.SetActive(true);
        followEventButtonNextButton.SetActive(true);

        followEventButtonImage.GetComponent<Image>().sprite = followObject.specialSprite;
        followEventButtonImage.GetComponent<Image>().SetNativeSize();
        followEventButtonImage.GetComponent<RectTransform>().localScale = new Vector3(followObject.scaleValue, followObject.scaleValue, followObject.scaleValue);

        followEventButtonNextButton.GetComponent<Button>().onClick.RemoveAllListeners();
        followEventButtonNextButton.GetComponent<Button>().onClick.AddListener(() => followObject.OnMouseDown_Normal());
        followEventButtonNextButton.GetComponent<Button>().onClick.AddListener(() => followEventButtonNextButton.SetActive(false));
        followEventButtonNextButton.GetComponent<Button>().onClick.AddListener(() => AnimateUI(followEventButton, false, true));
    }

    // <summary> 변수 설명
    // fadeObject는 fade 효과를 적용할 물체 (null을 주면 화면 전체)
    // start = 1, end = 0 이면 밝아짐 start = 0, end = 1이면 어두워짐
    // fadeTime은 밝아짐(또는 어두워짐)에 걸리는 시간
    // blink가 true이면 어두워졌다가 밝아짐
    // waitingTime은 blink가 true일 때 어두워져 있는 시간
    // changeFadeTime은 다시 밝아질 때 걸리는 시간을 조정하고 싶으면 쓰는 변수
    // </summary>
    public IEnumerator OnFade(Image fadeObject, float start, float end, float fadeTime, bool blink = false,
        float waitingTime = 0f, float changeFadeTime = 0f) {
        if (!fadeObject)
            fadeObject = coverPanel;

        if (!fadeObject.gameObject.activeSelf)
            fadeObject.gameObject.SetActive(true);
        
        Color fadeObjectColor = fadeObject.color;
        fadeObjectColor.a = start;
        fadeObject.color = fadeObjectColor;

        float t = 0f;
        while (t < 1f && fadeTime != 0f) {
            t += Time.unscaledDeltaTime / fadeTime;
            float a = Mathf.Lerp(start, end, t);

            fadeObjectColor.a = a;
            fadeObject.color = fadeObjectColor;

            coverText.color = new Color(1f, 1f, 1f, a);
            if (progressBarGroup) 
                progressBarGroup.alpha = a;
            if (Loading_AnimationGroup) 
                Loading_AnimationGroup.alpha = a;

            yield return null;
        }

        fadeObjectColor.a = end;
        fadeObject.color = fadeObjectColor;
        coverText.color = new Color(1f, 1f, 1f, end);
        if (progressBarGroup) 
            progressBarGroup.alpha = end;
        if (Loading_AnimationGroup) 
            Loading_AnimationGroup.alpha = end;

        if (blink) { // 곧바로 다시 어두워지거나 밝아지게 하고 싶을 때
            yield return new WaitForSecondsRealtime(waitingTime);
            StartCoroutine(OnFade(fadeObject, end, start, fadeTime + changeFadeTime, false, 0, 0));
            yield break;
        }

        if (fadeObject == coverPanel && Mathf.Approximately(end, 0f)) { // 투명해졌으면 끈다
            progressBarGroup.gameObject.SetActive(false);
            Loading_AnimationGroup.gameObject.SetActive(false);
            coverText.gameObject.SetActive(false);
            fadeObject.gameObject.SetActive(false);
        }
    }

    public IEnumerator OnFadeText(TMP_Text text, float startAlpha, float endAlpha, float fadeTime, bool deactivateOnEnd = false)
    {
        if (text == null) yield break;

        // 시작 알파 적용
        Color c = text.color;
        c.a = startAlpha;
        text.color = c;

        float elapsed = 0f;

        while (elapsed < fadeTime)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / fadeTime);

            float a = Mathf.Lerp(startAlpha, endAlpha, t);
            c = text.color;
            c.a = a;
            text.color = c;

            yield return null;
        }

        // 최종 알파 보정
        c = text.color;
        c.a = endAlpha;
        text.color = c;

        // 필요 시 오브젝트 비활성화
        if (deactivateOnEnd)
            text.gameObject.SetActive(false);
    }

    public void ResetLoadingUI() {
        if (coverText) 
            coverText.gameObject.SetActive(false);
        if (progressBarGroup) 
            progressBarGroup.gameObject.SetActive(false);
        if (Loading_AnimationGroup) 
            Loading_AnimationGroup.gameObject.SetActive(false);
        if (progressBar) 
            progressBar.fillAmount = 0f;
        foreach (GameObject character in loading_characters)
            if (character)
                character.gameObject.SetActive(false);
    }

    public void TextOnFade(string text) {
        coverText.gameObject.SetActive(true);
        coverText.text = text;
    
        progressBarGroup.gameObject.SetActive(true);
        Loading_AnimationGroup.gameObject.SetActive(true);
    }

    public IEnumerator SetLoadingAnimation(float start, float end, float fadeTime) {  // 로딩씬의 로딩 캐릭터 애니메이션 활성화 및 알파값 0->1 / 1->0
        int accidyGender = (int)GameManager.Instance.GetVariable("AccidyGender");
        loading_characters[accidyGender].gameObject.SetActive(true); // 우연의 성별에 맞게 캐릭터 애니메이션 재생 작동
        loading_characters[1 - accidyGender].gameObject.SetActive(false);

        float current = 0, percent = 0;
        while (percent < 1 && fadeTime != 0) {
            current += Time.deltaTime;
            percent = current / fadeTime;

            Loading_AnimationGroup.alpha = Mathf.Lerp(start, end, percent);
            yield return null;
        }
    }
    
    // <summary> 변수 설명
    // 화면 이동할 때 사용하기 위해 만든 거라 동작이 조금 특이합니다...
    // screen의 현재 위치가 목적지로 설정이 되고,
    // 출발지점은 현재 위치(목적지)에서 direction 방향으로 distance 만큼 이동한 곳이 됩니다
    // </summary>
    public IEnumerator OnMoveUI(GameObject screen, Vector3 direction, float distance, float time) {
        RectTransform screenRectTransform = screen.GetComponent<RectTransform>();
        var localPosition = screenRectTransform.localPosition; // 원래 위치 (목적지)
        Vector3 originPosition = localPosition;
        Vector3 startPosition = localPosition + direction * distance; // 출발 지점
        screen.GetComponent<RectTransform>().localPosition = startPosition;

        float elapsedTime = 0f;
        while (elapsedTime < time) {
            elapsedTime += Time.deltaTime;
            float percent = Mathf.Clamp01(elapsedTime / time);
            screen.transform.localPosition = Vector3.Lerp(startPosition, originPosition, percent);

            yield return null;
        }

        screen.GetComponent<RectTransform>().localPosition = originPosition; // 원래 위치로 설정
    }

    public void ChangeBgmOrSoundEffectValue(bool isChangeBGM) {
        float sliderValue = uiGameObjects[isChangeBGM
                ? eUIGameObjectName.BGMSlider
                : eUIGameObjectName.SoundEffectSlider].GetComponent<Slider>().value;
        SoundPlayer.Instance.ChangeVolume(isChangeBGM, sliderValue);
        uiGameObjects[isChangeBGM
                ? eUIGameObjectName.BGMValue
                : eUIGameObjectName.SoundEffectValue].GetComponent<TextMeshProUGUI>().text = (sliderValue * 100).ToString("F0");
    }

    public void ChangeSliderValue(eUIGameObjectName uiName, float absoluteValue, float addValue) {
        Slider slider = uiGameObjects[uiName].GetComponent<Slider>();
        slider.value = addValue == 0 ? absoluteValue : slider.value + addValue;
    }

    private void InitializeUIToCheck() {
        uiToCheck = new List<RectTransform> { // Manually add all UI GameObjects to check here
            leftButton.GetComponent<RectTransform>(),
            rightButton.GetComponent<RectTransform>(),
            exitButton.GetComponent<RectTransform>(),
            memoButton.GetComponent<RectTransform>(),
            actionPointsBackgroundImage.GetComponent<RectTransform>()
        };
    }

    public void AddUIToCheck(RectTransform uiRectTransform) {
        uiToCheck.Add(uiRectTransform);
    }

    public void RemoveUIToCheck(RectTransform uiRectTransform) {
        uiToCheck.Remove(uiRectTransform);
    }

    private void CheckCursorTouchingUIs() { // Check if the cursor is touching any of the buttons
        bool isCursorOverUIs = false;
        for (int i = uiToCheck.Count - 1; i >= 0; i--) {
            RectTransform uiRectTransform = uiToCheck[i];
            if (uiRectTransform == null) {
                uiToCheck.RemoveAt(i);
                continue;
            }

            GameObject go = uiRectTransform.gameObject;
            if ((go && go.activeSelf) == false)
                continue;

            if (RectTransformUtility.RectangleContainsScreenPoint(uiRectTransform, Input.mousePosition, uiCamera) == false)
                continue;

            isCursorOverUIs = true;
            break;
        }

        SetIsCursorTouchingUI(isCursorOverUIs);
    }

    private void SetIsCursorTouchingUI(bool isTouching) {
        bool previousState = isCursorTouchingUI;
        isCursorTouchingUI = isTouching;
        if (isCursorTouchingUI != previousState) // only call SetCursorAuto if state changes
            SetCursorAuto();
    }

    // method to switch mouse cursor
    public void SetCursorAuto() {
        bool isDefault = GameManager.Instance.GetIsBusy()
                         || GameSceneManager.Instance.GetActiveScene() is SceneType.START or SceneType.ENDING
                         || (RoomManager.Instance && RoomManager.Instance.imageAndLockPanelManager.isLockObjectActive)
                         || isCursorTouchingUI;
        ChangeCursor(isDefault);
    }

    public void ChangeCursor(bool isDefault = true) {
        Texture2D mouseCursorTexture = isDefault ? defaultCursorTexture : investigateCursorTexture;
        Cursor.SetCursor(mouseCursorTexture, Vector2.zero, CursorMode.Auto);
    }

    public void OpenAlbumPage(int endingIndex) {
        albumImage.sprite = endingSprites[endingIndex * 2 + (int)GameManager.Instance.GetVariable("AccidyGender")];
        string[] scriptIds = {
            "Dialogue_0417", 
            "Dialogue_0618", 
            "Dialogue_0678", 
            "Dialogue_0777"
        };
        string scriptId = scriptIds[endingIndex];
        endingNameText.text = DialogueManager.Instance.scripts[scriptId].GetScript().ProcessedText;
        LocalizedText localizedText = endingNameText.GetComponent<LocalizedText>();
        localizedText.SetScriptId(scriptId);

        albumImageGameObject.SetActive(true);
        endingNameGameObject.SetActive(true);
        albumPage.SetActive(true);
    }

    public float PlayEndingAlbumAnimation(EndingType endingType)
    {
        int endingIndex = 0;
        string endingVariable = "";
        switch (endingType)
        {
            case EndingType.BAD_A:
                endingIndex = 0;
                endingVariable = "BadACollect";
                break;
            case EndingType.BAD_B:
                endingIndex = 1;
                endingVariable = "BadBCollect";
                break;
            case EndingType.TRUE:
                endingIndex = 0;
                endingVariable = "TrueCollect";
                break;
            case EndingType.HIDDEN:
                endingIndex = 0;
                endingVariable = "HiddenCollect";
                break;
        }

        RectTransform target = targetAlbumImages[endingIndex].GetComponent<RectTransform>();
        if ((int)GameManager.Instance.GetVariable(endingVariable) > 0) // 이전에 본 엔딩이면 다른 연출
        {
            StartCoroutine(AnimationRoutine(target, false));
            return 1;
        }

        albumImageDestination = target.position;
        target.gameObject.SetActive(false);

        SetUI(eUIGameObjectName.AlbumEndingImage,true, true, FloatDirection.Up);
        albumEndingImage.GetComponent<Image>().sprite = endingSprites[endingIndex * 2 + 1 - (int)GameManager.Instance.GetVariable("AccidyGender")];
        albumEndingImage.GetComponent<RectTransform>().position = new Vector3(0, 0, 10);
        albumEndingImage.GetComponent<RectTransform>().localScale = new Vector3(2, 2, 2);
        StartCoroutine(AnimationRoutine(albumEndingImage.GetComponent<RectTransform>(), true));

        return 2.5f;
    }

    private IEnumerator AnimationRoutine(RectTransform target, bool move)
    {
        // 회전 시퀀스 리스트
        float[] rotations = { 10f, -10f, 10f, -10f, 0f };

        foreach (float rot in rotations)
        {
            yield return StartCoroutine(RotateToZ(target, rot, 0.2f));
        }

        // 이동 & 스케일 축소
        if (move) StartCoroutine(MoveAndScale(target, 1f));
    }

    private IEnumerator RotateToZ(RectTransform target, float targetZ, float duration)
    {
        float t = 0f;
        float startZ = target.localEulerAngles.z;

        // 360도 wrap-around 보정
        if (startZ > 180) startZ -= 360;

        while (t < duration)
        {
            t += Time.deltaTime;
            float z = Mathf.Lerp(startZ, targetZ, t / duration);
            target.localEulerAngles = new Vector3(0, 0, z);
            yield return null;
        }

        target.localEulerAngles = new Vector3(0, 0, targetZ);
    }

    private IEnumerator MoveAndScale(RectTransform target, float duration)
    {
        float t = 0f;
        Vector3 startPos = target.anchoredPosition;
        Vector3 startScale = target.localScale;
        Vector3 endScale = Vector3.one;

        while (t < duration)
        {
            t += Time.deltaTime;
            float lerp = Mathf.SmoothStep(0, 1, t / duration);

            target.position = Vector3.Lerp(startPos, albumImageDestination, lerp);
            target.localScale = Vector3.Lerp(startScale, endScale, lerp);

            yield return null;
        }

        target.position = albumImageDestination;
        target.localScale = endScale;
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
        float fadeOutDuration = 0.5f) {
        SetUI(eUIGameObjectName.WarningVignette, true); // 경고 표시 활성화

        float timeAccumulated = 0;
        while (timeAccumulated < fadeInDuration) {
            timeAccumulated += Time.deltaTime;
            warningVignetteQVignetteSingle.mainColor.a = Mathf.Lerp(startAlpha,
                endAlpha,
                timeAccumulated / fadeInDuration); // WarningVignette 투명도를 0에서 1로 선형 보간(Lerp)

            yield return null;
        }

        yield return new WaitForSeconds(warningTime); // warningTime 동안 경고 상태 유지

        timeAccumulated = 0; // 경고 종료: WarningVignette.mainColor.a를 다시 0으로 페이드 아웃
        while (timeAccumulated < fadeOutDuration) {
            timeAccumulated += Time.deltaTime * 2; // 페이드 아웃 속도를 더 빠르게 설정
            warningVignetteQVignetteSingle.mainColor.a = Mathf.Lerp(endAlpha,
                startAlpha,
                timeAccumulated / fadeOutDuration); // WarningVignette 투명도를 1에서 0으로 선형 보간(Lerp)

            yield return null;
        }

        SetUI(eUIGameObjectName.WarningVignette, false); // 경고 표시 비활성화
    }

    public void ToggleHighlightAnimationEffect(GameObject gameObject, bool isOn) {
        gameObject.GetComponent<HighlightAnimator>().ToggleHighlight(isOn);
    }

    public void ToggleHighlightAnimationEffect(eUIGameObjectName uiName, bool isOn) {
        ToggleHighlightAnimationEffect(uiGameObjects[uiName], isOn);
    }

    public IEnumerator HideTemporarily(eUIGameObjectName targetUIGameObjectName, float duration)
    {
        SetUI(targetUIGameObjectName, false);
        yield return new WaitForSeconds(duration);
        SetUI(targetUIGameObjectName, true);
    }
}