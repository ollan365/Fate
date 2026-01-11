using UnityEngine;

public class TutorialBlockingPanel : EventObject
{
    protected override bool CanInteract()
    {
        return !GameManager.Instance.GetIsBusy();
    }
}
