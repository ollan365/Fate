using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laptop : EventObject, IResultExecutable
{
    // 로그인 페이지 켜지면 시점 이동 안 되니까..시점 이동 제한
    [SerializeField]
    private GameObject laptopLock;

    private void Awake()
    {
        ResultManager.Instance.RegisterExecutable("Laptop", this);
    }

    public new void OnMouseDown()
    {
        base.OnMouseDown();
        GameManager.Instance.IncrementVariable("LaptopClick");
    }

    public void ExecuteAction()
    {
        ActivateLaptopLock();
    }

    // 노트북 잠금 장치 실행 (로그인 페이지 켜짐)
    public void ActivateLaptopLock()
    {
        laptopLock.SetActive(true);
    }
}
