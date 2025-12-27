using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Calendar : EventObject, IResultExecutable
{
    [SerializeField]
    private GameObject calendarPanel;

    private void Start() {
        RegisterWithResultManager();
    }

    private void OnEnable()
    {
        RegisterWithResultManager();
    }

    private void RegisterWithResultManager()
    {
        if (ResultManager.Instance != null)
            ResultManager.Instance.RegisterExecutable("Calendar", this);
    }

    public new void OnMouseDown() {
        bool isBusy = GameManager.Instance.GetIsBusy();
        if (isBusy) 
            return;
        
        base.OnMouseDown();
    }

    public void ExecuteAction() {
        ActivateCalendarPanel();
    }

    private void ActivateCalendarPanel() {
        UIManager.Instance.AnimateUI(calendarPanel, true, true);
        SetCurrentLockObjectCanvasGroup(calendarPanel);
    }
}