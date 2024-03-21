using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Diary : EventObject
{
    // ���̾ �����ġ ���� ���� �� ���� �̵� ����
    [SerializeField] private GameObject _object;

    private void Start()
    {
    }

    public void ClickBtn()
    {
        OnMouseDown();

        //Debug.Log("���̾Ŭ�� Ƚ��: "+ GameManager.Instance.GetVariable("DiaryClick"));

        // DiaryPasswordCorrect�� false �� ���� �۵���
        if (!(bool)GameManager.Instance.GetVariable("DiaryPasswordCorrect"))
        {
            // ���̾ ��� ��ġ ����
            // Diary_p2 ����
            _object.SetActive(true);
            RoomManager.Instance.AddScreenObjects(_object);
            RoomManager.Instance.isResearch = true;
        }

        // Ŭ�� Ƚ�� ����
        GameManager.Instance.IncrementVariable("DiaryClick");
    }
}
