using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class DiaryLock : EventObject, IResultExecutable
{
    private string passwordInput = "";

    [SerializeField] private GameObject diaryContent;
    [SerializeField] private TextMeshProUGUI passwordText;

    [SerializeField] private Diary diaryA;
    [SerializeField] private string diaryLockExecutableName;
    [SerializeField] private DiaryManager diaryManager;

    private bool isComparing = false;

    private void Start()
    {
        ResultManager.Instance.RegisterExecutable(diaryLockExecutableName, this);
    }

    public void ExecuteAction()
    {
        ShowDiaryContent();
    }

    // 다이어리 내용 보여짐
    private void ShowDiaryContent()
    {
        diaryManager.SetTotalPages();
        diaryContent.SetActive(true);
        gameObject.SetActive(false);

        // 방탈출2 다이어리 관련
        // 방탈출2에서 다이어리2 다시 조사할 때 2페이지를 펼쳐둔 상태이면
        // 열자마자 바로 다이어리 내용 확인 스크립트 출력됨
        if ((bool)GameManager.Instance.GetVariable("Diary2PasswordCorrect")
            && diaryManager.GetDiaryType() == "Diary2"&& diaryContent.activeSelf)
        {
            // 다이어리 내용 끝까지인 2페이지 확인하면 다이어리 내용 확인 스크립트 출력됨
            EventManager.Instance.CallEvent("EventDiary2Content");
        }
    }


    public void InputNumber(string buttonInput)
    {
        // 비밀번호 무한 입력 시도 방지
        RoomManager.Instance.ProhibitInput();

        Debug.Log(passwordInput.Length);

        if (passwordInput.Length < 4)
        {
            // 텍스트에도 숫자 들어가는거 보여짐
            passwordInput += buttonInput;
            passwordText.text = passwordInput;
            if (passwordInput.Length < 4) return;
        }

        if(!isComparing)
            StartCoroutine(ComparePassword());

    }

    IEnumerator ComparePassword()
    {
        isComparing = true;

        yield return new WaitForSeconds(0.2f);
        string correctPassword = (string)GameManager.Instance.GetVariable("FateBirthday");  // 다이어리 비밀번호 = 필연 생일

        //Debug.Log($"correctPassword: {correctPassword}\npasswordInput: {passwordInput}");

        string variableName = diaryLockExecutableName == "DiaryLock" ? "DiaryPasswordCorrect" : "Diary2PasswordCorrect";
        GameManager.Instance.SetVariable(variableName, passwordInput == correctPassword);
        
        // 다이어리 비번 맞춘 이후에 다이어리 다시 클릭하면 조사창 패스된거 다시 조사창 나오게 함.
        if ((bool)GameManager.Instance.GetVariable(variableName)) diaryA.SetIsInquiry(true);

        OnMouseDown();
        yield return new WaitForSeconds(0.3f);
        ResetPassword();

        isComparing = false;
    }
    
    // 비밀번호 입력 초기화
    private void ResetPassword()
    {
        passwordInput = "";
        passwordText.text = passwordInput;
    }

}
