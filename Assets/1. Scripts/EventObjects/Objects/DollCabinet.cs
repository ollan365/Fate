using UnityEngine;

public class DollCabinet : EventObject
{
    public void ClickBtn()
    {
        //if ((int)GameManager.Instance.GetVariable("DollCabinetClick") != 0)
        //    Debug.Log("�̹� Ȯ���� �ܼ���.");

        // Ŭ�� Ƚ�� ����
        GameManager.Instance.IncrementObjectClick("DollCabinetClick");
    }
}