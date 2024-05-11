using System;
using UnityEngine;
using UnityEngine.UI;
using System.Globalization;
using System.Linq;
using TMPro;
using UnityEngine.Serialization;

public class CalendarPanel : MonoBehaviour
{
    public GameObject dayPrefab;
    public Transform daysParent;
    public TextMeshProUGUI monthText;
    public Image calendarBackground;
    public Sprite[] monthBackgrounds;  // 월별 이미지

    private DateTime currentDate = DateTime.Now;

    void Start()
    {
        GameManager.Instance.SetVariable("CalendarMonth", Int32.Parse(currentDate.Month.ToString()));
        GenerateCalendar(currentDate.Year, currentDate.Month);
        UpdateBackgroundImage(currentDate.Month);
    }

    private void GenerateCalendar(int year, int month)
    {
        DateTime firstDay = new DateTime(year, month, 1);
        int daysInMonth = DateTime.DaysInMonth(year, month);
        int emptyDays = (int)firstDay.DayOfWeek;

        monthText.text = firstDay.ToString("MMMM", CultureInfo.InvariantCulture);

        foreach (Transform child in daysParent)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < emptyDays; i++)
        {
            Instantiate(dayPrefab, daysParent).GetComponentInChildren<TextMeshProUGUI>().text = "";
        }

        for (int i = 1; i <= daysInMonth; i++)
        {
            GameObject dayObj = Instantiate(dayPrefab, daysParent);
            dayObj.GetComponentInChildren<TextMeshProUGUI>().text = i.ToString();
        }
    }

    private void UpdateBackgroundImage(int month)
    {
        if (month >= 1 && month <= 12)
        {
            calendarBackground.sprite = monthBackgrounds[month - 1];
        }
        else
        {
            Debug.LogError("Invalid month for background image");
        }
    }

    public void ChangeMonth(int previousOrNext)  // -1 for previous, 1 for next
    {
        if (previousOrNext != -1 && previousOrNext != 1)
        {
            Debug.LogError("previousOrNext must be -1 or 1!");
            return;
        }

        currentDate = currentDate.AddMonths(previousOrNext);
        if (previousOrNext == 1)  GameManager.Instance.IncrementVariable("CalendarMonth");
        else GameManager.Instance.DecrementVariable("CalendarMonth");
        
        GenerateCalendar(currentDate.Year, currentDate.Month);
        UpdateBackgroundImage(currentDate.Month);
    }
}
