using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Fate.Managers;


namespace Fate.Events
{
    public class Day : MonoBehaviour
    {
        private Dictionary<string, string> specialDates = new Dictionary<string, string>();
        private bool sameSpecialDate = false;
        private string date;
    
        [Header("Special Dates")]
        [SerializeField] private string specialDate1 = "1013";
        [SerializeField] private string specialDate2 = "1102";
    
        [Header("Special Date Image Gameobjects")]
        [SerializeField] private GameObject specialDateImage1;  // over
        [SerializeField] private GameObject specialDateImage2;  // under
    
        [Header("Special Date Sprites")]
        public Sprite birthDayCakeSprite;  // 필연 생일
        public Sprite successSprite;  // 성공 (specialDate1)
        public Sprite ruinedBirthDayCakeSprite;  // 우연 생일
        public Sprite ruinedCircleSprite;  // 검은 동그라미 (specialDate2)

        void Start()
        {
            CheckSpecialDate();
        }

        private void CheckSpecialDate()
        {
            var day = GetComponentInChildren<TextMeshProUGUI>().text.PadLeft(2, '0');
            var month = (int)GameManager.Instance.GetVariable("CalendarMonth");
            var monthString = month.ToString().PadLeft(2, '0');

            date = monthString + day;

            var fateBirthday = (string)GameManager.Instance.GetVariable("FateBirthday");
            var accidyBirthday = (string)GameManager.Instance.GetVariable("AccidyBirthday");
        
            specialDates.Add("FateBirthday", fateBirthday);
            specialDates.Add("AccidyBirthday", accidyBirthday);
            specialDates.Add(specialDate2, specialDate2);
            specialDates.Add(specialDate1, specialDate1);

            if (specialDates["FateBirthday"] == specialDates["AccidyBirthday"] ||
                specialDates["FateBirthday"] == specialDate2 ||
                specialDates["FateBirthday"] == specialDate2)
            {
                sameSpecialDate = true;
            }

            var specialDateImage1Image = specialDateImage1.GetComponent<Image>();
            var specialDateImage2Image = specialDateImage2.GetComponent<Image>();
        
            if (date == specialDates["FateBirthday"])
            {
                specialDateImage1Image.sprite = birthDayCakeSprite;
                specialDateImage1Image.color = new Color(1,1,1,1);
                // Debug.Log($"{date}: fate birthday");
            }
            else
            {
                specialDateImage1Image.sprite = null;
                specialDateImage1Image.color = new Color(0, 0, 0, 0);
            }

            specialDateImage2Image.color = new Color(1, 1, 1, 1);
            if (date == specialDate1)
            {
                specialDateImage2Image.sprite = successSprite;
                // Debug.Log($"{date}: specialDate1");
            }
            else if (date == specialDate2)
            {
                specialDateImage2Image.sprite = ruinedCircleSprite;
                // Debug.Log($"{date}: specialDate2");
            }
            else if (date == specialDates["AccidyBirthday"])
            {
                specialDateImage2Image.sprite = ruinedBirthDayCakeSprite;
                // Debug.Log($"{date}: Accidy Birthday");
            }
            else
            {
                specialDateImage2Image.sprite = null;
                specialDateImage2Image.color = new Color(0, 0, 0, 0);
            }

            var heightScale = 1f;
            if (date.StartsWith("03") || date.StartsWith("06")) heightScale = 0.8f;
            specialDateImage1.GetComponent<RectTransform>().localScale = new Vector3(1, heightScale, 1);
            specialDateImage2.GetComponent<RectTransform>().localScale = new Vector3(1, heightScale, 1);

            // Debug.Log($"special dates: {specialDates["FateBirthday"]}, {specialDates["AccidyBirthday"]}, {specialDate1}, {specialDate2}");

            GameManager.Instance.AddEventObject("EventCalendarBirthdayFate");
            GameManager.Instance.AddEventObject("EventCalendarBirthdayAccidy");
            GameManager.Instance.AddEventObject("EventCalendarSpecialDateA");
            GameManager.Instance.AddEventObject("EventCalendarSpecialDateB");
            GameManager.Instance.AddEventObject("EventCalendarSameBirthdays");
            GameManager.Instance.AddEventObject("EventCalendarSameAsSpecialDateA");
            GameManager.Instance.AddEventObject("EventCalendarSameAsSpecialDateB");
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
            if (date == specialDate2)
            {
                eventID = specialDates["FateBirthday"] == specialDate2 ? "EventCalendarSameAsSpecialDateA" : "EventCalendarSpecialDateA"; 
            }
            if (date == specialDate1)
            {
                eventID = specialDates["FateBirthday"] == specialDate1 ? "EventCalendarSameAsSpecialDateB" : "EventCalendarSpecialDateB"; 
            }

            if (string.IsNullOrEmpty(eventID)) return;

            // 조사하는 Day 중복조사 묻게 함
            // 여기의 Day들은 EventObject 상속 받아서 된 것이 아니기에
            // EventObject의 OnMouseDown에서 호출하던 메소드들을 똑같이 호출함
            GameManager.Instance.SetVariable("isInquiry", true);
            GameManager.Instance.SetCurrentInquiryObjectId(eventID);
            EventManager.Instance.CallEvent("Event_Inquiry");

            // CalendarCluesFound를 다 조사한 상태면 달력 중복 조사 상태로 변경
            if ((int)GameManager.Instance.GetVariable("CalendarCluesFound")>=4)
            {
                ResultManager.Instance.ExecuteResult("Result_IsFinishedEventCalendar");
            }
        }
    }
}
