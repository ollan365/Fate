using UnityEngine;

public class Doors : EventObject, IResultExecutable
{
    public bool isClosedDoors; 
    public GameObject otherDoors;
    public Collider2D objectBehindCollider;
    public string parentObjectName;  // 옷장, 수남장 등 부모 오브젝트 이름
    private string closedOrOpen;

    private void Awake()
    {
        closedOrOpen = isClosedDoors ? "Closed" : "Open";
        ResultManager.Instance.RegisterExecutable($"{closedOrOpen}{parentObjectName}Doors", this);
        objectBehindCollider = objectBehindCollider.GetComponent<Collider2D>();
    }

    public new void OnMouseDown()
    {
        if (DialogueManager.Instance.isDialogueActive) return;
        
        base.OnMouseDown();
        
        if (isClosedDoors) GameManager.Instance.IncrementVariable($"{closedOrOpen}{parentObjectName}DoorsClick");
    }

    public void ExecuteAction()
    {
        ToggleDoors();
    }

    private void ToggleDoors()
    {
        objectBehindCollider.enabled = isClosedDoors;
        GameManager.Instance.InverseVariable($"{parentObjectName}DoorsClosed");
        otherDoors.SetActive(true);
        gameObject.SetActive(false);
    }
}