using UnityEngine;
using UnityEngine.UI;
using static Constants;

public class FollowObject : EventObject
{
    [SerializeField] private FollowObjectName objectName;
    [SerializeField] private FollowExtra extraName;
    [SerializeField] private bool isSpecial;
    [SerializeField] private Sprite specialSprite;
    [SerializeField] private float scaleValue;
    public new void OnMouseDown()
    {
        if (!FollowManager.Instance.canClick) return; // 상호작용 할 수 없는 상태면 리턴
        FollowManager.Instance.ClickObject();

        if (isSpecial) OnMouseDown_Special();
        else OnMouseDown_Normal();
    }
    private void OnMouseDown_Special()
    {
        // isSpecial = false; // 이후에 클릭할 시에는 바로 OnMouseDown_Normal()가 호출되도록 한다
        
        SoundPlayer.Instance.UISoundPlay(Sound_FollowSpecialObject);

        if (objectName == FollowObjectName.Extra)
        {
            FollowManager.Instance.ClickExtra(extraName);
            OnMouseDown_Normal();
            return;
        }

        foreach (Transform child in FollowManager.Instance.blockingPanel.transform)
            Destroy(child.gameObject);

        var eventButton = Instantiate(FollowManager.Instance.eventButtonPrefab, FollowManager.Instance.blockingPanel.transform).GetComponent<Button>();

        // 자식의 이미지 변경 -> 나중에 변경 필요...
        eventButton.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = specialSprite;
        eventButton.transform.GetChild(0).GetChild(0).GetComponent<Image>().SetNativeSize();
        eventButton.transform.GetChild(0).GetChild(0).GetComponent<RectTransform>().localScale = new(scaleValue, scaleValue, scaleValue);

        eventButton.onClick.AddListener(() => OnMouseDown_Normal());
        eventButton.onClick.AddListener(() => eventButton.gameObject.SetActive(false));
    }
    public void OnMouseDown_Normal()
    {
        if (objectName != FollowObjectName.Extra)
        {
            eventId = objectName.EventID();
            GameManager.Instance.IncrementVariable(objectName.ClickVariable());
        }
        else
        {
            eventId = extraName.EventID();
            GameManager.Instance.IncrementVariable(extraName.ClickVariable());
        }
        base.OnMouseDown();
    }
}
