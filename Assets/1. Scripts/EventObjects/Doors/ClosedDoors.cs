using UnityEngine;

public class ClosedDoors : EventObject, IResultExecutable
{
    public GameObject openDoors;
    public Collider2D objectBehindCollider;

    public string parentObjectName;  // 옷장, 수남장 등 부모 오브젝트 이름

    private void Awake()
    {
        ResultManager.Instance.RegisterExecutable($"Closed{parentObjectName}Doors", this);
        objectBehindCollider = objectBehindCollider.GetComponent<Collider2D>();
    }

    public new void OnMouseDown()
    {
        if (DialogueManager.Instance.isDialogueActive) return;
        
        base.OnMouseDown();
        GameManager.Instance.IncrementVariable($"Closed{parentObjectName}DoorsClick");
    }

    public void ExecuteAction()
    {
        OpenDoors();
    }

    public void OpenDoors()
    {
        objectBehindCollider.enabled = true;
        GameManager.Instance.InverseVariable($"{parentObjectName}DoorsClosed");
        openDoors.SetActive(true);
        gameObject.SetActive(false);
    }
}
