using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laptop : EventObject, IResultExecutable
{
    // 로그인 페이지 켜지면 시점 이동 안 되니까..시점 이동 제한
    [SerializeField]
    private GameObject laptopLock;

    private void Start() {
        RegisterWithResultManager();
    }

    private void OnEnable()
    {
        RegisterWithResultManager();
    }

    private void RegisterWithResultManager()
    {
        if (ResultManager.Instance != null)
            ResultManager.Instance.RegisterExecutable("Laptop", this);
    }

    public void ExecuteAction() {
        ActivateLaptopLock();
    }

    public void ActivateLaptopLock() { // 노트북 잠금 장치 실행 (로그인 페이지 켜짐)
        //isInquiry = false;  // 조사 시스템 예 아니오 스킵
        UIManager.Instance.AnimateUI(laptopLock, true, true);
        SetCurrentLockObjectCanvasGroup(laptopLock);
    }
}
