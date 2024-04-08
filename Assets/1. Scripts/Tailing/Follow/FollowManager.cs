using UnityEngine;

public class FollowManager : MonoBehaviour
{
    // FollowManager�� �̱������� ����
    public static FollowManager Instance { get; private set; }

    [SerializeField] private FollowAnim followAnim;
    [SerializeField] private GameObject moveAndStopButton;
    [SerializeField] private GameObject frontCanvas;
    public GameObject blockingPanel;

    // Ư���� ������Ʈ�� Ŭ������ �� ��ư ����
    public GameObject eventButtonPrefab;

    // ���� ����
    private bool onMove; // ���� �̵� ���¿�����
    private bool canClick = true; // ���� ������Ʈ�� ���� �� �ִ���
    public bool CanClick { get => canClick; }
    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        DialogueManager.Instance.dialogueType = Constants.DialogueType.FOLLOW;
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
    public void EndScript()
    {
        canClick = true; // �ٸ� ������Ʈ�� ���� �� �ְ� �����
        frontCanvas.SetActive(true); // �÷��̾ ������ ��ü���� �ִ� canvas�� �Ҵ�
        blockingPanel.SetActive(false); // ȭ���� ������ �ǳ��� ����
        moveAndStopButton.SetActive(true); // �̵�&���� ��ư�� �ٽ� ȭ�鿡 �巯����
        if (onMove) followAnim.ChangeAnimStatus(); // ���� �̵� ���̾��ٸ� �ٽ� �̵��ϵ��� �����
    }
}
