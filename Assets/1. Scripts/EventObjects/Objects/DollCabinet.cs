using UnityEngine;

public class DollCabinet : EventObject
{
    public void ClickBtn()
    {
        //if ((int)GameManager.Instance.GetVariable("DollCabinetClick") != 0)
        //    Debug.Log("이미 확인한 단서다.");

        // 클릭 횟수 증가
        GameManager.Instance.IncrementObjectClick("DollCabinetClick");
    }
}