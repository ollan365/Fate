using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using static Constants;

public class FollowDialogueManager : MonoBehaviour
{
    [SerializeField] private GameObject frontCanvas;

    [Header("Dialogue")]
    [SerializeField] private GameObject extraBlockingPanel;
    [SerializeField] private GameObject[] extraCanvas;
    [SerializeField] private TextMeshProUGUI[] extraDialogueText;
    [SerializeField] private GameObject specialObjectButton;
    [SerializeField] private Image specialObjectButtonImage;
    private FollowExtra extra = FollowExtra.None;

    private GameObject BlockingPanel { get => FollowManager.Instance.blockingPanel; }
    private bool IsEnd { get => FollowManager.Instance.IsEnd; }
    private bool IsDialogueOpen { get => FollowManager.Instance.IsDialogueOpen; }

    private void Awake()
    {
        extraBlockingPanel.GetComponent<Button>().onClick.AddListener(() => DialogueManager.Instance.OnDialoguePanelClick());
    }
    public bool ClickObject()
    {
        // 엑스트라 캐릭터의 대사가 출력되는 중이면 끈다
        foreach (GameObject extra in extraCanvas) if (extra.activeSelf) extra.SetActive(false);

        frontCanvas.SetActive(false); // 플레이어를 가리는 물체들이 있는 canvas를 꺼버린다
        BlockingPanel.SetActive(true); // 화면을 어둡게 만든다
        return true;
    }

    public void EndScript()
    {
        frontCanvas.SetActive(true); // 플레이어를 가리는 물체들이 있는 canvas를 켠다
        BlockingPanel.SetActive(false); // 화면을 가리는 판넬을 끈다

        EndExtraDialogue(true);
    }
    public void OpenExtraDialogue(string extraName)
    {
        extra = ToEnum(extraName);

        extraBlockingPanel.SetActive(true); // 일반적인 블로킹 판넬이 아닌 다른 걸 켠다
        BlockingPanel.SetActive(false);

        extraCanvas[Int(extra)].GetComponentInChildren<Image>().color = new Color(1, 1, 1, 1);

        DialogueManager.Instance.dialogueSet[DialogueType.FOLLOW_EXTRA.ToInt()] = extraCanvas[Int(extra)];
        DialogueManager.Instance.scriptText[DialogueType.FOLLOW_EXTRA.ToInt()] = extraDialogueText[Int(extra)];
        DialogueManager.Instance.dialogueType = DialogueType.FOLLOW_EXTRA;

        extraCanvas[Int(extra)].SetActive(true);
    }
    public void EndExtraDialogue(bool dialogueEnd)
    {
        if (extra == FollowExtra.None) return;

        extraCanvas[Int(extra)].GetComponentInChildren<Image>().color = new Color(1, 1, 1, 0);

        extraBlockingPanel.SetActive(false);
        extraCanvas[Int(extra)].SetActive(false);

        extra = FollowExtra.None;

        if (!dialogueEnd) BlockingPanel.SetActive(true); // 아직 다른 대사가 출력되는 중
    }
    public void ClickSpecialObject(FollowObject followObject)
    {
        specialObjectButton.SetActive(true);
        specialObjectButtonImage.sprite = followObject.specialSprite;
        specialObjectButtonImage.SetNativeSize();
        specialObjectButtonImage.GetComponent<RectTransform>().localScale = new Vector3(followObject.scaleValue, followObject.scaleValue, followObject.scaleValue);

        specialObjectButton.GetComponent<Button>().onClick.RemoveAllListeners();
        specialObjectButton.GetComponent<Button>().onClick.AddListener(() => followObject.OnMouseDown_Normal());
        specialObjectButton.GetComponent<Button>().onClick.AddListener(() => specialObjectButton.SetActive(false));
    }
    // ========== 엑스트라 자동 대화창 ========== //
    private List<string> alreadyType = new();

