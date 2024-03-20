using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Laptop : EventObject
{
    // 로그인 페이지 켜지면 시점 이동 안 되니까..시점 이동 제한
    [SerializeField] private RoomMovManager roomMov;
    [SerializeField] private GameObject _object;

    private void Start()
    {
        roomMov = GameObject.Find("Room1 Manager").GetComponent<RoomMovManager>();
    }

    public void ClickBtn()
    {
        OnClick();

        // LaptopPasswordCorrect가 false 일 때만 작동함
        if (!(bool)GameManager.Instance.GetVariable("LaptopPasswordCorrect"))
        {
            // 노트북 잠금 장치 실행
            // 로그인 페이지 켜짐
            _object.SetActive(true);
            roomMov.addScreenObjects(_object);
            roomMov.isResearch = true;
        }

        // 클릭 횟수 증가
        GameManager.Instance.IncrementObjectClick("LaptopClick");
    }

}
