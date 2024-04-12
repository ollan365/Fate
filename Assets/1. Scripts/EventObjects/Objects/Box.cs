using UnityEngine;

public class Box : EventObject
{
    public new void OnMouseDown()
    {
        base.OnMouseDown();
        GameManager.Instance.IncrementVariable("BoxClick");
    }
}