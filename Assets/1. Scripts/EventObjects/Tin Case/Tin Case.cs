using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TinCase : EventObject, IResultExecutable
{
    [SerializeField] private GameObject tinCaseLock;
    
    private void Awake()
    {
        ResultManager.Instance.RegisterExecutable("TinCase", this);
    }
    

    public new void OnMouseDown()
    {
        bool isBusy = GameManager.Instance.GetIsBusy();
        if (isBusy) return;
        base.OnMouseDown();
    }

    public void ExecuteAction()
    {
        ActivateTinCaseLock();
    }

    // 틴케이스 잠금 장치 실행
    private void ActivateTinCaseLock()
    {
        isInquiry = false;  // 조사 시스템 예 아니오 스킵
        tinCaseLock.SetActive(true);
    }
}
