using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class FollowAnim : MonoBehaviour
{
    [SerializeField] private Transform backgroundPosition;
    [SerializeField] private Transform frontCanvasPosition;
    [SerializeField] private float moveSpeed;

    [SerializeField] private Image beaconImage;
    [SerializeField] private Sprite[] beaconSprites;

    [SerializeField] private Animator fate, accidyBoy, accidyGirl;
    private Animator accidy;

    [SerializeField] private TextMeshProUGUI stopButtonText; // 이동&멈춤 버튼의 글씨
    private bool isStop = true; // 현재 이동 중인지를 나타내는 변수
    public bool IsStop { get => isStop; }
    private float moveTime = 0;

    private void Start()
    {
        SetCharcter();
        StartCoroutine(ChangeBeaconSprite());
    }
    private void Update()
    {
        if (!isStop) // 배경 이동
        {
            if (FollowManager.Instance.isTutorial)
            {
                moveTime += Time.deltaTime;
                if (moveTime > 2) return;
            }

            Vector3 moveVector = Vector3.left * moveSpeed * Time.deltaTime;
            backgroundPosition.position += moveVector;
            frontCanvasPosition.position += moveVector;

            if(backgroundPosition.position.x < -39)
            {
                ChangeAnimStatus();
                FollowManager.Instance.FollowEndLogicStart();

                // 다음 배경 이동 시에는 반대 방향으로 1.5배 속도
                moveSpeed *= -1.5f;
            }
        }
    }
    public void ChangeAnimStatus()
    {
        // 이동을 멈춤 or 시작
        isStop = !isStop;

        // 이동 중에는 발자국 소리
        SoundPlayer.Instance.UISoundPlay_LOOP(Constants.Sound_FootStep_Accidy, !isStop, 1);
        SoundPlayer.Instance.UISoundPlay_LOOP(Constants.Sound_FootStep_Fate, !isStop, 1);

        if (isStop) stopButtonText.text = "이동";
        else stopButtonText.text = "정지";

        fate.SetBool("Walking", !isStop);
        accidy.SetBool("Walking", !isStop);
    }
    public void SetCharcter()
    {
        // 우연의 성별에 따라 다른 애니메이터 작동
        if ((int)GameManager.Instance.GetVariable("AccidyGender") == 0) accidy = accidyGirl;
        else accidy = accidyBoy;

        fate.gameObject.SetActive(true);
        accidy.gameObject.SetActive(true);
    }

    private IEnumerator ChangeBeaconSprite()
    {
        // 신호등의 색을 3초마다 바꿔준다
        while (gameObject.activeSelf)
        {
            foreach (Sprite sprite in beaconSprites)
            {
                beaconImage.sprite = sprite;
                yield return new WaitForSeconds(3f);
            }
        }
    }

    public void ChangeAnimStatusOnEnd(int num)
    {
        if (num == 0)
        {
            accidy.SetBool("Back", true);
        }
        else if (num == 1)
        {
            fate.speed = 0.6f;
            fate.SetBool("Walking", true);
        }
        else if (num == 2)
        {
            fate.SetBool("Walking", false);
            fate.SetTrigger("Turn");

            // 이동을 시작
            isStop = false;
        }
    }
    public IEnumerator MoveFate()
    {
        accidy.gameObject.SetActive(false); // 꺼두지 않으면 카메라 원상 복귀 때 화면에 잡힘

        Vector3 originPosition = fate.transform.position;
        Vector3 targetPosition = originPosition + new Vector3(-0.3f, 0, 0);

        float elapsedTime = 0f;

        while (elapsedTime < 1.2f)
        {
            fate.transform.position = Vector3.Lerp(originPosition, targetPosition, elapsedTime / 1.2f);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        fate.transform.position = targetPosition;
    }

}
