using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SewingBox : EventObject, IResultExecutable
{
    // �α��� ������ ������ ���� �̵� �� �Ǵϱ�..���� �̵� ����
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

    // �������� ��� ��ġ ����
    public void ActivateSewingBoxPuzzle()
    {
        isInquiry = false;  // ���� �ý��� �� �ƴϿ� ��ŵ
        SewingBoxPuzzle.SetActive(true);
    }
}