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
        GameManager.Instance.IncrementVariable("DiaryClick");
    }

    public void ExecuteAction()
    {
        ActivateDiaryLock();
    }

    // 다이어리 잠금 장치 실행
    public void ActivateDiaryLock()
    {
        diaryLock.SetActive(true);
    }
}