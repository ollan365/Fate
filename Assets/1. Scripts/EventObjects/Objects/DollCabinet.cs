using UnityEngine;

public class DollCabinet : EventObject
{
    public void ClickBtn()
    {
        OnClick();

        //// ù��° Ŭ��
        //if ((int)GameManager.Instance.GetVariable("PillowClick") == 0)
        //{
        //    // ���� �����忡 ���� ����

        //    // �ൿ�� ����

        //}
        //else // �� ���� Ŭ��
        //{
        //    // ���� �Ϸ� ��ũ��Ʈ

        //}

        // Ŭ�� Ƚ�� ����
        GameManager.Instance.IncrementObjectClick("DollCabinetClick");

    }

}