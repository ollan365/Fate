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
    protected GameObject heartParent = UIManager.Instance.heartParent;
    protected TextMeshProUGUI dayText = UIManager.Instance.dayTextTextMeshProUGUI;

    [SerializeField] protected int maxDayNum;   // 방탈출에서 지내는 최대 일수
    [SerializeField] protected int nowDayNum;
    [SerializeField] protected int actionPointsPerDay;

    // 행동력 감소로 터질 하트 자리
    [SerializeField] protected int presentHeartIndex;

    // 설정한 actionPointsPerDay에 따라 달라지는 actionpoints배열
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

    // 귀가 스크립트 출력 부분
    public abstract void RefillHeartsOrEndDay();

    // 외출(아침) 스크립트 출력 부분
    public abstract void nextMorningDay();

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

    // 침대에서 휴식하면 행동력 강제로 다음날로 넘어감
    // Day1에 하트 4개 남아있어도 Day2로 넘어가고 actionPointsPerDay 만큼 채워짐
    public IEnumerator TakeRest()
    {
        float time = 3f;
        StartCoroutine(ScreenEffect.Instance.DayPass(time));   // fade in/out effect

        // 휴식 대사 출력. 
        StartCoroutine(DialogueManager.Instance.StartDialogue("RoomEscape_035", time));

        yield return new WaitForSeconds(time / 2);

        // 휴식하면 그날 하루에 남아있는 행동력 다 사용되기에 현재 있는 하트들 삭제
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
            // 마지막 날인 5일에 휴식했을 경우
            // 행동력이 0이 된 상태
            actionPoint = 0;
        }

        GameManager.Instance.SetVariable("ActionPoint", actionPoint);

        SaveManager.Instance.SaveGameData();
    }

    protected void Warning()
    {
        int actionPoint = (int)GameManager.Instance.GetVariable("ActionPoint");
        if (actionPoint <= 0) // actionPoint가 0보다 클 때만 코루틴 실행
            return;
        
        StartCoroutine(UIManager.Instance.WarningCoroutine());
    }

}
