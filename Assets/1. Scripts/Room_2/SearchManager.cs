using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SearchManager : MonoBehaviour
{
    [SerializeField]
    private GameObject diary_p2;

    [SerializeField]
    private GameObject diary_p3;

    [SerializeField]
    private RoomMovManager roomMov;

    //public GameObject S_Diary;
    [SerializeField] private GameObject diary_p2;
    [SerializeField] private GameObject diary_p3;
    //public static bool clearDiary = false;

    [SerializeField] private GameObject laptop_login;
    [SerializeField] private GameObject laptop_p1;
    //public static bool clearLaptop = false;

    [SerializeField] private GameObject Clock_bg;
    [SerializeField] private GameObject key_bg;
    //public static bool clearClock = false;

    //public Button[] buttons;

    //public void diaryBtn()
    //{
    //    if (!clearDiary)
    //        ActivateObject(diary_p2);
    //    else
    //        Debug.Log("이미 살펴본 단서다.");
    //}

    //public void laptopBtn()
    //{
    //    if (!clearLaptop)
    //        ActivateObject(laptop_login);
    //    else
    //        Debug.Log("이미 살펴본 단서다.");
    //}

    //public void clockBtn()
    //{
    //    if (!clearClock)
    //        ActivateObject(Clock_bg);
    //    else
    //        Debug.Log("이미 살펴본 단서다.");
    //}

    public void SearchExitBtn()
    {
        if (diary_p2.activeSelf)
        {
            // 상세 화면..에서 나가기
            DeactivateObjects(diary_p2, diary_p3);
        }
        else if (diary_p3.activeSelf)
        {
            DeactivateObjects(laptop_login, laptop_p1);
        }

        if (key_bg.activeSelf || Clock_bg.activeSelf)
        {
            DeactivateObjects(key_bg, Clock_bg);
        }

        roomMov.isResearch = false;
        //ActivateButtons();
    }

    //private void ActivateObject(GameObject obj)
    //{
    //    foreach (Button button in buttons)
    //        button.gameObject.SetActive(false);

    //    roomMov.isResearch = true;
    //    obj.SetActive(true);
    //}

            clearClock = true;
        }
        else if (Clock_bg.activeSelf)
        {
            obj.SetActive(false);
        }

    //private void ActivateButtons()
    //{
    //    foreach (Button button in buttons)
    //        button.gameObject.SetActive(true);
    //}
}
