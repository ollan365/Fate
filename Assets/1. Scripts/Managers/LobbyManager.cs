using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour
{
    public static LobbyManager Instance { get; private set; }
    
    public GameObject lobbyButtons;
    public Image backgroundImage;
    public Sprite titleWithLogo, room1Side1BackgroundSprite, blackBgSprite;
    [SerializeField] private GameObject gotoSceneButtons;
    [SerializeField] private GameObject lobbyPanels;
    [SerializeField] private GameObject clockSecondGameObject;
    [SerializeField] private TMP_InputField nameInput;
    [SerializeField] private TextMeshProUGUI nameCheckQuestion;
    [SerializeField] private TMP_Dropdown monthDropdown, dayDropdown;
    
    private string fateName;
    private int language = 1;
    public int Language { set => language = value; }
    private bool isLobby;

    private void Awake() {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start() {
        isLobby = true;
        lobbyButtons.SetActive(true);
        lobbyPanels.SetActive(true);
        // backgroundImage.sprite = titleWithLogo;

        if (GameManager.Instance.isDemoBuild) { // default: -20
            lobbyButtons.GetComponent<VerticalLayoutGroup>().spacing = -70;
            lobbyButtons.transform.GetChild(1).gameObject.SetActive(false);
            gotoSceneButtons.SetActive(false);
        }
        
        StartCoroutine(WaitForGameManagerStartFunction());
    }
    
    private IEnumerator WaitForGameManagerStartFunction() {
        yield return null;
        SaveManager.Instance.ApplySavedGameData();

        if ((bool)GameManager.Instance.GetVariable("SkipLobby")) {
            GameManager.Instance.SetVariable("SkipLobby", false);
            SaveManager.Instance.SaveGameData();
            lobbyButtons.SetActive(false);
            backgroundImage.sprite = blackBgSprite;
            StartCoroutine(StartPrologue());
        } else if (UIManager.Instance) {
            StartCoroutine(UIManager.Instance.OnFade(null, 1, 0, 2f, false, 0, 0));
            clockSecondGameObject.SetActive(true);
            StartCoroutine(RotateSecond());
        }
    }
    
    private IEnumerator RotateSecond() {
        while (isLobby) {
            for (float t = 0; t < 60f; t += Time.deltaTime) { // 1초 동안 한 바퀴 (360도) 회전
                float angle = Mathf.Lerp(0, 360, t / 60); // 현재 시간 대비 회전 각도 계산
                clockSecondGameObject.transform.rotation = Quaternion.Euler(0, 0, -angle);
                yield return null; // 한 프레임 기다림
            }
            clockSecondGameObject.transform.rotation = Quaternion.Euler(0, 0, 0);  // 정확히 360도 회전하도록 설정
        }
    }
    
    public void StartNewGame() {
        if (!GameManager.Instance.isDemoBuild && SaveManager.Instance.CheckGameData())  // 저장된 게임 데이터가 없는 경우
            UIManager.Instance?.SetUI(eUIGameObjectName.NewGamePanel, true);
        else
            StartCoroutine(StartPrologue());
    }
    
    public void LoadGame() {
        if (SaveManager.Instance.CheckGameData()) {
            lobbyButtons.SetActive(false);
            Constants.SceneType savedScene = ((int)GameManager.Instance.GetVariable("SavedCurrentSceneIndex")).ToEnum();
            if (savedScene == Constants.SceneType.START)
                StartCoroutine(StartPrologue());
            else 
                GameSceneManager.Instance.LoadScene(savedScene);
        } else 
            UIManager.Instance?.SetUI(eUIGameObjectName.NoGameDataPanel, true);
    }
    
    public void GoScene(int sceneNum) {
        SaveManager.Instance.CreateNewGameData();
        GameManager.Instance.SetVariable("isTutorial", !GameManager.Instance.skipTutorial);
        GameManager.Instance.SetVariable("EndTutorial_ROOM_1", GameManager.Instance.skipTutorial);
        GameSceneManager.Instance.LoadScene(sceneNum.ToEnum());
    }

    public void QuitGame() {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    // ========== 프롤로그 시작 ========== //
    private IEnumerator StartPrologue() {
        StartCoroutine(UIManager.Instance.OnFade(null, 0, 1, 1, false, 0, 0));
        lobbyButtons.SetActive(false);
        clockSecondGameObject.SetActive(false);
        MemoManager.Instance.SetShouldHideMemoButton(true);
        UIManager.Instance.SetUI(eUIGameObjectName.AlbumButton, false);
        SoundPlayer.Instance.ChangeBGM(Constants.BGM_PROLOGUE);
        isLobby = false;
        
        yield return new WaitForSeconds(1f);
        backgroundImage.sprite = blackBgSprite;
        UIManager.Instance.coverPanel.gameObject.SetActive(false);
        EventManager.Instance.CallEvent("EventFirstPrologue");
    }
    
    // ===== 이름 설정 ===== //
    public void OpenNamePanel() {
        UIManager.Instance?.SetUI(eUIGameObjectName.NamePanel, true);
    }
    
    public void SetName() {
        fateName = nameInput.text == "" ? "필연" : nameInput.text;
        nameCheckQuestion.text = $"\"{fateName}\"으로 확정하시겠습니까?";
    }
    
    public void NameSetting() {
        GameManager.Instance.SetVariable("FateName", fateName);
        EventManager.Instance.CallEvent("Event_NameSetting");
    }

    // ===== 생일 설정 ===== //
    public void OpenBirthPanel() {
        UIManager.Instance?.SetUI(eUIGameObjectName.BirthdayPanel, true);
        ChangeDayOption();
    }
    
    public void ChangeDayOption() {
        List<TMP_Dropdown.OptionData> optionList = new();
        for (int i = 1; i <= 29; i++)
            optionList.Add(new TMP_Dropdown.OptionData(i.ToString()));

        switch (monthDropdown.value + 1) {
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

    public void BirthSetting() {
        SettingsComplete();
        EventManager.Instance.CallEvent("Event_BirthSetting");
    }

    // 필연 설정 완료
    private void SettingsComplete() {
        GameManager.Instance.SetVariable("Language", language);
        GameManager.Instance.SetVariable("FateGender", 0);  // 필연 성별 설정 (선택 기능이 없어졌음)

        string birthday = ((monthDropdown.value + 1) * 100 + (dayDropdown.value + 1)).ToString();
        if (birthday.Length == 3) 
            birthday = "0" + birthday;
        GameManager.Instance.SetVariable("FateBirthday", birthday);
    }
}
