using UnityEngine;
using UnityEngine.UI;
using static Constants;

public class FollowObject : EventObject
{
    [SerializeField] private FirstFollowObject objectName;
    [SerializeField] private bool isSpecial;
    [SerializeField] private Sprite specialSprite;
    [SerializeField] private float scaleValue;
    public new void OnMouseDown()
    {
        if (!FollowManager.Instance.canClick) return; // ��ȣ�ۿ� �� �� ���� ���¸� ����
        FollowManager.Instance.ClickObject();

        if (isSpecial) OnMouseDown_Special();
        else OnMouseDown_Normal();
    }
    private void OnMouseDown_Special()
    {
        // isSpecial = false; // ���Ŀ� Ŭ���� �ÿ��� �ٷ� OnMouseDown_Normal()�� ȣ��ǵ��� �Ѵ�

        if(objectName == FirstFollowObject.Angry)
        {
            FollowManager.Instance.ClickAngry();
            OnMouseDown_Normal();
            return;
        }

        foreach (Transform child in FollowManager.Instance.blockingPanel.transform)
            Destroy(child.gameObject);

        var eventButton = Instantiate(FollowManager.Instance.eventButtonPrefab, FollowManager.Instance.blockingPanel.transform).GetComponent<Button>();

        // �ڽ��� �̹��� ���� -> ���߿� ���� �ʿ�...
        eventButton.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = specialSprite;
        eventButton.transform.GetChild(0).GetChild(0).GetComponent<Image>().SetNativeSize();
        eventButton.transform.GetChild(0).GetChild(0).GetComponent<RectTransform>().localScale = new(scaleValue, scaleValue, scaleValue);

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
