using System.Collections;
using System.Collections.Generic;
using TMPro;
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

    private Dictionary<string, GameObject> uiGameObjects = new Dictionary<string, GameObject>();
    private Q_Vignette_Single warningVignetteQVignetteSingle;
    [HideInInspector] public TextMeshProUGUI dayTextTextMeshProUGUI;
    
    [Header("Warning Vignette Settings")]
    [SerializeField] protected float warningTime;
    
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
        uiGameObjects.Add(eUIGameObjectName.NormalVignette.ToString(), normalVignette);
        uiGameObjects.Add(eUIGameObjectName.WarningVignette.ToString(), warningVignette);

        uiGameObjects.Add(eUIGameObjectName.ActionPoints.ToString(), actionPoints);
        uiGameObjects.Add(eUIGameObjectName.HeartParent.ToString(), heartParent);
        uiGameObjects.Add(eUIGameObjectName.DayText.ToString(), dayText);

        uiGameObjects.Add(eUIGameObjectName.ExitButton.ToString(), exitButton);
        uiGameObjects.Add(eUIGameObjectName.LeftButton.ToString(), leftButton);
        uiGameObjects.Add(eUIGameObjectName.RightButton.ToString(), rightButton);

        uiGameObjects.Add(eUIGameObjectName.MemoContents.ToString(), memoContents);
        uiGameObjects.Add(eUIGameObjectName.MemoGauge.ToString(), memoGauge);

        uiGameObjects.Add(eUIGameObjectName.MenuUI.ToString(), menuUI);
        uiGameObjects.Add(eUIGameObjectName.WhiteMenu.ToString(), whiteMenu);
        uiGameObjects.Add(eUIGameObjectName.BlackMenu.ToString(), blackMenu);

        uiGameObjects.Add(eUIGameObjectName.OptionUI.ToString(), optionUI);
        uiGameObjects.Add(eUIGameObjectName.BGMSlider.ToString(), BGMSlider);
        uiGameObjects.Add(eUIGameObjectName.SoundEffectSlider.ToString(), SoundEffectSlider);
        uiGameObjects.Add(eUIGameObjectName.BGMValue.ToString(), BGMValueText);
        uiGameObjects.Add(eUIGameObjectName.SoundEffectValue.ToString(), SoundEffectValueText);

        uiGameObjects.Add(eUIGameObjectName.FollowMemoGauge.ToString(), followMemoGauge);

        uiGameObjects.Add(eUIGameObjectName.FollowUIBackground.ToString(), followUIBackground);

        uiGameObjects.Add(eUIGameObjectName.DoubtGaugeSlider.ToString(), doubtGaugeSlider);

        uiGameObjects.Add(eUIGameObjectName.FatePositionSlider.ToString(), fatePositionSlider);
        uiGameObjects.Add(eUIGameObjectName.AccidyPositionSlider.ToString(), accidyPositionSlider);

        warningVignetteQVignetteSingle = warningVignette.GetComponent<Q_Vignette_Single>();
        dayTextTextMeshProUGUI = dayText.GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) SetMenuUI();
    }

    public void SetAllUI(bool isActive)
    {
        foreach (var ui in uiGameObjects)
            SetUI(ui.Key, isActive);
    }

    public void SetUI(string uiName, bool isActive)
    {
        uiGameObjects[uiName].SetActive(isActive);
    }
    public void SetUI(eUIGameObjectName uiEnum, bool isActive)
    {
        SetUI(uiEnum.ToString(), isActive);
    }

    public GameObject GetUI(string uiName)
    {
        return uiGameObjects[uiName];
    }
    public GameObject GetUI(eUIGameObjectName uiEnum)
    {
        return uiGameObjects[uiEnum.ToString()];
    }

    public void SetMenuUI()
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
            if (Random.Range(0, 2) == 0)
                SetUI(eUIGameObjectName.WhiteMenu, true);
            else
                SetUI(eUIGameObjectName.BlackMenu, true);
            Time.timeScale = 0f;
        }
    }

    public void SetTimeScale()
    {
        Time.timeScale = 1f;
    }
    public void SetOptionUI()
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
            text = uiGameObjects[eUIGameObjectName.BGMValue.ToString()].GetComponent<TextMeshProUGUI>();
            slider = uiGameObjects[eUIGameObjectName.BGMSlider.ToString()].GetComponent<Slider>();
            SoundPlayer.Instance.ChangeVolume(slider.value, -1);
        }
        else
        {
            text = uiGameObjects[eUIGameObjectName.SoundEffectValue.ToString()].GetComponent<TextMeshProUGUI>();
            slider = uiGameObjects[eUIGameObjectName.SoundEffectSlider.ToString()].GetComponent<Slider>();
            SoundPlayer.Instance.ChangeVolume(-1, slider.value);
        }

        text.text = (slider.value * 100).ToString("F0");
    }

    public void ChangeSliderValue(string uiName, float absoluteValue, float addValue)
    {
        Slider slider = uiGameObjects[uiName].GetComponent<Slider>();
        if (addValue != 0) slider.value += addValue;
        else slider.value = absoluteValue;
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
        SetUI(eUIGameObjectName.WarningVignette.ToString(), true); // 경고 표시 활성화
        
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

        SetUI(eUIGameObjectName.WarningVignette.ToString(), false); // 경고 표시 비활성화
    }
    
    private int GetCurrentSceneIndex()
    {
        return (int)GameManager.Instance.GetVariable("CurrentScene");
    }
}
