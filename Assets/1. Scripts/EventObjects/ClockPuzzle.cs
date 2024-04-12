using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


public class ClockPuzzle : EventObject
{
    [SerializeField]
    private GameObject hourHand, minuteHand;
    [SerializeField]
    private GameObject keys;

    public void TryPassword()
    {
        float currentHourAngle = hourHand.transform.rotation.eulerAngles.z;
        float currentMinuteAngle = minuteHand.transform.rotation.eulerAngles.z;
        StartCoroutine(CompareClockHands(currentHourAngle, currentMinuteAngle));
    }

    IEnumerator CompareClockHands(float hourAngle, float minuteAngle)
    {
        yield return new WaitForSeconds(0.2f);
        
        float[] correctAngles = (float[])GameManager.Instance.GetVariable("ClockPassword");

        float correctHourAngle = correctAngles[0], correctMinuteAngle = correctAngles[1];
        bool isTimeCorrect = Mathf.Approximately(correctHourAngle, hourAngle) &&
                             Mathf.Approximately(correctMinuteAngle, minuteAngle);
        
        // # - # - # - # - # 디버깅용 # - # - # - # - #
        // isTimeCorrect = true;
        
        GameManager.Instance.SetVariable("ClockTimeCorrect", isTimeCorrect);
        
        OnMouseDown();
        yield return new WaitForSeconds(0.3f);
        ResetClockHands();
    }
    
    private void ResetClockHands()
    {
        hourHand.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        minuteHand.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
    }

}
