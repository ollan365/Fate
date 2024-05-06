using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Calendar_ : EventObject, IResultExecutable
{
    [SerializeField]
    private GameObject calendarUI;

    private void Awake()
    {
        ResultManager.Instance.RegisterExecutable("Calendar_", this);
    }

    public new void OnMouseDown()
    {
        if (DialogueManager.Instance.isDialogueActive) return;
        base.OnMouseDown();
        GameManager.Instance.IncrementVariable("CalendarClick");
    }

    public void ExecuteAction()
    {
        ShowCalendarUI();
    }

    private void ShowCalendarUI()   // 달력 상세로 보여줌
    {
        if ((int)GameManager.Instance.GetVariable("CalendarCluesFound")<4)
        {
            calendarUI.SetActive(true);
            // RoomManager.Instance.AddScreenObjects(calendarUI);
            // RoomManager.Instance.isInvestigating = true;
        }
        
    }

    public void CalendarHomeBtn()
    {
        GameManager.Instance.SetVariable("CalendarMonth", false);
    }

    // month버튼들 마다 붙임
    public void OnCalendarMonth()
    {
        GameManager.Instance.SetVariable("CalendarMonth", true);
    }
}