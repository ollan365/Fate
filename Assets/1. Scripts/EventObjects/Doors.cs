using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Doors : EventObject, IResultExecutable
{
    [SerializeField] private bool isClosedDoors;
    [SerializeField] private GameObject otherDoors;
    [SerializeField] private List<Collider2D> objectBehindColliders;
    [SerializeField] private string parentObjectName;  // 옷장, 수남장 등 부모 오브젝트 이름
    private string closedOrOpen;

    [SerializeField] private List<GameObject> sideClosedDoorObjects;
    [SerializeField] private List<GameObject> sideOpenDoorObjects;

    private string DoorsClosedVariable;

    protected override void Awake()
    {
        base.Awake();
        closedOrOpen = isClosedDoors ? "Closed" : "Open";
        ResultManager.Instance.RegisterExecutable($"{closedOrOpen}{parentObjectName}Doors", this);

        for (int i = 0; i < objectBehindColliders.Count; i++)
        {
            objectBehindColliders[i] = objectBehindColliders[i].GetComponent<Collider2D>();
        }

        DoorsClosedVariable = (parentObjectName == "Closet") ? "ClosetDoorsClosed" : "CabinetDoorsClosed";
    }

    private void OnEnable()
    {
        UpdateImageState();
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
        //isInquiry = false;  // 조사 시스템 예 아니오 스킵
        UpdateImageState();
    }

    private void UpdateImageState()
    {
        bool DoorsClosed = (bool)GameManager.Instance.GetVariable(DoorsClosedVariable);

        foreach (GameObject closedDoor in sideClosedDoorObjects)
            closedDoor.SetActive(DoorsClosed);

        foreach (GameObject openDoor in sideOpenDoorObjects)
            openDoor.SetActive(!DoorsClosed);

        // objectBehindColliders의 enabled를 Doors 열림 상태에 따라 활성화, 비활성화 적용
        foreach (var behindCollider in objectBehindColliders)
        {
            behindCollider.enabled = !DoorsClosed;
        }
    }
}