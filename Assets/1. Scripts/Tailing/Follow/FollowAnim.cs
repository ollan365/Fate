using UnityEngine;
using TMPro;

public class FollowAnim : MonoBehaviour
{
    [SerializeField] private Transform backgroundPosition;
    [SerializeField] private float moveSpeed;

    [SerializeField] private Animator fateBoy, fateGirl, accidyBoy, accidyGirl;
    private Animator fate, accidy;

    [SerializeField] private TextMeshProUGUI stopButtonText; // 이동&멈춤 버튼의 글씨
    private bool isStop = true; // 현재 이동 중인지를 나타내는 변수
    public bool IsStop { get => isStop; }
    
    private void Start()
    {
        SetCharcter();
    }
    private void Update()
    {
        if (!isStop) // 배경 이동
            backgroundPosition.position += Vector3.left * moveSpeed * Time.deltaTime;
    }
    public void ChangeAnimStatus()
    {
        // 이동을 멈춤 or 시작
        isStop = !isStop;

        if (isStop) stopButtonText.text = "이동";
        else stopButtonText.text = "정지";

        fate.SetBool("Walking", !isStop);
        accidy.SetBool("Walking", !isStop);
    }
    public void SetCharcter()
    {
        // 필연과 우연의 성별에 따라 다른 애니메이터 작동
        if ((int)GameManager.Instance.GetVariable("FateGender") == 0) fate = fateGirl;
        else fate = fateBoy;

        if ((int)GameManager.Instance.GetVariable("AccidyGender") == 0) accidy = accidyGirl;
        else accidy = accidyBoy;

        fate.gameObject.SetActive(true);
        accidy.gameObject.SetActive(true);
    }
}
