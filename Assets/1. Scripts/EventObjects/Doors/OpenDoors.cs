using UnityEngine;

public class OpenDoors : EventObject, IResultExecutable
{
    public GameObject closedDoors;
    public Collider2D objectBehindCollider;
   
    public string parentObjectName;  // 옷장, 수남장 등 부모 오브젝트 이름

    private void Awake()
    {
        ResultManager.Instance.RegisterExecutable($"Open{parentObjectName}Doors", this);
        objectBehindCollider = objectBehindCollider.GetComponent<Collider2D>();
    }

    public new void OnMouseDown()
    {
        if (DialogueManager.Instance.isDialogueActive) return;
        
        base.OnMouseDown();
    }

    public void ExecuteAction()
    {
        CloseDoors();
    }

    public void CloseDoors()
    {
        objectBehindCollider.enabled = false;
        GameManager.Instance.InverseVariable($"{parentObjectName}DoorsClosed");
        closedDoors.SetActive(true);
        gameObject.SetActive(false);
    }
}
