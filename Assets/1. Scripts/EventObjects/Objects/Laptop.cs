using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Laptop : EventObject
{
    // �α��� ������ ������ ���� �̵� �� �Ǵϱ�..���� �̵� ����
    [SerializeField] private GameObject _object;

    private void Start()
    {
    }

    public void ClickBtn()
    {
        OnMouseDown();

        // LaptopPasswordCorrect�� false �� ���� �۵���
        if (!(bool)GameManager.Instance.GetVariable("LaptopPasswordCorrect"))
        {
            // ��Ʈ�� ��� ��ġ ����
            // �α��� ������ ����
            _object.SetActive(true);
            RoomManager.Instance.AddScreenObjects(_object);
            RoomManager.Instance.isResearch = true;
        }

        // Ŭ�� Ƚ�� ����
        GameManager.Instance.IncrementVariable("LaptopClick");
    }

}
