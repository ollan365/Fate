using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _1031 : EventObject
{
    public new void OnMouseDown()
    {
        base.OnMouseDown();

        // �޷� �ܼ� ����
        if ((int)GameManager.Instance.GetVariable("_1031Click") == 0)
            GameManager.Instance.IncrementVariable("CalendarClue");

        GameManager.Instance.IncrementVariable("_1031Click");


        Debug.Log("���� �޷� �ܼ� : " + GameManager.Instance.GetVariable("CalendarClue"));
    }
}