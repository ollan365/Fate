using UnityEngine;
using static Constants;

public class FollowObject : EventObject
{
    [SerializeField] private FirstFollowObject objectName;
    public new void OnMouseDown()
    {
        if (!FollowManager.Instance.CanClick) return; // ��ȣ�ۿ� �� �� ���� ���¸� ����
        FollowManager.Instance.ClickObject();

        eventId = objectName.EventID();
        base.OnMouseDown();
        GameManager.Instance.IncrementVariable(objectName.ClickVariable());
    }
}
