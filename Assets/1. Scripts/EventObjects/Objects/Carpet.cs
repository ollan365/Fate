using UnityEngine;

public class Carpet : EventObject, IResultExecutable
{
    public bool isClosedCarpet; 
    public GameObject otherCarpet;
    public Collider2D objectBehindCollider;
    private string closedOrOpen;

    private void Awake()
    {
        closedOrOpen = isClosedCarpet ? "Closed" : "Open";
        ResultManager.Instance.RegisterExecutable($"{closedOrOpen}Carpet{sideNum}", this);
        if(objectBehindCollider!=null)
            objectBehindCollider = objectBehindCollider.GetComponent<Collider2D>();

        //Debug.Log($"{closedOrOpen}Carpet{sideNum}");

        if(!isClosedCarpet)
            gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        UpdateImageState();
    }

    public new void OnMouseDown()
    {
        GameManager.Instance.SetVariable("isInquiry", false);

        bool isBusy = GameManager.Instance.GetIsBusy();
        if (isBusy) return;
        
        base.OnMouseDown();
    }

    public void ExecuteAction()
    {
        if (isClosedCarpet) GameManager.Instance.IncrementVariable("ClosedCarpetClick");
        
        ToggleCarpet();
    }

    private void ToggleCarpet()
    {
        if (objectBehindCollider != null)
            objectBehindCollider.enabled = isClosedCarpet;
        GameManager.Instance.InverseVariable("CarpetClosed");
        otherCarpet.SetActive(true);
        gameObject.SetActive(false);
    }

    private void UpdateImageState()
    {
        bool carpetClosed = (bool)GameManager.Instance.GetVariable("CarpetClosed");
        bool shouldActivateSelf = (carpetClosed == isClosedCarpet);

        gameObject.SetActive(shouldActivateSelf);
        otherCarpet.SetActive(!shouldActivateSelf);
    }

}