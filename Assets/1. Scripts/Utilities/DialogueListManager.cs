using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueListManager : MonoBehaviour
{
    public GameObject buttonPrefab;
    public Transform scrollViewContent;
    public DialogueManager dialogueManager;

    void Start()
    {
        PopulateDialogueList();
    }

    void PopulateDialogueList()
    {
        foreach (var dialogueID in dialogueManager.dialogues.Keys)
        {
            GameObject buttonObj = Instantiate(buttonPrefab, scrollViewContent);
            buttonObj.GetComponentInChildren<TextMeshProUGUI>().text = dialogueID;
            buttonObj.GetComponent<Button>().onClick.AddListener(() => dialogueManager.StartDialogue(dialogueID));
        }
    }
}