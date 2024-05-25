using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SewingBox : EventObject, IResultExecutable
{
    // �α��� ������ ������ ���� �̵� �� �Ǵϱ�..���� �̵� ����
    [SerializeField]
    private GameObject SewingBoxLock;

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
        ActivateSewingBoxLock();
    }

    // ������ ��� ��ġ ����
    public void ActivateSewingBoxLock()
    {
        isInquiry = false;  // ���� �ý��� �� �ƴϿ� ��ŵ
        SewingBoxLock.SetActive(true);
    }
}
