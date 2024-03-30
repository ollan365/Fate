using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : EventObject, IResultExecutable
{
    [SerializeField]
    private GameObject photos; // 상자 속 사진들

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

    private void ShowPhotos()  // 열쇠로 열면 상자 속 전단지 발견
    {
        if ((bool)GameManager.Instance.GetVariable("ClockTimeCorrect")&& !(bool)GameManager.Instance.GetVariable("BoxCorrect"))
        {
            photos.SetActive(true);
            RoomManager.Instance.AddScreenObjects(photos);
            GameManager.Instance.SetVariable("BoxCorrect", true);
            RoomManager.Instance.isResearch = true;
        }
    }

}