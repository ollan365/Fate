using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : EventObject, IResultExecutable
{
    [SerializeField]
    private GameObject photos; // ���� �� ������

    private void Start()
    {
        ResultManager.Instance.RegisterExecutable("Box", this);
    }

    public new void OnMouseDown()
    {
        base.OnMouseDown();
        GameManager.Instance.IncrementVariable("BoxClick");
    }

    public void ExecuteAction()
    {
        ShowPhotos();
    }

    private void ShowPhotos()  // ����� ���� ���� �� ������ �߰�
    {
        if ((bool)GameManager.Instance.GetVariable("ClockTimeCorrect")&& !(bool)GameManager.Instance.GetVariable("BoxCorrect"))
        {
            photos.SetActive(true);
            RoomManager.Instance.AddScreenObjects(photos);
            GameManager.Instance.SetVariable("BoxCorrect", true);
            RoomManager.Instance.isInvestigating = true;
        }
    }

}