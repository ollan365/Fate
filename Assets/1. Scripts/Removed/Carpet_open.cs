using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Carpet_open : EventObject, IResultExecutable
{
    [SerializeField] private GameObject CarpetPaper;

    private void Awake()
    {
        ResultManager.Instance.RegisterExecutable("Carpet_Open", this);
    }

    public new void OnMouseDown()
    {
        base.OnMouseDown();
        if ((bool)GameManager.Instance.GetVariable("ChairMoved")) GameManager.Instance.IncrementVariable("CarpetClick");
    }

    public void ExecuteAction()
    {
        CarpetClose();
    }

    [Header("���� ī�� ��ư")]
    [SerializeField] private GameObject carpetClosed;

    private void CarpetClose()
    {
        gameObject.SetActive(false);
        carpetClosed.SetActive(true);

        // ī�� �� ���� ��Ȱ��ȭ
        CarpetPaper.SetActive(false);
    }
}
