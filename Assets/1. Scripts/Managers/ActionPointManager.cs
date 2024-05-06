using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ActionPointManager : MonoBehaviour
{
    // 행동력에 따라 변하는 날짜 텍스트
    [SerializeField] private TextMeshProUGUI NowDateText;

    // 날짜 10.02, 03, 04, 05, 06 

    private void Start()
    {
        // 이벤트 구독
        GameManager.OnActionPointChanged += UpdateNowDate;
    }

    private void OnDestroy()
    {
        // 이벤트 구독 해제
        GameManager.OnActionPointChanged -= UpdateNowDate;
    }

    // 이벤트 핸들러
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
