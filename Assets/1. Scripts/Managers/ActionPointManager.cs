using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

abstract public class ActionPointManager : MonoBehaviour
{
    public GameObject heartPrefab;
    protected GameObject heartParent;
    protected TextMeshProUGUI dayText;

    [SerializeField] protected int maxDayNum;
    [SerializeField] protected int nowDayNum;
    [SerializeField] protected int actionPointsPerDay;

    [SerializeField] protected int presentHeartIndex;

    protected int[,] actionPointsArray;

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

    public abstract void RefillHeartsOrEndDay();

    public abstract void nextMorningDay();

    public void Awake()
    {
        heartParent = UIManager.Instance.heartParent;
        dayText = UIManager.Instance.dayTextTextMeshProUGUI;
    }

    protected static IEnumerator DeactivateHeart(Object heart)
    {
        GameManager.Instance.SetVariable("IsHeartPopping", true);
        UIManager.Instance.SetCursorMode(false);
        yield return new WaitForSeconds(0.5f);
        Destroy(heart);
        UIManager.Instance.SetCursorMode(true);
        GameManager.Instance.SetVariable("IsHeartPopping", false);
    }

    protected IEnumerator RefillHearts(float totalTime)
    {
        yield return new WaitForSeconds(totalTime);
        CreateHearts();
        RoomManager.Instance.ExitToRoot();
    }

    public IEnumerator TakeRest()
    {
        float time = 3f;
        StartCoroutine(ScreenEffect.Instance.DayPass(time));   // fade in/out effect

        StartCoroutine(DialogueManager.Instance.StartDialogue("RoomEscape_035", time));

        yield return new WaitForSeconds(time / 2);

        foreach (Transform child in heartParent.transform)
            Destroy(child.gameObject);

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
            actionPoint = 0;

        GameManager.Instance.SetVariable("ActionPoint", actionPoint);
        SaveManager.Instance.SaveGameData();
    }

    protected void Warning()
    {
        int actionPoint = (int)GameManager.Instance.GetVariable("ActionPoint");
        if (actionPoint <= 0)
            return;
        
        StartCoroutine(UIManager.Instance.WarningCoroutine());
    }
}
