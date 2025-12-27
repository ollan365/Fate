using System.Collections.Generic;
using UnityEngine;

public class Box : EventObject, IResultExecutable
{
    // ************************* temporary members for open animation *************************
    [SerializeField] private Animator boxAnimator;
    // ********************************************************************************

    public List<GameObject> sideClosedBox;
    public List<GameObject> sideOpenBox;

    private void Start()
    {
        RegisterWithResultManager();
    }

    private void OnEnable()
    {
        RegisterWithResultManager();
        UpdateImageState();
    }

    private void RegisterWithResultManager()
    {
        if (ResultManager.Instance != null)
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
            GameManager.Instance.SetVariable("BoxOpened", true);

            UpdateImageState();
        }
    }
    // *******************************************************************************

    private void UpdateImageState()
    {
        bool boxOpened = (bool)GameManager.Instance.GetVariable("BoxOpened");

        foreach (GameObject closedBox in sideClosedBox)
            closedBox.SetActive(!boxOpened);

        foreach (GameObject openBox in sideOpenBox)
            openBox.SetActive(boxOpened);
    }

}