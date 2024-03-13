using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LoginPassword : MonoBehaviour
{
    public string pw = "0410";

    public GameObject Laptop_p1;

    private string inputPw;

    [SerializeField]
    private InputField pwInput;

    void Start()
    {
        pwInput = gameObject.transform.GetChild(0).GetComponent<InputField>();
    }

    // 로그인 화면에서 암호 입력 후 엔터 치면
    public void loginPW(InputField input)
    {
        inputPw = input.text;
        
        StartCoroutine(Compare_password());
    }

    // 비밀번호 입력 초기화
    private void delete_inputPassword()
    {
        inputPw = "";

        pwInput.text = "";
    }

    IEnumerator Compare_password()
    {
        yield return new WaitForSeconds(0.2f);
        if (Enumerable.SequenceEqual(pw, inputPw))
        {
            // 노트북 잠금 풀림
            Laptop_p1.SetActive(true);
        }
        else
        {
            delete_inputPassword();
        }

        yield return new WaitForSeconds(0.3f);
        inputPw = "";
    }
}
