using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static Constants;
public class FollowManager : MonoBehaviour
{
    // FollowManager�� �̱������� ����
    public static FollowManager Instance { get; private set; }

    [SerializeField] private FollowTutorial followTutorial;
    [SerializeField] private MiniGame miniGame;

    [SerializeField] private FollowAnim followAnim;
    [SerializeField] private GameObject moveAndStopButton;
    [SerializeField] private GameObject frontCanvas;
    [SerializeField] private GameObject angryCanvas;
    [SerializeField] private Button angryDialoguePanel;
    [SerializeField] private TextMeshProUGUI angryDialogueText;
    public GameObject blockingPanel;

    // Ư���� ������Ʈ�� Ŭ������ �� ��ư ����
    public GameObject eventButtonPrefab;

    // ���� ����
    public bool isTutorial = false;
    [SerializeField] private bool onMove; // ���� �̵� ���¿�����
    public bool canClick = true; // ���� ������Ʈ�� ���� �� �ִ���
    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        SoundPlayer.Instance.ChangeBGM(DialogueType.FOLLOW);

        MemoManager.Instance.isFollow = true;
        MemoManager.Instance.MemoButtonAlphaChange();
        MemoManager.Instance.HideMemoButton(false);
        DialogueManager.Instance.dialogueType = DialogueType.FOLLOW;

        StartCoroutine(followTutorial.StartTutorial());
    }

    public void ClickObject()
    {
        canClick = false; // �ٸ� ������Ʈ�� ���� �� ���� �����
        frontCanvas.SetActive(false); // �÷��̾ ������ ��ü���� �ִ� canvas�� ��������
        blockingPanel.SetActive(true); // ȭ���� ��Ӱ� �����
        moveAndStopButton.SetActive(false); // �̵�&���� ��ư�� ���� �� ������ ȭ�鿡�� ���ش�
        onMove = !followAnim.IsStop; // ���� �̵� ���̾������� ����
        if (onMove) followAnim.ChangeAnimStatus(); // �̵� ���̾��ٸ� �����
    }
    public void EndScript(bool changeCount)
    {
        if (isTutorial) // Ʃ�丮�� �߿��� �ٸ��� �۵�
        {
            moveAndStopButton.SetActive(true);
            followTutorial.NextStep();
            return;
        }

        if (changeCount) miniGame.ClickCount++;
        if (miniGame.ClickCount % 5 == 0) onMove = false; // �̴� ������ ������ ���� �������� �ʵ��� �����

        if (angryCanvas.activeSelf) EndAngryDialogue();

        canClick = true; // �ٸ� ������Ʈ�� ���� �� �ְ� �����
        frontCanvas.SetActive(true); // �÷��̾ ������ ��ü���� �ִ� canvas�� �Ҵ�
        blockingPanel.SetActive(false); // ȭ���� ������ �ǳ��� ����
        moveAndStopButton.SetActive(true); // �̵�&���� ��ư�� �ٽ� ȭ�鿡 �巯����

        if (onMove) followAnim.ChangeAnimStatus(); // ���� �̵� ���̾��ٸ� �ٽ� �̵��ϵ��� �����
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
