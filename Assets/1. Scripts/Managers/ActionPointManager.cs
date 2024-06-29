using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = System.Random;
using static Constants;

public class ActionPointManager : MonoBehaviour
{
    public static ActionPointManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    [Header("하트 애니메이터")]
    [SerializeField] private Animator[] heartAnimator;

    public int heartCount = 5; // 하루 하트 개수
    private int NowActionPoint;

    public bool isEnding = false;

    public int standardEndingActionPoint;

    private void Start()
    {
        // 이벤트 구독
        GameManager.OnActionPointChanged += UpdateNowHeartState;
    }

    private void OnDestroy()
    {
        // 이벤트 구독 해제
        GameManager.OnActionPointChanged -= UpdateNowHeartState;
    }

    // 이벤트 핸들러
    private void UpdateNowHeartState(int NowActionPoint)
    {
        this.NowActionPoint = NowActionPoint;
        heartCount = NowActionPoint % 5;
        
        //Debug.Log("현재 피로도 상태 : " + NowActionPoint);
        //Debug.Log("현재 하루 하트 개수 : " + heartCount);


        heartAnimator[heartCount].SetTrigger("Break");

        Invoke("HeartSetInvisible", 0.5f);

    }

    private void HeartSetInvisible()
    {
        heartAnimator[heartCount].gameObject.SetActive(false);
    }

    private void HeartSetAllVisible()
    {
        for (int i = 0; i < 5; i++)
        {
            heartAnimator[i].gameObject.SetActive(true);
        }
    }

    // DialogueManager의 EndDialogue() 메소드 마지막 줄에서 호출하도록 함
    public void HeartSetAllCharge()
    {
        if (heartCount == 0)
        {
            // 눈깜빡
            ScreenEffect.Instance.RestButtonEffect();

            // 하트 5개 다 채우고 보이게 함
            Invoke("HeartSetAllVisible", 2f);

            //Debug.Log(NowActionPoint);

            //if(NowActionPoint == 20)
            //{
            //    DialogueManager.Instance.StartDialogue("RoomEscapeS_004");
            //    return;
            //}
            //return;
        }
    }

    //private bool isFirstDayEventProceeding=false;
    //public bool isFirstDayAnswer = false;

    //public void RoomHeartCheck()
    //{
    //    // 첫날 넘어가는 상태
    //    if (NowActionPoint == 20&&!isFirstDayEventProceeding)
    //    {
    //        isFirstDayEventProceeding = true;
    //        // 스크립트 RoomEscapeS_001 ,RoomEscapeS_003 중 랜덤 출력
    //        Random random = new Random();

    //        // 1 또는 2를 랜덤으로 선택
    //        int randomNumber = random.Next(1, 3);

    //        switch (randomNumber)
    //        {
    //            case 1:
    //                // 스크립트 RoomEscapeS_001 출력
    //                DialogueManager.Instance.StartDialogue("RoomEscapeS_001");

    //                Debug.Log("스크립트 RoomEscapeS_001 출력");
    //                break;

    //            case 2:
    //                // 스크립트 RoomEscapeS_003 출력
    //                DialogueManager.Instance.StartDialogue("RoomEscapeS_003");

    //                Debug.Log("스크립트 RoomEscapeS_003 출력");
    //                break;
    //        }

    //    }

    //    if (!RoomManager.Instance.isInvestigating)
    //    {
    //        if (isFirstDayAnswer)
    //            return;
    //        HeartSetAllCharge();
    //    }

    //}

}
