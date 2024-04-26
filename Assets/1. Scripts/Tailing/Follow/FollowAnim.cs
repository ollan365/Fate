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

    [SerializeField] private GameObject[] tutorialFate, tutorialAccidy;
    [SerializeField] private Animator fateBoy, fateGirl, accidyBoy, accidyGirl;
    private Animator fate, accidy;

    [SerializeField] private TextMeshProUGUI stopButtonText; // �̵�&���� ��ư�� �۾�
    private bool isStop = true; // ���� �̵� �������� ��Ÿ���� ����
    public bool IsStop { get => isStop; }

    private void Start()
    {
        SetCharcter();
        StartCoroutine(ChangeBeaconSprite());
    }
    private void Update()
    {
        if (!isStop) // ��� �̵�
        {
            Vector3 moveVector = Vector3.left * moveSpeed * Time.deltaTime;
            backgroundPosition.position += moveVector;
            frontCanvasPosition.position += moveVector;
        }
    }
    public void ChangeAnimStatus()
    {
        // �̵��� ���� or ����
        isStop = !isStop;

        // �̵� �߿��� ���ڱ� �Ҹ�
        SoundPlayer.Instance.UISoundPlay_LOOP(Constants.Sound_FootStep, !isStop);

        if (isStop) stopButtonText.text = "�̵�";
        else stopButtonText.text = "����";

        fate.SetBool("Walking", !isStop);
        accidy.SetBool("Walking", !isStop);
    }
    public void SetCharcter()
    {
        // �ʿ��� �쿬�� ������ ���� �ٸ� �ִϸ����� �۵�
        if ((int)GameManager.Instance.GetVariable("FateGender") == 0)
        {
            fate = fateGirl;
            tutorialFate[0].SetActive(true);
        }
        else
        {
            fate = fateBoy;
            tutorialFate[1].SetActive(true);
        }

        if ((int)GameManager.Instance.GetVariable("AccidyGender") == 0)
        {
            accidy = accidyGirl;
            tutorialAccidy[0].SetActive(true);
        }
        else
        {
            accidy = accidyBoy;
            tutorialAccidy[1].SetActive(true);
        }

        fate.gameObject.SetActive(true);
        accidy.gameObject.SetActive(true);
    }

    private IEnumerator ChangeBeaconSprite()
    {
        // ��ȣ���� ���� 3�ʸ��� �ٲ��ش�
        while (true)
        {
            foreach (Sprite sprite in beaconSprites)
            {
                beaconImage.sprite = sprite;
                yield return new WaitForSeconds(3f);
            }
        }
    }
}
