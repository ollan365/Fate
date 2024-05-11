using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Diary : EventObject, IResultExecutable
{
    [SerializeField]
    private GameObject diaryLock;

    private void Awake()
    {
        ResultManager.Instance.RegisterExecutable("Diary", this);
    }
    
    public new void OnMouseDown()
    {
        base.OnMouseDown();
    }

    public void ExecuteAction()
    {
        ActivateDiaryLock();
    }

    // 다이어리 잠금 장치 실행
    public void ActivateDiaryLock()
    {
        isInquiry = false;  // 조사 시스템 예 아니오 스킵
        diaryLock.SetActive(true);
    }
}