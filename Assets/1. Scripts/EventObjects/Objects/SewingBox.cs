using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SewingBox : EventObject, IResultExecutable
{
    // 로그인 페이지 켜지면 시점 이동 안 되니까..시점 이동 제한
    [SerializeField]
    private GameObject SewingBoxPuzzle;

    private void Awake()
    {
        ResultManager.Instance.RegisterExecutable("SewingBox", this);
    }

    public new void OnMouseDown()
    {
        base.OnMouseDown();
    }

    public void ExecuteAction()
    {
        ActivateSewingBoxPuzzle();
    }

    // 반짇고리 잠금 장치 실행
    public void ActivateSewingBoxPuzzle()
    {
        isInquiry = false;  // 조사 시스템 예 아니오 스킵
        SewingBoxPuzzle.SetActive(true);
    }
}
