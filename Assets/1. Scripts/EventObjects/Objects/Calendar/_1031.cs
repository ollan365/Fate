using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _1031 : EventObject
{
    public new void OnMouseDown()
    {
        base.OnMouseDown();

        // 달력 단서 증가
        if ((int)GameManager.Instance.GetVariable("_1031Click") == 0)
            GameManager.Instance.IncrementVariable("CalendarClue");

        GameManager.Instance.IncrementVariable("_1031Click");


        Debug.Log("현재 달력 단서 : " + GameManager.Instance.GetVariable("CalendarClue"));
    }
}