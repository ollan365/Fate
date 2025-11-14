using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room2ActionPointManager : ActionPointManager
{
    // 회복제 먹었을 경우
    [SerializeField] private bool isEatenEnergySupplement;

    private bool isChoosingBrokenBearChoice = false;

    private const int INITIAL_DAY_NUM = 1;

    protected override void Awake()
    {
        base.Awake();
        
        maxDayNum = (int)GameManager.Instance.GetVariable("MaxDayNum");
        // 방1 지난 후에는 NowDayNum이 5로 되어 있기에 1로 초기화
        nowDayNum = INITIAL_DAY_NUM;
        GameManager.Instance.SetVariable("NowDayNum",nowDayNum);
        nowDayNum = (int)GameManager.Instance.GetVariable("NowDayNum");
        actionPointsPerDay = (int)GameManager.Instance.GetVariable("ActionPointsPerDay");
        presentHeartIndex = (int)GameManager.Instance.GetVariable("PresentHeartIndex");
        isEatenEnergySupplement = (bool)GameManager.Instance.GetVariable("IsEatenEnergySupplement");

        CreateActionPointsArray(actionPointsPerDay);

        // 처음 방탈출의 actionPoint
        GameManager.Instance.SetVariable("ActionPoint", actionPointsArray[0, presentHeartIndex]);

        GameManager.Instance.AddEventObject("EventRoom2HomeComing");
        GameManager.Instance.AddEventObject("EventRoom2Morning");
    }

    // create 5 hearts on screen on room start
    public override void CreateHearts()
    {
        int actionPoint = actionPointsArray[nowDayNum - 1, presentHeartIndex];
        // 25 action points -> 5 hearts, 24 action points -> 4 hearts, so on...
        int heartCount = presentHeartIndex + 1;

        // 회복제 먹어서 actionPoint가 2개 더 늘어남
        if (isEatenEnergySupplement)
        {
            // 하트 수가 actionPointsPerDay일 때 먹은 것이면 DecrementActionPoint에서 하트가 0이 되면 
            // 다음날과 최대 하트로 미리 업데이트 해두기에 하트가 0일 때 먹은 것. 하트를 0개에서 2개로 만들어줌
            if (heartCount == actionPointsPerDay)
            {
                nowDayNum -= 1;
                GameManager.Instance.SetVariable("NowDayNum", nowDayNum);

                // presentHeartIndex도 2-1로 업데이트
                presentHeartIndex = 1;

                actionPoint = actionPointsArray[nowDayNum - 1, presentHeartIndex];
            }
            else
            {
                actionPointsPerDay = 7;
                GameManager.Instance.SetVariable("ActionPointsPerDay", actionPointsPerDay);
                // actionPointPerDay가 변경되어 다시 actionPointsArray 생성
                CreateActionPointsArray(actionPointsPerDay);

                presentHeartIndex += 2;
            }

            heartCount = presentHeartIndex + 1;

            // actionPoint도 2개 더 늘어난 상태로 수정
            actionPoint = actionPointsArray[nowDayNum - 1, presentHeartIndex];

            GameManager.Instance.SetVariable("ActionPoint", actionPoint);
            GameManager.Instance.SetVariable("PresentHeartIndex", presentHeartIndex);
        }
        // 하트가 0이 되면
        if (heartCount == 0)
        {
            if ((bool)GameManager.Instance.GetVariable("TeddyBearFixed"))
            {
                // 회복제 먹은 상태면 하루에 하트 최대 5개였던 것이 7개로 늘어남
                if (actionPointsPerDay == 5)
                {
                    actionPointsPerDay = 7;
                    GameManager.Instance.SetVariable("ActionPointsPerDay", actionPointsPerDay);
                }
            }
            heartCount = actionPointsPerDay;
        }

        for (int i = 0; i < heartCount; i++)
        {
            // create heart on screen by creating instances of heart prefab under heart parent
            Instantiate(heartPrefab, heartParent.transform);
        }

        // change Day text on screen
        dayText.text = $"Day {nowDayNum}";

        if (isEatenEnergySupplement)
            isEatenEnergySupplement = false;

        //Debug.Log(heartParent.transform.childCount);
    }

    public override void DecrementActionPoint()
    {
        if ((bool)GameManager.Instance.GetVariable("TeddyBearFixed"))
        {
            actionPointsPerDay = 7;
            GameManager.Instance.SetVariable("ActionPointsPerDay", actionPointsPerDay);
            // actionPointPerDay가 변경되어 다시 actionPointsArray 생성
            CreateActionPointsArray(actionPointsPerDay);
        }

        // 시계 퍼즐에서 연속으로 클릭했을 때 포인트 감소 오류 뜨지 않게 함
        if (heartParent.transform.childCount < 1)
            return;

        // pop heart on screen
        GameObject heart = heartParent.transform.GetChild(presentHeartIndex).gameObject;

        // animate heart by triggering "break" animation
        heart.GetComponent<Animator>().SetTrigger("Break");

        // deactivate heart after animation
        StartCoroutine(DeactivateHeart(heart));

        presentHeartIndex--;

        int actionPoint;

        // 하트가 다 없어지면
        if (presentHeartIndex == -1)
        {
            if (nowDayNum < maxDayNum)
            {
                // 현재 날짜를 다음날로 업데이트
                nowDayNum += 1;
                GameManager.Instance.SetVariable("NowDayNum", nowDayNum);

                // presentHeartIndex도 맨 끝 row로 업데이트
                presentHeartIndex = (int)GameManager.Instance.GetVariable("ActionPointsPerDay") - 1;
                GameManager.Instance.SetVariable("PresentHeartIndex", presentHeartIndex);

                actionPoint = actionPointsArray[nowDayNum - 1, presentHeartIndex];
            }
            else
            {
                // 마지막 날
                // 행동력이 0이 된 상태
                actionPoint = 0;
            }
        }
        else
        {
            // actionPoint 업데이트하고 GameManager의 ActionPoint도 수정
            actionPoint = actionPointsArray[nowDayNum - 1, presentHeartIndex];
        }

        GameManager.Instance.SetVariable("ActionPoint", actionPoint);
        GameManager.Instance.SetVariable("PresentHeartIndex", presentHeartIndex);

        Warning();

        if (actionPoint % actionPointsPerDay == 0)
        {
            bool isDialogueActive = DialogueManager.Instance.isDialogueActive;
            bool isInvestigating = RoomManager.Instance.GetIsInvestigating();
            if (!isDialogueActive && !isInvestigating) RefillHeartsOrEndDay();
            //else if (!isDialogueActive) RefillHeartsOrEndDay();
            else if (isInvestigating) GameManager.Instance.SetVariable("RefillHeartsOrEndDay", true);
            else if (isDialogueActive) GameManager.Instance.SetVariable("RefillHeartsOrEndDay", true);
        }

        SaveManager.Instance.SaveGameData();
    }


    // 귀가 스크립트 출력 부분
    public override void RefillHeartsOrEndDay()
    {
        // update Day text on screen
        if(nowDayNum!=maxDayNum)
            dayText.text = $"Day {nowDayNum - 1}";

        // turn off all ImageAndLockPanel objects and zoom out
        RoomManager.Instance.ExitToRoot();

        // if all action points are used, load "Follow 1" scene
        int actionPoint = (int)GameManager.Instance.GetVariable("ActionPoint");
        if (actionPoint == 0)
        {
            EventManager.Instance.CallEvent("EventEndRoom2");

            return;
        }

        // isHomeComingComplete 귀가스크립트 진행 상태 false로 변경
        GameManager.Instance.SetVariable("isHomeComingComplete", false);
        SaveManager.Instance.SaveGameData();

        // 귀가 스크립트 출력
        EventManager.Instance.CallEvent("EventRoom2HomeComing");
        GameManager.Instance.SetVariable("RefillHeartsOrEndDay", false);
    }

    // 외출(아침) 스크립트 출력 부분
    public override IEnumerator nextMorningDay()
    {
        RoomManager.Instance.SetIsInvestigating(true);
        UIManager.Instance.SetUI(eUIGameObjectName.MemoGauge, false);
        UIManager.Instance.SetUI(eUIGameObjectName.MemoButton, false);
        UIManager.Instance.SetUI(eUIGameObjectName.LeftButton, false);
        UIManager.Instance.SetUI(eUIGameObjectName.RightButton, false);

        // 다음날이 되고(fade in/out effect 실행) 아침 스크립트 출력
        //const float totalTime = 3f;
        //StartCoroutine(ScreenEffect.Instance.DayPass(totalTime));  // fade in/out effect

        const float totalTime = 5f;

        StartCoroutine(StartNextDayUIChange(nowDayNum));

        // 아침 스크립트 출력
        yield return new WaitForSeconds(totalTime);
        EventManager.Instance.CallEvent("EventRoom2Morning");

        yield return new WaitWhile(() => isDayChanging);

        // 여기서 하트 생성 및 다음날로 날짜 업데이트
        StartCoroutine(RefillHearts(0f));

        RoomManager.Instance.SetIsInvestigating(false);
        UIManager.Instance.SetCursorAuto();
    }

    // 곰인형 속 기력 보충제 먹고 스크립트 끝나면 바로 하트 2개 회복됨.
    public void EatEnergySupplement()
    {
        // 현재 보이는 하트 삭제하고
        foreach (Transform child in heartParent.transform)
        {
            Destroy(child.gameObject);
        }

        // 현재 하트 개수에 2개 추가 후
        // 하트 다시 만들게 해서 하트가 2개 더 채워지게 함.

        isEatenEnergySupplement = true;

        CreateHearts();
    }

    public void SetChoosingBrokenBearChoice(bool isChoosing)
    {
        isChoosingBrokenBearChoice = isChoosing;
    }

    public bool GetChoosingBrokenBearChoice()
    {
        return isChoosingBrokenBearChoice;
    }
}
