using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


public class ClockPuzzle : EventObject
{
    [SerializeField] private GameObject hourHand;
    [SerializeField] private GameObject minuteHand;

    public void TryPassword(float currentHour)
    {
        // Debug.Log("TryPassword() called");
        //float currentHourAngle = hourHand.transform.rotation.eulerAngles.z;
        float currentHourAngle = currentHour;
        float currentMinuteAngle = minuteHand.transform.rotation.eulerAngles.z;

        Debug.Log("현재 시침 각도"+currentHourAngle);
        Debug.Log("현재 분침 각도" + currentMinuteAngle);

        StartCoroutine(CompareClockHands(currentHourAngle, currentMinuteAngle));
        
    }

    // private float[] correctAngles = { 210f, 180f };     //오후 5시 30분
    // private float[] correctAngles = { 180f, 180f };   //오후 6시 30분

    IEnumerator CompareClockHands(float hourAngle, float minuteAngle)
    {
        yield return new WaitForSeconds(0.2f);
        
        float[] correctAngles = (float[])GameManager.Instance.GetVariable("ClockPassword");

        float correctHourAngle = correctAngles[0], correctMinuteAngle = correctAngles[1];
        bool isTimeCorrect = Mathf.Approximately(correctHourAngle, hourAngle) &&
                             Mathf.Approximately(correctMinuteAngle, minuteAngle);

        // # - # - # - # - # 디버깅용 # - # - # - # - #
        // isTimeCorrect = true;
        Debug.Log("correctHourAngle : " + correctHourAngle);
        Debug.Log("correctMinuteAngle : " + correctMinuteAngle);

        GameManager.Instance.SetVariable("ClockTimeCorrect", isTimeCorrect);
        
        OnMouseDown();
        yield return new WaitForSeconds(0.3f);
        //ResetClockHands();
    }
    
    //private void ResetClockHands()
    //{
    //    hourHand.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
    //    minuteHand.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
    //}

}
