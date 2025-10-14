using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
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
    [Header("Localization")]
    [SerializeField] private LocalizedText[] scriptLocalizedTexts;
    public TextEffect accidyMultiScript;
    public Image[] backgroundImages;
    public Image[] characterImages;
    public Image[] characterFadeImages;
    public Transform[] choicesContainer;
    public GameObject choicePrefab;
    public GameObject[] skipText;
    
    // 엔딩 연출용
    public Image endingDialougeBlackImage;
    public GameObject endingDialogueNextPanel;

    [Header("teddyBearIcons")] public GameObject[] teddyBearIcons;

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
    private bool isMulti = false;
    private bool isEnding = false;
    private string fullSentence;

    // Dialogue Queue
    private readonly Queue<string> dialogueQueue = new Queue<string>();

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

        // Cache LocalizedText components attached to script text objects
        if (scriptText != null && scriptText.Length > 0)
        {
            scriptLocalizedTexts = new LocalizedText[scriptText.Length];
            for (int i = 0; i < scriptText.Length; i++)
                if (scriptText[i])
                    scriptLocalizedTexts[i] = scriptText[i].GetComponent<LocalizedText>();
        }
    }

    // ---------------------------------------------- Dialogue methods ----------------------------------------------
    public void StartDialogue(string dialogueID)
    {
        if (isDialogueActive)  // 이미 대화가 진행중이면 큐에 넣음
        {
            if (GameManager.Instance.isDebug)
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
        UIManager.Instance.SetUI(eUIGameObjectName.BlurImage, initialDialogueLine.Blur == "TRUE", true);
        DisplayDialogueLine(initialDialogueLine);

        if (RoomManager.Instance) 
            RoomManager.Instance.SetButtons();
        if (FollowManager.Instance) 
            FollowManager.Instance.ClickObject();
        if (MemoManager.Instance)
            MemoManager.Instance.SetMemoButtons(false);

        GameManager.Instance.SetVariable("currentDialogueID", currentDialogueID);
        GameManager.Instance.SetVariable("isDialogueActive", isDialogueActive);
        SaveManager.Instance.SaveGameData();
    }
    
    public IEnumerator StartDialogue(string dialogueID, float delay = 0f)
    {
        if (delay > 0f) 
            yield return new WaitForSeconds(delay);

        if (isDialogueActive)  // 이미 대화가 진행중이면 큐에 넣음
        {
            if (GameManager.Instance.isDebug)
                Debug.Log($"dialogue ID: {dialogueID} queued!");

            dialogueQueue.Enqueue(dialogueID);
            if (delay > 0f) 
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
            UIManager.Instance.SetUI(eUIGameObjectName.BlurImage, true, true);
        else if (!GetIsImageOrLockPanelActive())
            UIManager.Instance.SetUI(eUIGameObjectName.BlurImage, false, true);

        if (FollowManager.Instance) FollowManager.Instance.ClickObject();

        DisplayDialogueLine(initialDialogueLine);

        if (RoomManager.Instance) 
            RoomManager.Instance.SetButtons();
        if (FollowManager.Instance) 
            FollowManager.Instance.ClickObject();
        if (MemoManager.Instance)
            MemoManager.Instance.SetMemoButtons(false);

        GameManager.Instance.SetVariable("currentDialogueID", currentDialogueID);
        GameManager.Instance.SetVariable("isDialogueActive", isDialogueActive);
        SaveManager.Instance.SaveGameData();
    }

    private void ClearPreviousChoices()
    {
        if (choicesContainer.Length > dialogueType.ToInt())
            foreach (Transform child in choicesContainer[dialogueType.ToInt()])
                Destroy(child.gameObject);
    }
    
    private void SetupCanvasAndSpeakerText(DialogueLine dialogueLine) {
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
                    : scripts[dialogueLine.SpeakerID].GetScript().ProcessedText;
    }
    
    private string ProcessPlaceholders(DialogueLine dialogueLine, out bool auto, out bool fast, out bool multi, out bool ending) {
        auto = false;
        fast = false;
        multi = false;
        ending = false;

        Script.ScriptPlaceholderResult processed = scripts[dialogueLine.ScriptID].GetScript();
        if (processed.Auto) {
            auto = true;
            foreach (GameObject skip in skipText)
                skip.SetActive(false);
        }
        if (processed.Fast)
            fast = true;
        if (processed.Multi) {
            multi = true;
            accidyMultiScript.gameObject.SetActive(true);
        }
        if (processed.Ending) {
            ending = true;
            dialogueType = DialogueType.ENDING;
            foreach (GameObject canvas in dialogueSet)
                if (canvas)
                    canvas.SetActive(false);
            dialogueSet[dialogueType.ToInt()].SetActive(true);
        }

        return processed.ProcessedText;
    }

    private void UpdateBackground(DialogueLine dialogueLine) {
        int accidyGender = (int)GameManager.Instance.GetVariable("AccidyGender");
        var backgroundID = dialogueLine.BackgroundID;

        foreach (var currentBackgroundImage in backgroundImages) {
            if (string.IsNullOrWhiteSpace(backgroundID))
                currentBackgroundImage.color = new Color(1, 1, 1, 0);
            else {
                var backgroundSprite = Resources.Load<Sprite>(backgrounds[backgroundID].GetPath(accidyGender));
                currentBackgroundImage.sprite = backgroundSprite;
                currentBackgroundImage.color = new Color(1, 1, 1, 1);
            }
        }
    }

    private void PlayDialogueSound(DialogueLine dialogueLine) {
        if (string.IsNullOrWhiteSpace(dialogueLine.SoundID))
            return;

        if (dialogueLine.SoundID.Contains("BGM_"))
        {
            var bgmNum = (int)typeof(Constants).GetField(dialogueLine.SoundID).GetValue(null);
            SoundPlayer.Instance.ChangeBGM(bgmNum);
        }
        else
        {
            var soundID = "Sound_" + dialogueLine.SoundID;
            var soundNum = (int)typeof(Constants).GetField(soundID).GetValue(null);
            SoundPlayer.Instance.UISoundPlay(soundNum);
        }
    }

    private void UpdateCharacterImages(DialogueLine dialogueLine) {
        int accidyGender = (int)GameManager.Instance.GetVariable("AccidyGender");
        var imageID = dialogueLine.ImageID;
        if (string.IsNullOrWhiteSpace(imageID)) {
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

        foreach (Image characterImage in characterImages) {
            characterImage.color = new Color(1, 1, 1, 1);
            characterImage.sprite = characterSprite;
            characterImage.SetNativeSize();
            RectTransform rect = characterImage.GetComponent<RectTransform>();
            rect.anchoredPosition = new Vector3(0, yOffset, 0);
            rect.localScale = new Vector3(scale, scale, 1);
            characterImage.gameObject.SetActive(true);
        }

        if (dialogueLine.SpeakerID is "DialogueC_001" or "DialogueC_002") {
            foreach (Image fadeImage in characterFadeImages)
                fadeImage.gameObject.SetActive(false);
        } else
            foreach (Image fadeImage in characterFadeImages) {
                fadeImage.color = new Color(0, 0, 0, 0.8f);
                fadeImage.sprite = characterSprite;
                fadeImage.SetNativeSize();
                RectTransform rect = fadeImage.GetComponent<RectTransform>();
                rect.anchoredPosition = new Vector3(0, yOffset, 0);
                rect.localScale = new Vector3(scale, scale, 1);
                fadeImage.gameObject.SetActive(true);
            }
    }

    private void DisplayDialogueLine(DialogueLine dialogueLine) {
        UIManager.Instance.GetUI(eUIGameObjectName.ObjectImageParentRoom).GetComponent<CanvasGroup>().blocksRaycasts = false;
        if (RoomManager.Instance && 
            RoomManager.Instance.imageAndLockPanelManager &&
            RoomManager.Instance.imageAndLockPanelManager.currentLockObjectCanvasGroup)
            RoomManager.Instance.imageAndLockPanelManager.currentLockObjectCanvasGroup.blocksRaycasts = false;
        
        ClearPreviousChoices();
        SetupCanvasAndSpeakerText(dialogueLine);

        // Store script ID on LocalizedText so it can re-localize on language changes
        int canvasIndex = dialogueType.ToInt();
        if (scriptLocalizedTexts != null && canvasIndex < scriptLocalizedTexts.Length && scriptLocalizedTexts[canvasIndex] != null)
            scriptLocalizedTexts[canvasIndex].SetScriptId(dialogueLine.ScriptID);

        // Process placeholders and get final sentence.
        string sentence = ProcessPlaceholders(dialogueLine, out bool auto, out bool fast, out bool multi, out bool ending);
        isAuto = auto;
        isFast = fast;
        isMulti = multi;
        isEnding = ending;

        isTyping = true;
        StartCoroutine(TypeSentence(sentence));
        
        UpdateBackground(dialogueLine);
        PlayDialogueSound(dialogueLine);
        UpdateCharacterImages(dialogueLine);
        
        UIManager.Instance.SetCursorAuto();
    }

    private int GetCharacterYOffset(int accidyGender, int characterImageZoomLevel) {
        if (characterImageZoomLevel is < 0 or > 10) {
            Debug.LogWarning("Invalid characterImageZoomLevel!");
            return 0;
        }
        
        return accidyGender == 0
            ? yOffsetMaximumGirl - (yOffsetMaximumGirl - yOffsetMinimumGirl) / 10 * characterImageZoomLevel
            : yOffsetMaximumBoy - (yOffsetMaximumBoy - yOffsetMinimumBoy) / 10 * characterImageZoomLevel;
    }
    
    private float GetCharacterScale(int accidyGender, int characterImageZoomLevel) {
        if (characterImageZoomLevel is < 0 or > 10) {
            Debug.LogWarning("Invalid characterImageZoomLevel!");
            return 0;
        }
        
        return accidyGender == 0
            ? scaleMinimumGirl + (scaleMaximumGirl - scaleMinimumGirl) / 10 * characterImageZoomLevel
            : scaleMinimumBoy + (scaleMaximumBoy - scaleMinimumBoy) / 10 * characterImageZoomLevel;
    }

    private void ChangeDialogueCanvas(string speaker) {
        // 미행 대화창
        if (GameSceneManager.Instance.GetActiveScene() is SceneType.FOLLOW_1 or SceneType.FOLLOW_2) {
            switch (speaker) {
                case "DialogueC_004":
                    FollowManager.Instance.EndExtraDialogue(false);
                    dialogueType = DialogueType.FOLLOW_THINKING;
                    break;
                case "DialogueC_002" or "DialogueC_003":
                    FollowManager.Instance.EndExtraDialogue(false);
                    dialogueType = DialogueType.FOLLOW;
                    break;
                default: // 행인의 대사인 경우
                    FollowManager.Instance.EndExtraDialogue(false);
                    FollowManager.Instance.OpenExtraDialogue(speaker);
                    break;
            }
            return;
        }

        switch (speaker) {
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
    
    private void EndDialogue() {
        isDialogueActive = false;
        dialogueSet[dialogueType.ToInt()].SetActive(false);
        foreach (Image characterImage in characterImages)
            characterImage.gameObject.SetActive(false);
        
        if (dialogueQueue.Count > 0) {  // 큐에 다이얼로그가 들어있으면 다시 대화 시작
            string queuedDialogueID = dialogueQueue.Dequeue();
            StartDialogue(queuedDialogueID);
            return;
        }

        MemoManager.Instance.SetMemoButtons(true);
        
        if (!GetIsImageOrLockPanelActive())
            UIManager.Instance.SetUI(eUIGameObjectName.BlurImage, false, true);
        
        UIManager.Instance.SetCursorAuto();
        
        if (FollowManager.Instance) 
            FollowManager.Instance.EndScript();
        
        if (!RoomManager.Instance)
            return;

        var refillHeartsOrEndDay = (bool)GameManager.Instance.GetVariable("RefillHeartsOrEndDay");
        var isChoosingBrokenBearChoice = false;
        var isInvestigating = RoomManager.Instance.GetIsInvestigating();

        if (GameSceneManager.Instance.GetActiveScene() == SceneType.ROOM_2)
            isChoosingBrokenBearChoice = RoomManager.Instance.room2ActionPointManager.GetChoosingBrokenBearChoice();

        if (refillHeartsOrEndDay && !isChoosingBrokenBearChoice && !isInvestigating)
            RoomManager.Instance.actionPointManager.RefillHeartsOrEndDay();
        
        RoomManager.Instance.SetButtons();
        UIManager.Instance.GetUI(eUIGameObjectName.ObjectImageParentRoom).GetComponent<CanvasGroup>().blocksRaycasts = true;
        if (RoomManager.Instance &&
            RoomManager.Instance.imageAndLockPanelManager &&
            RoomManager.Instance.imageAndLockPanelManager.currentLockObjectCanvasGroup)
            RoomManager.Instance.imageAndLockPanelManager.currentLockObjectCanvasGroup.blocksRaycasts = true;

        GameManager.Instance.SetVariable("currentDialogueID", "NONE");
        GameManager.Instance.SetVariable("isDialogueActive", isDialogueActive);
        SaveManager.Instance.SaveGameData();
    }

    public void SkipButtonClick() {
        StopAllCoroutines();

        dialogues[currentDialogueID].SetCurrentLineIndex(dialogues[currentDialogueID].Lines.Count - 2);
        StartCoroutine(SkipDialogue());
    }

    private IEnumerator SkipDialogue() {
        ProceedToNext();

        while (!isTyping) 
            yield return null;

        CompleteSentence();
        if (isFast) {
            typeSpeed *= 1.75f; // 타이핑 속도 되돌려 놓기
            isFast = false;
        }
        if (isAuto) 
            isAuto = false;

        OnDialoguePanelClick();
    }

    // ---------------------------------------------- Script methods ----------------------------------------------
    private void ProceedToNext() {
        int currentDialogueLineIndex = dialogues[currentDialogueID].CurrentLineIndex;
        string next = dialogues[currentDialogueID].Lines[currentDialogueLineIndex].Next;

        if (EventManager.Instance.events.ContainsKey(next)) { // Event인 경우
            EndDialogue();
            EventManager.Instance.CallEvent(next);
        } else if (dialogues.ContainsKey(next)) {  // Dialogue인 경우
            EndDialogue();
            StartDialogue(next);
        } else if (string.IsNullOrWhiteSpace(next)) { // 빈칸인 경우 다음 줄(대사)로 이동
            currentDialogueLineIndex++;

            if (currentDialogueLineIndex >= dialogues[currentDialogueID].Lines.Count) {
                EndDialogue();  // 더 이상 DialogueLine이 존재하지 않으면 대화 종료
                return;
            }
            
            if (currentDialogueLineIndex == dialogues[currentDialogueID].Lines.Count - 1)
                foreach (GameObject skip in skipText) skip.SetActive(false); //  다이얼로그의 마지막 대사는 스킵 불가능
            dialogues[currentDialogueID].SetCurrentLineIndex(currentDialogueLineIndex);
            DialogueLine nextDialogueLine = dialogues[currentDialogueID].Lines[currentDialogueLineIndex];
            DisplayDialogueLine(nextDialogueLine);
        } else if (choices.ContainsKey(next)) // Choice인 경우
            DisplayChoices(next);
    }
    
    IEnumerator TypeSentence(string sentence) {
        if (teddyBearIcons.Length > dialogueType.ToInt())
            teddyBearIcons[dialogueType.ToInt()].SetActive(false);
        scriptText[dialogueType.ToInt()].text = "";
        fullSentence = sentence;

        // <color=red> 같은 글씨 효과들은 출력되지 않도록 변수 설정
        var isEffect = false;
        var effectText = "";

        // FAST 인 경우 두배의 속도로 타이핑
        if (isFast) 
            typeSpeed /= 1.75f;

        if (isEnding)
        {
            if (EndingManager.Instance) EndingManager.Instance.particle.SetActive(true);
            StartCoroutine(UIManager.Instance.OnFade(endingDialougeBlackImage, 1, 0, 1, true, 1));
            yield return new WaitForSeconds(3.5f);
            typeSpeed *= 1.5f;
        }

        foreach (char letter in sentence) {
            if (letter == '<') {
                effectText = ""; // effectText 초기화
                isEffect = true;
            }
            else if (letter == '>') { // > 가 나오면 scriptText에 한번에 붙인다
                effectText += letter;
                scriptText[dialogueType.ToInt()].text += effectText;
                if (isMulti) accidyMultiScript.GetComponent<TextMeshProUGUI>().text += effectText;
                isEffect = false;
                continue;
            }

            if (isEffect) {// < 가 나온 이후부터는 effectText에 붙인다
                effectText += letter;
                continue;
            }

            scriptText[dialogueType.ToInt()].text += letter;
            if (isMulti) accidyMultiScript.Typing(letter);
            SoundPlayer.Instance.UISoundPlay(Sound_Typing); // 타자 소리 한번씩만
            yield return new WaitForSeconds(typeSpeed);
        }
        isTyping = false;
        if (teddyBearIcons.Length > dialogueType.ToInt()) 
            teddyBearIcons[dialogueType.ToInt()].SetActive(true);

        if (isFast) {
            typeSpeed *= 1.75f; // 타이핑 속도 되돌려 놓기
            isFast = false;
        }
        if (isEnding)
        {
            SoundPlayer.Instance.UISoundPlay(Sound_EndingImpact);
            typeSpeed /= 1.5f;
            yield return new WaitForSeconds(1f);
            StartCoroutine(UIManager.Instance.OnFade(endingDialougeBlackImage, 1, 0, 1, false));
            StartCoroutine(UIManager.Instance.OnFadeText(scriptText[dialogueType.ToInt()], 1, 0, 1, true));
            yield return new WaitForSeconds(1f);
            endingDialougeBlackImage.gameObject.SetActive(false);
            endingDialogueNextPanel.SetActive(true);
        }
        if (isAuto) {
            isAuto = false;
            yield return new WaitForSeconds(0.25f);
            OnDialoguePanelClick(); // 자동으로 넘어감

            foreach (GameObject skip in skipText) 
                skip.SetActive(true);
        }
    }

    public void OnDialoguePanelClick() {
        if (isAuto) 
            return;
        if (isEnding)
        {
            endingDialogueNextPanel.SetActive(false);
            scriptText[dialogueType.ToInt()].gameObject.SetActive(true);
            scriptText[dialogueType.ToInt()].color = new Color(1, 1, 1, 1);
            if (EndingManager.Instance) EndingManager.Instance.particle.SetActive(false);
        }
        
        if (isMulti) {
            isMulti = false;
            accidyMultiScript.GetComponent<TextMeshProUGUI>().text = "";
            accidyMultiScript.gameObject.SetActive(false);
        }

        if (isTyping)
            CompleteSentence();
        else
            ProceedToNext();
    }

    private void CompleteSentence()
    {
        StopAllCoroutines();
        scriptText[dialogueType.ToInt()].text = fullSentence;
        isTyping = false;
        if (teddyBearIcons.Length > dialogueType.ToInt()) teddyBearIcons[dialogueType.ToInt()].SetActive(true);
    }

    // ---------------------------------------------- Choice methods ----------------------------------------------
    private void DisplayChoices(string choiceID) {
        if (choicesContainer.Length <= dialogueType.ToInt()) 
            return;

        foreach (Transform child in choicesContainer[dialogueType.ToInt()])
            Destroy(child.gameObject);

        List<ChoiceLine> choiceLines = choices[choiceID].Lines;

        foreach (ChoiceLine choiceLine in choiceLines) {
            var choiceButton = Instantiate(choicePrefab, choicesContainer[dialogueType.ToInt()]).GetComponent<Button>();
            var choiceText = choiceButton.GetComponentInChildren<TextMeshProUGUI>();

            // Use LocalizedText if present; otherwise set text directly
            var localized = choiceText ? choiceText.GetComponent<LocalizedText>() : null;
            if (localized != null)
                localized.SetScriptId(choiceLine.ScriptID);
            else
                choiceText.text = scripts[choiceLine.ScriptID].GetScript().ProcessedText;

            choiceButton.onClick.AddListener(() => OnChoiceSelected(choiceLine.Next));

            // 만약 잠김 선택지(엔딩)라면 잠김으로 뜨도록 표시
            if (scripts[choiceLine.ScriptID].Placeholder.Length <= 0) 
                continue;
            
            foreach (var effect in scripts[choiceLine.ScriptID].Placeholder.Split('/'))
                switch (effect) {
                    case "END_RUN":
                    case "END_STAY":
                        choiceButton.onClick.AddListener(() => EndingManager.Instance.ChoiceEnding());
                        choiceButton.onClick.AddListener(() => EventManager.Instance.CallEvent($"EventChangeBGMOf{effect}"));
                        break;
                    case "LOCK":
                        if (localized != null) {
                            // If locked, still show localized but dimmed
                            var locked = scripts[choiceLine.ScriptID].GetScript().ProcessedText;
                            choiceText.text = $"<color=#101010>{locked}</color>";
                        } else {
                            choiceText.text = $"<color=#101010>{scripts[choiceLine.ScriptID].GetScript()}</color>";
                        }
                        choiceButton.onClick.RemoveAllListeners();
                        break;
                }
        }
    }

    private void OnChoiceSelected(string next)
    {
        EndDialogue();
        if (dialogues.ContainsKey(next))
            StartDialogue(next);
        else if (EventManager.Instance.events.ContainsKey(next))
            EventManager.Instance.CallEvent(next);

        foreach (Transform child in choicesContainer[dialogueType.ToInt()])
            Destroy(child.gameObject);
    }
    
    private bool GetIsImageOrLockPanelActive()
    {
        SceneType currentScene = GameSceneManager.Instance.GetActiveScene();
        if ((currentScene is SceneType.ROOM_1 or SceneType.ROOM_2) &&
            RoomManager.Instance.imageAndLockPanelManager.GetIsImageOrLockPanelActive())
            return true;
        
        return false;
    }

    public bool IsSkipActive(){
        if (skipText == null || skipText.Length == 0)
            return false;

        foreach (GameObject skip in skipText) {
            if (skip != null && skip.activeInHierarchy)
                return true;
        }

        return false;
    }

    public void UpdateSkipHoldProgress(float progress)
    {
        if (skipText == null || skipText.Length == 0)
            return;

        foreach (GameObject skip in skipText)
        {
            if (skip == null || !skip.activeInHierarchy)
                continue;

            var tmp = skip.GetComponentInChildren<TextMeshProUGUI>(true);
            if (tmp == null)
                continue;

            var effect = tmp.GetComponent<SkipHoldProgressEffect>();
            if (effect == null)
                effect = tmp.gameObject.AddComponent<SkipHoldProgressEffect>();

            effect.UpdateProgress(progress);
        }
    }

    public void ResetSkipHoldProgress()
    {
        UpdateSkipHoldProgress(0f);
    }
}
