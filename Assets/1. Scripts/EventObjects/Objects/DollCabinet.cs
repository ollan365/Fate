using UnityEngine;

public class DollCabinet : EventObject
{
    public void ClickBtn()
    {
        OnClick();

        //// 첫번째 클릭
        //if ((int)GameManager.Instance.GetVariable("PillowClick") == 0)
        //{
        //    // 인형 수납장에 대한 설명

        //    // 행동력 감소

        //}
        //else // 그 이후 클릭
        //{
        //    // 조사 완료 스크립트

        //}

        // 클릭 횟수 증가
        GameManager.Instance.IncrementObjectClick("DollCabinetClick");

    }

}