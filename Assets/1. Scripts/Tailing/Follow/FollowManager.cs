using UnityEngine;

public class FollowManager : MonoBehaviour
{
    // FollowManager를 싱글턴으로 생성
    public static FollowManager Instance { get; private set; }

    [SerializeField] private FollowAnim followAnim;
    [SerializeField] private GameObject moveAndStopButton;
    [SerializeField] private GameObject frontCanvas;
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

        DialogueManager.Instance.dialogueType = Constants.DialogueType.FOLLOW;
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
        canClick = true; // 다른 오브젝트를 누를 수 있게 만든다
        frontCanvas.SetActive(true); // 플레이어를 가리는 물체들이 있는 canvas를 켠다
        blockingPanel.SetActive(false); // 화면을 가리는 판넬을 끈다
        moveAndStopButton.SetActive(true); // 이동&정지 버튼을 다시 화면에 드러낸다
        if (onMove) followAnim.ChangeAnimStatus(); // 원래 이동 중이었다면 다시 이동하도록 만든다
    }
}
