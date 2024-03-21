
using UnityEngine;

public class Chair : EventObject, IResultExecutable
{
    private Vector3 originalPosition;
    private Vector3 movedPosition;
    private RectTransform buttonRectTransform;
    
    private void Start()
    {
        ResultManager.Instance.RegisterExecutable("Chair", this);
        
        buttonRectTransform = GetComponent<RectTransform>();

        movedPosition = originalPosition = buttonRectTransform.anchoredPosition;
        movedPosition.x = -125f;
    }
    
    public new void OnMouseDown()
    {
        base.OnMouseDown();
        GameManager.Instance.InverseVariable("ChairMoved");
    }
    
    public void ExecuteAction()
    {
        MoveChair();
    }
    
    public void MoveChair()
    {
        if ((bool)GameManager.Instance.GetVariable("ChairMoved"))  // 의자가 이동한 상태인 경우
        {
            buttonRectTransform.anchoredPosition = originalPosition;
        }
        else  // 의자가 이동하지 않은 상태인 경우
        {
            buttonRectTransform.anchoredPosition = movedPosition;
        }
    }

}