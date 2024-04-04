using UnityEngine;

public class FollowManager : MonoBehaviour
{
    // FollowManager를 싱글턴으로 생성
    public static FollowManager Instance { get; private set; }
    [SerializeField] private FollowAnim followAnim;
    [SerializeField] private GameObject moveAndStopButton;

    private bool onMove; // 원래 이동 상태였는지
    private bool canClick = true; // 현재 오브젝트를 누를 수 있는지
    public bool CanClick { get => canClick; }
    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void ClickObject()
    {
        canClick = false; // 다른 오브젝트를 누를 수 없게 만든다
        moveAndStopButton.SetActive(false); // 이동&정지 버튼을 누를 수 없도록 화면에서 없앤다
        onMove = !followAnim.IsStop; // 원래 이동 중이었는지를 저장
        if (onMove) followAnim.ChangeAnimStatus(); // 이동 중이었다면 멈춘다
    }
    public void EndScript()
    {
        canClick = true; // 다른 오브젝트를 누를 수 있게 만든다
        moveAndStopButton.SetActive(true); // 이동&정지 버튼을 다시 화면에 드러낸다
        if (onMove) followAnim.ChangeAnimStatus(); // 원래 이동 중이었다면 다시 이동하도록 만든다
    }
}
