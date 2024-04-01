using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Diary : EventObject, IResultExecutable
{
    // 다이어리 잠금장치 보고 있을 때 시점 이동 제한
    [SerializeField]
    private GameObject diaryLock;

    private void Start()
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
        RoomManager.Instance.AddScreenObjects(diaryLock);
        RoomManager.Instance.isInvestigating = true;
    }
}