using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class FollowTutorial : MonoBehaviour
{
    [SerializeField] private FollowAnim followAnim;

    [SerializeField] private GameObject frontCanvas;
    [SerializeField] private GameObject highlightPanel;
    [SerializeField] private GameObject blockingPanel;
    [SerializeField] private Image arrow;
    [SerializeField] private TextMeshProUGUI startText;
    [SerializeField] private GameObject[] accidyDialogueBox;
    [SerializeField] private GameObject fate;
    [SerializeField] private GameObject[] accidys;
    private GameObject accidy;

    private Coroutine accidyDialogueCoroutine;
    private bool clickSpaceBar = false;
    private int tutorialStep = 0;

    public IEnumerator StartTutorial()
    {
        if ((int)GameManager.Instance.GetVariable("AccidyGender") == 0) accidy = accidys[0];
        else accidy = accidys[1];

        // 다른 물체를 누를 수 없도록 만든다
        FollowManager.Instance.IsTutorial = true;
        frontCanvas.SetActive(false);

        // 튜토리얼 중에 메모 버튼이 켜지지 않도록 설정
        MemoManager.Instance.HideMemoButton = true;
        MemoManager.Instance.SetMemoButtons(false);

        StartCoroutine(ScreenEffect.Instance.OnFade(null, 1, 0, 1, false, 0, 0));
        accidyDialogueCoroutine = StartCoroutine(AccidyDialogueBoxLogic(0));
        yield return new WaitForSeconds(1.5f);

        highlightPanel.SetActive(false);
        blockingPanel.SetActive(true);
        DialogueManager.Instance.StartDialogue("FollowTutorial_002");
    }
    private IEnumerator AccidyDialogueBoxLogic(int dialogueBoxIndex)
    {
        accidyDialogueBox[dialogueBoxIndex].SetActive(true);
        TMP_Text text = accidyDialogueBox[dialogueBoxIndex].GetComponentInChildren<TextMeshProUGUI>();
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
        tutorialStep++;

        switch (tutorialStep)
        {
            case 2:
                ObjectTutorial(); // 빌딩 하이라이트
                break;
            case 3:
                MoveButtonTutorial(); // 이동 버튼 설명
                break;
            case 5:
                AdditionalTutorialSet();
                break;
            case 7:
                EndTutorialSet();
                break;
        }
    }

    // 빌딩 하이라이트
    private void ObjectTutorial()
    {
        blockingPanel.SetActive(false);
        highlightPanel.SetActive(true);
    }

    // 이동 버튼 설명
    private void MoveButtonTutorial()
    {
        // 이동 버튼에 대한 설명
        blockingPanel.SetActive(true);
        StopCoroutine(accidyDialogueCoroutine);
        accidy.SetActive(true);
        accidyDialogueBox[1].SetActive(true);
        accidyDialogueBox[1].GetComponentInChildren<TextMeshProUGUI>().text = "...?";
        DialogueManager.Instance.StartDialogue("FollowTutorial_004");
    }

    private void AdditionalTutorialSet()
    {
        StartCoroutine(AdditionalTutorial());
    }
    private IEnumerator AdditionalTutorial()
    {
        clickSpaceBar = false;

        StartCoroutine(WaitingSpaceBarClick());

        while(!clickSpaceBar) // 이동 버튼을 한번이라도 누를 때까지 깜빡인다
        {
            StartCoroutine(ScreenEffect.Instance.OnFade(arrow, 1, 0, 0.5f, true, 0.1f, 0));
            yield return new WaitForSeconds(1.2f);
        }

        arrow.gameObject.SetActive(false);
    }
    private IEnumerator WaitingSpaceBarClick()
    {
        while (true)
        {
            if (Input.GetKeyDown(KeyCode.Space)) break;
            yield return null;
        }
        clickSpaceBar = true;

        fate.GetComponent<Animator>().SetBool("Hide", true);
        yield return new WaitForSeconds(0.2f);
        accidy.GetComponent<Animator>().SetTrigger("Turn");
        yield return new WaitForSeconds(0.5f);
        accidyDialogueBox[1].GetComponentInChildren<TextMeshProUGUI>().text = "?";
        yield return new WaitForSeconds(0.5f);

        DialogueManager.Instance.StartDialogue("FollowTutorial_006");

        yield return new WaitForSeconds(1.5f);
        accidy.GetComponent<Animator>().SetTrigger("Turn");
        accidyDialogueCoroutine = StartCoroutine(AccidyDialogueBoxLogic(1));
    }
    private void EndTutorialSet()
    {
        StartCoroutine(EndTutorial());
    }
    private IEnumerator EndTutorial()
    {
        fate.GetComponent<Animator>().SetBool("Hide", false);

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
        blockingPanel.SetActive(false);

        MemoManager.Instance.HideMemoButton = false;
        MemoManager.Instance.SetMemoButtons(true);

        FollowManager.Instance.IsTutorial = false;
        frontCanvas.SetActive(true);

        StopCoroutine(accidyDialogueCoroutine);
        accidyDialogueBox[1].SetActive(false);
        accidy.SetActive(false);

        followAnim.ChangeAnimStatusToStop(false);
        StartCoroutine(FollowManager.Instance.StartGame());
    }
}