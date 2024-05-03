using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ActionPointManager : MonoBehaviour
{
    // �ൿ�¿� ���� ���ϴ� ��¥ �ؽ�Ʈ
    [SerializeField] private TextMeshProUGUI NowDateText;

    // ��¥ 10.02, 03, 04, 05, 06 

    private void Start()
    {
        // �̺�Ʈ ����
        GameManager.OnActionPointChanged += UpdateNowDate;
    }

    private void OnDestroy()
    {
        // �̺�Ʈ ���� ����
        GameManager.OnActionPointChanged -= UpdateNowDate;
    }

    // �̺�Ʈ �ڵ鷯
    private void UpdateNowDate(int NowActionPoint)
    {
        if (NowActionPoint % 5 == 0)
        {
            NowDateText.text = (10.00 + (0.01 * (7 - NowActionPoint / 5))).ToString("F2");
        }
        else
            return;
    }
}
