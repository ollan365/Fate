using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clock : EventObject, IResultExecutable
{
    // 시계 시간 맞추는 화면일 때 시점 이동 제한
    [SerializeField]
    private GameObject clockPuzzle;

    private void Start()
    {
        ResultManager.Instance.RegisterExecutable("Clock", this);
    }

    public new void OnMouseDown()
    {
        base.OnMouseDown();
        GameManager.Instance.IncrementVariable("ClockClick");
    }

    public void ExecuteAction()
    {
        ActivateClockPuzzle();
    }

    // 시계 시간 맞추는 장치 실행
    private void ActivateClockPuzzle()
    {
        clockPuzzle.SetActive(true);
        RoomManager.Instance.AddScreenObjects(clockPuzzle);
        RoomManager.Instance.isResearch = true;
    }

}
