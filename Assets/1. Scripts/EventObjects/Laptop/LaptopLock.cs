using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LaptopLock : EventObject, IResultExecutable
{
    private string passwordInput = "";

    [SerializeField]
    private GameObject laptopContent;
    [SerializeField]
    private TMP_InputField passwordInputField;

    [SerializeField]
    private Sprite girlVersionBackground;
    [SerializeField]
    private Sprite boyVersionBackground;

    private Image imageComponent;

    [SerializeField] private Laptop laptopA;
    
    private void Start()
    {
        imageComponent = GetComponent<Image>();

        ResultManager.Instance.RegisterExecutable("LaptopLock", this);

        imageComponent.sprite = (int)GameManager.Instance.GetVariable("AccidyGender") == 0
            ? girlVersionBackground
            : boyVersionBackground;
    }

    public void ExecuteAction()
    {
        ShowLaptopContent();
    }

    // 노트북 잠금 풀림
    private void ShowLaptopContent()
    {
        gameObject.SetActive(false);
        laptopContent.SetActive(true);
        GameManager.Instance.SetVariable("isLaptopOpen", true);
    } 

    // 로그인 화면에서 암호 입력 후 엔터 치면
    public void TryLogin(string input)
    {
        //passwordInput = input;
        //StartCoroutine(ComparePassword());
        bool isDialogueActive = DialogueManager.Instance.isDialogueActive;

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            // 이미 처리 중이라면 추가 실행 방지
            if (isDialogueActive) return;

            passwordInput = input;
            StartCoroutine(ComparePassword());

            //// 이미 처리 중이라면 추가 실행 방지
            //if (isProcessing|| isDialogueActive) return;

            //else if (!isProcessing && !isDialogueActive)
            //{
            //    isProcessing = true; // 플래그 설정

            //    passwordInput = input;
            //    StartCoroutine(ComparePassword());
            //}
        }
    }

    IEnumerator ComparePassword()
    {
        yield return new WaitForSeconds(0.2f);
        string correctPassword = (string)GameManager.Instance.GetVariable("LaptopPassword");
        GameManager.Instance.SetVariable("LaptopPasswordCorrect", passwordInput == correctPassword);

        // 노트북 비번 맞춘 이후에 노트북 다시 클릭하면 조사창 패스된거 다시 조사창 나오게 함.
        if ((bool)GameManager.Instance.GetVariable("LaptopPasswordCorrect"))
            laptopA.SetIsInquiry(true);

        OnMouseDown();
        yield return new WaitForSeconds(0.3f);
        ResetPassword();

        //isProcessing = false;   // 플래그 초기화
    }
    
    // 비밀번호 입력 초기화
    private void ResetPassword()
    {
        passwordInput = "";
        passwordInputField.text = passwordInput;
    }
}