    public void ExtraAutoDialogue(string dialogueID)
    {
        foreach (string id in alreadyType) // 이미 출력한 대사라면 다시 출력하지 않는다
            if (id == dialogueID) return;

        alreadyType.Add(dialogueID);

        StartCoroutine(TypeSentence(dialogueID));
    }
    private IEnumerator TypeSentence(string dialogueID)
    {
        int currentDialogueLineIndex = 0;

        while (!IsEnd)
        {
            DialogueManager.Instance.dialogues[dialogueID].SetCurrentLineIndex(currentDialogueLineIndex);
            DialogueLine dialogueLine = DialogueManager.Instance.dialogues[dialogueID].Lines[currentDialogueLineIndex];
            string sentence = DialogueManager.Instance.scripts[dialogueLine.ScriptID].GetScript();
            int speakerIndex = Int(ToEnum(dialogueLine.SpeakerID));

            // 대사가 출력된 판넬과 텍스트 받아오기
            extraCanvas[speakerIndex].SetActive(true);
            extraDialogueText[speakerIndex].text = "";

            foreach (char letter in sentence.ToCharArray())
            {
                // 다른 물체가 클릭되었을 시
                if (IsDialogueOpen)
                {
                    if (IsEnd) // 미행이 끝났으면 즉시 종료
                    {
                        extraCanvas[speakerIndex].SetActive(false);
                        break;
                    }
                    else
                    {
                        // 기존에 출력한 대사까지 저장 후 대화창을 비운다
                        string saveText = extraDialogueText[speakerIndex].text;
                        extraDialogueText[speakerIndex].text = "";

                        // 다른 물체의 스크립트가 끝나기를 기다린다
                        while (!IsDialogueOpen) yield return null;

                        // 다른 물체의 스크립트가 끝나면 다시 이어서 출력
                        extraCanvas[speakerIndex].SetActive(true);
                        extraDialogueText[speakerIndex].text = saveText;
                    }
                }

                extraDialogueText[speakerIndex].text += letter;
                SoundPlayer.Instance.UISoundPlay(Sound_Typing); // 타자 소리 한번씩만
                yield return new WaitForSeconds(0.1f);
            }

            yield return new WaitForSeconds(0.25f);

            // 대사가 끝난 후
            extraDialogueText[speakerIndex].text = "";
            extraCanvas[speakerIndex].SetActive(false);

            currentDialogueLineIndex++;

            // 더 이상 DialogueLine이 존재하지 않으면 반복문 종료
            if (currentDialogueLineIndex >= DialogueManager.Instance.dialogues[dialogueID].Lines.Count) break;

        }
    }

    public int Int(FollowExtra extraType)
    {
        switch (extraType)
        {
            case FollowExtra.Angry: return 0;
            case FollowExtra.Employee: return 0;
            case FollowExtra.RunAway_1: return 1;
            case FollowExtra.RunAway_2: return 2;
            case FollowExtra.Police: return 3;
            case FollowExtra.Smoker_1: return 4;
            case FollowExtra.Smoker_2: return 5;
            case FollowExtra.Clubber_1: return 6;
            case FollowExtra.Clubber_2: return 7;
            default: return -1;
        }
    }
    public FollowExtra ToEnum(string extraName)
    {
        switch (extraName)
        {
            case "Angry": return FollowExtra.Angry;
            case "The_Solicitation": return FollowExtra.Employee;
            case "Teenage_A": return FollowExtra.RunAway_1;
            case "Teenage_B": return FollowExtra.RunAway_2;
            case "The_police": return FollowExtra.Police;
            case "Smoker_1": return FollowExtra.Smoker_1;
            case "Smoker_2": return FollowExtra.Smoker_2;
            case "Clubber_1": return FollowExtra.Clubber_1;
            case "Clubber_2": return FollowExtra.Clubber_2;
            default: return FollowExtra.None;
        }
    }
}
