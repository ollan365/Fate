using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Calendar : EventObject, IResultExecutable
{
    [SerializeField]
    private GameObject calendarPanel;

    private void Awake()
    {
        ResultManager.Instance.RegisterExecutable("Calendar", this);
    }

    public new void OnMouseDown()
    {
        bool isBusy = GameManager.Instance.GetIsBusy();
        if (isBusy) return;
        base.OnMouseDown();
        // GameManager.Instance.IncrementVariable("CalendarClick");
    }

    public void ExecuteAction()
    {
        ActivateCalendarPanel();
    }

    private void ActivateCalendarPanel()
    {
        calendarPanel.SetActive(true);
    }
}