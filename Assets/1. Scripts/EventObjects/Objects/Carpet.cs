using UnityEngine;

public class Carpet : EventObject, IResultExecutable
{
    public bool isClosedCarpet; 
    public GameObject otherCarpet;
    public Collider2D objectBehindCollider;
    private string closedOrOpen;

    private void Start()
    {
        closedOrOpen = isClosedCarpet ? "Closed" : "Open";
        ResultManager.Instance.RegisterExecutable($"{closedOrOpen}Carpet", this);
        objectBehindCollider = objectBehindCollider.GetComponent<Collider2D>();
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
        objectBehindCollider.enabled = isClosedCarpet;
        GameManager.Instance.InverseVariable($"CarpetClosed");
        otherCarpet.SetActive(true);
        gameObject.SetActive(false);
    }
}