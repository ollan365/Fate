using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knife : EventObject, IResultExecutable
{

    private void Start()
    {
        ResultManager.Instance.RegisterExecutable("Knife", this);

        // �ٸ� ���̵忡�� �̹� Ŀ��Į ì������ Ŀ��Į �� ���̰� ��
        if ((int)GameManager.Instance.GetVariable("KnifeClick") > 0)
        {
            getKnife();
        }
    }

    public new void OnMouseDown()
    {
        base.OnMouseDown();
        GameManager.Instance.IncrementVariable("KnifeClick");
    }

    public void ExecuteAction()
    {
        getKnife();
    }

    // Ŀ��Į �����
    private void getKnife()
    {
        this.gameObject.SetActive(false);
    }
}