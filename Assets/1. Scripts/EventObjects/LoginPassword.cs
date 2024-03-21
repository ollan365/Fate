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

    [SerializeField] private TMP_InputField pwInput_TMpro;

    void Start()
    {
        pwInput_TMpro = gameObject.transform.GetChild(0).GetComponent<TMP_InputField>();
        // inputField�� OnEndEdit �̺�Ʈ�� OnInputSubmit �޼��带 ����.
        pwInput_TMpro.onEndEdit.AddListener(loginPW);
    }

    // �α��� ȭ�鿡�� ��ȣ �Է� �� ���� ġ��
    public void loginPW(string input)
    {
        inputPw = input;

        StartCoroutine(Compare_password());
    }

    // ��й�ȣ �Է� �ʱ�ȭ
    private void delete_inputPassword()
    {
        inputPw = "";

        pwInput_TMpro.text = "";
    }

    IEnumerator Compare_password()
    {
        yield return new WaitForSeconds(0.2f);
        if (Enumerable.SequenceEqual(pw, inputPw))
        {
            // ��Ʈ�� ��� Ǯ��
            Laptop_p1.SetActive(true);

            RoomManager.Instance.AddScreenObjects(Laptop_p1);
            GameManager.Instance.SetVariable("LaptopPasswordCorrect", true);
        }
        else
        {
            delete_inputPassword();
        }

        yield return new WaitForSeconds(0.3f);
        inputPw = "";
    }
}
