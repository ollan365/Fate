using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    ObjectImageParentRoom, // object image panel parent in room scene
    ObjectImageRoom, // object image panel in room scene
    NormalVignette,
    WarningVignette,
    ActionPoints,
    ActionPointsBackgroundImage,
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
    FollowUI,
    FollowUIBackground,
    DoubtGaugeSlider,
    FatePositionSlider,
    AccidyPositionSlider,
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
    AlbumImage,
    AlbumImageGameObject,
    EndingTypeGameObject,
    EndingNameGameObject,
}

public class UIManager : MonoBehaviour {
    [Header("Screen Effect")]
    public Image coverPanel;
    [SerializeField] private TextMeshProUGUI coverText;
    
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
    
    [Header("UI Game Objects - Album")]
    public GameObject album;
    public GameObject albumButton;
    public GameObject albumPage;
    public GameObject albumImageGameObject;
    public GameObject endingTypeGameObject;
    public GameObject endingNameGameObject;
    public Sprite[] endingSprites;

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
    private TextMeshProUGUI endingTypeText;
    private TextMeshProUGUI endingNameText;
    private Image albumImage;
    
    [Header("UI Game Objects - Menu")]
    public GameObject menuUI;
    public GameObject whiteMenu;
    public GameObject blackMenu;
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

    private readonly Dictionary<eUIGameObjectName, GameObject> uiGameObjects = new();
    private Q_Vignette_Single warningVignetteQVignetteSingle;
    [HideInInspector] public TextMeshProUGUI dayTextTextMeshProUGUI;
    
    [Header("Warning Vignette Settings")]
    [SerializeField] protected float warningTime;
    
    [Header("Cursor Settings")]
    public Texture2D defaultCursorTexture;
    public Texture2D investigateCursorTexture;

    [Header("UI Camera")]
    public Camera uiCamera;
    
    private bool isCursorTouchingUI;
    // UI GameObjects to explicitly check for cursor hover
    private List<RectTransform> uiToCheck;
    
    [Header("Animation Settings")]
    public float fadeAnimationDuration = 0.3f;
    public float floatAnimationDuration = 0.3f;
    [SerializeField] private float floatDistance = 50f;

    public static UIManager Instance { get; private set; }
    
    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else
            Destroy(gameObject);
        
        AddUIGameObjects();
        SetAllUI(false);
        SetOptionUI();
        InitializeUIToCheck();
        
