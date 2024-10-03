using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room1ActionPointManager : ActionPointManager
{
    private void Awake()
    {
        maxDayNum = (int)GameManager.Instance.GetVariable("MaxDayNum");
        nowDayNum = (int)GameManager.Instance.GetVariable("NowDayNum");
        actionPointsPerDay = (int)GameManager.Instance.GetVariable("ActionPointsPerDay");
        presentHeartIndex = (int)GameManager.Instance.GetVariable("PresentHeartIndex");

        CreateActionPointsArray(actionPointsPerDay);

        // 처음 방탈출의 actionPoint
        GameManager.Instance.SetVariable("ActionPoint", actionPointsArray[0, presentHeartIndex]);


        GameManager.Instance.AddEventObject("EventRoom1HomeComing");
        GameManager.Instance.AddEventObject("EventRoom1Morning");

    }

    // create 5 hearts on screen on room start
    public override void CreateHearts()
    {
        int actionPoint = actionPointsArray[nowDayNum - 1, presentHeartIndex];
        // 25 action points -> 5 hearts, 24 action points -> 4 hearts, so on...
        int heartCount = presentHeartIndex + 1;

        // 하트가 0이 되면
        if (heartCount == 0)
        {
            heartCount = actionPointsPerDay;
        }

        for (int i = 0; i < heartCount; i++)
        {
            // create heart on screen by creating instances of heart prefab under heart parent
            Instantiate(heartPrefab, heartParent.transform);
        }

        // change Day text on screen
        dayText.text = $"Day {nowDayNum}";

        //Debug.Log(heartParent.transform.childCount);
    }

    public override void DecrementActionPoint()
    {
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

        StartCoroutine(Warning());

        if (actionPoint % actionPointsPerDay == 0)
        {
            bool isDialogueActive = DialogueManager.Instance.isDialogueActive;
            if (!isDialogueActive) RefillHeartsOrEndDay();
            else GameManager.Instance.SetVariable("RefillHeartsOrEndDay", true);
        }

        SaveManager.Instance.SaveGameData();
    }

    public override void RefillHeartsOrEndDay()
    {
        // turn off all ImageAndLockPanel objects and zoom out
        RoomManager.Instance.ExitToRoot();

        // if all action points are used, load "Follow 1" scene
        int actionPoint = (int)GameManager.Instance.GetVariable("ActionPoint");
        if (actionPoint == 0)
        {
            SceneManager.Instance.LoadScene(Constants.SceneType.ENDING);

            return;
        }
        // 귀가 스크립트 출력
        EventManager.Instance.CallEvent("EventRoom1HomeComing");

        GameManager.Instance.SetVariable("RefillHeartsOrEndDay", false);
        // 귀가 스크립트 이후 끝나면 Next의 Event_NextMorningDay fade in/out 이펙트 나옴
    }

    // 외출(아침) 스크립트 출력 부분
    public override void nextMorningDay()
    {
        // 다음날이 되고(fade in/out effect 실행) 아침 스크립트 출력
        const float totalTime = 3f;
        StartCoroutine(ScreenEffect.Instance.DayPass(totalTime));  // fade in/out effect

        // 아침 스크립트 출력
        EventManager.Instance.CallEvent("EventRoom1Morning");

        // 여기서 하트 생성 및 다음날로 날짜 업데이트
        StartCoroutine(RefillHearts(totalTime / 2));
    }

}
