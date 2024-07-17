using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class Diary2Lock : EventObject, IResultExecutable
{
    private string passwordInput = "";

    [SerializeField]
    private GameObject diaryContent;
    [SerializeField]
    private TextMeshProUGUI passwordText;

    [SerializeField]
    private Diary2 diaryA;

    private void Awake()
    {
        ResultManager.Instance.RegisterExecutable("Diary2Lock", this);
    }
    
    public void ExecuteAction()
    {
        ShowDiaryContent();
    }

    // 다이어리 내용 보여짐
    private void ShowDiaryContent()
    {
        diaryContent.SetActive(true);
        gameObject.SetActive(false);
    }

    public void InputNumber(string buttonInput)
    {
        // 현재 클릭된 게임오브젝트 가져옴(pw버튼 이름)
        // string numberInput = EventSystem.current.currentSelectedGameObject.name;

        if (passwordInput.Length < 4)
        {
            // 텍스트에도 숫자 들어가는거 보여짐
            passwordInput += buttonInput;
            passwordText.text = passwordInput;
            if (passwordInput.Length < 4) return;
        }

        StartCoroutine(ComparePassword());
    }

    IEnumerator ComparePassword()
    {
        yield return new WaitForSeconds(0.2f);
        string correctPassword = (string)GameManager.Instance.GetVariable("FateBirthday");  // 다이어리 비밀번호 = 필연 생일
        GameManager.Instance.SetVariable("Diary2PasswordCorrect", passwordInput == correctPassword);
        // 다이어리 비번 맞춘 이후에 다이어리 다시 클릭하면 조사창 패스된거 다시 조사창 나오게 함.
        if ((bool)GameManager.Instance.GetVariable("Diary2PasswordCorrect"))
            diaryA.SetIsInquiry(true);

        OnMouseDown();
        yield return new WaitForSeconds(0.3f);
        ResetPassword();
    }
    
    // 비밀번호 입력 초기화
    private void ResetPassword()
    {
        passwordInput = "";
        passwordText.text = passwordInput;
    }

}
