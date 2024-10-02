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
    public GameObject heartParent;
    public TextMeshProUGUI dayText;

    [SerializeField] protected int maxDayNum;   // 방탈출에서 지내는 최대 일수
    [SerializeField] protected int nowDayNum;
    [SerializeField] protected int actionPointsPerDay;

    // 행동력 감소로 터질 하트 자리
    [SerializeField] protected int presentHeartIndex;

    // 설정한 actionPointsPerDay에 따라 달라지는 actionpoints배열
    protected int[,] actionPointsArray;

    [SerializeField] protected Q_Vignette_Single WarningVignette;

    [SerializeField] protected float warningTime;

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

    protected IEnumerator Warning()
    {
        int actionPoint = (int)GameManager.Instance.GetVariable("ActionPoint");
        // actionPoint가 0보다 클 때만 코루틴 실행
        if (actionPoint <= 0)
        {
            yield break; // actionPoint가 0 이하면 코루틴 종료
        }

        float start = 0, end = 1;
        float fadeInTime = 0.5f;  // 경고 표시 페이드 인 시간
        float fadeOutTime = 0.5f;  // 경고 종료 페이드 아웃 시간
        float current = 0, percent = 0;

        // 경고 시작: WarningVignette.mainColor.a를 start에서 end로 페이드 인
        while (percent < 1 && fadeInTime != 0)
        {
            current += Time.deltaTime;
            percent = current / fadeInTime;

            // vignette의 투명도(alpha)를 0에서 1로 선형 보간(Lerp)
            WarningVignette.mainColor.a = Mathf.Lerp(start, end, percent);

            yield return null;
        }

        // warningTime 동안 경고 상태 유지
        yield return new WaitForSeconds(warningTime);

        // 경고 종료: WarningVignette.mainColor.a를 다시 0으로 페이드 아웃
        current = 0;
        percent = 0;

        while (percent < 1 && fadeOutTime != 0)
        {
            current += Time.deltaTime * 2;  // 페이드 아웃 속도를 더 빠르게 설정
            percent = current / fadeOutTime;

            // WarningVignette 투명도를 1에서 0으로 선형 보간(Lerp)
            WarningVignette.mainColor.a = Mathf.Lerp(end, start, percent);

            yield return null;
        }

        // 코루틴 종료 시 WarningVignette.mainColor.a를 0으로 설정하여 경고 완전히 숨기기
        WarningVignette.mainColor.a = 0;
    }


}
