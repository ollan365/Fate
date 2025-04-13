using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;

public class TinCaseLock : MonoBehaviour
{
    [SerializeField] private Wheel[] wheels;
    
    private string correctAnswer = "1102";
    //private bool showCorrectAnswer = false;

    [SerializeField] private TinCase tinCaseA;

    [SerializeField] private GameObject completeLockImage;

    public void CheckAnswer()
    {
        // 비밀번호 무한 입력 시도 방지
        RoomManager.Instance.ProhibitInput();

        string answer = "";
        foreach (var wheel in wheels)
        {
            answer += wheel.currentNumber.ToString();
        }

        if (answer == correctAnswer)
        {
            //showCorrectAnswer = true;
            StartCoroutine(ClearMessageAfterDelay(2));
            tinCaseA.SetIsInquiry(true);
            GameManager.Instance.SetVariable("TinCaseCorrect", true);

            SetCompleteLockImage(true);
            SetWheels(false);
        }
        else
        {
            //showCorrectAnswer = false;
            GameManager.Instance.SetVariable("TinCaseCorrect", false);
        }

        EventManager.Instance.CallEvent("EventTinCaseB");
    }

    //void OnGUI()
    //{
    //    if (showCorrectAnswer)
    //    {
    //        GUIStyle guiStyle = new GUIStyle(GUI.skin.label);
    //        guiStyle.fontSize = 24;
    //        guiStyle.normal.textColor = Color.green;

    //        GUI.Label(new Rect(Screen.width / 2 - 100, Screen.height / 2 + 200, 200, 50), "Correct Answer!", guiStyle);
    //    }
    //}

    private void SetCompleteLockImage(bool isShow)
    {
        completeLockImage.SetActive(isShow);
    }

    private void SetWheels(bool isShow)
    {
        foreach (var wheel in wheels)
        {
            wheel.gameObject.SetActive(isShow);
        }
    }

    IEnumerator ClearMessageAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        //showCorrectAnswer = false;
    }
}