using UnityEngine;

public class ZoomView : EventObject, IResultExecutable
{
    [SerializeField] private GameObject zoomedView;

    private void Awake()
    {
        string executableName = $"{name} {sideNum}";  // "{이 오브젝트의 이름} + {사이드 번호}"로 executable 등록
        ResultManager.Instance.RegisterExecutable(executableName, this);
    }

    public new void OnMouseDown()
    {
        base.OnMouseDown();
    }
    
    public void ExecuteAction()
    {
        //Debug.Log($"{name} {sideNum}" + " 줌 켜기");
        if (!isActiveAndEnabled) return;
        
        RoomManager.Instance.SetCurrentView(zoomedView);
        RoomManager.Instance.SetIsZoomed(true);
        RoomManager.Instance.SetButtons();
    }
}