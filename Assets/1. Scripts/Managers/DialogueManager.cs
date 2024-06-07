using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
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
    public Image characterImage;
    public Transform[] choicesContainer;
    public GameObject choicePrefab;
    [Header("teddyBearIcons")]public GameObject[] teddyBearIcons;
    [Header("Blocking Panels")] public Image[] blockingPanels;
    
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
    private bool isAuto = false;
    private bool isFast = false;
    private string fullSentence;
    
    // Dialogue Queue
    private Queue<string> dialogueQueue = new Queue<string>();
    
    // Blocking Panel
     

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
        
        dialogues[dialogueID].SetCurrentLineIndex(0);
        currentDialogueID = dialogueID;
        DialogueLine initialDialogueLine = dialogues[dialogueID].Lines[0];
        DisplayDialogueLine(initialDialogueLine);
        
        if (RoomManager.Instance) RoomManager.Instance.SetButtons();
    }

    // 대사 출력을 second초 후에 출력을 시작함. (휴식 시스템에서 눈 깜빡이는 5초 후에 출력되게 함)
    // 기본값 second 0으로 넣기
    public IEnumerator StartDialogue(string dialogueID, float second = 0f)
    {
        yield return new WaitForSeconds(second);

        if (isDialogueActive)  // 이미 대화가 진행중이면 큐에 넣음
        {
            Debug.Log($"dialogue ID: {dialogueID} queued!");

            dialogueQueue.Enqueue(dialogueID);
            yield break;
        }

        isDialogueActive = true;

        dialogues[dialogueID].SetCurrentLineIndex(0);
        currentDialogueID = dialogueID;
        DialogueLine initialDialogueLine = dialogues[dialogueID].Lines[0];
        DisplayDialogueLine(initialDialogueLine);

        if (RoomManager.Instance) RoomManager.Instance.SetButtons();
    }

    private void DisplayDialogueLine(DialogueLine dialogueLine)
    {
        foreach (Transform child in choicesContainer[dialogueType.ToInt()])
        {
            Destroy(child.gameObject);
        }

        // 화자에 따라 대화창 변경
        ChangeDialogueCanvas(dialogueLine.SpeakerID);

        // 사용할 대화창을 제외한 다른 대화창을 꺼둔다
        foreach (GameObject canvas in dialogueCanvas) canvas.SetActive(false);
        dialogueCanvas[dialogueType.ToInt()].SetActive(true);

        // 미행의 행인은 별도의 SpeakerID를 가짐
        if(dialogueType != DialogueType.FOLLOW_EXTRA)
            speakerText.text = scripts[dialogueLine.SpeakerID].GetScript();

        // 타자 효과 적용
        isTyping = true;
        string sentence = scripts[dialogueLine.ScriptID].GetScript();
        isAuto = false;
        if (scripts[dialogueLine.ScriptID].Placeholder.Length > 0)
        {
            string[] effects = scripts[dialogueLine.ScriptID].Placeholder.Split('/');
            for (int i = 0; i < effects.Length; i++)
            {
                switch (effects[i])
                {
                    case "RED":
                        sentence = "<color=red>" + sentence + "</color>";
                        break;
                    case "AUTO":
                        isAuto = true;
                        break;
                    case "FAST":
                        isFast = true;
                        break;
                    case "TRUE":
                        string fateName = (string)GameManager.Instance.GetVariable("FateName");
                        sentence = sentence.Replace("{PlayerName}", fateName);
                        break;
                }
            }
        }
        StartCoroutine(TypeSentence(sentence));

        // 화자 이미지 표시
        string imageID = dialogueLine.ImageID;
        if (string.IsNullOrWhiteSpace(imageID))
        {
            characterImage.color = new Color(1, 1, 1, 0);
        }
        else
        {
            int accidyGender = (int)GameManager.Instance.GetVariable("AccidyGender");
            Sprite characterSprite = Resources.Load<Sprite>(imagePaths[imageID].GetPath(accidyGender));
            int yOffset = (accidyGender == 0) ? -195 : -150;

            characterImage.color = new Color(1, 1, 1, 1);
            characterImage.sprite = characterSprite;
            characterImage.SetNativeSize();
            characterImage.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, yOffset, 0);
            characterImage.gameObject.SetActive(true);
        }
        
    }
    private void ChangeDialogueCanvas(string speaker)
    {
        // 방 대화창
        if (dialogueType == DialogueType.ROOM && speaker == "DialogueC_004")
            dialogueType = DialogueType.ROOM_THINKING;
        else if (dialogueType == DialogueType.ROOM_THINKING && speaker != "DialogueC_004")
            dialogueType = DialogueType.ROOM;

        // 미행 대화창
        else if(dialogueType == DialogueType.FOLLOW || dialogueType == DialogueType.FOLLOW_THINKING || dialogueType == DialogueType.FOLLOW_EXTRA)
        {
            if (speaker == "DialogueC_004")
            {
                FollowManager.Instance.EndExtraDialogue(false);
                dialogueType = DialogueType.FOLLOW_THINKING;
            }
            else if (speaker == "DialogueC_002" || speaker == "DialogueC_003")
            {
                FollowManager.Instance.EndExtraDialogue(false);
                dialogueType = DialogueType.FOLLOW;
            }
            else // 행인의 대사인 경우
            {
                FollowManager.Instance.EndExtraDialogue(false);
                FollowManager.Instance.OpenExtraDialogue(FollowManager.Instance.ToEnum(speaker));
            }
        }
    }
    public void EndDialogue()
    {
        // 대화가 끝날 때 현재 미행 파트라면 추가적인 로직 처리 (애니메이션 재생 등)
        if (SceneManager.Instance.CurrentScene == SceneType.FOLLOW_1 || SceneManager.Instance.CurrentScene == SceneType.FOLLOW_2)
            FollowManager.Instance.EndScript(true);

        isDialogueActive = false;
        dialogueCanvas[dialogueType.ToInt()].SetActive(false);
        characterImage.gameObject.SetActive(false);
        if (dialogueQueue.Count > 0)  // 큐에 다이얼로그가 들어있으면 다시 대화 시작
        {
            string queuedDialogueID = dialogueQueue.Dequeue();
            StartDialogue(queuedDialogueID);

            if (SceneManager.Instance.CurrentScene == SceneType.FOLLOW_1 || SceneManager.Instance.CurrentScene == SceneType.FOLLOW_2)
                FollowManager.Instance.ClickObject();

            return;
        }

        if (RoomManager.Instance) 
        {
            // 튜토리얼 중 다른 곳 클릭하면 나오는 강조 이미지가 해당 "ㅁㅁ를 조사해보자" 스크립트 다 끝나면 자동으로 강조 이미지 꺼지게 함.
            if ((bool)GameManager.Instance.GetVariable("isTutorial") && RoomManager.Instance.imageAndLockPanelManager.GetIsTutorialObjectActive())
            {
                RoomManager.Instance.imageAndLockPanelManager.OnExitButtonClick();
            }
            RoomManager.Instance.SetButtons(); 
        }
        
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
            EndDialogue();
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
    IEnumerator TypeSentence(string sentence)
    {
        teddyBearIcons[dialogueType.ToInt()].SetActive(false);
        scriptText[dialogueType.ToInt()].text = "";
        fullSentence = sentence;

        // <color=red> 같은 글씨 효과들은 출력되지 않도록 변수 설정
        bool isEffect = false;
        string effectText = "";

        // FAST 인 경우 두배의 속도로 타이핑 + 끝나면 자동으로 넘어감
        if (isFast) typeSpeed /= 1.75f;

        foreach (char letter in sentence.ToCharArray())
        {
            if (letter == '<')
            {
                effectText = ""; // effectText 초기화
                isEffect = true;
            }
            else if (letter == '>') // > 가 나오면 scriptText에 한번에 붙인다
            {
                effectText += letter;
                scriptText[dialogueType.ToInt()].text += effectText;
                isEffect = false;
                continue;
            }

            if (isEffect) // < 가 나온 이후부터는 effectText에 붙인다
            {
                effectText += letter;
                continue;
            }

            scriptText[dialogueType.ToInt()].text += letter;
            SoundPlayer.Instance.UISoundPlay(Sound_Typing); // 타자 소리 한번씩만
            yield return new WaitForSeconds(typeSpeed);
        }
        isTyping = false;
        teddyBearIcons[dialogueType.ToInt()].SetActive(true);

        if (isFast)
        {
            typeSpeed *= 1.75f; // 타이핑 속도 되돌려 놓기
            isFast = false;
        }
        if (isAuto)
        {
            isAuto = false;
            yield return new WaitForSeconds(0.25f);
            OnDialoguePanelClick(); // 자동으로 넘어감
        }
    }
    
    public void OnDialoguePanelClick()
    {
        if (isDialogueActive && !isAuto)
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
        teddyBearIcons[dialogueType.ToInt()].SetActive(true);
    }
    
    // ---------------------------------------------- Choice methods ----------------------------------------------
    private void DisplayChoices(string choiceID)
    {
        blockingPanels[dialogueType.ToInt()].color = new Color(0, 0, 0, 0.7f);
        
        foreach (Transform child in choicesContainer[dialogueType.ToInt()])
        {
            Destroy(child.gameObject);
        }

        List<ChoiceLine> choiceLines = choices[choiceID].Lines;

        foreach (ChoiceLine choiceLine in choiceLines)
        {
            var choiceButton = Instantiate(choicePrefab, choicesContainer[dialogueType.ToInt()]).GetComponent<Button>();
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
            EndDialogue();
            StartDialogue(next);
        }
        else if (EventManager.Instance.events.ContainsKey(next))
        {
            EventManager.Instance.CallEvent(next);
        }
        
        foreach (Transform child in choicesContainer[dialogueType.ToInt()])
        {
            Destroy(child.gameObject);
        }
        
        blockingPanels[dialogueType.ToInt()].color = new Color(0, 0, 0, 0);

    }
    
}
