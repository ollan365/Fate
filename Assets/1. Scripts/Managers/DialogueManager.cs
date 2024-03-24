using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }
    
    // CSV 파일
    [Header("CSV files")]
    public TextAsset dialoguesCSV;
    public TextAsset scriptsCSV;
    public TextAsset choicesCSV;
    public TextAsset imagePathsCSV;
    
    // Dialogue UI
    [Header("Dialogue UI")]
    public GameObject dialogueCanvas;
    public TextMeshProUGUI speakerText;
    public TextMeshProUGUI scriptText;
    public SpriteRenderer characterImage;
    public Transform choicesContainer;
    public GameObject choicePrefab;
    public SpriteRenderer teddyBearIcon;
    
    // 타자 효과 속도
    [Header("Type Sentence Speed")]
    public float typeSpeed = 0.05f;
    
    // 자료 구조
    public Dictionary<string, Dialogue> dialogues = new Dictionary<string, Dialogue>();
    public Dictionary<string, Script> scripts = new Dictionary<string, Script>();
    public Dictionary<string, Choice> choices = new Dictionary<string, Choice>();
    public Dictionary<string, ImagePath> imagePaths = new Dictionary<string, ImagePath>();

    // 상태 변수
    private string currentDialogueID = "";
    public bool isDialogueActive = false;
    private bool isChoiceActive = false;
    private bool isTyping = false;


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            ParseDialogues();
            ParseScripts();
            ParseChoices();
            ParseImagePaths();
        }
        else
        {
            Destroy(gameObject);
        }
        dialogueCanvas.SetActive(false);
    }

    private void Update()
    {
        if (isDialogueActive && Input.GetKeyDown(KeyCode.Space) && !isChoiceActive &&!isTyping)
        {
            ProceedToNext();
        }
    }

    // ---------------------------------------------- Dialogue methods ----------------------------------------------

    private void ProceedToNext()
    {
        int currentDialogueLineIndex = dialogues[currentDialogueID].CurrentLineIndex;
        string next = dialogues[currentDialogueID].Lines[currentDialogueLineIndex].Next;
        
        if (EventManager.Instance.events.ContainsKey(next))  // Event인 경우
        {
            EventManager.Instance.CallEvent(next);
            EndDialogue();
        }
        else if (dialogues.ContainsKey(next))  // Dialogue인 경우
        {
            StartDialogue(next);
        }
        else if (string.IsNullOrWhiteSpace(next))  // 빈칸인 경우 다음 줄(대사)로 이동
        {
            currentDialogueLineIndex++;
            
            // Debug.Log($"currentDialogueLineIndex: {currentDialogueLineIndex}, len(dialogueLines): {dialogues[currentDialogueID].Lines.Count}");
            // Debug.Log($"dialogueID: {currentDialogueID}");
            
            if (currentDialogueLineIndex >= dialogues[currentDialogueID].Lines.Count)
            {
                EndDialogue();  // 더 이상 DialogueLine이 존재하지 않으면 대화 종료
                return;
            }
            dialogues[currentDialogueID].SetCurrentLineIndex(currentDialogueLineIndex);
            DialogueLine nextDialogueLine = dialogues[currentDialogueID].Lines[currentDialogueLineIndex]; 
            DisplayDialogueLine(nextDialogueLine);
        }
        else if (choices.ContainsKey(next)) // Choice인 경우
        {
            DisplayChoices(next);
        }
    }

    public void StartDialogue(string dialogueID)
    {
        isDialogueActive = true;
        dialogueCanvas.SetActive(true);
        dialogues[dialogueID].SetCurrentLineIndex(0);
        currentDialogueID = dialogueID;
        DialogueLine initialDialogueLine = dialogues[dialogueID].Lines[0]; 
        DisplayDialogueLine(initialDialogueLine);
    }

    private void DisplayDialogueLine(DialogueLine dialogueLine)
    {
        foreach (Transform child in choicesContainer)
        {
            Destroy(child.gameObject);
        }
        
        // 언어마다 다르게 불러오도록 변경 필요
        speakerText.text = scripts[dialogueLine.SpeakerID].KorScript;

        // 타자 효과 적용
        // scriptText.text = scripts[dialogueLine.ScriptID].KorScript;
        isTyping = true;
        StartCoroutine(TypeSentence(scripts[dialogueLine.ScriptID].KorScript));
        
        string imageID = dialogueLine.ImageID;
        if (string.IsNullOrWhiteSpace(imageID))
        {
            // Debug.Log("image ID does not exist!");
            characterImage.sprite = null;
            return;
        }

        // 성별마다 다르게 불러오도록 변경 필요
        Sprite characterSprite = Resources.Load<Sprite>(imagePaths[imageID].GirlPath);
        
        characterImage.sprite = characterSprite;
        characterImage.gameObject.SetActive(true);
    }

    IEnumerator TypeSentence(string sentence)
    {
        teddyBearIcon.gameObject.SetActive(false);
        scriptText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            scriptText.text += letter;
            yield return new WaitForSeconds(typeSpeed);
        }
        isTyping = false;
        teddyBearIcon.gameObject.SetActive(true);
    }
    
    private void EndDialogue()
    {
        isDialogueActive = false;
        dialogueCanvas.SetActive(false);
        characterImage.gameObject.SetActive(false);
    }
    
    // ---------------------------------------------- Choice methods ----------------------------------------------
    private void DisplayChoices(string choiceID)
    {
        foreach (Transform child in choicesContainer)
        {
            Destroy(child.gameObject);
        }

        List<ChoiceLine> choiceLines = choices[choiceID].Lines;

        foreach (ChoiceLine choiceLine in choiceLines)
        {
            var choiceButton = Instantiate(choicePrefab, choicesContainer).GetComponent<Button>();
            var choiceText = choiceButton.GetComponentInChildren<TextMeshProUGUI>();
            
            // 언어마다 다르게 불러오도록 변경 필요
            choiceText.text = scripts[choiceLine.ScriptID].KorScript;
            choiceButton.onClick.AddListener(() => OnChoiceSelected(choiceLine.Next));
        }
    }

    private void OnChoiceSelected(string dialogueID)
    {
        StartDialogue(dialogueID);
        foreach (Transform child in choicesContainer)
        {
            Destroy(child.gameObject);
        }
    }
    
    // ---------------------------------------------- Parse methods ----------------------------------------------
    private void ParseDialogues()
    {
        string[] lines = dialoguesCSV.text.Split('\n');

        string lastDialogueID = "";

        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;
            
            string[] fields = lines[i].Split(',');
            if (string.IsNullOrWhiteSpace(fields[0]) && string.IsNullOrWhiteSpace(fields[1])) continue;

            string dialogueID = fields[0].Trim();
            if (string.IsNullOrWhiteSpace(dialogueID)) dialogueID = lastDialogueID;
            else lastDialogueID = dialogueID;

            string speakerID = fields[1].Trim();
            string scriptID = fields[2].Trim();
            string imageID = fields[3].Trim();
            string next = fields[4].Trim();

            if (!dialogues.ContainsKey(dialogueID))
            {
                Dialogue dialogue = new Dialogue(dialogueID);
                dialogues[dialogueID] = dialogue;
            }

            dialogues[dialogueID].AddLine(speakerID, scriptID, imageID, next);
        }
    }

    private void ParseScripts()
    {
        string[] lines = scriptsCSV.text.Split("EOL");

        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;

            string[] fields = lines[i].Split(',');

            string scriptID = fields[0].Trim().Trim('\n');
            string engScript = fields[1].Trim().Replace("\\n", "\n");
            string korScript = fields[2].Trim().Replace("\\n", "\n");
            string jpMScript = fields[3].Trim().Replace("\\n", "\n");
            string jpWScript = fields[4].Trim().Replace("\\n", "\n");

            Script script = new Script(
                scriptID,
                engScript,
                korScript,
                jpMScript,
                jpWScript
            );
            scripts[scriptID] = script;
        }
    }

    private void ParseChoices()
    {
        string[] lines = choicesCSV.text.Split('\n');

        string lastChoiceID = "";
        
        for (int i = 1; i < lines.Length; i++)
        {
            if(string.IsNullOrWhiteSpace(lines[i])) continue;

            string[] fields = lines[i].Split(',');

            string choiceID = fields[0].Trim();
            if (string.IsNullOrWhiteSpace(choiceID)) choiceID = lastChoiceID;
            else lastChoiceID = choiceID;
            
            string scriptID = fields[1].Trim();
            string next = fields[2].Trim();

            if (!choices.ContainsKey(choiceID))
            {
                choices[choiceID] = new Choice(choiceID);
            }
            
            choices[choiceID].AddLine(scriptID, next);
        }
    }

    private void ParseImagePaths()
    {
        string[] lines = imagePathsCSV.text.Split('\n');

        for (int i = 1; i < lines.Length; i++)
        {
            if(string.IsNullOrWhiteSpace(lines[i])) continue;

            string[] fields = lines[i].Split(',');

            string imageID = fields[0].Trim();
            string girlPath = fields[1].Trim();
            string boyPath = fields[2].Trim();
            girlPath = $"Characters/{girlPath}";
            boyPath = $"Characters/{boyPath}";
            ImagePath imagePath = new ImagePath(
                imageID,
                girlPath,
                boyPath
            );
            imagePaths[imageID] = imagePath;
        }
    }
}
