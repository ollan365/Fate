using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Constants;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    // Dialogue UI
    [Header("Dialogue UI")]
    public DialogueType dialogueType = DialogueType.ROOM; // 사용할 대화창 종류
    public GameObject[] dialogueCanvas;
    public TextMeshProUGUI speakerText;
    public TextMeshProUGUI[] scriptText;
    public SpriteRenderer characterImage;
    public GameObject[] dialoguePanel;
    public Transform choicesContainer;
    public GameObject choicePrefab;
    public GameObject teddyBearIcon;
    
    // 타자 효과 속도
    [Header("Typing Speed")]
    public float typeSpeed = 0.05f;
    
    // 자료 구조
    public Dictionary<string, Dialogue> dialogues = new Dictionary<string, Dialogue>();
    public Dictionary<string, Script> scripts = new Dictionary<string, Script>();
    private Dictionary<string, Choice> choices = new Dictionary<string, Choice>();
    private Dictionary<string, ImagePath> imagePaths = new Dictionary<string, ImagePath>();

    // 상태 변수
    private string currentDialogueID = "";
    public bool isDialogueActive = false;
    private bool isTyping = false;
    private string fullSentence;
    
    // Dialogue Queue
    private Queue<string> dialogueQueue = new Queue<string>();

    void Awake()
    {
        if (Instance == null)
        {
            DialoguesParser dialoguesParser = new DialoguesParser();
            dialogues = dialoguesParser.ParseDialogues();
            scripts = dialoguesParser.ParseScripts();
            choices = dialoguesParser.ParseChoices();
            imagePaths = dialoguesParser.ParseImagePaths();
            
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        dialogueCanvas[dialogueType.ToInt()].SetActive(false);
    }

    // ---------------------------------------------- Dialogue methods ----------------------------------------------
    public void StartDialogue(string dialogueID)
    {
        if (isDialogueActive)  // 이미 대화가 진행중이면 큐에 넣음
        {
            Debug.Log($"dialogue ID: {dialogueID} queued!");
            
            dialogueQueue.Enqueue(dialogueID);
            return;
        }
        
        isDialogueActive = true;

        // 사용할 대화창을 제외한 다른 대화창을 꺼둔다
        foreach (GameObject canvas in dialogueCanvas) canvas.SetActive(false);
        dialogueCanvas[dialogueType.ToInt()].SetActive(true);

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
        
        speakerText.text = scripts[dialogueLine.SpeakerID].GetScript();

        // 타자 효과 적용
        isTyping = true;
        string sentence = scripts[dialogueLine.ScriptID].GetScript();
        if (scripts[dialogueLine.ScriptID].Placeholder.Length > 0)
        {
            string fateName = (string)GameManager.Instance.GetVariable("FateName");
            sentence = sentence.Replace("{PlayerName", fateName);
        }
        StartCoroutine(TypeSentence(sentence));

        // 미행 파트라면 대화창 크기 조절
        if (dialogueType == DialogueType.FOLLOW || dialogueType == DialogueType.FOLLOW_ANGRY) SetPanelSize(sentence);

        // 화자 이미지 표시
        string imageID = dialogueLine.ImageID;
        if (string.IsNullOrWhiteSpace(imageID))
        {
            characterImage.sprite = null;
        }
        else
        {
            int accidyGender = (int)GameManager.Instance.GetVariable("AccidyGender");
            Sprite characterSprite = Resources.Load<Sprite>(imagePaths[imageID].GetPath(accidyGender));
            int yOffset = (accidyGender == 0) ? -650 : -844;

            characterImage.sprite = characterSprite;
            characterImage.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, yOffset, 0);
            characterImage.gameObject.SetActive(true);
        }
        
    }

    private void EndDialogue()
    {
        isDialogueActive = false;
        dialogueCanvas[dialogueType.ToInt()].SetActive(false);
        characterImage.gameObject.SetActive(false);
        if (dialogueQueue.Count > 0)  // 큐에 다이얼로그가 들어있으면 다시 대화 시작
        {
            string queuedDialogueID = dialogueQueue.Dequeue();
            StartDialogue(queuedDialogueID);
            return;
        }

        // 대화가 끝날 때 현재 미행 파트라면 추가적인 로직 처리 (애니메이션 재생 등)
        if (dialogueType == DialogueType.FOLLOW) FollowManager.Instance.EndScript();
    }
    
    // ---------------------------------------------- Script methods ----------------------------------------------
    private void ProceedToNext()
    {
        int currentDialogueLineIndex = dialogues[currentDialogueID].CurrentLineIndex;
        string next = dialogues[currentDialogueID].Lines[currentDialogueLineIndex].Next;
        
        if (EventManager.Instance.events.ContainsKey(next))  // Event인 경우
        {
            EndDialogue();
            EventManager.Instance.CallEvent(next);
        }
        else if (dialogues.ContainsKey(next))  // Dialogue인 경우
        {
            StartDialogue(next);
        }
        else if (string.IsNullOrWhiteSpace(next))  // 빈칸인 경우 다음 줄(대사)로 이동
        {
            currentDialogueLineIndex++;
            
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
    private void SetPanelSize(string scriptText)
    {
        // scriptText의 길이를 기반으로 대화창의 크기를 조절
        float preferredWidth = Mathf.Clamp(scriptText.Length, 21, 60) * 100;

        // 대화창의 RectTransform 가져오기
        RectTransform panelRect = dialoguePanel[dialogueType.ToInt()].GetComponent<RectTransform>();

        // 대화창의 너비를 Text의 Preferred Width로 설정
        panelRect.sizeDelta = new Vector2(preferredWidth, panelRect.sizeDelta.y);
    }
    IEnumerator TypeSentence(string sentence)
    {
        teddyBearIcon.SetActive(false);
        scriptText[dialogueType.ToInt()].text = "";
        fullSentence = sentence;
        foreach (char letter in sentence.ToCharArray())
        {
            scriptText[dialogueType.ToInt()].text += letter;
            yield return new WaitForSeconds(typeSpeed);
        }
        isTyping = false;
        teddyBearIcon.SetActive(true);
    }
    
    public void OnDialoguePanelClick()
    {
        if (isDialogueActive)
        {
            if (isTyping)
            {
                CompleteSentence();
            }
            else
            {
                ProceedToNext();
            }
        }
    }
    
    private void CompleteSentence()
    {
        StopAllCoroutines();
        scriptText[dialogueType.ToInt()].text = fullSentence;
        isTyping = false;
        teddyBearIcon.SetActive(true);
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
            choiceText.text = scripts[choiceLine.ScriptID].GetScript();
            choiceButton.onClick.AddListener(() => OnChoiceSelected(choiceLine.Next));
        }
    }

    private void OnChoiceSelected(string next)
    {
        if (dialogues.ContainsKey(next))
        {
            StartDialogue(next);
        }
        else if (EventManager.Instance.events.ContainsKey(next))
        {
            EventManager.Instance.CallEvent(next);
        }
        
        foreach (Transform child in choicesContainer)
        {
            Destroy(child.gameObject);
        }
    }
    
}
