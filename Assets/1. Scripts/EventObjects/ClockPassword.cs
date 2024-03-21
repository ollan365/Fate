using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


public class ClockPassword : MonoBehaviour
{
    [SerializeField]
    private GameObject min_hand;

    [SerializeField]
    private GameObject hour_hand;

    public float pw_hour = 210f;  // 5��
    public float pw_min = 180f;   // 30��

    private float input_hour;
    private float input_min;

    [SerializeField]
    private GameObject key_bg;


    [SerializeField] private EventObject clock;

    private void Start()
    {
        clock = GameObject.Find("Clock").GetComponent<EventObject>();

        min_hand = GameObject.Find("minute_hand");
        hour_hand = GameObject.Find("hour_hand");
    }

    public void loginPW()
    {
        input_hour = hour_hand.transform.rotation.eulerAngles.z;
        input_min = min_hand.transform.rotation.eulerAngles.z;

        //Debug.Log(input_hour + "�� "+ input_min + "��");

        StartCoroutine(Compare_password());
    }

    // ��ħ ��ħ �Է� �ʱ�ȭ
    private void delete_inputPassword()
    {
        input_hour = 0f;
        input_min = 0f;

        hour_hand.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        min_hand.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
    }

    IEnumerator Compare_password()
    {
        yield return new WaitForSeconds(0.5f);
        if (pw_hour == input_hour && pw_min == input_min)
        {
            // �ð� ������ ���� ȹ��
            key_bg.SetActive(true);

            RoomManager.Instance.AddScreenObjects(key_bg);
            GameManager.Instance.SetVariable("ClockTimeCorrect", true);

            EventManager.Instance.CallEvent(clock.GetEventId());
        }
        else
        {
            EventManager.Instance.CallEvent(clock.GetEventId());
            delete_inputPassword();
        }

        yield return new WaitForSeconds(0.3f);

        input_hour = 0f;
        input_min = 0f;

        hour_hand.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        min_hand.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
    }
}
