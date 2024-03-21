using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clock : EventObject
{
    // �ð� �ð� ���ߴ� ȭ���� �� ���� �̵� ����
    [SerializeField] private GameObject _object;

    private void Start()
    {
    }

    public void ClickBtn()
    {
        OnMouseDown();

        if (!(bool)GameManager.Instance.GetVariable("ClockTimeCorrect"))
        {
            // �ð� �ð� ���ߴ� ��ġ ����
            _object.SetActive(true);
            RoomManager.Instance.AddScreenObjects(_object);
            RoomManager.Instance.isResearch = true;
        }

        // Ŭ�� Ƚ�� ����
        GameManager.Instance.IncrementVariable("ClockClick");
    }

}
