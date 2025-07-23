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
    public DialogueType dialogueType = DialogueType.ROOM_ACCIDY; // 사용할 대화창 종류
    public GameObject[] dialogueSet;
    public TextMeshProUGUI[] speakerTexts;
    public TextMeshProUGUI[] scriptText;
    public Image[] backgroundImages;
    public Image[] characterImages;
    public Image[] characterFadeImages;
    public Transform[] choicesContainer;
    public GameObject choicePrefab;
    public GameObject[] skipText;
    public GameObject blurImage;
    [Header("teddyBearIcons")] public GameObject[] teddyBearIcons;
    [Header("Blocking Panels")] public Image[] blockingPanels;

    // 타자 효과 속도
    [Header("Typing Speed")]
    public float typeSpeed = 0.05f;

    // 자료 구조
    public Dictionary<string, Dialogue> dialogues = new Dictionary<string, Dialogue>();
    public Dictionary<string, Script> scripts = new Dictionary<string, Script>();
    private Dictionary<string, Choice> choices = new Dictionary<string, Choice>();
    private Dictionary<string, ImagePath> imagePaths = new Dictionary<string, ImagePath>();
    private Dictionary<string, ImagePath> backgrounds = new Dictionary<string, ImagePath>();

    // 상태 변수
    private string currentDialogueID = "";
    public bool isDialogueActive = false;
    private bool isTyping = false;
    private bool isAuto = false;
    private bool isFast = false;
    private string fullSentence;

    // Dialogue Queue
    private Queue<string> dialogueQueue = new Queue<string>();

    [SerializeField] private int yOffsetMinimumGirl;
    [SerializeField] private int yOffsetMaximumGirl;
    [SerializeField] private int yOffsetMinimumBoy;
    [SerializeField] private int yOffsetMaximumBoy;
    [SerializeField] private float scaleMinimumGirl;
    [SerializeField] private float scaleMaximumGirl;
    [SerializeField] private float scaleMinimumBoy;
    [SerializeField] private float scaleMaximumBoy;
    
    void Awake()
    {
        if (Instance == null)
        {
            DialoguesParser dialoguesParser = new DialoguesParser();
            dialogues = dialoguesParser.ParseDialogues();
            scripts = dialoguesParser.ParseScripts();
            choices = dialoguesParser.ParseChoices();
            imagePaths = dialoguesParser.ParseImagePaths();
            backgrounds = dialoguesParser.ParseBackgrounds();

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        dialogueSet[dialogueType.ToInt()].SetActive(false);
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

        // 대사가 2개 이상이라면 skip 버튼 활성화
        if (dialogues[dialogueID].Lines.Count > 1) 
            foreach (GameObject skip in skipText) 
                skip.SetActive(true);

        dialogues[dialogueID].SetCurrentLineIndex(0);
        currentDialogueID = dialogueID;
        DialogueLine initialDialogueLine = dialogues[dialogueID].Lines[0];

        blurImage.SetActive(initialDialogueLine.Blur == "TRUE");

        if (FollowManager.Instance)
            FollowManager.Instance.ClickObject();

        DisplayDialogueLine(initialDialogueLine);

        if (RoomManager.Instance) 
            RoomManager.Instance.SetButtons();

        MemoManager.Instance.SetMemoButtons(false);
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

        // 대사가 2개 이상이라면 skip 버튼 활성화
        if (dialogues[dialogueID].Lines.Count > 1) 
            foreach (GameObject skip in skipText) 
                skip.SetActive(true);

        dialogues[dialogueID].SetCurrentLineIndex(0);
        currentDialogueID = dialogueID;
        DialogueLine initialDialogueLine = dialogues[dialogueID].Lines[0];

        if (initialDialogueLine.Blur == "TRUE") 
            blurImage.SetActive(true);
        else 
            blurImage.SetActive(false);

        if (FollowManager.Instance) FollowManager.Instance.ClickObject();

        DisplayDialogueLine(initialDialogueLine);

        if (RoomManager.Instance) RoomManager.Instance.SetButtons();

        MemoManager.Instance.SetMemoButtons(false);
    }

    private void ClearPreviousChoices()
    {
        if (choicesContainer.Length > dialogueType.ToInt())
            foreach (Transform child in choicesContainer[dialogueType.ToInt()])
                Destroy(child.gameObject);
    }
    
    private void SetupCanvasAndSpeakerText(DialogueLine dialogueLine)
    {
        ChangeDialogueCanvas(dialogueLine.SpeakerID);

        // Deactivate all canvases and then activate the selected one.
        foreach (GameObject canvas in dialogueSet)
            if (canvas) 
                canvas.SetActive(false);
        dialogueSet[dialogueType.ToInt()].SetActive(true);

        // Update speaker text if not a special case.
        if (dialogueType != DialogueType.FOLLOW_EXTRA)
            foreach (TextMeshProUGUI speakerText in speakerTexts)
                speakerText.text = dialogueLine.SpeakerID == "DialogueC_003"
                    ? GameManager.Instance.GetVariable("FateName").ToString()
                    : scripts[dialogueLine.SpeakerID].GetScript();
    }
    
    private string ProcessPlaceholders(DialogueLine dialogueLine, out bool auto, out bool fast)
    {
        auto = false;
        fast = false;
    
        var sentence = scripts[dialogueLine.ScriptID].GetScript();
    
        if (scripts[dialogueLine.ScriptID].Placeholder.Length > 0)
        {
            var effects = scripts[dialogueLine.ScriptID].Placeholder.Split('/');
            foreach (var effect in effects)
                switch (effect)
                {
                    case "RED":
                        sentence = $"<color=red>{sentence}</color>";
                        break;
                    case "AUTO":
                        auto = true;
                        foreach (GameObject skip in skipText)
                            skip.SetActive(false);
                        break;
                    case "FAST":
                        fast = true;
                        break;
                    case "TRUE":
                        var fateName = (string)GameManager.Instance.GetVariable("FateName");
                        sentence = sentence.Replace("{PlayerName}", fateName);
                        break;
                    case "CENTER":
                        dialogueType = DialogueType.CENTER;
                        foreach (GameObject canvas in dialogueSet)
                            if (canvas)
                                canvas.SetActive(false);
                        dialogueSet[dialogueType.ToInt()].SetActive(true);
                        break;
                }
        }
        return sentence;
    }

    private void UpdateBackground(DialogueLine dialogueLine)
    {
        int accidyGender = (int)GameManager.Instance.GetVariable("AccidyGender");
        var backgroundID = dialogueLine.BackgroundID;
    
        foreach (var currentBackgroundImage in backgroundImages)
            if (string.IsNullOrWhiteSpace(backgroundID))
                currentBackgroundImage.color = new Color(1, 1, 1, 0);
            else
            {
                var backgroundSprite = Resources.Load<Sprite>(backgrounds[backgroundID].GetPath(accidyGender));
                currentBackgroundImage.sprite = backgroundSprite;
                currentBackgroundImage.color = new Color(1, 1, 1, 1);
            }
    }
    
    private void PlayDialogueSound(DialogueLine dialogueLine)
    {
        if (string.IsNullOrWhiteSpace(dialogueLine.SoundID))
            return;
        var soundID = "Sound_" + dialogueLine.SoundID;
        var soundNum = (int)typeof(Constants).GetField(soundID).GetValue(null);
        SoundPlayer.Instance.UISoundPlay(soundNum);
    }

    private void UpdateCharacterImages(DialogueLine dialogueLine)
    {
        int accidyGender = (int)GameManager.Instance.GetVariable("AccidyGender");
        var imageID = dialogueLine.ImageID;

        if (string.IsNullOrWhiteSpace(imageID))
        {
            foreach (var characterImage in characterImages)
                characterImage.color = new Color(1, 1, 1, 0);
            foreach (Image fadeImage in characterFadeImages)
                fadeImage.gameObject.SetActive(false);
            return;
        }

        var characterSprite = Resources.Load<Sprite>(imagePaths[imageID].GetPath(accidyGender));
        int zoomLevel = dialogueLine.ImageZoomLevel;
        int yOffset = GetCharacterYOffset(accidyGender, zoomLevel);
        float scale = GetCharacterScale(accidyGender, zoomLevel);

        foreach (Image characterImage in characterImages)
        {
            characterImage.color = new Color(1, 1, 1, 1);
            characterImage.sprite = characterSprite;
            characterImage.SetNativeSize();
            RectTransform rect = characterImage.GetComponent<RectTransform>();
            rect.anchoredPosition = new Vector3(0, yOffset, 0);
            rect.localScale = new Vector3(scale, scale, 1);
            characterImage.gameObject.SetActive(true);
        }

        if (dialogueLine.SpeakerID is "DialogueC_001" or "DialogueC_002")
            foreach (Image fadeImage in characterFadeImages)
                fadeImage.gameObject.SetActive(false);
        else
            foreach (Image fadeImage in characterFadeImages)
            {
                fadeImage.color = new Color(0, 0, 0, 0.8f);
                fadeImage.sprite = characterSprite;
                fadeImage.SetNativeSize();
                RectTransform rect = fadeImage.GetComponent<RectTransform>();
                rect.anchoredPosition = new Vector3(0, yOffset, 0);
                rect.localScale = new Vector3(scale, scale, 1);
                fadeImage.gameObject.SetActive(true);
            }
    }

    private void DisplayDialogueLine(DialogueLine dialogueLine)
    {
        ClearPreviousChoices();
        SetupCanvasAndSpeakerText(dialogueLine);

        // Process placeholders and get final sentence.
        string sentence = ProcessPlaceholders(dialogueLine, out bool auto, out bool fast);
        isAuto = auto;
        isFast = fast;

        isTyping = true;
        StartCoroutine(TypeSentence(sentence));
        
        UpdateBackground(dialogueLine);
        PlayDialogueSound(dialogueLine);
        UpdateCharacterImages(dialogueLine);
        
        UIManager.Instance.SetCursorAuto();
    }

    private int GetCharacterYOffset(int accidyGender, int characterImageZoomLevel)
    {
        if (characterImageZoomLevel is < 0 or > 10)
        {
            Debug.LogWarning("Invalid characterImageZoomLevel!");
            return 0;
        }
        
        return accidyGender == 0
            ? yOffsetMaximumGirl - (yOffsetMaximumGirl - yOffsetMinimumGirl) / 10 * characterImageZoomLevel
            : yOffsetMaximumBoy - (yOffsetMaximumBoy - yOffsetMinimumBoy) / 10 * characterImageZoomLevel;
    }
    
    private float GetCharacterScale(int accidyGender, int characterImageZoomLevel)
    {
        if (characterImageZoomLevel is < 0 or > 10)
        {
            Debug.LogWarning("Invalid characterImageZoomLevel!");
            return 0;
        }
        
        return accidyGender == 0
            ? scaleMinimumGirl + (scaleMaximumGirl - scaleMinimumGirl) / 10 * characterImageZoomLevel
            : scaleMinimumBoy + (scaleMaximumBoy - scaleMinimumBoy) / 10 * characterImageZoomLevel;
    }

    private void ChangeDialogueCanvas(string speaker)
    {
        // 미행 대화창
        if ((int)GameManager.Instance.GetVariable("CurrentScene") is 2 or 4)
        {
            switch (speaker)
            {
                case "DialogueC_004":
                    FollowManager.Instance.EndExtraDialogue(false);
                    dialogueType = DialogueType.FOLLOW_THINKING;
                    break;
                case "DialogueC_002" or "DialogueC_003":
                    FollowManager.Instance.EndExtraDialogue(false);
                    dialogueType = DialogueType.FOLLOW;
                    break;
                // 행인의 대사인 경우
                default:
                    FollowManager.Instance.EndExtraDialogue(false);
                    FollowManager.Instance.OpenExtraDialogue(speaker);
                    break;
            }

            return;
        }

        switch (speaker)
        {
            case "DialogueC_001" or "DialogueC_002":
                dialogueType = DialogueType.ROOM_ACCIDY;
                break;
            case "DialogueC_003" or "DialogueC_008":
                dialogueType = DialogueType.ROOM_FATE;
                break;
            case "DialogueC_004":
                dialogueType = DialogueType.ROOM_THINKING;
                break;
        }
    }
    
    public void EndDialogue()
    {
        isDialogueActive = false;
        dialogueSet[dialogueType.ToInt()].SetActive(false);
        foreach (Image characterImage in characterImages)
            characterImage.gameObject.SetActive(false);
        if (dialogueQueue.Count > 0)  // 큐에 다이얼로그가 들어있으면 다시 대화 시작
        {
            string queuedDialogueID = dialogueQueue.Dequeue();
            StartDialogue(queuedDialogueID);

            return;
        }

        MemoManager.Instance.SetMemoButtons(true);
        
        blurImage.SetActive(false);
        
        UIManager.Instance.SetCursorAuto();
        
        if (FollowManager.Instance) FollowManager.Instance.EndScript();
        if (!RoomManager.Instance) return;

        var refillHeartsOrEndDay = (bool)GameManager.Instance.GetVariable("RefillHeartsOrEndDay");
        var isChoosingBrokenBearChoice = false;
        var isInvestigating = RoomManager.Instance.GetIsInvestigating();

        if ((int)GameManager.Instance.GetVariable("CurrentScene") == SceneType.ROOM_2.ToInt())
            isChoosingBrokenBearChoice = RoomManager.Instance.room2ActionPointManager.GetChoosingBrokenBearChoice();

        if (refillHeartsOrEndDay && !isChoosingBrokenBearChoice && !isInvestigating)
            RoomManager.Instance.actionPointManager.RefillHeartsOrEndDay();

        // 튜토리얼 중 다른 곳 클릭하면 나오는 강조 이미지가 해당 "ㅁㅁ를 조사해보자" 스크립트 다 끝나면 자동으로 강조 이미지 꺼지게 함.
        if ((bool)GameManager.Instance.GetVariable("isTutorial") &&
            RoomManager.Instance.imageAndLockPanelManager.GetIsTutorialObjectActive())
            RoomManager.Instance.imageAndLockPanelManager.OnExitButtonClick();
        RoomManager.Instance.SetButtons();
    }

    public void SkipButtonClick()
    {
        StopAllCoroutines();

        dialogues[currentDialogueID].SetCurrentLineIndex(dialogues[currentDialogueID].Lines.Count - 2);
        StartCoroutine(SkipDialogue());
    }

    private IEnumerator SkipDialogue()
    {
        ProceedToNext();

        while (!isTyping) yield return null;

        CompleteSentence();
        if (isFast)
        {
            typeSpeed *= 1.75f; // 타이핑 속도 되돌려 놓기
            isFast = false;
        }
        if (isAuto) isAuto = false;

        OnDialoguePanelClick();
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
            else if (currentDialogueLineIndex == dialogues[currentDialogueID].Lines.Count - 1)
            {
                foreach (GameObject skip in skipText) skip.SetActive(false); //  다이얼로그의 마지막 대사는 스킵 불가능
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
        if(teddyBearIcons.Length > dialogueType.ToInt()) teddyBearIcons[dialogueType.ToInt()].SetActive(false);
        scriptText[dialogueType.ToInt()].text = "";
        fullSentence = sentence;

        // <color=red> 같은 글씨 효과들은 출력되지 않도록 변수 설정
        var isEffect = false;
        var effectText = "";

        // FAST 인 경우 두배의 속도로 타이핑
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
        if (teddyBearIcons.Length > dialogueType.ToInt()) teddyBearIcons[dialogueType.ToInt()].SetActive(true);

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

            foreach (GameObject skip in skipText) skip.SetActive(true);
        }
    }

    public void OnDialoguePanelClick()
    {
        if (!isDialogueActive || isAuto) return;

        if (isTyping)
        {
            CompleteSentence();
        }
        else
        {
            ProceedToNext();
        }
    }

    private void CompleteSentence()
    {
        StopAllCoroutines();
        scriptText[dialogueType.ToInt()].text = fullSentence;
        isTyping = false;
        if (teddyBearIcons.Length > dialogueType.ToInt()) teddyBearIcons[dialogueType.ToInt()].SetActive(true);
    }

    // ---------------------------------------------- Choice methods ----------------------------------------------
    private void DisplayChoices(string choiceID)
    {
        if (choicesContainer.Length <= dialogueType.ToInt()) return;

        if (blockingPanels.Length > dialogueType.ToInt()) blockingPanels[dialogueType.ToInt()].color = new Color(0, 0, 0, 0.7f);

        foreach (Transform child in choicesContainer[dialogueType.ToInt()])
        {
            Destroy(child.gameObject);
        }

        List<ChoiceLine> choiceLines = choices[choiceID].Lines;

        foreach (ChoiceLine choiceLine in choiceLines)
        {
            var choiceButton = Instantiate(choicePrefab, choicesContainer[dialogueType.ToInt()]).GetComponent<Button>();
            var choiceText = choiceButton.GetComponentInChildren<TextMeshProUGUI>();

            choiceText.text = scripts[choiceLine.ScriptID].GetScript();
            choiceButton.onClick.AddListener(() => OnChoiceSelected(choiceLine.Next));

            // 만약 잠김 선택지(엔딩)라면 잠김으로 뜨도록 표시
            if (scripts[choiceLine.ScriptID].Placeholder.Length <= 0) continue;
            var effects = scripts[choiceLine.ScriptID].Placeholder.Split('/');
            foreach (var effect in effects)
            {
                switch (effect)
                {
                    case "END_RUN":
                        choiceButton.onClick.AddListener(() => EndingManager.Instance.ChoiceEnding());
                        choiceButton.onClick.AddListener(() => EventManager.Instance.CallEvent("EventChangeBGMOfEnd_RUN"));
                        break;
                    case "END_STAY":
                        choiceButton.onClick.AddListener(() => EndingManager.Instance.ChoiceEnding());
                        choiceButton.onClick.AddListener(() => EventManager.Instance.CallEvent("EventChangeBGMOfEnd_STAY"));
                        break;
                    case "LOCK":
                        choiceText.text = "<color=#101010>" + scripts[choiceLine.ScriptID].GetScript() + "</color>";
                        choiceButton.onClick.RemoveAllListeners();
                        break;
                }
            }
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

        if (blockingPanels.Length > dialogueType.ToInt()) blockingPanels[dialogueType.ToInt()].color = new Color(0, 0, 0, 0);

    }

}
