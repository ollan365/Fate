using UnityEngine;

public class LetterOfResignation : EventObject
{
    private void Start()
    {
        GetComponent<BoxCollider2D>().enabled = false;
    }

    protected override bool CanInteract()
    {
        return !GameManager.Instance.GetIsBusy();
    }
}
