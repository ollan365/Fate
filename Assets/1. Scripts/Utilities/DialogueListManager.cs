using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueListManager : MonoBehaviour
{
    public GameObject buttonPrefab;
    public Transform scrollViewContent;

    void Start()
    {
        PopulateDialogueList();
    }

    void PopulateDialogueList()
    {
        foreach (var dialogueID in DialogueManager.Instance.dialogues.Keys)
        {
            GameObject buttonObj = Instantiate(buttonPrefab, scrollViewContent);
            buttonObj.GetComponentInChildren<TextMeshProUGUI>().text = dialogueID;
            buttonObj.GetComponent<Button>().onClick.AddListener(() => DialogueManager.Instance.StartDialogue(dialogueID));
        }
    }
}