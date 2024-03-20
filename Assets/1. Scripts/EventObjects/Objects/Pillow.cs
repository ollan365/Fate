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

        // ù��° Ŭ��
        if ((int)GameManager.Instance.GetVariable("PillowClick") == 0)
        {
            Debug.Log("��� Ŭ�� Ƚ��: " + GameManager.Instance.GetVariable("PillowClick"));
            // ��Կ� ���� ����
           
            // �ൿ�� ���ҵ�

        }
        else if ((int)GameManager.Instance.GetVariable("PillowClick") == 1) // �ι�° Ŭ��
        {
            Debug.Log("��� Ŭ�� Ƚ��: " + GameManager.Instance.GetVariable("PillowClick"));
            // ��� �ȿ� �ִ� ���� �߰�
            _object.SetActive(true);
            roomMov.addScreenObjects(_object);
            roomMov.isResearch = true;

            // �ൿ�� ���ҵ�

            // ������ ���� �޸� �ۼ�

        }
        else // ���� �̻� Ŭ��
        {
            Debug.Log("��� Ŭ�� Ƚ��: " + GameManager.Instance.GetVariable("PillowClick"));
            // ���� �Ϸ� ��ũ��Ʈ

        }

        // Ŭ�� Ƚ�� ����
        GameManager.Instance.IncrementObjectClick("PillowClick");

    }

}