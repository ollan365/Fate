using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SewingBox : EventObject, IResultExecutable
{
    [SerializeField]
    private GameObject SewingBoxPuzzle;

    private void Start()
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

    // ������ ��� ��ġ ����
    public void ActivateSewingBoxPuzzle()
    {
        isInquiry = false;  // ���� �ý��� �� �ƴϿ� ��ŵ
        SewingBoxPuzzle.SetActive(true);
    }
}
