using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DreamDiary : EventObject, IResultExecutable
{
    [SerializeField]
    private GameObject DreamDiaryContents;

    private void Start()
    {
        ResultManager.Instance.RegisterExecutable("DreamDiary", this);
    }

    public new void OnMouseDown()
    {
        base.OnMouseDown();
    }

    public void ExecuteAction()
    {
        ReadBooks();
    }

    private void ReadBooks()
    {
        //isInquiry = false;  // 조사 시스템 예 아니오 스킵
        DreamDiaryContents.SetActive(true);
        UIManager.Instance.AnimateUI(DreamDiaryContents, true, true);
    }
}