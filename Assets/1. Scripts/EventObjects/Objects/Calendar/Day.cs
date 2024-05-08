using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Day: MonoBehaviour
{
    public void OnMouseDown()
    {
        int month = (int)GameManager.Instance.GetVariable("CalendarMonth");
        string monthString = month.ToString();
        if (month < 10) monthString = "0" + monthString;

        string day = GetComponentInChildren<TextMeshProUGUI>().text;
        if (day.Length == 1) day = "0" + day;

        string date = monthString + day;
        
        Debug.Log(date);

        string fateBirthday = (string)GameManager.Instance.GetVariable("FateBirthday");
        string accidyBirthday = (string)GameManager.Instance.GetVariable("AccidyBirthday");

        if (date == fateBirthday)
        {
            EventManager.Instance.CallEvent("EventStorageCalendarFateBirth");
        }
        else if (date == accidyBirthday)
        {
            EventManager.Instance.CallEvent("EventStorageCalendarAccidyBirth");
        }
        else if (date == "1031")
        {
            EventManager.Instance.CallEvent("EventStorageCalendar1031");
        }
        else if (date == "1001")
        {
            EventManager.Instance.CallEvent("EventStorageCalendar1001");
        }
    }
}