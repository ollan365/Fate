using UnityEngine;

public class Box : EventObject, IResultExecutable
{
    [SerializeField]
    private GameObject photos; // 상자 속 사진들

    private void Awake()
    {
        ResultManager.Instance.RegisterExecutable("Box", this);
    }

    public new void OnMouseDown()
    {
        base.OnMouseDown();
        GameManager.Instance.IncrementVariable("BoxClick");
    }

    public void ExecuteAction()
    {
        ShowPhotos();
    }

    private void ShowPhotos()  // 열쇠로 열면 상자 속 전단지 발견
    {
        photos.SetActive(true);
        RoomManager.Instance.AddScreenObjects(photos);
        RoomManager.Instance.isInvestigating = true;
        GameManager.Instance.SetVariable("BoxOpened", true);
    }

}