        // UI Objects that should be active by default
        SetUI(eUIGameObjectName.ObjectImageRoom, true);
        SetUI(eUIGameObjectName.AlbumButton, true);
    }

    private void AddUIGameObjects() {
        uiGameObjects.Add(eUIGameObjectName.ObjectImageParentRoom, objectImageParentRoom);
        uiGameObjects.Add(eUIGameObjectName.ObjectImageRoom, objectImageRoom);
        
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

        uiGameObjects.Add(eUIGameObjectName.MenuUI, menuUI);
        uiGameObjects.Add(eUIGameObjectName.WhiteMenu, whiteMenu);
        uiGameObjects.Add(eUIGameObjectName.BlackMenu, blackMenu);
        
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
        uiGameObjects.Add(eUIGameObjectName.EndingTypeGameObject, endingTypeGameObject);
        uiGameObjects.Add(eUIGameObjectName.EndingNameGameObject, endingNameGameObject);

        // uiGameObjects.Add(eUIGameObjectName.FollowUIBackground, followUIBackground);

        yesterdayNumTextTextMeshProUGUI = yesterdayNumText.GetComponent<TextMeshProUGUI>();
        todayNumTextTextMeshProUGUI = todayNumText.GetComponent<TextMeshProUGUI>();
        yesterdayRectTransform = yesterday.GetComponent<RectTransform>();
        dayChangingGroupRectTransform = dayChangingGameObject.GetComponent<RectTransform>();

        warningVignetteQVignetteSingle = warningVignette.GetComponent<Q_Vignette_Single>();
        dayTextTextMeshProUGUI = dayText.GetComponent<TextMeshProUGUI>();
        
        endingTypeText = endingTypeGameObject.GetComponentInChildren<TextMeshProUGUI>();
        endingNameText = endingNameGameObject.GetComponent<TextMeshProUGUI>();
        albumImage = albumImageGameObject.GetComponent<Image>();
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) 
            SetMenuUI();
        
        CheckCursorTouchingUIs();
    }
    
    private void SetAllUI(bool isActive) {
        foreach (var ui in uiGameObjects)
            SetUI(ui.Key, isActive);
    }

    public void SetUI(eUIGameObjectName uiName,
        bool isActive,
        bool fade = false,
        FloatDirection floatDir = FloatDirection.None) {
        GameObject targetUI = uiGameObjects[uiName];
        AnimateUI(targetUI, isActive, fade, floatDir);
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

        CanvasGroup canvasGroup = targetUI.GetComponent<CanvasGroup>();
        if (!canvasGroup) {
            Debug.LogWarning($"CanvasGroup is not found in the target object: {targetUI.name}");
            targetUI.SetActive(isActive);
            return;
        }

        if (isActive)
            targetUI.SetActive(true);

        string coroutineName = $"Animate_{targetUI.name}";
        if (IsInvoking(coroutineName))
            StopCoroutine(coroutineName);
        StartCoroutine(AnimateUICoroutine(targetUI, canvasGroup, isActive, fade, floatDir));
    }

    private IEnumerator AnimateUICoroutine(GameObject targetUI,
        CanvasGroup canvasGroup,
        bool show,
        bool fade,
        FloatDirection floatDir) {
        float targetAlpha = show ? 1f : 0f;
        float startAlpha = show ? 0f : 1f;
        float fadeTime = fadeAnimationDuration;
        float elapsedTime = 0f;
        
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
        
        while (elapsedTime < fadeTime) {
            if (fade)
                canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / fadeTime);
            
            if (floatDir != FloatDirection.None && rectTransform)
                rectTransform.anchoredPosition = Vector2.Lerp(startPos, targetPos, elapsedTime / floatAnimationDuration);
            
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        if (fade)
            canvasGroup.alpha = targetAlpha;
        
        if (floatDir != FloatDirection.None && rectTransform)
            rectTransform.anchoredPosition = show ? targetPos : basePosition;
        
        if (!show)
            targetUI.SetActive(false);
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

    private void GetAnimationPositions(FloatDirection floatDir, bool show, Vector2 basePosition, out Vector2 startPos, out Vector2 targetPos) {
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

    public void SetMenuUI(bool startSceneButtonClick = false)
    {
        if(startSceneButtonClick) menuOpenByStartSceneButton = startSceneButtonClick;

        if (GetUI(eUIGameObjectName.MenuUI).activeSelf)
        {
            SetUI(eUIGameObjectName.MenuUI, false);
            SetUI(eUIGameObjectName.WhiteMenu, false);
            SetUI(eUIGameObjectName.BlackMenu, false);
            if (menuOpenByStartSceneButton)
            {
                StartLogic.Instance.SetButtons();
                menuOpenByStartSceneButton = false;
            }
            Time.timeScale = 1f;
        } else if (GetUI(eUIGameObjectName.OptionUI).activeSelf) {
            SetUI(eUIGameObjectName.OptionUI, false);
            if (menuOpenByStartSceneButton)
            {
                StartLogic.Instance.SetButtons();
                menuOpenByStartSceneButton = false;
            }
            Time.timeScale = 1f;
        } else {
            SetUI(eUIGameObjectName.MenuUI, true);
            SetUI(Random.Range(0, 2) == 0 
                ? eUIGameObjectName.WhiteMenu 
                : eUIGameObjectName.BlackMenu, true);
            Time.timeScale = 0f;
        }
    }

    public void SetTimeScale() {
        Time.timeScale = 1f;
    }

    private void SetOptionUI() {
        SetUI(eUIGameObjectName.BGMSlider, true);
        SetUI(eUIGameObjectName.SoundEffectSlider, true);
        SetUI(eUIGameObjectName.BGMValue, true);
        SetUI(eUIGameObjectName.SoundEffectValue, true);
    }
    
    public void OnLeftButtonClick() {
        switch (SceneManager.Instance.GetActiveScene()) {
            case (int)SceneType.ROOM_1:
            case (int)SceneType.ROOM_2:
                RoomManager.Instance.MoveSides(-1);
                break;
        }
    }
    
    public void OnRightButtonClick() {
        switch (SceneManager.Instance.GetActiveScene()) {
            case (int)SceneType.ROOM_1:
            case (int)SceneType.ROOM_2:
                RoomManager.Instance.MoveSides(1);
                break;
        }
    }
    
    public void OnExitButtonClick() {
        if (MemoManager.Instance && MemoManager.Instance.isMemoOpen)
            MemoManager.Instance.OnExit();
        else
            switch (SceneManager.Instance.GetActiveScene()) {
                case (int)SceneType.START:
                    SetUI(eUIGameObjectName.Album, false, true, FloatDirection.Down);
                    SetUI(eUIGameObjectName.AlbumButton, true);
                    SetUI(eUIGameObjectName.ExitButton, false);
                    break;
                case (int)SceneType.ROOM_1:
                case (int)SceneType.ROOM_2:
                    RoomManager.Instance.OnExitButtonClick();
                    break;
                default:
                    Debug.Log("Exit button is not implemented in this scene.");
                    break;
            }
        SetCursorAuto();
    }
    
    // 방에서 이동 버튼 눌렀을 때
    public void MoveButtonEffect(GameObject screen, Vector3 direction)
    {
        StartCoroutine(OnMoveUI(screen, direction, 100, 0.5f));
        StartCoroutine(OnFade(null, 0, 1, 0, true, 0.2f, +0.25f));
    }

    // <summary> 변수 설명
    // fadeObject는 fade 효과를 적용할 물체 (null을 주면 화면 전체)
    // start = 1, end = 0 이면 밝아짐 start = 0, end = 1이면 어두워짐
    // fadeTime은 밝아짐(또는 어두워짐)에 걸리는 시간
    // blink가 true이면 어두워졌다가 밝아짐
    // waitingTime은 blink가 true일 때 어두워져 있는 시간
    // changeFadeTime은 다시 밝아질 때 걸리는 시간을 조정하고 싶으면 쓰는 변수
    // </summary>
    public IEnumerator OnFade(Image fadeObject, float start, float end, float fadeTime, bool blink=false, float waitingTime=0f, float changeFadeTime=0f)
    {
        if (!fadeObject) 
            fadeObject = coverPanel;

        if (!fadeObject.gameObject.activeSelf) 
            fadeObject.gameObject.SetActive(true);
        Color newColor = fadeObject.color;
        newColor.a = start;
        fadeObject.color = newColor;

        float current = 0, percent = 0;

        while (percent < 1 && fadeTime != 0)
        {
            current += Time.deltaTime;
            percent = current / fadeTime;

            newColor.a = Mathf.Lerp(start, end, percent);
            fadeObject.color = newColor;

            coverText.color = new(1, 1, 1, Mathf.Lerp(start, end, percent));

            yield return null;
        }
        newColor.a = end;
        fadeObject.color = newColor;

        // 곧바로 다시 어두워지거나 밝아지게 하고 싶을 때
        if (blink)
        {
            yield return new WaitForSeconds(waitingTime);
            StartCoroutine(OnFade(fadeObject, end, start, fadeTime + changeFadeTime, false, 0, 0));
        }

        // 투명해졌으면 끈다
        if (fadeObject == coverPanel && end == 0)
        {
            fadeObject.gameObject.SetActive(false);
            coverText.gameObject.SetActive(false);
        }
    }

    public void TextOnFade(string text)
    {
        coverText.gameObject.SetActive(true);
        coverText.text = text;
    }
    // <summary> 변수 설명
    // 화면 이동할 때 사용하기 위해 만든 거라 동작이 조금 특이합니다...
    // screen의 현재 위치가 목적지로 설정이 되고,
    // 출발지점은 현재 위치(목적지)에서 direction 방향으로 distance 만큼 이동한 곳이 됩니다
    // </summary>
    public IEnumerator OnMoveUI(GameObject screen, Vector3 direction, float distance, float time)
    {
        RectTransform screenRectTransform = screen.GetComponent<RectTransform>();
        // 원래 위치 (목적지)
        var localPosition = screenRectTransform.localPosition;
        Vector3 originPosition = localPosition;
        // 출발 지점
        Vector3 startPosition = localPosition + direction * distance;
        screen.GetComponent<RectTransform>().localPosition = startPosition;

        float elapsedTime = 0f;
        while (elapsedTime < time)
        {
            elapsedTime += Time.deltaTime;
            float percent = Mathf.Clamp01(elapsedTime / time);

            screen.transform.localPosition = Vector3.Lerp(startPosition, originPosition, percent);

            yield return null;
        }

        // 원래 위치로 설정
        screen.GetComponent<RectTransform>().localPosition = originPosition;
    }
    
    public void ChangeSoundValue(string uiName) {
        TMP_Text text;
        Slider slider;

        if (uiName == eUIGameObjectName.BGMSlider.ToString()) {
            text = uiGameObjects[eUIGameObjectName.BGMValue].GetComponent<TextMeshProUGUI>();
            slider = uiGameObjects[eUIGameObjectName.BGMSlider].GetComponent<Slider>();
            SoundPlayer.Instance.ChangeVolume(slider.value, -1);
        }
        else {
            text = uiGameObjects[eUIGameObjectName.SoundEffectValue].GetComponent<TextMeshProUGUI>();
            slider = uiGameObjects[eUIGameObjectName.SoundEffectSlider].GetComponent<Slider>();
            SoundPlayer.Instance.ChangeVolume(-1, slider.value);
        }

        text.text = (slider.value * 100).ToString("F0");
    }

    public void ChangeSliderValue(eUIGameObjectName uiName, float absoluteValue, float addValue) {
        Slider slider = uiGameObjects[uiName].GetComponent<Slider>();
        if (addValue != 0) 
            slider.value += addValue;
        else 
            slider.value = absoluteValue;
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

    // Check if the cursor is touching any of the buttons
    private void CheckCursorTouchingUIs() {
        bool isCursorOverUIs = false;
        foreach (RectTransform uiRectTransform in uiToCheck) {
            GameObject uiGameObject = uiRectTransform.gameObject;
            if (!uiGameObject.activeSelf)
                continue;
            
            bool isCursorOverUI = RectTransformUtility.RectangleContainsScreenPoint(uiRectTransform,
                Input.mousePosition,
                uiCamera);
            if (isCursorOverUI) {
                isCursorOverUIs = true;
                break;
            }
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
                         || SceneManager.Instance.GetActiveScene() is (int)SceneType.START or (int)SceneType.ENDING
                         || (RoomManager.Instance && RoomManager.Instance.imageAndLockPanelManager.isLockObjectActive)
                         || isCursorTouchingUI;
        ChangeCursor(isDefault);
    }
    
    public void ChangeCursor(bool isDefault=true) {
        Texture2D mouseCursorTexture = isDefault ? defaultCursorTexture : investigateCursorTexture;
        Cursor.SetCursor(mouseCursorTexture, Vector2.zero, CursorMode.Auto);
    }
    
    public void OpenAlbumPage(int endingIndex) {
        Debug.Log($"Opening album page for ending index: {endingIndex}");
        const int badA = 0;
        const int badB = 1;
        const int trueEnding = 2;
        const int hidden = 3;
        albumImage.sprite = endingSprites[endingIndex * 2 + (int)GameManager.Instance.GetVariable("AccidyGender")];
        switch (endingIndex) {
            case badA:
                endingTypeText.text = "# 배드 엔딩 A";
                endingNameText.text = DialogueManager.Instance.scripts["Dialogue_0417"].GetScript();
                break;
            case badB:
                endingTypeText.text = "# 배드 엔딩 B";
                endingNameText.text = DialogueManager.Instance.scripts["Dialogue_0618"].GetScript();
                break;
            case trueEnding:
                endingTypeText.text = "# 트루 엔딩";
                endingNameText.text = DialogueManager.Instance.scripts["Dialogue_0678"].GetScript();
                break;
            case hidden:
                endingTypeText.text = "# 히든 엔딩";
                endingNameText.text = DialogueManager.Instance.scripts["Dialogue_0777"].GetScript();
                break;
        }
        albumImageGameObject.SetActive(true);
        endingTypeGameObject.SetActive(true);
        endingNameGameObject.SetActive(true);
        albumPage.SetActive(true);
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
}
