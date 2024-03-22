using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

public class StartLogic : MonoBehaviour, IResultExecutable
{
    [SerializeField] private TMP_InputField nameInput;
    [SerializeField] private TextMeshProUGUI nameCheckQuestion;
    [SerializeField] private TMP_Dropdown monthDropdown, dayDropdown;
    
    [SerializeField] private GameObject background;
    [SerializeField] private GameObject fadeEffectImage;
    
    private Animator fadeEffectAnimator;

    private void Awake()
    {
        fadeEffectAnimator = fadeEffectImage.GetComponent<Animator>();
    }
    
    private void Start()
    {
        ResultManager.Instance.RegisterExecutable("StartLogic", this);
    }
    
    public void ExecuteAction()
    {
        PlayFadeOutAnimation();
        StartCoroutine(GoSceneAfterDelay(1, 2.5f));
    }
    
    public void GoScene(int sceneNum)
    {
        SceneManager.LoadScene(sceneNum);
    }

    public void SetName()
    {
        Player.Instance.Name = nameInput.text == "" ? "필연" : nameInput.text;
        nameCheckQuestion.text = $"\"{Player.Instance.Name}\"으로 확정하시겠습니까?";
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
    public void SetBirth()
    {
        Player.Instance.Month = monthDropdown.value + 1;
        Player.Instance.Day = dayDropdown.value + 1;
    }
    
    // Background 이미지 변경 및 Fade Effect Image 활성화 메서드
    public void SettingsComplete()
    {
        fadeEffectImage.SetActive(true); // Fade Effect Image 활성화
        background.GetComponent<Image>().sprite = Resources.Load<Sprite>("PrototypeImage/Side 1"); // Background 이미지 변경
        PlayFadeInAnimation();
        StartCoroutine(StartDialogueAfterDelay("Prologue_002", 2.5f));
    }

    IEnumerator StartDialogueAfterDelay(string dialogueID, float delay)
    {
        yield return new WaitForSeconds(delay);
        DialogueManager.Instance.StartDialogue(dialogueID);
    }

    IEnumerator GoSceneAfterDelay(int sceneNumber, float delaySeconds)
    {
        yield return new WaitForSeconds(delaySeconds);
        GoScene(sceneNumber);
    }

    // "Fade In" 애니메이션 재생 메서드
    public void PlayFadeInAnimation()
    {
        fadeEffectAnimator.Play("Fade In");
    }

    // "Fade Out" 애니메이션 재생 메서드
    public void PlayFadeOutAnimation()
    {
        fadeEffectAnimator.Play("Fade Out");
    }
}
