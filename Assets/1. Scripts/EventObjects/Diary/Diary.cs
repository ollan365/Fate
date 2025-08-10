using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Diary : EventObject, IResultExecutable
{
    [SerializeField] private GameObject diaryLock;
    [SerializeField] private string diaryExecutableName;

    private void Start() {
        ResultManager.Instance.RegisterExecutable(diaryExecutableName, this);
    }

    public new void OnMouseDown() {
        base.OnMouseDown();
    }

    public void ExecuteAction() {
        ActivateDiaryLock();
    }

    // 다이어리 잠금 장치 실행
    private void ActivateDiaryLock() {
        isInquiry = false;  // 조사 시스템 예 아니오 스킵
        UIManager.Instance.AnimateUI(diaryLock, true, true);
        SetCurrentLockObjectCanvasGroup(diaryLock);
    }
}