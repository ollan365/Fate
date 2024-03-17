using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SearchManager : MonoBehaviour
{
    [SerializeField] private RoomMovManager roomMov;

    //public GameObject S_Diary;
    [SerializeField] private GameObject diary_p2;
    [SerializeField] private GameObject diary_p3;
    public static bool clearDiary = false;

    [SerializeField] private GameObject laptop_login;
    [SerializeField] private GameObject laptop_p1;
    public static bool clearLaptop = false;

    [SerializeField] private GameObject Clock_bg;
    [SerializeField] private GameObject key_bg;
    public static bool clearClock = false;

    public Button[] buttons;

    public void diaryBtn()
    {
        if (!clearDiary)
            ActivateObject(diary_p2);
        else
            Debug.Log("이미 살펴본 단서다.");
    }

    public void laptopBtn()
    {
        if (!clearLaptop)
            ActivateObject(laptop_login);
        else
            Debug.Log("이미 살펴본 단서다.");
    }

    public void clockBtn()
    {
        if (!clearClock)
            ActivateObject(Clock_bg);
        else
            Debug.Log("이미 살펴본 단서다.");
    }

    public void SearchExitBtn()
    {
        if (diary_p2.activeSelf || diary_p3.activeSelf)
        {
            if (diary_p3.activeSelf) clearDiary = true;
            DeactivateObjects(diary_p2, diary_p3);
        }

        if (laptop_p1.activeSelf || laptop_login.activeSelf)
        {
            if (laptop_p1.activeSelf) clearLaptop = true;
            DeactivateObjects(laptop_login, laptop_p1);
        }

        if (key_bg.activeSelf || Clock_bg.activeSelf)
        {
            if (key_bg.activeSelf) clearClock = true;
            DeactivateObjects(key_bg, Clock_bg);
        }

        ActivateButtons();
    }

    private void ActivateObject(GameObject obj)
    {
        foreach (Button button in buttons)
            button.gameObject.SetActive(false);

        roomMov.isResearch = true;
        obj.SetActive(true);
    }

    private void DeactivateObjects(params GameObject[] objects)
    {
        foreach (GameObject obj in objects)
        {
            roomMov.isResearch = false;
            obj.SetActive(false);
        }
    }

    private void ActivateButtons()
    {
        foreach (Button button in buttons)
            button.gameObject.SetActive(true);
    }
}
