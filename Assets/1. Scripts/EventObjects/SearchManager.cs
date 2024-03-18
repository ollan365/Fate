using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SearchManager : MonoBehaviour
{
    [SerializeField] private RoomMovManager roomMov;

    [SerializeField] private GameObject diary_p2;
    [SerializeField] private GameObject diary_p3;

    [SerializeField] private GameObject laptop_login;
    [SerializeField] private GameObject laptop_p1;

    [SerializeField] private GameObject Clock_bg;
    [SerializeField] private GameObject key_bg;

    public void SearchExitBtn()
    {
        if (diary_p2.activeSelf || diary_p3.activeSelf)
        {
            // 상세 화면..에서 나가기
            DeactivateObjects(diary_p2, diary_p3);
        }

        if (laptop_p1.activeSelf || laptop_login.activeSelf)
        {
            DeactivateObjects(laptop_login, laptop_p1);
        }

        if (key_bg.activeSelf || Clock_bg.activeSelf)
        {
            DeactivateObjects(key_bg, Clock_bg);
        }

        roomMov.isResearch = false;
    }

    private void DeactivateObjects(params GameObject[] objects)
    {
        foreach (GameObject obj in objects)
        {
            obj.SetActive(false);
        }
    }
}
