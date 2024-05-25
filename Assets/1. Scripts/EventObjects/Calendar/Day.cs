using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Day : MonoBehaviour
{
    private Dictionary<string, string> specialDates = new Dictionary<string, string>();
    private bool sameSpecialDate = false;
    private string date;
    
    void Start()
    {
        CheckSpecialDate();
    }

    private void CheckSpecialDate()
    {
        string day = GetComponentInChildren<TextMeshProUGUI>().text.PadLeft(2, '0');
        int month = (int)GameManager.Instance.GetVariable("CalendarMonth");
        string monthString = month.ToString().PadLeft(2, '0');

        date = monthString + day;

        string fateBirthday = (string)GameManager.Instance.GetVariable("FateBirthday");
        string accidyBirthday = (string)GameManager.Instance.GetVariable("AccidyBirthday");
        
        specialDates.Add("FateBirthday", fateBirthday);
        specialDates.Add("AccidyBirthday", accidyBirthday);
        specialDates.Add("1031", "1031");
        specialDates.Add("1001", "1001");

        if (specialDates["FateBirthday"] == specialDates["AccidyBirthday"] ||
            specialDates["FateBirthday"] == "1031" ||
            specialDates["FateBirthday"] == "1001")
        {
            sameSpecialDate = true;
        }

        if (specialDates.ContainsValue(date))
        {
            GetComponent<Image>().color = Color.green;
        }

    }

    public void OnMouseDown()
    {
        string eventID = null;

        if (date == specialDates["FateBirthday"] && !sameSpecialDate)
        {
            eventID = "EventCalendarBirthdayFate";
        }
        if (date == specialDates["AccidyBirthday"])
        {
            eventID = specialDates["FateBirthday"] == specialDates["AccidyBirthday"] ? "EventCalendarSameBirthdays" : "EventCalendarBirthdayAccidy";
        }
        if (date == "1031")
        {
            eventID = specialDates["FateBirthday"] == "1031" ? "EventCalendarSameAsSpecialDateA" : "EventCalendarSpecialDateA"; 
        }
        if (date == "1001")
        {
            eventID = specialDates["FateBirthday"] == "1001" ? "EventCalendarSameAsSpecialDateB" : "EventCalendarSpecialDateB"; 
        }

        if (string.IsNullOrEmpty(eventID)) return;
        EventManager.Instance.CallEvent(eventID);
    }
}