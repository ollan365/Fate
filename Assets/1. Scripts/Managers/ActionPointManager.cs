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

    private void Ending(int NowActionPoint)
    {
        // 행동력을 모두 소모했을 시, 엔딩 시작
        if(NowActionPoint == 0)
        {
            switch (SceneManager.Instance.CurrentScene) // 현재 씬에 따라 엔딩 호출
            {
                case Constants.SceneType.ROOM_1: DialogueManager.Instance.StartDialogue("BadEndingA_ver1_01"); break;
            }
        }
    }
}
