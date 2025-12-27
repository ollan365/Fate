using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DreamDiary : EventObject, IResultExecutable
{
    [SerializeField]
    private GameObject DreamDiaryContents;

    private void Start()
    {
        RegisterWithResultManager();
    }

    private void OnEnable()
    {
        RegisterWithResultManager();
    }

    private void RegisterWithResultManager()
    {
        if (ResultManager.Instance != null)
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
        //isInquiry = false;  // ���� �ý��� �� �ƴϿ� ��ŵ
        DreamDiaryContents.SetActive(true);
        UIManager.Instance.AnimateUI(DreamDiaryContents, true, true);
        SetCurrentLockObjectCanvasGroup(DreamDiaryContents);
    }
}