using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clock : EventObject
{
    // 시계 시간 맞추는 화면일 때 시점 이동 제한
    [SerializeField] private RoomMovManager roomMov;
    [SerializeField] private GameObject _object;

    private void Start()
    {
        roomMov = GameObject.Find("Room1 Manager").GetComponent<RoomMovManager>();
    }

    public void ClickBtn()
    {
        OnClick();

        if (!(bool)GameManager.Instance.GetVariable("ClockTimeCorrect"))
        {
            // 시계 시간 맞추는 장치 실행
            _object.SetActive(true);
            roomMov.addScreenObjects(_object);
            roomMov.isResearch = true;
        }

        // 클릭 횟수 증가
        GameManager.Instance.IncrementObjectClick("ClockClick");
    }

}
