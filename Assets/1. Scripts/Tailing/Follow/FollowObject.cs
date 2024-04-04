using UnityEngine;
using UnityEngine.UI;
using static Constants;

public class FollowObject : EventObject
{
    [SerializeField] private FirstFollowObject objectName;
    [SerializeField] private bool isSpecial;
    public new void OnMouseDown()
    {
        if (!FollowManager.Instance.CanClick) return; // 상호작용 할 수 없는 상태면 리턴
        FollowManager.Instance.ClickObject();

        if (isSpecial) OnMouseDown_Special();
        else OnMouseDown_Normal();
    }
    private void OnMouseDown_Special()
    {
        isSpecial = false; // 이후에 클릭할 시에는 바로 OnMouseDown_Normal()가 호출되도록 한다

        foreach (Transform child in FollowManager.Instance.blockingPanel.transform)
            Destroy(child.gameObject);

        var eventButton = Instantiate(FollowManager.Instance.eventButtonPrefab, FollowManager.Instance.blockingPanel.transform).GetComponent<Button>();
        eventButton.onClick.AddListener(() => OnMouseDown_Normal());
        eventButton.onClick.AddListener(() => eventButton.gameObject.SetActive(false));
    }
    public void OnMouseDown_Normal()
    {
        eventId = objectName.EventID();
        base.OnMouseDown();
        GameManager.Instance.IncrementVariable(objectName.ClickVariable());
    }
}
