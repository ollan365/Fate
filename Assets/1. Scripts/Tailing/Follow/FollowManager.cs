using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using static Constants;
public class FollowManager : MonoBehaviour
{
    // FollowManager를 싱글턴으로 생성
    public static FollowManager Instance { get; private set; }

    [Header("UI")]
    [SerializeField] private GameObject UICanvas;
    public Slider memoGaugeSlider;
    [SerializeField] private Slider[] doubtGaugeSliders;
    [SerializeField] private Image[] overHeadDoubtGaugeSliderImages;
    [SerializeField] private GameObject accidyDialogueBox;
    [SerializeField] private GameObject frontCanvas;
    public GameObject blockingPanel;
    public GameObject eventButtonPrefab; // 특별한 오브젝트를 클릭했을 때 버튼 생성

    [Header("Character")]
    [SerializeField] private GameObject fate;
    [SerializeField] private GameObject accidyGirl, accidyBoy;
    private GameObject accidy;
    [SerializeField] private float accidyAnimatorSpeed;
    [SerializeField] private float fateAnimatorSpeed;

    [Header("Another Follow Manager")]
    [SerializeField] private FollowTutorial followTutorial;
    [SerializeField] private FollowEnd followEnd;
    public FollowAnim followAnim;

    [Header("Extra")]
    [SerializeField] private GameObject extraNextButton;
    [SerializeField] private GameObject extraBlockingPanel;
    public GameObject[] extraCanvas;
    public TextMeshProUGUI[] extraDialogueText;
    private FollowExtra extra = FollowExtra.None;

    [Header("Variables")]
    public float totalFollowSpecialObjectCount = 9;
    private AccidyStatus accidyStatus = AccidyStatus.GREEN;
    public AccidyStatus NowAccidyStatus { get => accidyStatus; }
    public bool CanClick { get { return !IsFateHide && accidyStatus != AccidyStatus.RED; } }
    public bool IsTutorial { set; get; } // 튜토리얼 중인지 아닌지
    public bool IsEnd { set; get; } // 현재 미행이 끝났는지 아닌지
    public bool IsDialogueOpen { set; get; } // 현재 대화창이 열려있는지
    public bool IsFateHide { set; get; } // 필연이 뒤를 보고 있는가

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        IsTutorial = false;
        IsEnd = false;
        IsDialogueOpen = false;
        IsFateHide = false;

        extraNextButton.GetComponent<Button>().onClick.AddListener(() => DialogueManager.Instance.OnDialoguePanelClick());

