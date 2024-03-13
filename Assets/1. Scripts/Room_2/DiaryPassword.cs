using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DiaryPassword : MonoBehaviour
{
    public List<string> pw = new List<string>() { "0", "4", "1", "0" };

    private int pw_Count = 0;

    public GameObject Diary_p2;
    public GameObject Diary_p3;

    private List<string> inputPw = new List<string>();

    [SerializeField]
    private Text pwTxt;

    private void Start()
    {
        pwTxt = gameObject.transform.GetChild(0).GetComponent<Text>();
    }

    public void Add_inputPwBtn()
    {
        // 현재 클릭된 게임오브젝트 가져옴(pw버튼 이름)
        string Number = EventSystem.current.currentSelectedGameObject.name;
        inputPw.Add(Number);
        // 텍스트인 pwTxt에도 숫자 들어가는거 보여짐
        pwTxt.text += Number;

        pw_Count++;
        if (pw_Count == 4)
        {
            pw_Count = 0;
            StartCoroutine(Compare_password());
        }
    }

    // 비밀번호 입력 초기화
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
            // 다이어리 내용 보여짐
            Diary_p2.SetActive(false);
            Diary_p3.SetActive(true);
        }
        else
        {
            delete_inputPassword();
        }

        yield return new WaitForSeconds(0.3f);
        inputPw.Clear();
    }

}
