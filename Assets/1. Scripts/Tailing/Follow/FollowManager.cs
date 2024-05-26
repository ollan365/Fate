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

    [SerializeField] private GameObject[] extraCanvas;
    [SerializeField] private TextMeshProUGUI[] extraDialogueText;

    public GameObject blockingPanel;

    // 특별한 오브젝트를 클릭했을 때 버튼 생성
    public GameObject eventButtonPrefab;

    private FollowExtra extra;

    // 상태 변수
    public bool isTutorial = false; // 튜토리얼 중인지 아닌지
    private bool isEnd = false; // 현재 미행이 끝났는지 아닌지
    public bool canClick = true; // 현재 오브젝트를 누를 수 있는지
    private bool onMove; // 원래 이동 상태였는지

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (SceneManager.Instance.CurrentScene == SceneType.FOLLOW_1) StartCoroutine(followTutorial.StartTutorial());
    }

    public void ClickObject()
    {
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

        if (extraCanvas[Int(extra)].activeSelf) { EndExtraDialogue(); return; }

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
    }
    public void ClickExtra(FollowExtra extra)
    {
        this.extra = extra;

        DialogueManager.Instance.dialogueCanvas[DialogueType.FOLLOW_EXTRA.ToInt()] = extraCanvas[Int(extra)];
        DialogueManager.Instance.scriptText[DialogueType.FOLLOW_EXTRA.ToInt()] = extraDialogueText[Int(extra)];
        DialogueManager.Instance.dialogueType = DialogueType.FOLLOW_EXTRA;

        extraCanvas[Int(extra)].GetComponentInChildren<Button>().onClick.AddListener(()
            => DialogueManager.Instance.OnDialoguePanelClick());

        blockingPanel.SetActive(false);
        extraCanvas[Int(extra)].SetActive(true);
    }
    public void EndExtraDialogue()
    {
        DialogueManager.Instance.dialogueType = DialogueType.FOLLOW;
        extraCanvas[Int(extra)].SetActive(false);
        blockingPanel.SetActive(true);
    }
    public void ClickCat()
    {
        SoundPlayer.Instance.UISoundPlay(Sound_Cat);
    }
    public void FollowEndLogicStart()
    {
        isEnd = true;
        canClick = false;
        moveAndStopButton.SetActive(false);
        MemoManager.Instance.HideMemoButton(true);

        if (SceneManager.Instance.CurrentScene == SceneType.FOLLOW_1) StartCoroutine(followEnd.EndFollow());
    }
    public void FollowFinishGameStart()
    {
        StartCoroutine(followFinishMiniGame.FinishGameStart(miniGame.heartCount));
    }
    public void FollowEnd()
    {
        // 저장도 해야함
        if (SceneManager.Instance.CurrentScene == SceneType.FOLLOW_1) SceneManager.Instance.LoadScene(SceneType.ROOM_2);
    }

    public int Int(FollowExtra extraType)
    {
        switch (extraType)
        {
            case FollowExtra.Angry: return 0;
            case FollowExtra.Employee: return 0;
            case FollowExtra.RunAway: return 1;
            case FollowExtra.Police: return 2;
            case FollowExtra.Someone: return 3;
            case FollowExtra.Smoker: return 4;
            case FollowExtra.Clubber: return 5;
            default: return -1;
        }
    }
}
