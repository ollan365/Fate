using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room1ActionPointManager : ActionPointManager
{
    protected override void Awake()
    {
        base.Awake();
        
        maxDayNum = (int)GameManager.Instance.GetVariable("MaxDayNum");
        nowDayNum = (int)GameManager.Instance.GetVariable("NowDayNum");
        actionPointsPerDay = (int)GameManager.Instance.GetVariable("ActionPointsPerDay");
        presentHeartIndex = (int)GameManager.Instance.GetVariable("PresentHeartIndex");

        CreateActionPointsArray(actionPointsPerDay);

        // The First Room Escape ActionPoint
        GameManager.Instance.SetVariable("ActionPoint", actionPointsArray[0, presentHeartIndex]);

        GameManager.Instance.AddEventObject("EventRoom1HomeComing");
        GameManager.Instance.AddEventObject("EventRoom1Morning");
    }

    // create 5 hearts on screen on room start
    public override void CreateHearts()
    {
        int heartCount = presentHeartIndex + 1;

        if (heartCount == 0)
            heartCount = actionPointsPerDay;

        // create heart on screen by creating instances of heart prefab under heart parent
        for (int i = 0; i < heartCount; i++) {
            GameObject heartGameObject = Instantiate(heartPrefab, heartParent.transform);
            UIManager.Instance.AddUIToCheck(heartGameObject.GetComponent<RectTransform>());
        }

        // change Day text on screen
        dayText.text = $"Day {nowDayNum}";
    }

    public override void DecrementActionPoint()
    {
        // Avoid ActionPoint Decrease errors when you click a puzzle in a row
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
        // If all the hearts are used
        if (presentHeartIndex == -1)
        {
            if (nowDayNum < maxDayNum)
            {
                // update current date to next day
                nowDayNum += 1;
                GameManager.Instance.SetVariable("NowDayNum", nowDayNum);
                // presentHeartIndex를 맨 끝 row로 업데이트
                presentHeartIndex = (int)GameManager.Instance.GetVariable("ActionPointsPerDay") - 1;
                GameManager.Instance.SetVariable("PresentHeartIndex", presentHeartIndex);
                actionPoint = actionPointsArray[nowDayNum - 1, presentHeartIndex];
            }
            else
                actionPoint = 0;
        }
        else
            actionPoint = actionPointsArray[nowDayNum - 1, presentHeartIndex];

        GameManager.Instance.SetVariable("ActionPoint", actionPoint);
        GameManager.Instance.SetVariable("PresentHeartIndex", presentHeartIndex);
        Warning();

        if (actionPoint % actionPointsPerDay == 0)
        {
            bool isDialogueActive = DialogueManager.Instance.isDialogueActive;
            bool isInvestigating = RoomManager.Instance.GetIsInvestigating();
            if (!isDialogueActive && !isInvestigating) 
                RefillHeartsOrEndDay();
            else if (isInvestigating) 
                GameManager.Instance.SetVariable("RefillHeartsOrEndDay", true);
            else
                GameManager.Instance.SetVariable("RefillHeartsOrEndDay", true);
        }
        SaveManager.Instance.SaveGameData();
    }

    public override void RefillHeartsOrEndDay()
    {
        // update Day text on screen
        dayText.text = $"Day {nowDayNum - 1}";

        // turn off all ImageAndLockPanel objects and zoom out
        RoomManager.Instance.ExitToRoot();

        // if all action points are used, load "Follow 1" scene
        int actionPoint = (int)GameManager.Instance.GetVariable("ActionPoint");
        if (actionPoint == 0)
        {
            GameSceneManager.Instance.LoadScene(Constants.SceneType.ENDING);
            return;
        }

        // isHomeComingComplete 귀가스크립트 진행 상태 false로 변경
        GameManager.Instance.SetVariable("isHomeComingComplete", false);
        SaveManager.Instance.SaveGameData();

        // 귀가 스크립트 출력
        EventManager.Instance.CallEvent("EventRoom1HomeComing");
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
        UIManager.Instance.SetCursorAuto();

        const float totalTime = 5f;
        StartCoroutine(StartNextDayUIChange(nowDayNum));

        // 아침 스크립트 출력
        yield return new WaitForSeconds(totalTime);
        EventManager.Instance.CallEvent("EventRoom1Morning");

        yield return new WaitWhile(() => isDayChanging);

        // 하트 생성, 다음날로 날짜 업데이트
        StartCoroutine(RefillHearts(0f));

        RoomManager.Instance.SetIsInvestigating(false);
        UIManager.Instance.SetCursorAuto();
    }
}
