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
    }

    public void ExecuteAction()
    {
        ActivateClockPuzzle();
    }

    // 시계 시간 맞추는 장치 실행
    private void ActivateClockPuzzle()
    {
        isInquiry = false;  // 조사 시스템 예 아니오 스킵
        clockPuzzle.SetActive(true);
    }

}