        followAnim.SetCharcter(0);
        if (GameManager.Instance.skipTutorial) { followAnim.ChangeAnimStatusToStop(false); return; }
        if (SceneManager.Instance.CurrentScene == SceneType.FOLLOW_1) { StartCoroutine(followTutorial.StartTutorial()); }
    }
    private void Start()
    {
        // 우연의 성별에 따라 다른 이미지
        if ((int)GameManager.Instance.GetVariable("AccidyGender") == 0)accidy = accidyGirl;
        else  accidy = accidyBoy;
        
        StartCoroutine(StartGame());
    }
    void Update()
    {
        // 스페이스바를 눌렀을 때
        if (!IsEnd && Input.GetKeyDown(KeyCode.Space)) FateHide(true);

        // 스페이스바를 뗐을 때
        if (!IsEnd && Input.GetKeyUp(KeyCode.Space)) FateHide(false);
    }
    public bool ClickObject()
    {
        if (IsTutorial || IsEnd || IsDialogueOpen || IsFateHide || accidyStatus == AccidyStatus.RED) return false;

        // 엑스트라 캐릭터의 대사가 출력되는 중이면 끈다
        foreach(GameObject extra in extraCanvas) if (extra.activeSelf) extra.SetActive(false);

        IsDialogueOpen = true; // 다른 오브젝트를 누를 수 없게 만든다
        frontCanvas.SetActive(false); // 플레이어를 가리는 물체들이 있는 canvas를 꺼버린다
        blockingPanel.SetActive(true); // 화면을 어둡게 만든다

        accidyAnimatorSpeed = accidy.GetComponent<Animator>().speed;
        fateAnimatorSpeed = accidy.GetComponent<Animator>().speed;

        accidy.GetComponent<Animator>().speed = 0;
        fate.GetComponent<Animator>().speed = 0;

        followAnim.ChangeAnimStatusToStop(true);

        return true;
    }

    public void EndScript()
    {
        if (IsTutorial) // 튜토리얼 중에는 다르게 작동
        {
            frontCanvas.SetActive(true);
            followTutorial.NextStep();
            return;
        }
        else if (IsEnd)
        {
            blockingPanel.SetActive(false);
            return;
        }

        IsDialogueOpen = false; // 다른 오브젝트를 누를 수 있게 만든다
        frontCanvas.SetActive(true); // 플레이어를 가리는 물체들이 있는 canvas를 켠다
        blockingPanel.SetActive(false); // 화면을 가리는 판넬을 끈다

        EndExtraDialogue(true);

        accidy.GetComponent<Animator>().speed = accidyAnimatorSpeed;
        fate.GetComponent<Animator>().speed = fateAnimatorSpeed;

        followAnim.ChangeAnimStatusToStop(false);
    }
    public void OpenExtraDialogue(FollowExtra extra)
    {
        this.extra = extra;

        extraNextButton.SetActive(true);
        extraBlockingPanel.SetActive(true); // 일반적인 블로킹 판넬이 아닌 다른 걸 켠다
        blockingPanel.SetActive(false);

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

        extraNextButton.SetActive(false);
        extraBlockingPanel.SetActive(false);
        extraCanvas[Int(extra)].SetActive(false);

        extra = FollowExtra.None;

        if(!dialogueEnd) blockingPanel.SetActive(true); // 아직 다른 대사가 출력되는 중
    }
    public void ClickCat()
    {
        SoundPlayer.Instance.UISoundPlay(Sound_Cat);
    }

    public void FollowEndLogicStart()
    {
        IsEnd = true;

        followAnim.ChangeAnimStatusToStop(true);
        followAnim.SetCharcter(1);
        fate.GetComponent<Animator>().SetBool("Ending", true);

        UICanvas.SetActive(false);
        MemoManager.Instance.HideMemoButton = true;
        MemoManager.Instance.SetMemoButtons(false);

        followAnim.moveSpeed *= -1.5f;

        StartCoroutine(followEnd.EndFollow());
    }

    // ========== 미행 게임 ========== //
    private IEnumerator StartGame()
    {
        // 우연의 움직임, 우연의 말풍선 애니메이션 시작
        StartCoroutine(AccidyLogic());
        StartCoroutine(AccidyDialogueBoxLogic());

        // 미행이 끝날 때까지 반복
        while (!IsEnd)
        {
            // 필연이 움직였고 우연이 뒤를 돌아본 상태가 중첩되면 의심 게이지 증가
            if (!IsFateHide && accidyStatus == AccidyStatus.RED)
            {
                ChangeGaugeAlpha(Time.deltaTime * 3);
                doubtGaugeSliders[0].value += 0.001f;
                doubtGaugeSliders[1].value = doubtGaugeSliders[0].value;
                if (doubtGaugeSliders[0].value == 1) FollowEndLogicStart();
            }
            else ChangeGaugeAlpha(-Time.deltaTime);

            yield return null;
        }
    }

    private void ChangeGaugeAlpha(float value)
    {
        Color color = overHeadDoubtGaugeSliderImages[0].color;
        color.a = Mathf.Clamp(color.a + value, 0, 1);
        foreach (Image image in overHeadDoubtGaugeSliderImages) image.color = color;
    }

    private void FateHide(bool hide)
    {
        IsFateHide = hide;
        fate.GetComponent<Animator>().SetBool("Hide", hide);

        followAnim.ChangeAnimStatusToStop(hide);

        CursorManager.Instance.ChangeCursorInFollow();
    }

    private IEnumerator AccidyLogic()
    {
        Animator accidyAnimator = accidy.GetComponent<Animator>();

        // 3초에서 6초 사이 랜덤한 시간 동안 우연이 움직임
        AccidyAction nextAccidyAction = AccidyAction.Stop;
        float waitingTimeForNextAction = WaitingTimeForNextAccidyAction(nextAccidyAction), currentTime = 0;
        while (!IsEnd)
        {
            while (IsDialogueOpen) { yield return null; continue; } // 대화창이 열려있음
            if (currentTime < waitingTimeForNextAction) { currentTime += Time.deltaTime; yield return null; continue; } // 아직 다음 행동을 할 만큼 시간이 흐르지 않음

            switch (nextAccidyAction)
            {
                case AccidyAction.Move:
                    followAnim.ChangeAnimStatusToStop(false);
                    nextAccidyAction = AccidyAction.Stop;
                    break;

                case AccidyAction.Stop: // 우연의 움직임을 멈춤
                    followAnim.ChangeAnimStatusToStop(true);
                    accidyStatus = AccidyStatus.YELLOW;
                    nextAccidyAction = AccidyAction.Turn;
                    break;

                case AccidyAction.Turn:
                    accidyStatus = AccidyStatus.RED;
                    CursorManager.Instance.ChangeCursorInFollow();
                    accidyAnimator.SetTrigger("Turn");
                    nextAccidyAction = AccidyAction.Inverse_Stop;
                    break;

                case AccidyAction.Inverse_Stop:
                    nextAccidyAction = AccidyAction.Inverse_Turn;
                    break;

                case AccidyAction.Inverse_Turn:
                    accidyStatus = AccidyStatus.GREEN;
                    CursorManager.Instance.ChangeCursorInFollow();
                    accidyAnimator.SetTrigger("Turn");
                    nextAccidyAction = AccidyAction.Move;
                    break;
            }

            currentTime = 0;
            waitingTimeForNextAction = WaitingTimeForNextAccidyAction(nextAccidyAction);
            yield return null;
        }
    }

    private IEnumerator AccidyDialogueBoxLogic()
    {
        TMP_Text text = accidyDialogueBox.GetComponentInChildren<TextMeshProUGUI>();
        float currentTime = 0;
        while (!IsEnd)
        {
            while (IsDialogueOpen) { yield return null; } // 대화창이 열려있음

            if (accidyStatus == AccidyStatus.RED && IsFateHide) { text.text = "  ?  "; currentTime = 0; }
            else if (accidyStatus == AccidyStatus.RED && !IsFateHide) { text.text = "  !  "; currentTime = 0; }
            else if (accidyStatus == AccidyStatus.YELLOW) text.text = "...?";
            else
            {
                currentTime += Time.deltaTime;
                if (currentTime > 1)
                {
                    if (text.text.Length < 3) text.text += ".";
                    else text.text = "";

                    currentTime = 0;
                }
            }
            yield return null;
        }
        accidyDialogueBox.SetActive(false);
    }



    // ================================================================================== //
    public enum AccidyStatus { RED, YELLOW, GREEN } // 빨강: 우연이 보고 있음, 노랑: 우연이 보기 직전, 초록: 우연이 안 봄
    private enum AccidyAction { Move, Stop, Turn, Inverse_Stop, Inverse_Turn }
    private float WaitingTimeForNextAccidyAction(AccidyAction nextAction)
    {
        switch (nextAction)
        {
            case AccidyAction.Move: return 0.5f;
            case AccidyAction.Stop: return Random.Range(2.5f, 5.5f);
            case AccidyAction.Turn: return 0.5f;
            case AccidyAction.Inverse_Stop: return 0.5f;
            case AccidyAction.Inverse_Turn: return 2;

            default: return 0;
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
