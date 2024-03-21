using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pillow : EventObject, IResultExecutable
{
    [SerializeField]
    private GameObject amulet;

    private void Start()
    {
        ResultManager.Instance.RegisterExecutable("Pillow", this);
    }

    public new void OnMouseDown()
    {
        base.OnMouseDown();
        GameManager.Instance.IncrementVariable("PillowClick");
    }
    
    public void ExecuteAction()
    {
        ShowAmulet();
    }

    private void ShowAmulet()  // 배게 안에 있는 부적 발견
    {
        amulet.SetActive(true);
        RoomManager.Instance.AddScreenObjects(amulet);
        RoomManager.Instance.isResearch = true;
    }
    
}