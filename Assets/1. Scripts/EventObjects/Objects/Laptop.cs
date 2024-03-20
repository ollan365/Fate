using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Laptop : EventObject
{
    // �α��� ������ ������ ���� �̵� �� �Ǵϱ�..���� �̵� ����
    [SerializeField] private RoomMovManager roomMov;
    [SerializeField] private GameObject _object;

    private void Start()
    {
        roomMov = GameObject.Find("Room1 Manager").GetComponent<RoomMovManager>();
    }

    public void ClickBtn()
    {
        OnClick();

        // LaptopPasswordCorrect�� false �� ���� �۵���
        if (!(bool)GameManager.Instance.GetVariable("LaptopPasswordCorrect"))
        {
            // ��Ʈ�� ��� ��ġ ����
            // �α��� ������ ����
            _object.SetActive(true);
            roomMov.addScreenObjects(_object);
            roomMov.isResearch = true;
        }

        // Ŭ�� Ƚ�� ����
        GameManager.Instance.IncrementObjectClick("LaptopClick");
    }

}
