using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Diary : EventObject
{
    // 다이어리 잠금장치 보고 있을 때 시점 이동 제한
    [SerializeField] private RoomMovManager roomMov;
    [SerializeField] private GameObject _object;

    private void Start()
    {
        roomMov = GameObject.Find("Room1 Manager").GetComponent<RoomMovManager>();
    }

    public void ClickBtn()
    {
        OnClick();

        //Debug.Log("다이어리클릭 횟수: "+ GameManager.Instance.GetVariable("DiaryClick"));

        // DiaryPasswordCorrect가 false 일 때만 작동함
        if (!(bool)GameManager.Instance.GetVariable("DiaryPasswordCorrect"))
        {
            // 다이어리 잠금 장치 실행
            // Diary_p2 켜짐
            _object.SetActive(true);
            roomMov.addScreenObjects(_object);
            roomMov.isResearch = true;
        }

        // 클릭 횟수 증가
        GameManager.Instance.IncrementObjectClick("DiaryClick");
    }
}
