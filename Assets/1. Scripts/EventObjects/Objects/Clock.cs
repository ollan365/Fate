using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clock : EventObject
{
    // �ð� �ð� ���ߴ� ȭ���� �� ���� �̵� ����
    [SerializeField] private RoomMovManager roomMov;
    [SerializeField] private GameObject _object;

    private void Start()
    {
        roomMov = GameObject.Find("Room1 Manager").GetComponent<RoomMovManager>();
    }

    public void ClickBtn()
    {
        OnClick();

        if (!(bool)GameManager.Instance.GetVariable("ClockTimeCorrect"))
        {
            // �ð� �ð� ���ߴ� ��ġ ����
            _object.SetActive(true);
            roomMov.addScreenObjects(_object);
            roomMov.isResearch = true;
        }

        // Ŭ�� Ƚ�� ����
        GameManager.Instance.IncrementObjectClick("ClockClick");
    }

}
