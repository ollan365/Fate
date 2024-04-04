using UnityEngine;

public class FollowManager : MonoBehaviour
{
    // FollowManager�� �̱������� ����
    public static FollowManager Instance { get; private set; }
    [SerializeField] private FollowAnim followAnim;
    [SerializeField] private GameObject moveAndStopButton;

    private bool onMove; // ���� �̵� ���¿�����
    private bool canClick = true; // ���� ������Ʈ�� ���� �� �ִ���
    public bool CanClick { get => canClick; }
    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void ClickObject()
    {
        canClick = false; // �ٸ� ������Ʈ�� ���� �� ���� �����
        moveAndStopButton.SetActive(false); // �̵�&���� ��ư�� ���� �� ������ ȭ�鿡�� ���ش�
        onMove = !followAnim.IsStop; // ���� �̵� ���̾������� ����
        if (onMove) followAnim.ChangeAnimStatus(); // �̵� ���̾��ٸ� �����
    }
    public void EndScript()
    {
        canClick = true; // �ٸ� ������Ʈ�� ���� �� �ְ� �����
        moveAndStopButton.SetActive(true); // �̵�&���� ��ư�� �ٽ� ȭ�鿡 �巯����
        if (onMove) followAnim.ChangeAnimStatus(); // ���� �̵� ���̾��ٸ� �ٽ� �̵��ϵ��� �����
    }
}
