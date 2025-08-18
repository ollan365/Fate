using System.Collections.Generic;
using UnityEngine;

public class Box : EventObject, IResultExecutable
{
    // ************************* temporary members for open animation *************************
    [SerializeField] private Animator boxAnimator;
    // ********************************************************************************

    public bool isClosed;

    public List<GameObject> sideClosedBox;
    public List<GameObject> sideOpenBox;

    private void Start()
    {
        ResultManager.Instance.RegisterExecutable("Box", this);
    }

    public new void OnMouseDown()
    {
        bool isBusy = GameManager.Instance.GetIsBusy();
        if (isBusy) return;

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
            isClosed = false;

            ChangeBoxOpenImage();
        }
    }
    // *******************************************************************************

    private void ChangeBoxOpenImage()
    {
        // Side 2, 3에 있는 Box 이미지들 open 상태로 변경
        if(!isClosed)
        {
            foreach (GameObject closedBox in sideClosedBox)
                closedBox.SetActive(false);

            foreach (GameObject openBox in sideOpenBox)
                openBox.SetActive(true);
        }
    }

}