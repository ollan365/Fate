using System;
using UnityEngine;
using UnityEngine.UI;
using System.Globalization;
using TMPro;

public class CalendarPanel : MonoBehaviour
{
    public GameObject dayPrefab;
    public Transform daysParent;
    public TextMeshProUGUI monthText;
    public Image calendarBackground;
    public Sprite[] monthBackgrounds;  // 월별 이미지
    public Button nextButton;
    public Button prevButton;
    public RectTransform daysRectTransform;
    public GridLayoutGroup layoutGroup;

    private DateTime currentDate;

    void Start()
    {
        currentDate = new DateTime(2024, 5, 1);  // 2024년 5월 시작으로 고정
        GameManager.Instance.SetVariable("CalendarMonth", currentDate.Month);
        UpdateCalendar();
    }

    void UpdateCalendar()
    {
        GenerateCalendar(currentDate.Year, currentDate.Month);
        UpdateBackgroundImage(currentDate.Month);
        ManageNavigationButtons();
        CheckRowsRequirement();
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
            Instantiate(dayPrefab, daysParent).GetComponentInChildren<TextMeshProUGUI>().text = i.ToString();
        }
    }

    private void UpdateBackgroundImage(int month)
    {
        if (month >= 1 && month <= 12)
        {
            calendarBackground.sprite = monthBackgrounds[month - 1];
        }
    }

    private void ManageNavigationButtons()
    {
        nextButton.gameObject.SetActive(currentDate.Month != 12);
        prevButton.gameObject.SetActive(currentDate.Month != 1);
    }

    private void CheckRowsRequirement()
    {
        int totalCells = (int)currentDate.DayOfWeek + DateTime.DaysInMonth(currentDate.Year, currentDate.Month);
        if (totalCells > 35)  // 6줄이 필요한 경우
        {
            daysRectTransform.anchoredPosition = new Vector2(-323, daysRectTransform.anchoredPosition.y);
            layoutGroup.cellSize = new Vector2(layoutGroup.cellSize.x, 81.5f);
            layoutGroup.spacing = new Vector2(10, 7.4f);
        }
        else  // 5줄이 필요한 경우
        {
            daysRectTransform.anchoredPosition = new Vector2(-322, daysRectTransform.anchoredPosition.y);
            layoutGroup.cellSize = new Vector2(layoutGroup.cellSize.x, 98f);
            layoutGroup.spacing = new Vector2(9.65f, 10.7f);
        }
    }

    public void ChangeMonth(int previousOrNext)  // -1 for previous, 1 for next
    {
        currentDate = currentDate.AddMonths(previousOrNext);
        GameManager.Instance.SetVariable("CalendarMonth", currentDate.Month);
        UpdateCalendar();
    }
}
