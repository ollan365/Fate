using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

//public enum RoomType
//{
//    Room1,
//    Room2
//}

abstract public class ActionPointManager : MonoBehaviour
{
    // ************************* temporary members for action points *************************
    public GameObject heartPrefab;
    protected GameObject heartParent;
    protected TextMeshProUGUI dayText;

    [SerializeField] protected int maxDayNum;   // ¹æÅ»Ãâ¿¡¼­ Áö³»´Â ÃÖ´ë ÀÏ¼ö
    [SerializeField] protected int nowDayNum;
    [SerializeField] protected int actionPointsPerDay;

    // Çàµ¿·Â °¨¼Ò·Î ÅÍÁú ÇÏÆ® ÀÚ¸®
    [SerializeField] protected int presentHeartIndex;

    // ¼³Á¤ÇÑ actionPointsPerDay¿¡ µû¶ó ´Þ¶óÁö´Â actionpoints¹è¿­
    protected int[,] actionPointsArray;

    // ************************* temporary methods for action points *************************
    // create actionPointsArray
    protected void CreateActionPointsArray(int actionPointsPerDay)
    {
        actionPointsArray = new int[maxDayNum, actionPointsPerDay];

        int actionPoint = 1;
        for (int day = maxDayNum - 1; day >= 0; day--)
        {
            for (int index = 0; index < actionPointsPerDay; index++)
            {
                actionPointsArray[day, index] = actionPoint;
                actionPoint++;
            }
        }
    }

    // create 5 hearts on screen on room start
    public abstract void CreateHearts();

    public abstract void DecrementActionPoint();

    // ±Í°¡ ½ºÅ©¸³Æ® Ãâ·Â ºÎºÐ
    public abstract void RefillHeartsOrEndDay();

    // ¿ÜÃâ(¾ÆÄ§) ½ºÅ©¸³Æ® Ãâ·Â ºÎºÐ
    public abstract void nextMorningDay();

    public void Awake()
    {
        heartParent = UIManager.Instance.heartParent;
        dayText = UIManager.Instance.dayTextTextMeshProUGUI;
    }

    protected static IEnumerator DeactivateHeart(Object heart)
    {
        yield return new WaitForSeconds(0.5f);
        Destroy(heart);
    }

    protected IEnumerator RefillHearts(float totalTime)
    {
        yield return new WaitForSeconds(totalTime);
        CreateHearts();
        // turn off all ImageAndLockPanel objects and zoom out
        RoomManager.Instance.ExitToRoot();
    }

    // Ä§´ë¿¡¼­ ÈÞ½ÄÇÏ¸é Çàµ¿·Â °­Á¦·Î ´ÙÀ½³¯·Î ³Ñ¾î°¨
    // Day1¿¡ ÇÏÆ® 4°³ ³²¾ÆÀÖ¾îµµ Day2·Î ³Ñ¾î°¡°í actionPointsPerDay ¸¸Å­ Ã¤¿öÁü
    public IEnumerator TakeRest()
    {
        float time = 3f;
        StartCoroutine(ScreenEffect.Instance.DayPass(time));   // fade in/out effect

        // ÈÞ½Ä ´ë»ç Ãâ·Â. 
        StartCoroutine(DialogueManager.Instance.StartDialogue("RoomEscape_035", time));

        yield return new WaitForSeconds(time / 2);

        // ÈÞ½ÄÇÏ¸é ±×³¯ ÇÏ·ç¿¡ ³²¾ÆÀÖ´Â Çàµ¿·Â ´Ù »ç¿ëµÇ±â¿¡ ÇöÀç ÀÖ´Â ÇÏÆ®µé »èÁ¦
        foreach (Transform child in heartParent.transform)
        {
            Destroy(child.gameObject);
        }

        int actionPoint;

        if (nowDayNum < maxDayNum) 
        {
            nowDayNum += 1;
            presentHeartIndex = (int)GameManager.Instance.GetVariable("ActionPointsPerDay") - 1;

            GameManager.Instance.SetVariable("NowDayNum", nowDayNum);
            GameManager.Instance.SetVariable("PresentHeartIndex", presentHeartIndex);

            actionPoint = actionPointsArray[nowDayNum - 1, presentHeartIndex];

        }
        else
        {
            // ¸¶Áö¸· ³¯ÀÎ 5ÀÏ¿¡ ÈÞ½ÄÇßÀ» °æ¿ì
            // Çàµ¿·ÂÀÌ 0ÀÌ µÈ »óÅÂ
            actionPoint = 0;
        }

        GameManager.Instance.SetVariable("ActionPoint", actionPoint);

        SaveManager.Instance.SaveGameData();
    }

    protected void Warning()
    {
        int actionPoint = (int)GameManager.Instance.GetVariable("ActionPoint");
        if (actionPoint <= 0) // actionPoint°¡ 0º¸´Ù Å¬ ¶§¸¸ ÄÚ·çÆ¾ ½ÇÇà
            return;
        
        StartCoroutine(UIManager.Instance.WarningCoroutine());
    }
}
