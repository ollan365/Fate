using System;
using UnityEngine;
using UnityEngine.UI;
using System.Globalization;
using System.Linq;
using TMPro;

public class CalendarPanel : MonoBehaviour
{
    public GameObject dayPrefab;  // 날짜 프리팹
    public Transform daysParent;  // 날짜를 배치할 부모 객체
    public TextMeshProUGUI monthYearText;  // 월과 년도를 표시할 텍스트

    private DateTime currentDate = DateTime.Now;

    void Start()
    {
        GameManager.Instance.SetVariable("CalendarMonth", Int32.Parse(currentDate.Month.ToString()));
        GenerateCalendar(currentDate.Year, currentDate.Month);
    }

    private void GenerateCalendar(int year, int month)
    {
        DateTime firstDay = new DateTime(year, month, 1);
        int daysInMonth = DateTime.DaysInMonth(year, month);
        int dayOfWeek = (int)firstDay.DayOfWeek;

        // Update month/year text
        monthYearText.text = firstDay.ToString("MMMM yyyy", CultureInfo.InvariantCulture);

        // Clear previous days
        foreach (Transform child in daysParent)
        {
            Destroy(child.gameObject);
        }

        // Generate days
        for (int i = 1; i <= daysInMonth; i++)
        {
            GameObject dayObj = Instantiate(dayPrefab, daysParent);
            dayObj.GetComponentInChildren<TextMeshProUGUI>().text = i.ToString();
            
            string monthString = month.ToString();
            if (month < 10) monthString = "0" + monthString;
            string day = i.ToString();
            if (day.Length == 1) day = "0" + day;
            string date = monthString + day;
            string fateBirthday = (string)GameManager.Instance.GetVariable("FateBirthday");
            string accidyBirthday = (string)GameManager.Instance.GetVariable("AccidyBirthday");

            string[] specialDates = new[] { fateBirthday, accidyBirthday, "1031", "1001" };

            if (specialDates.Contains(date)) dayObj.GetComponent<Image>().color = Color.magenta;
        }
    }

    public void ChangeMonth(int previousOrNext)  // -1 for previous, 1 for next
    {
        if (previousOrNext != -1 && previousOrNext != 1)
        {
            Debug.Log("previousOrNext must be -1 or 1!");
            return;
        }

        currentDate = currentDate.AddMonths(previousOrNext);
        if (previousOrNext == 1)  GameManager.Instance.IncrementVariable("CalendarMonth");
        else GameManager.Instance.DecrementVariable("CalendarMonth");
        
        GenerateCalendar(currentDate.Year, currentDate.Month);
    }
}