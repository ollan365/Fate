using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Clock : EventObject, IResultExecutable
{
    [SerializeField] private GameObject clockPuzzle;
    [SerializeField] private List<Image> Clocks;
    [SerializeField] private List<Sprite> AfterClockImages;

    private void Start() {
        RegisterWithResultManager();
    }

    private void OnEnable()
    {
        RegisterWithResultManager();
        UpdateImageState();
    }

    private void RegisterWithResultManager()
    {
        if (ResultManager.Instance != null)
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
        //isInquiry = false;  // 조사 시스템 예 아니오 스킵
        UIManager.Instance.AnimateUI(clockPuzzle, true, true);
        SetCurrentLockObjectCanvasGroup(clockPuzzle);
    }

    public void UpdateImageState() {
        if ((bool)GameManager.Instance.GetVariable("ClockTimeCorrect"))
        {
            for (int i = 0; i < Clocks.Count; i++)
            {
                Clocks[i].sprite = (i == 2) ? AfterClockImages[1] : AfterClockImages[0];
            }
        }
    }
}
