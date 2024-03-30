using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


public class ClockPuzzle : EventObject, IResultExecutable
{
    [SerializeField]
    private GameObject hourHand, minuteHand;
    [SerializeField]
    private GameObject keys;

    private void Start()
    {
        ResultManager.Instance.RegisterExecutable("ClockPuzzle", this);
    }
    
    public void ExecuteAction()
    {
        ShowKey();
    }

    // 시계 열리고 열쇠 획득
    private void ShowKey()
    {
        keys.SetActive(true);
        RoomManager.Instance.AddScreenObjects(keys);
        RoomManager.Instance.isResearch = true;
    }

    public void TryPassword()
    {
        Debug.Log("TryPassword() called");
        float currentHourAngle = hourHand.transform.rotation.eulerAngles.z;
        float currentMinuteAngle = minuteHand.transform.rotation.eulerAngles.z;
        StartCoroutine(CompareClockHands(currentHourAngle, currentMinuteAngle));
    }

    private float[] correctAngles = { 210f, 180f };

    IEnumerator CompareClockHands(float hourAngle, float minuteAngle)
    {
        yield return new WaitForSeconds(0.2f);
        
        //float[] correctAngles = (float[])GameManager.Instance.GetVariable("ClockPassword");

        //Debug.Log("시계정답 : " + (float[])GameManager.Instance.GetVariable("ClockPassword"));
        //Debug.Log("시계정답 : "+correctAngles[0]+" "+ correctAngles[1]);

        float correctHourAngle = correctAngles[0], correctMinuteAngle = correctAngles[1];
        bool isTimeCorrect = Mathf.Approximately(correctHourAngle, hourAngle) &&
                             Mathf.Approximately(correctMinuteAngle, minuteAngle);
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
