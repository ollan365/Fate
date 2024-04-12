using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


public class ClockPuzzle : EventObject, IResultExecutable
{
    [SerializeField]
    private GameObject hourHand;
    [SerializeField]
    private GameObject minuteHand;
    [SerializeField]
    private GameObject keys;

    private void Awake()
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
        RoomManager.Instance.SetEventObjectPanel(true, "ClockKeys");
        //keys.SetActive(true);
        //RoomManager.Instance.AddScreenObjects(keys);
        RoomManager.Instance.isInvestigating = true;
    }

    public void TryPassword(float hourTime)
    {
        // Debug.Log("TryPassword() called");
        float currentHourTime = hourTime;
        float currentMinuteAngle = minuteHand.transform.rotation.eulerAngles.z;
        StartCoroutine(CompareClockHands(currentHourTime, currentMinuteAngle));
    }

    // private float[] correctAngles = { 210f, 180f };     //오후 5시 30분
    // private float[] correctAngles = { 180f, 180f };   //오후 6시 30분

    IEnumerator CompareClockHands(float hourTime, float minuteAngle)
    {
        yield return new WaitForSeconds(0.2f);
        
        float[] correctAngles = (float[])GameManager.Instance.GetVariable("ClockPassword");

        //Debug.Log("시계정답 : " + (float[])GameManager.Instance.GetVariable("ClockPassword"));
        //Debug.Log("시계정답 : "+correctAngles[0]+" "+ correctAngles[1]);

        float correctHourAngle = correctAngles[0], correctMinuteAngle = correctAngles[1];
        bool isTimeCorrect = Mathf.Approximately(correctHourAngle, hourTime) &&
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
