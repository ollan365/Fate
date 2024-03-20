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

    public GameObject Diary_p3;

    private List<string> inputPw = new List<string>();

    [SerializeField]
    private TextMeshProUGUI pwTxt;

    private RoomMovManager roomMov;

    [SerializeField] private EventObject diary;

    private void Start()
    {
        roomMov = GameObject.Find("Room1 Manager").GetComponent<RoomMovManager>();
        diary = GameObject.Find("Diary").GetComponent<EventObject>();
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
            Diary_p3.SetActive(true);

            roomMov.addScreenObjects(Diary_p3);
            // 여기서 게임 매니저의 DiaryPasswordCorrect를 setter로 호출해서 true로 만들기.
            GameManager.Instance.SetVariable("DiaryPasswordCorrect", true);

            EventManager.Instance.CallEvent(diary.getEventId());
            //Debug.Log("DiaryPasswordCorrect는 " + GameManager.Instance.GetVariable("DiaryPasswordCorrect"));
        }
        else
        {
            EventManager.Instance.CallEvent(diary.getEventId());

            delete_inputPassword();
        }

        yield return new WaitForSeconds(0.3f);
        inputPw.Clear();
    }

}
