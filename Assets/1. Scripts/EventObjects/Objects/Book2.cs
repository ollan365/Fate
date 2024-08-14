using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Book2 : EventObject, IResultExecutable
{
    [SerializeField]
    private GameObject Book2Contents;

    private void Start()
    {
        ResultManager.Instance.RegisterExecutable("Book2", this);
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
        Book2Contents.SetActive(true);
    }
}