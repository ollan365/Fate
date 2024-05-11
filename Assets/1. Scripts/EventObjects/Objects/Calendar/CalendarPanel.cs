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

        // 월 년도 텍스트
        monthYearText.text = firstDay.ToString("MMMM yyyy", CultureInfo.InvariantCulture);

        foreach (Transform child in daysParent)
        {
            Destroy(child.gameObject);
        }

        // 달력 페이지 생성
        for (int i = 1; i <= daysInMonth; i++)
        {
            GameObject dayObj = Instantiate(dayPrefab, daysParent);
            dayObj.GetComponentInChildren<TextMeshProUGUI>().text = i.ToString();
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