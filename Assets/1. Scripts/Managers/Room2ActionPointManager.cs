using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room2ActionPointManager : ActionPointManager
{
    // È¸º¹Á¦ ¸Ô¾úÀ» °æ¿ì
    [SerializeField] private bool isEatenEnergySupplement;

    //// ¹æÅ»Ãâ2¿¡¼­ È¸º¹Á¦ ¸ÔÀº »óÅÂ¸é ¹è°æÁöµµ extended·Î ¹Ù²Þ
    //[SerializeField] private GameObject backgroundImageDefault;
    //[SerializeField] private GameObject backgroundImageExtended;

    private bool isChoosingBrokenBearChoice = false;

    private new void Awake()
    {
        base.Awake();
        
        maxDayNum = (int)GameManager.Instance.GetVariable("MaxDayNum");
        nowDayNum = (int)GameManager.Instance.GetVariable("NowDayNum");
        actionPointsPerDay = (int)GameManager.Instance.GetVariable("ActionPointsPerDay");
        presentHeartIndex = (int)GameManager.Instance.GetVariable("PresentHeartIndex");
        isEatenEnergySupplement = (bool)GameManager.Instance.GetVariable("IsEatenEnergySupplement");

        CreateActionPointsArray(actionPointsPerDay);

        // Ã³À½ ¹æÅ»ÃâÀÇ actionPoint
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

        // È¸º¹Á¦ ¸Ô¾î¼­ actionPoint°¡ 2°³ ´õ ´Ã¾î³²
        if (isEatenEnergySupplement)
        {
            // ÇÏÆ® ¼ö°¡ actionPointsPerDayÀÏ ¶§ ¸ÔÀº °ÍÀÌ¸é DecrementActionPoint¿¡¼­ ÇÏÆ®°¡ 0ÀÌ µÇ¸é 
            // ´ÙÀ½³¯°ú ÃÖ´ë ÇÏÆ®·Î ¹Ì¸® ¾÷µ¥ÀÌÆ® ÇØµÎ±â¿¡ ÇÏÆ®°¡ 0ÀÏ ¶§ ¸ÔÀº °Í. ÇÏÆ®¸¦ 0°³¿¡¼­ 2°³·Î ¸¸µé¾îÁÜ
            if (heartCount == actionPointsPerDay)
            {
                nowDayNum -= 1;
                GameManager.Instance.SetVariable("NowDayNum", nowDayNum);

                // presentHeartIndexµµ 2-1·Î ¾÷µ¥ÀÌÆ®
                presentHeartIndex = 1;

                actionPoint = actionPointsArray[nowDayNum - 1, presentHeartIndex];
            }
            else
            {
                actionPointsPerDay = 7;
                GameManager.Instance.SetVariable("ActionPointsPerDay", actionPointsPerDay);
                // actionPointPerDay°¡ º¯°æµÇ¾î ´Ù½Ã actionPointsArray »ý¼º
                CreateActionPointsArray(actionPointsPerDay);

                presentHeartIndex += 2;
            }

            heartCount = presentHeartIndex + 1;

            // actionPointµµ 2°³ ´õ ´Ã¾î³­ »óÅÂ·Î ¼öÁ¤
            actionPoint = actionPointsArray[nowDayNum - 1, presentHeartIndex];

            GameManager.Instance.SetVariable("ActionPoint", actionPoint);
            GameManager.Instance.SetVariable("PresentHeartIndex", presentHeartIndex);
        }
        // ÇÏÆ®°¡ 0ÀÌ µÇ¸é
        if (heartCount == 0)
        {
            if ((bool)GameManager.Instance.GetVariable("TeddyBearFixed"))
            {
                // È¸º¹Á¦ ¸ÔÀº »óÅÂ¸é ÇÏ·ç¿¡ ÇÏÆ® ÃÖ´ë 5°³¿´´ø °ÍÀÌ 7°³·Î ´Ã¾î³²
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

        //// ÇÏÆ® ¹è°æÁö ¹Ù²Þ
        //ChangeHeartBackgroundImageExtended((bool)GameManager.Instance.GetVariable("TeddyBearFixed"));

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
            // actionPointPerDay°¡ º¯°æµÇ¾î ´Ù½Ã actionPointsArray »ý¼º
            CreateActionPointsArray(actionPointsPerDay);
        }

        // ½Ã°è ÆÛÁñ¿¡¼­ ¿¬¼ÓÀ¸·Î Å¬¸¯ÇßÀ» ¶§ Æ÷ÀÎÆ® °¨¼Ò ¿À·ù ¶ßÁö ¾Ê°Ô ÇÔ
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

        // ÇÏÆ®°¡ ´Ù ¾ø¾îÁö¸é
        if (presentHeartIndex == -1)
        {
            if (nowDayNum < maxDayNum)
            {
                // ÇöÀç ³¯Â¥¸¦ ´ÙÀ½³¯·Î ¾÷µ¥ÀÌÆ®
                nowDayNum += 1;
                GameManager.Instance.SetVariable("NowDayNum", nowDayNum);

                // presentHeartIndexµµ ¸Ç ³¡ row·Î ¾÷µ¥ÀÌÆ®
                presentHeartIndex = (int)GameManager.Instance.GetVariable("ActionPointsPerDay") - 1;
                GameManager.Instance.SetVariable("PresentHeartIndex", presentHeartIndex);

                actionPoint = actionPointsArray[nowDayNum - 1, presentHeartIndex];
            }
            else
            {
                // ¸¶Áö¸· ³¯
                // Çàµ¿·ÂÀÌ 0ÀÌ µÈ »óÅÂ
                actionPoint = 0;
            }
        }
        else
        {
            // actionPoint ¾÷µ¥ÀÌÆ®ÇÏ°í GameManagerÀÇ ActionPointµµ ¼öÁ¤
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


    // ±Í°¡ ½ºÅ©¸³Æ® Ãâ·Â ºÎºÐ
    public override void RefillHeartsOrEndDay()
    {
        // turn off all ImageAndLockPanel objects and zoom out
        RoomManager.Instance.ExitToRoot();

        // if all action points are used, load "Follow 1" scene
        int actionPoint = (int)GameManager.Instance.GetVariable("ActionPoint");
        if (actionPoint == 0)
        {
            EventManager.Instance.CallEvent("EventEndRoom2");

            return;
        }
        // ±Í°¡ ½ºÅ©¸³Æ® Ãâ·Â
        EventManager.Instance.CallEvent("EventRoom2HomeComing");

        GameManager.Instance.SetVariable("RefillHeartsOrEndDay", false);
        // ±Í°¡ ½ºÅ©¸³Æ® ÀÌÈÄ ³¡³ª¸é NextÀÇ Event_NextMorningDay fade in/out ÀÌÆåÆ® ³ª¿È
    }

    public override IEnumerator nextMorningDay()
    {
        RoomManager.Instance.SetIsInvestigating(true);
        UIManager.Instance.SetUI(eUIGameObjectName.MemoGauge, false);
        UIManager.Instance.SetUI(eUIGameObjectName.MemoButton, false);
        UIManager.Instance.SetUI(eUIGameObjectName.LeftButton, false);
        UIManager.Instance.SetUI(eUIGameObjectName.RightButton, false);

        // ´ÙÀ½³¯ÀÌ µÇ°í(fade in/out effect ½ÇÇà) ¾ÆÄ§ ½ºÅ©¸³Æ® Ãâ·Â
        //const float totalTime = 3f;
        //StartCoroutine(ScreenEffect.Instance.DayPass(totalTime));  // fade in/out effect

        const float totalTime = 5f;

        StartCoroutine(StartNextDayUIChange(nowDayNum));

        // ¾ÆÄ§ ½ºÅ©¸³Æ® Ãâ·Â
        yield return new WaitForSeconds(totalTime);
        EventManager.Instance.CallEvent("EventRoom2Morning");

        yield return new WaitWhile(() => isDayChanging);

        StartCoroutine(RefillHearts(0f));

        // ¿©±â¼­ ÇÏÆ® »ý¼º ¹× ´ÙÀ½³¯·Î ³¯Â¥ ¾÷µ¥ÀÌÆ®
        RoomManager.Instance.SetIsInvestigating(false);
    }

    // °õÀÎÇü ¼Ó ±â·Â º¸ÃæÁ¦ ¸ÔÀ¸¸é ½ºÅ©¸³Æ® ³¡³ª¸é ¹Ù·Î ÇÏÆ® 2°³ È¸º¹µÊ.
    public void EatEnergySupplement()
    {
        // ÀÏ´Ü ÇöÀç º¸ÀÌ´Â ÇÏÆ® Áö¿ì°í
        foreach (Transform child in heartParent.transform)
        {
            Destroy(child.gameObject);
        }

        // ÇÏÆ® +2°³ Ãß°¡ÇÏ°í
        // ÇÏÆ® ´Ù½Ã ¸¸µé°Ô ÇØ¼­ ÇÏÆ®°¡ 2°³ ´õ Ã¤¿öÁø °ÍÃ³·³ º¸ÀÌ°Ô ÇÔ.

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
