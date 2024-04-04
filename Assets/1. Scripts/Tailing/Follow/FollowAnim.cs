using UnityEngine;
using TMPro;

public class FollowAnim : MonoBehaviour
{
    [SerializeField] private Transform backgroundPosition;
    [SerializeField] private float moveSpeed;

    [SerializeField] private Animator fateBoy, fateGirl, accidyBoy, accidyGirl;
    private Animator fate, accidy;

    [SerializeField] private TextMeshProUGUI stopButtonText; // �̵�&���� ��ư�� �۾�
    private bool isStop = true; // ���� �̵� �������� ��Ÿ���� ����
    public bool IsStop { get => isStop; }
    
    private void Start()
    {
        SetCharcter();
    }
    private void Update()
    {
        if (!isStop) // ��� �̵�
            backgroundPosition.position += Vector3.left * moveSpeed * Time.deltaTime;
    }
    public void ChangeAnimStatus()
    {
        // �̵��� ���� or ����
        isStop = !isStop;

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
}
