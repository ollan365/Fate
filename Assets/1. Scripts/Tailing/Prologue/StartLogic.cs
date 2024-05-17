using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

public class StartLogic : MonoBehaviour, IResultExecutable
{
    [Header("Start Page")]
    [SerializeField] private GameObject newStartPanel; // 처음부터를 눌렀을 때, 저장된 데이터가 있으면 켜지는 판넬
    [SerializeField] private GameObject start;
    [SerializeField] private GameObject setting;
    [SerializeField] private Slider[] soundSliders;
    [SerializeField] private TextMeshProUGUI[] soundValueTexts;
    private int language = 1;
    public int Language { set => language = value; }

    [Header("Name, Gender, Birth Page")]
    [SerializeField] private TMP_InputField nameInput;
    [SerializeField] private TextMeshProUGUI nameCheckQuestion;
    [SerializeField] private TMP_Dropdown monthDropdown, dayDropdown;
    private string fateName;
    private int fateGender;
    public int FateGender { set => fateGender = value; }

    [Header("Prologue")]
    [SerializeField] private GameObject blockingPanel;
    [SerializeField] private GameObject background;
    [SerializeField] private Sprite room1Side1BackgroundSprite;
    
    private void Start()
    {
        ResultManager.Instance.RegisterExecutable("StartLogic", this);
    }
    public void ExecuteAction()
    {
        StartCoroutine(EndPrologue());
    }
    public void StartNewGame()
    {
        if (SaveManager.Instance.CheckGameData())
        {
            newStartPanel.SetActive(true);
        }
        else // 저장된 게임 데이터가 없는 경우
        {
            start.SetActive(false);
            setting.SetActive(true);
        }
    }
    public void ChangeSoundValue(int index)
    {
        soundValueTexts[index].text = (soundSliders[index].value * 100).ToString("F0");
        SoundPlayer.Instance.ChangeVolume(soundSliders[0].value, soundSliders[1].value);
    }
    public void GoScene(int sceneNum)
    {
        SceneManager.LoadScene(sceneNum);
    }

    public void SetName()
    {
        fateName = nameInput.text == "" ? "필연" : nameInput.text;
        nameCheckQuestion.text = $"\"{fateName}\"으로 확정하시겠습니까?";
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
    
    // Background 이미지 변경 및 Fade Effect Image 활성화 메서드
    public void SettingsComplete()
    {
        SetVariable();
        StartCoroutine(Prologue());
    }
    private void SetVariable()
    {
        if (language == 2 && fateGender == 1) language++;

        GameManager.Instance.SetVariable("Language", language);
        GameManager.Instance.SetVariable("FateName", fateName);
        GameManager.Instance.SetVariable("FateGender", fateGender);  // 필연 성별 설정
        
        string birthday = ((monthDropdown.value + 1) * 100 + (dayDropdown.value + 1)).ToString();
        if (birthday.Length == 3) birthday = "0" + birthday;
        GameManager.Instance.SetVariable("FateBirthday", birthday);
    }
    private IEnumerator Prologue()
    {
        StartCoroutine(ScreenEffect.Instance.OnFade(null, 0, 1, 1, true, 1, 0));
        yield return new WaitForSeconds(1);
        background.GetComponent<Image>().color = Color.white;
        background.GetComponent<Image>().sprite = room1Side1BackgroundSprite; // Background 이미지 변경
        yield return new WaitForSeconds(2);

        blockingPanel.SetActive(true);
        DialogueManager.Instance.StartDialogue("Prologue_002");
    }
    private IEnumerator EndPrologue()
    {
        blockingPanel.SetActive(false);
        StartCoroutine(ScreenEffect.Instance.OnFade(null, 0, 1, 1, false, 0, 0));
        yield return new WaitForSeconds(1);
        GoScene(1); // Room 씬으로 이동
    }
}
