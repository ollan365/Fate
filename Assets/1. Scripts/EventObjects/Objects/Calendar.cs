using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Calendar : EventObject, IResultExecutable
{
    [SerializeField]
    private GameObject calendarUI;

    private void Start()
    {
        ResultManager.Instance.RegisterExecutable("Calendar", this);
    }

    public new void OnMouseDown()
    {
        base.OnMouseDown();
        GameManager.Instance.IncrementVariable("CalendarClick");
    }

    public void ExecuteAction()
    {
        ShowCalendarUI();
    }

    private void ShowCalendarUI()   // �޷� �󼼷� ������
    {
        if ((int)GameManager.Instance.GetVariable("CalendarClue")<4)
        {
            calendarUI.SetActive(true);
            RoomManager.Instance.AddScreenObjects(calendarUI);
            RoomManager.Instance.isResearch = true;
        }
        
    }

    public void CalendarHomeBtn()
    {
        GameManager.Instance.SetVariable("CalendarMonth", false);
    }

    // month��ư�� ���� ����
    public void OnCalendarMonth()
    {
        GameManager.Instance.SetVariable("CalendarMonth", true);
    }
}