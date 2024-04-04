using UnityEngine;
using static Constants;

public class FollowObject : EventObject
{
    [SerializeField] private FirstFollowObject objectName;
    public new void OnMouseDown()
    {
        if (!FollowManager.Instance.CanClick) return; // 상호작용 할 수 없는 상태면 리턴
        FollowManager.Instance.ClickObject();

        eventId = objectName.EventID();
        base.OnMouseDown();
        GameManager.Instance.IncrementVariable(objectName.ClickVariable());
    }
}
