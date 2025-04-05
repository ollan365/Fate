using UnityEngine;

public class Box : EventObject, IResultExecutable
{
    // ************************* temporary members for open animation *************************
    [SerializeField] private Animator boxAnimator;
    // ********************************************************************************

    private void Start()
    {
        ResultManager.Instance.RegisterExecutable("Box", this);
    }

    public new void OnMouseDown()
    {
        base.OnMouseDown();
    }

    public void ExecuteAction()
    {
        OpenBox();
    }

    // ************************* temporary methods for open animation *************************
    private void OpenBox()
    {
        bool clockTimeCorrect = (bool)GameManager.Instance.GetVariable("ClockTimeCorrect");
        if (clockTimeCorrect)
        {
            RoomManager.Instance.SetIsInvestigating(true);
            boxAnimator.SetBool("open_Box", true);
        }
    }

    // *******************************************************************************
}