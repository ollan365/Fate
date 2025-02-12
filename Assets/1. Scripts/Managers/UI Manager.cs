using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Constants;

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

    [Header("UI Game Objects - Follow")]
    public GameObject memoGauge;
    public GameObject gaugeImage; // Image
    public GameObject clearFlagSlider; // Slider
    public GameObject clearFlageImage; //Image
    public GameObject[] doubtGaugeSliders; // Slider[]
    public GameObject[] overHeadDoubtGaugeSliderImages; // Image[]
    public GameObject accidyDialogueBox;
    public GameObject fatePositionSlider; // Scrollbar -> Slider
    public GameObject accidyPositionSlider; // Scrollbar -> Slider

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
    }

    private void AddUIGameObjects()
    {
        uiGameObjects.Add("NormalVignette", normalVignette);
        uiGameObjects.Add("WarningVignette", warningVignette);

        uiGameObjects.Add("ActionPoints", actionPoints);
        uiGameObjects.Add("HeartParent", heartParent);
        uiGameObjects.Add("DayText", dayText);

        uiGameObjects.Add("ExitButton", exitButton);
        uiGameObjects.Add("LeftButton", leftButton);
        uiGameObjects.Add("RightButton", rightButton);

        uiGameObjects.Add("MemoButton", memoButton);
        uiGameObjects.Add("MemoContents", memoContents);
        uiGameObjects.Add("MemoGaugeRoom", memoGaugeRoom);
        uiGameObjects.Add("MemoGaugeFollow", memoGaugeFollow);

        uiGameObjects.Add("DoubtGaugeSlider", doubtGaugeSliders[0]);
        uiGameObjects.Add("OverheadDoubtGaugeSlider", doubtGaugeSliders[1]);

        uiGameObjects.Add("OverHeadDoubtGaugeSliderImage_0", overHeadDoubtGaugeSliderImages[0]);
        uiGameObjects.Add("OverHeadDoubtGaugeSliderImage_1", overHeadDoubtGaugeSliderImages[1]);

        uiGameObjects.Add("AccidyDialogueBox", accidyDialogueBox);

        uiGameObjects.Add("FatePositionSlider", fatePositionSlider);
        uiGameObjects.Add("AccidyPositionSlider", accidyPositionSlider);

        warningVignetteQVignetteSingle = warningVignette.GetComponent<Q_Vignette_Single>();
        dayTextTextMeshProUGUI = dayText.GetComponent<TextMeshProUGUI>(); 
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

    public GameObject GetUI(string uiName)
    {
        return uiGameObjects[uiName];
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
    public TMP_Text GetAccidyDialogueBoxText()
    {
        return uiGameObjects["AccidyDialogueBox"].GetComponentInChildren<TextMeshProUGUI>();
    }
    public void ChangeMemoGauge()
    {
        MemoManager.Instance.SetMemoGauge(memoGauge, gaugeImage.GetComponent<Image>(), clearFlagSlider.GetComponent<Slider>(), clearFlageImage.GetComponent<Image>());
    }

    public void ChangeUIPosition(string uiName, Vector3 absolutePosition, Vector3 addVector)
    {
        if (addVector != Vector3.zero)
            uiGameObjects[uiName].transform.position += addVector;
        else
            uiGameObjects[uiName].transform.position = absolutePosition;
    }
    public float GetSliderValue(string uiName)
    {
        return uiGameObjects[uiName].GetComponent<Slider>().value;
    }
    public void ChangeSliderValue(string uiName, float absoluteValue, float addValue)
    {
        Slider slider = uiGameObjects[uiName].GetComponent<Slider>();
        if (addValue != 0) slider.value += addValue;
        else slider.value = absoluteValue;
    }
    public void ChangeImageAlpha(string uiName, float addValue)
    {
        Color color = uiGameObjects[uiName].GetComponent<Image>().color;
        color.a = Mathf.Clamp(color.a + addValue, 0, 1);
        uiGameObjects[uiName].GetComponent<Image>().color = color;
    }

    /*
     * startAlpha: 경고 표시 시작 시 투명도
     * endAlpha: 경고 표시 종료 시 투명도
     * fadeInDuration: 경고 표시 페이드 인 소요 시간
     * fadeOutDuration: 경고 표시 페이드 아웃 소요 시간
     */
    public IEnumerator WarningCoroutine(float startAlpha = 0f, float endAlpha = 1f, float fadeInDuration = 0.5f, float fadeOutDuration = 0.5f)
    {
        SetUI("WarningVignette", true); // 경고 표시 활성화
        
        float timeAccumulated = 0;
        while (timeAccumulated < fadeInDuration)
        {
            timeAccumulated += Time.deltaTime;
            warningVignetteQVignetteSingle.mainColor.a = Mathf.Lerp(startAlpha, endAlpha, timeAccumulated / fadeInDuration); // WarningVignette 투명도를 0에서 1로 선형 보간(Lerp)

            yield return null;
        }

        yield return new WaitForSeconds(warningTime); // warningTime 동안 경고 상태 유지

        timeAccumulated = 0; // 경고 종료: WarningVignette.mainColor.a를 다시 0으로 페이드 아웃
        while (timeAccumulated < fadeOutDuration)
        {
            timeAccumulated += Time.deltaTime * 2;  // 페이드 아웃 속도를 더 빠르게 설정
            warningVignetteQVignetteSingle.mainColor.a = Mathf.Lerp(endAlpha, startAlpha, timeAccumulated / fadeOutDuration); // WarningVignette 투명도를 1에서 0으로 선형 보간(Lerp)

            yield return null;
        }

        SetUI("WarningVignette", false); // 경고 표시 비활성화
    }
    
    private int GetCurrentSceneIndex()
    {
        return (int)GameManager.Instance.GetVariable("CurrentScene");
    }
}
