using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using static Constants;

public enum eUIGameObjectName
{
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
    GearMinuteHand
}

public class UIManager : MonoBehaviour
{
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
    
    [Header("Cursor Settings")]
    public Texture2D defaultCursorTexture;
    public Texture2D investigateCursorTexture;

    [Header("UI Camera")]
    public Camera uiCamera;
    
    private bool isCursorTouchingUI;
    // UI GameObjects to explicitly check for cursor hover
    private List<RectTransform> uiToCheck;

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
        InitializeUIToCheck();
        
        SetUI(eUIGameObjectName.ObjectImageRoom, true);
    }

    private void AddUIGameObjects()
    {
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

        // uiGameObjects.Add(eUIGameObjectName.OptionUI, optionUI);
        // uiGameObjects.Add(eUIGameObjectName.BGMSlider, BGMSlider);
        // uiGameObjects.Add(eUIGameObjectName.SoundEffectSlider, SoundEffectSlider);
        // uiGameObjects.Add(eUIGameObjectName.BGMValue, BGMValueText);
        // uiGameObjects.Add(eUIGameObjectName.SoundEffectValue, SoundEffectValueText);

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

        // uiGameObjects.Add(eUIGameObjectName.FollowUIBackground, followUIBackground);

        yesterdayNumTextTextMeshProUGUI = yesterdayNumText.GetComponent<TextMeshProUGUI>();
        todayNumTextTextMeshProUGUI = todayNumText.GetComponent<TextMeshProUGUI>();
        yesterdayRectTransform = yesterday.GetComponent<RectTransform>();
        dayChangingGroupRectTransform = dayChangingGameObject.GetComponent<RectTransform>();

        warningVignetteQVignetteSingle = warningVignette.GetComponent<Q_Vignette_Single>();
        dayTextTextMeshProUGUI = dayText.GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) 
            SetMenuUI();
        
        CheckCursorTouchingUIs();
    }
    
    private void SetAllUI(bool isActive)
    {
        foreach (var ui in uiGameObjects)
            SetUI(ui.Key, isActive);
    }

    public void SetUI(eUIGameObjectName uiName, bool isActive)
    {
        uiGameObjects[uiName].SetActive(isActive);
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
        else if (GetUI(eUIGameObjectName.OptionUI).activeSelf)
        {
            SetUI(eUIGameObjectName.OptionUI, false);
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
        switch (SceneManager.Instance.GetActiveScene())
        {
            case (int)SceneType.ROOM_1:
            case (int)SceneType.ROOM_2:
                RoomManager.Instance.MoveSides(-1);
                break;
        }
    }
    
    public void OnRightButtonClick()
    {
        switch (SceneManager.Instance.GetActiveScene())
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
            MemoManager.Instance.OnExit();
        else
            switch (SceneManager.Instance.GetActiveScene())
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
        SetCursorAuto();
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
    
    private void InitializeUIToCheck()
    {
        uiToCheck = new List<RectTransform> { // Add all UI GameObjects to check here
            leftButton.GetComponent<RectTransform>(), 
            rightButton.GetComponent<RectTransform>(), 
            exitButton.GetComponent<RectTransform>(),
            memoButton.GetComponent<RectTransform>(),
            actionPointsBackgroundImage.GetComponent<RectTransform>()
        };
    }
    
    public void AddUIToCheck(RectTransform uiRectTransform)
    {
        uiToCheck.Add(uiRectTransform);
    }
    
    public void RemoveUIToCheck(RectTransform uiRectTransform)
    {
        uiToCheck.Remove(uiRectTransform);
    }

    // Check if the cursor is touching any of the buttons
    private void CheckCursorTouchingUIs()
    {
        bool isCursorOverUIs = false;
        foreach (RectTransform uiRectTransform in uiToCheck)
        {
            GameObject uiGameObject = uiRectTransform.gameObject;
            if (!uiGameObject.activeSelf)
                continue;
            
            bool isCursorOverUI = RectTransformUtility.RectangleContainsScreenPoint(uiRectTransform,
                Input.mousePosition,
                uiCamera);
            if (isCursorOverUI)
            {
                isCursorOverUIs = true;
                break;
            }
        }

        SetIsCursorTouchingUI(isCursorOverUIs);
    }

    private void SetIsCursorTouchingUI(bool isTouching)
    {
        bool previousState = isCursorTouchingUI;
        isCursorTouchingUI = isTouching;
        if (isCursorTouchingUI != previousState) // only call SetCursorAuto if state changes
            SetCursorAuto(); 
    }
    
    // method to switch mouse cursor
    public void SetCursorAuto()
    {
        bool isDefault = GameManager.Instance.GetIsBusy()
                         || SceneManager.Instance.GetActiveScene() is (int)SceneType.START or (int)SceneType.ENDING
                         || (RoomManager.Instance && RoomManager.Instance.imageAndLockPanelManager.isLockObjectActive)
                         || isCursorTouchingUI;
        ChangeCursor(isDefault);
    }
    
    public void ChangeCursor(bool isDefault=true)
    {
        Texture2D mouseCursorTexture = isDefault ? defaultCursorTexture : investigateCursorTexture;
        Cursor.SetCursor(mouseCursorTexture, Vector2.zero, CursorMode.Auto);
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
            timeAccumulated += Time.deltaTime * 2; // 페이드 아웃 속도를 더 빠르게 설정
            warningVignetteQVignetteSingle.mainColor.a = Mathf.Lerp(endAlpha,
                startAlpha,
                timeAccumulated / fadeOutDuration); // WarningVignette 투명도를 1에서 0으로 선형 보간(Lerp)

            yield return null;
        }

        SetUI(eUIGameObjectName.WarningVignette, false); // 경고 표시 비활성화
    }
}
