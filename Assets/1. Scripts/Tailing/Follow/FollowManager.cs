using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static Constants;
public class FollowManager : MonoBehaviour
{
    // FollowManager를 싱글턴으로 생성
    public static FollowManager Instance { get; private set; }

    [SerializeField] private MiniGame miniGame;

    [SerializeField] private FollowAnim followAnim;
    [SerializeField] private GameObject moveAndStopButton;
    [SerializeField] private GameObject frontCanvas;
    [SerializeField] private GameObject angryCanvas;
    [SerializeField] private Button angryDialoguePanel;
    [SerializeField] private TextMeshProUGUI angryDialogueText;
    public GameObject blockingPanel;

    // 특별한 오브젝트를 클릭했을 때 버튼 생성
    public GameObject eventButtonPrefab;

    // 상태 변수
    private bool onMove; // 원래 이동 상태였는지
    private bool canClick = true; // 현재 오브젝트를 누를 수 있는지
    public bool CanClick { get => canClick; }
    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        MemoManager.Instance.HideMemoButton(false);
        DialogueManager.Instance.dialogueType = DialogueType.FOLLOW;
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
    public void EndScript()
    {
        miniGame.ClickCount++;
        if (miniGame.ClickCount % 10 == 0) onMove = false; // 미니 게임이 끝나고 오면 움직이지 않도록 만든다

        if (angryCanvas.activeSelf) EndAngryDialogue();

        canClick = true; // 다른 오브젝트를 누를 수 있게 만든다
        frontCanvas.SetActive(true); // 플레이어를 가리는 물체들이 있는 canvas를 켠다
        blockingPanel.SetActive(false); // 화면을 가리는 판넬을 끈다
        moveAndStopButton.SetActive(true); // 이동&정지 버튼을 다시 화면에 드러낸다

        if (onMove) followAnim.ChangeAnimStatus(); // 원래 이동 중이었다면 다시 이동하도록 만든다
    }
    public void ClickAngry()
    {
        DialogueManager.Instance.dialogueCanvas[DialogueType.FOLLOW_ANGRY.ToInt()] = angryCanvas;
        DialogueManager.Instance.dialoguePanel[DialogueType.FOLLOW_ANGRY.ToInt()] = angryDialoguePanel.gameObject;
        DialogueManager.Instance.scriptText[DialogueType.FOLLOW_ANGRY.ToInt()] = angryDialogueText;
        DialogueManager.Instance.dialogueType = DialogueType.FOLLOW_ANGRY;

        angryDialoguePanel.onClick.AddListener(() => DialogueManager.Instance.OnDialoguePanelClick());
        angryCanvas.SetActive(true);
    }
    public void EndAngryDialogue()
    {
        DialogueManager.Instance.dialogueType = DialogueType.FOLLOW;
        angryCanvas.SetActive(false);
    }
}
