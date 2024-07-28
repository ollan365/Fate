using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

public class StartLogic : MonoBehaviour
{
    public static StartLogic Instance { get; private set; }

    [Header("Start Page")]
    [SerializeField] private GameObject newStartPanel; // 처음부터를 눌렀을 때, 저장된 데이터가 있으면 켜지는 판넬
    [SerializeField] private GameObject noGameDataPanel; // 이어서를 눌렀을 때, 저장된 데이터가 없으면 켜지는 판넬
    [SerializeField] private GameObject start;
    [SerializeField] private GameObject second;
    [SerializeField] private Slider[] soundSliders;
    [SerializeField] private TextMeshProUGUI[] soundValueTexts;
    [SerializeField] private GameObject buttons;
    private int language = 1;
    public int Language { set => language = value; }

    [Header("Name, Birth Page")]
    [SerializeField] private GameObject namePanel;
    [SerializeField] private GameObject birthPanel;
    [SerializeField] private TMP_InputField nameInput;
    [SerializeField] private TextMeshProUGUI nameCheckQuestion;
    [SerializeField] private TMP_Dropdown monthDropdown, dayDropdown;
    private string fateName;

    [Header("Prologue")]
    [SerializeField] private GameObject blockingPanel;
    [SerializeField] private GameObject background;
    [SerializeField] private Sprite titleWithLogo, titleWithOutLogo, room1Side1BackgroundSprite;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        if (SaveManager.Instance && SaveManager.Instance.EndingData != null && !SaveManager.Instance.EndingData.isEndingLogicEnd
            && GameManager.Instance && GameManager.Instance.GetVariable("BadEndingCollect") != null)
            LoadGame(true);
        else if(ScreenEffect.Instance) StartCoroutine(ScreenEffect.Instance.OnFade(null, 1, 0, 0.1f, false, 0, 0));
    }
    private void Start()
    {
        StartCoroutine(RotateSecond());
    }
    private IEnumerator RotateSecond()
    {
        while (start.activeSelf)
        {
            // 1초 동안 한 바퀴 (360도) 회전
            for (float t = 0; t < 60f; t += Time.deltaTime)
            {
                // 현재 시간 대비 회전 각도 계산
                float angle = Mathf.Lerp(0, 360, t / 60);
                second.transform.rotation = Quaternion.Euler(0, 0, -angle);

                if (background.GetComponent<Image>().sprite == titleWithLogo)
                    second.GetComponent<Image>().color = new Color(1, 1, 1, 1);
                else // 다른 판넬(옵션 등)이 켜지면 투명해지도록 만든다
                    second.GetComponent<Image>().color = new Color(1, 1, 1, 0);

                yield return null; // 한 프레임 기다림
            }
            // 정확히 360도 회전하도록 설정
            second.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }
    public void StartNewGame()
    {
        if (!SaveManager.Instance.CheckGameData()) // 저장된 게임 데이터가 없는 경우
        {
            SaveManager.Instance.SaveInitGameData();

            start.SetActive(false);
            StartCoroutine(StartPrologue());
        }
        else
        {
            newStartPanel.SetActive(true);
            Image backgroundImage = background.GetComponent<Image>();
            backgroundImage.sprite = titleWithOutLogo;
        }
    }
    public void OriginBackground()
    {
        Image backgroundImage = background.GetComponent<Image>();
        backgroundImage.sprite = titleWithLogo;
    }
    public void LoadGame(bool loadPrologue)
    {
        SaveManager.Instance.SaveInitGameData();

        if(loadPrologue || (SaveManager.Instance.EndingData != null && !SaveManager.Instance.EndingData.isEndingLogicEnd)) { StartCoroutine(StartPrologue()); buttons.SetActive(false); }
        else if (SaveManager.Instance.CheckGameData()) { SaveManager.Instance.LoadGameData(); buttons.SetActive(false); }
        else noGameDataPanel.SetActive(true);
    }
    public void ChangeSoundValue(int index)
    {
        soundValueTexts[index].text = (soundSliders[index].value * 100).ToString("F0");
        SoundPlayer.Instance.ChangeVolume(soundSliders[0].value, soundSliders[1].value);
    }
    public void GoScene(int sceneNum)
    {
        SaveManager.Instance.SaveInitGameData();

        if (sceneNum == 1) SceneManager.Instance.LoadScene(Constants.SceneType.ROOM_1);
        if (sceneNum == 2) SceneManager.Instance.LoadScene(Constants.SceneType.FOLLOW_1);
        if (sceneNum == 3) SceneManager.Instance.LoadScene(Constants.SceneType.ROOM_2);
        if (sceneNum == 4) SceneManager.Instance.LoadScene(Constants.SceneType.FOLLOW_2);
    }

    // ========== 프롤로그 시작 ========== //
    private IEnumerator StartPrologue()
    {
        if (!ScreenEffect.Instance.coverPanel.gameObject.activeSelf)
            StartCoroutine(ScreenEffect.Instance.OnFade(null, 0, 1, 1, false, 0, 0));

        // 프롤로그 중에 메모 버튼이 켜지지 않도록 변경
        MemoManager.Instance.HideMemoButton = true;

        EventManager.Instance.CallEvent("EventFirstPrologue");
        yield return new WaitForSeconds(1);

        background.GetComponent<Image>().sprite = room1Side1BackgroundSprite; // Background 이미지 변경
        StartCoroutine(ScreenEffect.Instance.OnFade(null, 1, 0, 1, false, 0, 0));

        blockingPanel.SetActive(true);
    }

    // ===== 배경 변경 ===== //
    public void DarkBackground(bool open)
    {
        if (open) blockingPanel.GetComponent<Image>().color = new Color(0, 0, 0, 1);
        else StartCoroutine(DarkBackground());
    }
    private IEnumerator DarkBackground()
    {
        yield return new WaitForSeconds(1);
        SoundPlayer.Instance.ChangeBGM(Constants.BGM_STOP);
        blockingPanel.GetComponent<Image>().color = new Color(0, 0, 0, 0.8f);
    }

    // ===== 이름 설정 ===== //
    public void OpenNamePanel()
    {
        blockingPanel.SetActive(false);
        namePanel.SetActive(true);
    }
    public void SetName()
    {
        fateName = nameInput.text == "" ? "필연" : nameInput.text;
        nameCheckQuestion.text = $"\"{fateName}\"으로 확정하시겠습니까?";
    }
    public void NameSetting()
    {
        GameManager.Instance.SetVariable("FateName", fateName);
        DialogueManager.Instance.StartDialogue("Prologue_Birth");
        blockingPanel.SetActive(true);
    }

    // ===== 생일 설정 ===== //
    public void OpenBirthPanel()
    {
        blockingPanel.SetActive(false);
        birthPanel.SetActive(true);
        ChangeDayOption();
    }
    public void ChangeDayOption()
    {
        List<TMP_Dropdown.OptionData> optionList = new();
        for (int i = 1; i <= 29; i++)
            optionList.Add(new TMP_Dropdown.OptionData(i.ToString()));

        switch (monthDropdown.value + 1)
        {
            case 2:
                break;
            case 4:
            case 6:
            case 9:
            case 11:
                optionList.Add(new TMP_Dropdown.OptionData(30.ToString()));
                break;
            default:
                optionList.Add(new TMP_Dropdown.OptionData(30.ToString()));
                optionList.Add(new TMP_Dropdown.OptionData(31.ToString()));
                break;
        }
        dayDropdown.ClearOptions();
        dayDropdown.AddOptions(optionList);
    }

    public void BirthSetting()
    {
        DialogueManager.Instance.StartDialogue("Prologue_008_B");
        blockingPanel.SetActive(true);
        SettingsComplete();
    }

    // 필연 설정 완료
    public void SettingsComplete()
    {
        GameManager.Instance.SetVariable("Language", language);
        GameManager.Instance.SetVariable("FateGender", 0);  // 필연 성별 설정 (더 이상 선택 기능이 없어졌으므로 0으로 고정

        string birthday = ((monthDropdown.value + 1) * 100 + (dayDropdown.value + 1)).ToString();
        if (birthday.Length == 3) birthday = "0" + birthday;
        GameManager.Instance.SetVariable("FateBirthday", birthday);
    }
}
