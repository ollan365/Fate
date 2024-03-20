
using UnityEngine;

public class Chair : EventObject
{
    public void ClickBtn()
    {
        OnClick();

        // ���� ������ �����̱�
        if (buttonRectTransform.anchoredPosition.x != movePos.x)
        {
            // ������ ������
            buttonRectTransform.anchoredPosition = movePos;
            // �ൿ�� ����

            // Ŀ��Į ���� ����?

        }
        else
        {
            buttonRectTransform.anchoredPosition = originalPos;
        }

        // Ŭ�� Ƚ�� ����
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