using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static Constants;
public class FollowManager : MonoBehaviour
{
    // FollowManager를 싱글턴으로 생성
    public static FollowManager Instance { get; private set; }

    [SerializeField] private FollowTutorial followTutorial;
    [SerializeField] private FollowDayMiniGame miniGame;
    [SerializeField] private FollowEnd followEnd;
    [SerializeField] private FollowFinishMiniGame followFinishMiniGame;
    public FollowAnim followAnim;

    [SerializeField] private GameObject moveAndStopButton;
    [SerializeField] private GameObject frontCanvas;

    [SerializeField] private GameObject extraNextButton;
    [SerializeField] private GameObject extraBlockingPanel;
    public GameObject[] extraCanvas;
    public TextMeshProUGUI[] extraDialogueText;

    public GameObject blockingPanel;

    // 특별한 오브젝트를 클릭했을 때 버튼 생성
    public GameObject eventButtonPrefab;

    private FollowExtra extra = FollowExtra.None;

    // 상태 변수
    public bool isTutorial = false; // 튜토리얼 중인지 아닌지
    public bool isEnd = false; // 현재 미행이 끝났는지 아닌지
    public bool canClick = true; // 현재 오브젝트를 누를 수 있는지
    private bool onMove; // 원래 이동 상태였는지

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (SceneManager.Instance.CurrentScene == SceneType.FOLLOW_1) StartCoroutine(followTutorial.StartTutorial());
        extraNextButton.GetComponent<Button>().onClick.AddListener(()
            => DialogueManager.Instance.OnDialoguePanelClick());
    }

    public void ClickObject()
    {
        if (isTutorial || isEnd) return;

        // 엑스트라 캐릭터의 대사가 출력되는 중이면 끈다
        foreach(GameObject extra in extraCanvas) if (extra.activeSelf) extra.SetActive(false);

        canClick = false; // 다른 오브젝트를 누를 수 없게 만든다
        frontCanvas.SetActive(false); // 플레이어를 가리는 물체들이 있는 canvas를 꺼버린다
        blockingPanel.SetActive(true); // 화면을 어둡게 만든다
        moveAndStopButton.SetActive(false); // 이동&정지 버튼을 누를 수 없도록 화면에서 없앤다
        onMove = !followAnim.IsStop; // 원래 이동 중이었는지를 저장
        if (onMove) followAnim.ChangeAnimStatus(); // 이동 중이었다면 멈춘다
    }

    public void EndScript(bool changeCount)
    {
        if (isTutorial) // 튜토리얼 중에는 다르게 작동
        {
            frontCanvas.SetActive(true);
            followTutorial.NextStep();
            return;
        }
        else if (isEnd)
        {
            blockingPanel.SetActive(false);
            return;
        }

        if (SceneManager.Instance.CurrentScene == SceneType.FOLLOW_1)
        {
            if (changeCount) miniGame.ClickCount++;
            if (miniGame.ClickCount % 5 == 0) onMove = false; // 미니 게임이 끝나고 오면 움직이지 않도록 만든다
        }

        canClick = true; // 다른 오브젝트를 누를 수 있게 만든다
        frontCanvas.SetActive(true); // 플레이어를 가리는 물체들이 있는 canvas를 켠다
        blockingPanel.SetActive(false); // 화면을 가리는 판넬을 끈다
        moveAndStopButton.SetActive(true); // 이동&정지 버튼을 다시 화면에 드러낸다

        if (onMove) followAnim.ChangeAnimStatus(); // 원래 이동 중이었다면 다시 이동하도록 만든다

        EndExtraDialogue(true);
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
    
    public void FollowEndLogicStart(bool isDayMiniGame)
    {
        isEnd = true;
        canClick = false;
        moveAndStopButton.SetActive(false);
        MemoManager.Instance.SetMemoButtons(false);

        if (SceneManager.Instance.CurrentScene == SceneType.FOLLOW_1)
        {
            followAnim.moveSpeed *= -1.5f;

            StartCoroutine(followEnd.EndFollow(isDayMiniGame));
        }
    }
    public void FollowFinishGameStart()
    {
        StartCoroutine(followFinishMiniGame.FinishGameStart(miniGame.heartCount));
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
