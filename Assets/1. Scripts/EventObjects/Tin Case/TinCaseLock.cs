using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TinCaseLock : MonoBehaviour
{
    [SerializeField] private Wheel[] wheels;
    
    private string correctAnswer = "1102";
    //private bool showCorrectAnswer = false;

    [SerializeField] private TinCase tinCaseA;

    public void CheckAnswer()
    {
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

    IEnumerator ClearMessageAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        //showCorrectAnswer = false;
    }
}