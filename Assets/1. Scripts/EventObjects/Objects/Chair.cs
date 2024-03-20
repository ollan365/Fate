
using UnityEngine;

public class Chair : EventObject
{
    public void ClickBtn()
    {
        OnClick();

        // 의자 옆으로 움직이기
        if (buttonRectTransform.anchoredPosition.x != movePos.x)
        {
            // 옆으로 움직임
            buttonRectTransform.anchoredPosition = movePos;
            // 행동력 감소

            // 커터칼 조사 가능?

        }
        else
        {
            buttonRectTransform.anchoredPosition = originalPos;
        }

        // 클릭 횟수 증가
        GameManager.Instance.IncrementObjectClick("ChairClick");

    }

    private Vector3 originalPos;
    private Vector3 movePos;

    private RectTransform buttonRectTransform;

    private void Start()
    {
        buttonRectTransform = GetComponent<RectTransform>();

        movePos = originalPos = buttonRectTransform.anchoredPosition;
        movePos.x = -125f;
    }

}