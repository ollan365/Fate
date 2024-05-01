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

    [SerializeField] private Animator fateBoy, fateGirl, accidyBoy, accidyGirl;
    private Animator fate, accidy;

    [SerializeField] private TextMeshProUGUI stopButtonText; // �̵�&���� ��ư�� �۾�
    private bool isStop = true; // ���� �̵� �������� ��Ÿ���� ����
    public bool IsStop { get => isStop; }
    private float moveTime = 0;

    private void Start()
    {
        SetCharcter();
        StartCoroutine(ChangeBeaconSprite());
    }
    private void Update()
    {
        if (!isStop) // ��� �̵�
        {
            if (FollowManager.Instance.isTutorial)
            {
                moveTime += Time.deltaTime;
                if (moveTime > 2) return;
            }

            Vector3 moveVector = Vector3.left * moveSpeed * Time.deltaTime;
            backgroundPosition.position += moveVector;
            frontCanvasPosition.position += moveVector;

            if(backgroundPosition.position.x < -33)
            {
                ChangeAnimStatus();
                FollowManager.Instance.FollowEndLogicStart();

                // ���� ��� �̵� �ÿ��� �ݴ� �������� 1.5�� �ӵ�
                moveSpeed *= -1.5f;
            }
        }
    }
    public void ChangeAnimStatus()
    {
        // �̵��� ���� or ����
        isStop = !isStop;

        // �̵� �߿��� ���ڱ� �Ҹ�
        SoundPlayer.Instance.UISoundPlay_LOOP(Constants.Sound_FootStep_Accidy, !isStop, 1);
        SoundPlayer.Instance.UISoundPlay_LOOP(Constants.Sound_FootStep_Fate, !isStop, 1);

        if (isStop) stopButtonText.text = "�̵�";
        else stopButtonText.text = "����";

        fate.SetBool("Walking", !isStop);
        accidy.SetBool("Walking", !isStop);
    }
    public void SetCharcter()
    {
        // �ʿ��� �쿬�� ������ ���� �ٸ� �ִϸ����� �۵�
        if ((int)GameManager.Instance.GetVariable("FateGender") == 0) fate = fateGirl;
        else fate = fateBoy;

        if ((int)GameManager.Instance.GetVariable("AccidyGender") == 0) accidy = accidyGirl;
        else accidy = accidyBoy;

        fate.gameObject.SetActive(true);
        accidy.gameObject.SetActive(true);
    }

    private IEnumerator ChangeBeaconSprite()
    {
        // ��ȣ���� ���� 3�ʸ��� �ٲ��ش�
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
            fate.SetBool("Back", true);

            // �̵��� ����
            isStop = false;

            // ������ �̵��ϹǷ� ���� �ӵ��� ������
            SoundPlayer.Instance.UISoundPlay_LOOP(Constants.Sound_FootStep_Fate, !isStop, 2);
        }
    }
    public IEnumerator MoveFate()
    {
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
