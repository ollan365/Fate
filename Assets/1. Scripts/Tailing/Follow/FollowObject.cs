using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using static Constants;

public class FollowObject : EventObject, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private FollowObjectName objectName;
    [SerializeField] private FollowExtra extraName;
    [SerializeField] private bool isSpecial;
    public Sprite specialSprite;
    public float scaleValue;
    public new void OnMouseDown()
    {
        if (!FollowManager.Instance.ClickObject()) return; // 상호작용 할 수 없는 상태면 리턴

        FollowManager.Instance.ClickCount++;

        if (isSpecial) OnMouseDown_Special();
        else OnMouseDown_Normal();
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        CursorManager.Instance.ChangeCursorInFollow();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        CursorManager.Instance.ChangeCursorInFollow(true);
    }

    private void OnMouseDown_Special()
    {
        isSpecial = false; // 이후에 클릭할 시에는 바로 OnMouseDown_Normal()가 호출되도록 한다

        SoundPlayer.Instance.UISoundPlay(Sound_FollowSpecialObject);

        if (objectName == FollowObjectName.Extra) { OnMouseDown_Normal(); return; }

        FollowManager.Instance.ClickSpecialObject(this);
    }
    public void OnMouseDown_Normal()
    {
        if (objectName != FollowObjectName.Extra) eventId = objectName.EventID();
        else eventId = extraName.EventID();

        base.OnMouseDown();
        
        if (objectName != FollowObjectName.Extra) GameManager.Instance.IncrementVariable(objectName.ClickVariable());
        else GameManager.Instance.IncrementVariable(extraName.ClickVariable());
    }
}
