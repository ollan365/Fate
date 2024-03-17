using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Diary : EventObject
{
    // ���̾ �����ġ ���� ���� �� ���� �̵� ����
    [SerializeField] private RoomMovManager roomMov;
    [SerializeField] private GameObject _object;

    private void Start()
    {
        roomMov = GameObject.Find("Room2 Manager").GetComponent<RoomMovManager>();
    }

    public void ClickBtn()
    {
        // Ŭ�� Ƚ�� ����
        GameManager.Instance.IncrementObjectClick("DiaryClick");
        //Debug.Log("���̾Ŭ�� Ƚ��: "+ GameManager.Instance.GetVariable("DiaryClick"));

        // DiaryPasswordCorrect�� false �� ���� �۵���
        if (!(bool)GameManager.Instance.GetVariable("DiaryPasswordCorrect"))
        {
            // ���̾ ��� ��ġ ����
            // Diary_p2 ����
            _object.SetActive(true);
            roomMov.isResearch = true;
        }
    }
}
