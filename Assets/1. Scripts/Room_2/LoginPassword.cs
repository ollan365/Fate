using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoginPassword : MonoBehaviour
{
    public string pw = "0410";

    public GameObject Laptop_p1;

    private string inputPw;

    [SerializeField] // ���⵵ �����Ϸ� �ߴµ� ���� inputField�� OnSubmit�� ���� �Űܾ����� �𸣰ھ �ϴ� ���ܵ׽��ϴ� �Ф�
    private InputField pwInput;
    [SerializeField] private TMP_InputField pwInput_TMpro;

    void Start()
    {
        pwInput = gameObject.transform.GetChild(0).GetComponent<InputField>();
    }

    // �α��� ȭ�鿡�� ��ȣ �Է� �� ���� ġ��
    public void loginPW(InputField input)
    {
        inputPw = input.text;
        
        StartCoroutine(Compare_password());
    }

    // ��й�ȣ �Է� �ʱ�ȭ
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
            // ��Ʈ�� ��� Ǯ��
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
