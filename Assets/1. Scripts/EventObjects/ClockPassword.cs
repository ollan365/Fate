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

    public float pw_hour = 210f;  // 5시
    public float pw_min = 180f;   // 30분

    private float input_hour;
    private float input_min;

    [SerializeField]
    private GameObject key_bg;

    private void Start()
    {
        min_hand = GameObject.Find("minute_hand");
        hour_hand = GameObject.Find("hour_hand");
    }

    public void loginPW()
    {
        input_hour = hour_hand.transform.rotation.eulerAngles.z;
        input_min = min_hand.transform.rotation.eulerAngles.z;

        //Debug.Log(input_hour + "시 "+ input_min + "분");

        StartCoroutine(Compare_password());
    }

    // 시침 분침 입력 초기화
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
            // 시계 열리고 열쇠 획득
            key_bg.SetActive(true);
            GameManager.Instance.SetVariable("ClockTimeCorrect", true);
        }
        else
        {
            delete_inputPassword();
        }

        yield return new WaitForSeconds(0.3f);

        input_hour = 0f;
        input_min = 0f;

        hour_hand.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        min_hand.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
    }
}
