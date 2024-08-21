using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class FollowTutorial : MonoBehaviour
{
    [SerializeField] private GameObject frontCanvas;
    [SerializeField] private GameObject highlightPanel;
    [SerializeField] private GameObject moveButtons;
    [SerializeField] private GameObject hideButtons;
    [SerializeField] private TextMeshProUGUI startText;
    [SerializeField] private GameObject accidyDialogueBox;
    [SerializeField] private GameObject fate;
    [SerializeField] private Slider[] doubtGaugeSliders;
    [SerializeField] private Image[] overHeadDoubtGaugeSliderImages;

    private GameObject BlockingPanel { get => FollowManager.Instance.blockingPanel; }

    public bool fateMovable = false;
    public bool fateCanHide = false;

    private GameObject accidy;

    private Coroutine accidyDialogueCoroutine;
    private int tutorialStep = 0;

    public IEnumerator StartTutorial()
    {
        accidy = FollowManager.Instance.Accidy.gameObject;

        // 다른 물체를 누를 수 없도록 만든다
        FollowManager.Instance.IsTutorial = true;
        frontCanvas.SetActive(false);

        // 튜토리얼 중에 메모 버튼이 켜지지 않도록 설정
        MemoManager.Instance.HideMemoButton = true;
        MemoManager.Instance.SetMemoButtons(false);

        StartCoroutine(ScreenEffect.Instance.OnFade(null, 1, 0, 1, false, 0, 0));
        accidyDialogueCoroutine = StartCoroutine(AccidyDialogueBoxLogic());
        yield return new WaitForSeconds(1.5f);

        highlightPanel.SetActive(false);
        BlockingPanel.SetActive(true);
        DialogueManager.Instance.StartDialogue("FollowTutorial_002");
    }
    private IEnumerator AccidyDialogueBoxLogic()
    {
        TMP_Text text = accidyDialogueBox.GetComponentInChildren<TextMeshProUGUI>();
        float currentTime = 0;
        while (true)
        {
            currentTime += Time.deltaTime;
            if (currentTime > 1)
            {
                if (text.text.Length < 3) text.text += ".";
                else text.text = "";

                currentTime = 0;
            }
            yield return null;
        }
    }
    public void NextStep()
    {
        Debug.Log(tutorialStep);

        switch (tutorialStep)
        {
            case 0: // 신호등 판넬 켜기
                BlockingPanel.SetActive(false);
                highlightPanel.SetActive(true);
                break;
            case 1: // 신호등을 눌렀을 때
                BlockingPanel.SetActive(true);
                highlightPanel.SetActive(false);
                DialogueManager.Instance.StartDialogue("FollowTutorial_004");
                break;
            case 2: // 이동 가능
                StartCoroutine(MoveLogicTutorial());
                break;
            case 4: // 우연이 뒤를 돌았다가 다시 앞을 볼 때까지 숨기
                StartCoroutine(HideLogicTutorial());
                break;
            case 5: // 튜토리얼 끝
                StartCoroutine(EndTutorial());
                break;
        }
        tutorialStep++;
    }
    private IEnumerator MoveLogicTutorial()
    {
        moveButtons.SetActive(true);
        fateMovable = true;
        BlockingPanel.SetActive(true);
        DialogueManager.Instance.StartDialogue("FollowTutorial_005");

        // 필연이 일정 거리 이상 앞으로 이동할 때까지 대기
        while (fate.transform.position.x < 4 || tutorialStep == 3) yield return null;

        fateMovable = false;
        fate.GetComponent<Animator>().SetBool("Walking", false);
        fateCanHide = true;

        moveButtons.SetActive(false);

        // 우연이 뒤돌아보기 직전
        accidy.transform.parent.SetAsLastSibling();

        StopCoroutine(accidyDialogueCoroutine);
        accidyDialogueBox.GetComponentInChildren<TextMeshProUGUI>().text = "...?";

        BlockingPanel.SetActive(true);
        DialogueManager.Instance.StartDialogue("FollowTutorial_006");

        hideButtons.SetActive(true);
    }

    private IEnumerator HideLogicTutorial()
    {
        BlockingPanel.SetActive(false);

        accidy.GetComponent<Animator>().SetBool("Back", true);
        yield return new WaitForSeconds(0.5f);
        accidyDialogueBox.GetComponentInChildren<TextMeshProUGUI>().text = "?";
        yield return new WaitForSeconds(0.5f);

        float spaceBarClickTime = 3;
        while (spaceBarClickTime > 0)
        {
            while (Input.GetKey(KeyCode.Space) && spaceBarClickTime > 0)
            {
                spaceBarClickTime -= Time.deltaTime;
                yield return null;
            }

            if (spaceBarClickTime <= 0) break;

            ChangeGaugeAlpha(Time.deltaTime * 3);
            doubtGaugeSliders[0].value += 0.001f;
            doubtGaugeSliders[1].value = doubtGaugeSliders[0].value;

            if (doubtGaugeSliders[0].value == 1) FollowManager.Instance.FollowEndLogicStart();
            else ChangeGaugeAlpha(-Time.deltaTime);

            yield return null;
        }
        hideButtons.SetActive(false);

        accidy.transform.parent.SetAsFirstSibling();
        accidy.GetComponent<Animator>().SetBool("Back", false);
        accidyDialogueCoroutine = StartCoroutine(AccidyDialogueBoxLogic());
        yield return new WaitForSeconds(0.5f);
        BlockingPanel.SetActive(true);
        DialogueManager.Instance.StartDialogue("FollowTutorial_007");
    }

    private void ChangeGaugeAlpha(float value)
    {
        Color color = overHeadDoubtGaugeSliderImages[0].color;
        color.a = Mathf.Clamp(color.a + value, 0, 1);
        foreach (Image image in overHeadDoubtGaugeSliderImages) image.color = color;
    }

    //private void AdditionalTutorialSet()
    //{
    //    StartCoroutine(AdditionalTutorial());
    //}
    //private IEnumerator AdditionalTutorial()
    //{
    //    clickSpaceBar = false;

    //    StartCoroutine(WaitingSpaceBarClick());

    //    while(!clickSpaceBar) // 이동 버튼을 한번이라도 누를 때까지 깜빡인다
    //    {
    //        foreach (Image button in moveButtons)
    //            StartCoroutine(ScreenEffect.Instance.OnFade(button, 1, 0, 0.5f, true, 0.1f, 0));

    //        yield return new WaitForSeconds(1.2f);
    //    }
    //    foreach (Image button in moveButtons)
    //        button.gameObject.SetActive(false);
    //}

    
    private IEnumerator EndTutorial()
    {
        fateMovable = false;
        startText.gameObject.SetActive(true);
        float current = 0, fadeTime = 1;
        while (current < fadeTime)
        {
            current += Time.deltaTime;

            startText.color = new Color(1, 1, 1, Mathf.Lerp(1, 0, current / fadeTime));
            yield return null;
        }
        startText.gameObject.SetActive(false);
        yield return new WaitForSeconds(0.5f);
        BlockingPanel.SetActive(false);

        MemoManager.Instance.HideMemoButton = false;
        MemoManager.Instance.SetMemoButtons(true);

        FollowManager.Instance.IsTutorial = false;
        frontCanvas.SetActive(true);

        StopCoroutine(accidyDialogueCoroutine);

        FollowManager.Instance.StartFollow();
    }
}