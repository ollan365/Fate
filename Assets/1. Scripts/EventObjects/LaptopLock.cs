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

    private void Awake()
    {
        ResultManager.Instance.RegisterExecutable("LaptopLock", this);
    }

    public void ExecuteAction()
    {
        ShowLaptopContent();
    }

    // 노트북 잠금 풀림
    private void ShowLaptopContent()
    {
        laptopContent.SetActive(true);
        RoomManager.Instance.AddScreenObjects(laptopContent);
        RoomManager.Instance.isInvestigating = true;
    } 

    // 로그인 화면에서 암호 입력 후 엔터 치면
    public void TryLogin(string input)
    {
        passwordInput = input;
        StartCoroutine(ComparePassword());
    }

    IEnumerator ComparePassword()
    {
        yield return new WaitForSeconds(0.2f);
        string correctPassword = (string)GameManager.Instance.GetVariable("LaptopPassword");
        GameManager.Instance.SetVariable("LaptopPasswordCorrect", passwordInput == correctPassword);
        
        OnMouseDown();
        yield return new WaitForSeconds(0.3f);
        ResetPassword();
    }
    
    // 비밀번호 입력 초기화
    private void ResetPassword()
    {
        passwordInput = "";
        passwordInputField.text = passwordInput;
    }
}
