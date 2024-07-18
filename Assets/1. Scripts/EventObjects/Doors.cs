using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Doors : EventObject, IResultExecutable
{
    public bool isClosedDoors; 
    public GameObject otherDoors;
    public List<Collider2D> objectBehindColliders;
    public string parentObjectName;  // 옷장, 수남장 등 부모 오브젝트 이름
    private string closedOrOpen;

    private void Awake()
    {
        closedOrOpen = isClosedDoors ? "Closed" : "Open";
        ResultManager.Instance.RegisterExecutable($"{closedOrOpen}{parentObjectName}Doors", this);

        for (int i = 0; i < objectBehindColliders.Count; i++)
        {
            objectBehindColliders[i] = objectBehindColliders[i].GetComponent<Collider2D>();
        }
    }

    public new void OnMouseDown()
    {
        bool isBusy = GameManager.Instance.GetIsBusy();
        // Debug.Log($"isBusy: {isBusy}");
        if (isBusy) return;
        
        base.OnMouseDown();
        
        //if (isClosedDoors) GameManager.Instance.IncrementVariable($"{closedOrOpen}{parentObjectName}DoorsClick");
    }

    public void ExecuteAction()
    {
        ToggleDoors();
    }

    private void ToggleDoors()
    {
        isInquiry = false;  // 조사 시스템 예 아니오 스킵
        for (int i = 0; i < objectBehindColliders.Count; i++)
        {
            objectBehindColliders[i].enabled = isClosedDoors;
        }

        //GameManager.Instance.InverseVariable($"{parentObjectName}DoorsClosed");
        otherDoors.SetActive(true);
        gameObject.SetActive(false);

    }
}