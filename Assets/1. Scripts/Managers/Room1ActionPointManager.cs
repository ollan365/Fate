using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room1ActionPointManager : ActionPointManager
{
    private new void Awake()
    {
        base.Awake();
        
        maxDayNum = (int)GameManager.Instance.GetVariable("MaxDayNum");
        nowDayNum = (int)GameManager.Instance.GetVariable("NowDayNum");
        actionPointsPerDay = (int)GameManager.Instance.GetVariable("ActionPointsPerDay");
        presentHeartIndex = (int)GameManager.Instance.GetVariable("PresentHeartIndex");

        CreateActionPointsArray(actionPointsPerDay);

        // ??? ??????? actionPoint
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

        // ????? 0?? ???
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
        // ?©£? ????? ???????? ??????? ?? ????? ???? ???? ???? ??? ??
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

        // ????? ?? ????????
        if (presentHeartIndex == -1)
        {
            if (nowDayNum < maxDayNum)
            {
                // ???? ????? ???????? ???????
                nowDayNum += 1;
                GameManager.Instance.SetVariable("NowDayNum", nowDayNum);

                // presentHeartIndex?? ?? ?? row?? ???????
                presentHeartIndex = (int)GameManager.Instance.GetVariable("ActionPointsPerDay") - 1;
                GameManager.Instance.SetVariable("PresentHeartIndex", presentHeartIndex);

                actionPoint = actionPointsArray[nowDayNum - 1, presentHeartIndex];
            }
            else
            {
                // ?????? ??
                // ?????? 0?? ?? ????
                actionPoint = 0;
            }
        }
        else
        {
            // actionPoint ?????????? GameManager?? ActionPoint?? ????
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
        // ??? ?????? ???
        EventManager.Instance.CallEvent("EventRoom1HomeComing");

        GameManager.Instance.SetVariable("RefillHeartsOrEndDay", false);
        // ??? ?????? ???? ?????? Next?? Event_NextMorningDay fade in/out ????? ????
    }

    // ¿ÜÃâ(¾ÆÄ§) ½ºÅ©¸³Æ® Ãâ·Â ºÎºÐ
    public override IEnumerator nextMorningDay()
    {
        RoomManager.Instance.SetIsInvestigating(true);
        UIManager.Instance.SetUI("MemoGauge", false);
        UIManager.Instance.SetUI("MemoButton", false);
        UIManager.Instance.SetUI("LeftButton", false);
        UIManager.Instance.SetUI("RightButton", false);

        // ´ÙÀ½³¯ÀÌ µÇ°í(fade in/out effect ½ÇÇà) ¾ÆÄ§ ½ºÅ©¸³Æ® Ãâ·Â
        //const float totalTime = 3f;
        //StartCoroutine(ScreenEffect.Instance.DayPass(totalTime));  // fade in/out effect

        const float totalTime = 5f;

        StartCoroutine(StartNextDayUIChange(nowDayNum));

        // ¾ÆÄ§ ½ºÅ©¸³Æ® Ãâ·Â
        yield return new WaitForSeconds(totalTime);
        EventManager.Instance.CallEvent("EventRoom1Morning");

        yield return new WaitWhile(() => isDayChanging);

        StartCoroutine(RefillHearts(0f));

        // ¿©±â¼­ ÇÏÆ® »ý¼º ¹× ´ÙÀ½³¯·Î ³¯Â¥ ¾÷µ¥ÀÌÆ®
        RoomManager.Instance.SetIsInvestigating(false);
    }

}
