using UnityEngine;

public class ZoomView : EventObject, IResultExecutable
{
    [SerializeField] private GameObject zoomedView;

    public new void OnMouseDown()
    {
        base.OnMouseDown();
    }
    
    private void Awake()
    {
        string executableName = $"{name} {sideNum}";  // "{이 오브젝트의 이름} + {사이드 번호}"로 executable 등록
        ResultManager.Instance.RegisterExecutable(executableName, this);
    }
    
    public void ExecuteAction()
    {
        if (isActiveAndEnabled) RoomManager.Instance.SetCurrentView(zoomedView);
    }
}