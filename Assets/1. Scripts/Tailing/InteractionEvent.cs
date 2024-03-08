using UnityEngine;
using UnityEngine.UI;
using static Constants;
public class InteractionEvent : MonoBehaviour
{
    [SerializeField] private DialgoueParser dialgoueParser;
    [SerializeField] private Text text, choiceTextA, choiceTextB;
    [SerializeField] private GameObject[] choiceBTNs;
    [SerializeField] private Button[] prologueBTN;
    private Dialogue[] dialogues;
    private int nextIndex;
    private Choice choiceEvent;
    private bool getDialogue;
    private void Start()
    {
        getDialogue = false;

        foreach (Button b in prologueBTN)
            b.onClick.AddListener(() => GetDialogues(File.Prologue));
    }
    private void Update()
    {
        if (getDialogue && Input.GetMouseButtonDown(0))
            PrintDialogue();
    }
    public void PrintDialogue()
    {
        if (nextIndex >= dialogues.Length) { text.text = ""; getDialogue = false; return; }

        text.text = dialgoueParser.Dialogue(dialogues[nextIndex].localizations);
        if (dialogues[nextIndex].eventID != "")
        {
            choiceEvent = dialgoueParser.GetChoiceEvent(dialogues[nextIndex].eventID);

            choiceBTNs[0].SetActive(true); choiceBTNs[1].SetActive(true);

            choiceTextA.text = dialgoueParser.Dialogue(choiceEvent.choice_A);
            choiceTextB.text = dialgoueParser.Dialogue(choiceEvent.choice_B);

            getDialogue = false;
        }
        else if (dialogues[nextIndex].skipLine != "")
        {
            for (int i = nextIndex; i < dialogues.Length; i++)
            {
                if (dialogues[i].id == dialogues[nextIndex].skipLine)
                {
                    nextIndex = i;
                    break;
                }
            }
        }
        else
            nextIndex++;
    }
    public void GetDialogues(File file)
    {
        dialogues = dialgoueParser.Parse(file.ToString());
        nextIndex = 0;
        getDialogue = true;
    }
    public void GetChoice(int index)
    {
        if (index == 0)
            for (int i = nextIndex; i < dialogues.Length; i++)
            {
                if (dialogues[i].id.Equals(choiceEvent.result_A))
                {
                    nextIndex = i;
                    getDialogue = true;
                    break;
                }
            }
        else if (index == 1)
            for (int i = nextIndex; i < dialogues.Length; i++)
            {
                Debug.Log(dialogues[i].id.CompareTo(choiceEvent.result_B));
                if (dialogues[i].id.Equals(choiceEvent.result_B))
                {
                    nextIndex = i;
                    getDialogue = true;
                    break;
                }
            }
    }
}
