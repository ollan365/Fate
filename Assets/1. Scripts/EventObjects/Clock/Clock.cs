using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Clock : EventObject, IResultExecutable
{
    // 시계 시간 맞추는 화면일 때 시점 이동 제한
    [SerializeField]
    private GameObject clockPuzzle;

    [SerializeField] private List<Image> Clocks;

    [SerializeField] private Sprite AfterClockImage;

    private void Start()
    {
        ResultManager.Instance.RegisterExecutable("Clock", this);
    }

    public new void OnMouseDown() {
        base.OnMouseDown();
    }

    public void ExecuteAction() {
        ActivateClockPuzzle();
    }

    // 시계 시간 맞추는 장치 실행
    private void ActivateClockPuzzle() {
        isInquiry = false;  // 조사 시스템 예 아니오 스킵
        UIManager.Instance.AnimateUI(clockPuzzle, true, true);
    }

    public void SwapAfterImage() {
        bool isCorrect = (bool)GameManager.Instance.GetVariable("ClockTimeCorrect");
        if(isCorrect)
            foreach (Image beforeClock in Clocks)
                beforeClock.sprite = AfterClockImage;
    }
}
