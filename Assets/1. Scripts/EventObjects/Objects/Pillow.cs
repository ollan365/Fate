using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pillow : EventObject
{
    [SerializeField] private RoomMovManager roomMov;
    [SerializeField] private GameObject _object;

    private void Start()
    {
        roomMov = GameObject.Find("Room1 Manager").GetComponent<RoomMovManager>();
    }

    public void ClickBtn()
    {
        OnClick();

        // 첫번째 클릭
        if ((int)GameManager.Instance.GetVariable("PillowClick") == 0)
        {
            Debug.Log("배게 클릭 횟수: " + GameManager.Instance.GetVariable("PillowClick"));
            // 배게에 대한 설명
           
            // 행동력 감소됨

        }
        else if ((int)GameManager.Instance.GetVariable("PillowClick") == 1) // 두번째 클릭
        {
            Debug.Log("배게 클릭 횟수: " + GameManager.Instance.GetVariable("PillowClick"));
            // 배게 안에 있는 부적 발견
            _object.SetActive(true);
            roomMov.addScreenObjects(_object);
            roomMov.isResearch = true;

            // 행동력 감소됨

            // 부적에 대한 메모 작성

        }
        else // 세번 이상 클릭
        {
            Debug.Log("배게 클릭 횟수: " + GameManager.Instance.GetVariable("PillowClick"));
            // 조사 완료 스크립트

        }

        // 클릭 횟수 증가
        GameManager.Instance.IncrementObjectClick("PillowClick");

    }

}