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
        if (DialogueManager.Instance.isDialogueActive) return;
        base.OnMouseDown();
        GameManager.Instance.IncrementVariable("CalendarClick");
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