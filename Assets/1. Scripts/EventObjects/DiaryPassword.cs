using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class DiaryPassword : MonoBehaviour
{
    public List<string> pw = new List<string>() { "0", "4", "1", "0" };

    private int pw_Count = 0;

    public GameObject Diary_p2;
    public GameObject Diary_p3;

    private List<string> inputPw = new List<string>();

    [SerializeField]
    private TextMeshProUGUI pwTxt;

    public void Add_inputPwBtn()
    {
        // ���� Ŭ���� ���ӿ�����Ʈ ������(pw��ư �̸�)
        string Number = EventSystem.current.currentSelectedGameObject.name;
        inputPw.Add(Number);
        // �ؽ�Ʈ�� pwTxt���� ���� ���°� ������
        pwTxt.text += Number;

        pw_Count++;
        if (pw_Count == 4)
        {
            pw_Count = 0;
            StartCoroutine(Compare_password());
        }
    }

    // ��й�ȣ �Է� �ʱ�ȭ
    private void delete_inputPassword()
    {
        inputPw.Clear();

        pwTxt.text = "";
    }

    IEnumerator Compare_password()
    {
        yield return new WaitForSeconds(0.2f);
        if (Enumerable.SequenceEqual(pw, inputPw))
        {
            // ���̾ ���� ������
            Diary_p2.SetActive(false);
            Diary_p3.SetActive(true);

            // ���⼭ ���� �Ŵ����� DiaryPasswordCorrect�� setter�� ȣ���ؼ� true�� �����.
            GameManager.Instance.SetVariable("DiaryPasswordCorrect", true);
            //Debug.Log("DiaryPasswordCorrect�� " + GameManager.Instance.GetVariable("DiaryPasswordCorrect"));
        }
        else
        {
            delete_inputPassword();
        }

        yield return new WaitForSeconds(0.3f);
        inputPw.Clear();
    }

}
