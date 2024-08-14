using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Random = Unity.Mathematics.Random;

public class ActionPointManager : MonoBehaviour
{
    // ************************* temporary members for action points *************************
    public GameObject heartPrefab;
    public GameObject heartParent;
    public TextMeshProUGUI dayText;

    [SerializeField] private int maxDayNum;   // 방탈출에서 지내는 최대 일수
    [SerializeField] private int nowDayNum;
    [SerializeField] private int actionPointsPerDay;

    // 행동력 감소로 터질 하트 자리
    [SerializeField] private int presentHeartIndex;
    // 회복제 먹었을 경우
    [SerializeField] private bool isEatenEnergySupplement;

    // 설정한 actionPointsPerDay에 따라 달라지는 actionpoints배열
    private int[,] actionPointsArray;

    // 방탈출2에서 회복제 먹은 상태면 배경지도 extended로 바꿈
    [SerializeField] private GameObject backgroundImageDefault;
    [SerializeField] private GameObject backgroundImageExtended;

    private void Awake()
    {
        maxDayNum = (int)GameManager.Instance.GetVariable("MaxDayNum");
        nowDayNum = (int)GameManager.Instance.GetVariable("NowDayNum");
        actionPointsPerDay = (int)GameManager.Instance.GetVariable("ActionPointsPerDay");
        presentHeartIndex = (int)GameManager.Instance.GetVariable("PresentHeartIndex");
        isEatenEnergySupplement = (bool)GameManager.Instance.GetVariable("IsEatenEnergySupplement");

        CreateActionPointsArray(actionPointsPerDay);

        // 처음 방탈출의 actionPoint
        GameManager.Instance.SetVariable("ActionPoint", actionPointsArray[0, presentHeartIndex]);
    }

    // ************************* temporary methods for action points *************************
    // create actionPointsArray
    private void CreateActionPointsArray(int actionPointsPerDay)
    {
        actionPointsArray = new int[5, actionPointsPerDay];

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
    public void CreateHearts()
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

        // 하트 배경지 바꿈
        ChangeHeartBackgroundImageExtended((bool)GameManager.Instance.GetVariable("TeddyBearFixed"));

        if (isEatenEnergySupplement)
            isEatenEnergySupplement = false;

        //Debug.Log(heartParent.transform.childCount);
    }

    public void DecrementActionPoint()
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
            if(nowDayNum < maxDayNum)
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

        if (actionPoint % actionPointsPerDay == 0)
        {
            bool isDialogueActive = DialogueManager.Instance.isDialogueActive;
            if (!isDialogueActive) RefillHeartsOrEndDay();
            else GameManager.Instance.SetVariable("RefillHeartsOrEndDay", true);
        }

        SaveManager.Instance.SaveGameData();
    }

    public void RefillHeartsOrEndDay()
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
        var randomHomeComing = new Random((uint)System.DateTime.Now.Ticks);  // choose a random dialogue ID

        var HomeComingDialogueID = "";
        switch (nowDayNum)
        {
            case 2:
                //랜덤대사 1일차 귀가
                string[] random1HomeComingDialogueIDs = { "RoomEscapeS_001", "RoomEscapeS_003" };
                HomeComingDialogueID = random1HomeComingDialogueIDs[randomHomeComing.NextInt(0, 2)];
                break;

            case 3:
                //특정대사 2일차 귀가
                HomeComingDialogueID = "RoomEscapeS_005";
                break;

            case 4:
                //특정대사 3일차 귀가
                HomeComingDialogueID = "RoomEscapeS_008";
                break;

            case 5:
                //랜덤대사 4일차 귀가
                string[] random2HomeComingDialogueIDs = { "RoomEscapeS_012", "RoomEscapeS_013" };
                HomeComingDialogueID = random2HomeComingDialogueIDs[randomHomeComing.NextInt(0, 2)];
                break;

            default:
                break;
        }
        DialogueManager.Instance.StartDialogue(HomeComingDialogueID);
        GameManager.Instance.SetVariable("RefillHeartsOrEndDay", false);
        // 귀가 스크립트 이후 끝나면 Next의 Event_NextMorningDay fade in/out 이펙트 나옴
    }

    public void nextMorningDay()
    {
        // 다음날이 되고(fade in/out effect 실행) 아침 스크립트 출력
        const float totalTime = 3f;
        StartCoroutine(ScreenEffect.Instance.DayPass(totalTime));  // fade in/out effect

        var MorningDialogueID = "";

        switch (nowDayNum)
        {
            case 2:
                //특정 대사
                MorningDialogueID = "RoomEscapeS_004";
                break;

            case 3: case 4:
                //특정 대사
                MorningDialogueID = "RoomEscapeS_015";
                break;

            case 5:
                //특정 대사
                MorningDialogueID = "RoomEscapeS_014";
                break;
        }
        StartCoroutine(DialogueManager.Instance.StartDialogue(MorningDialogueID, totalTime));

        // 여기서 하트 생성 및 다음날로 날짜 업데이트
        StartCoroutine(RefillHearts(totalTime / 2));
    }

    private static IEnumerator DeactivateHeart(Object heart)
    {
        yield return new WaitForSeconds(0.5f);
        Destroy(heart);
    }

    private IEnumerator RefillHearts(float totalTime)
    {
        yield return new WaitForSeconds(totalTime);
        CreateHearts();
        // turn off all ImageAndLockPanel objects and zoom out
        RoomManager.Instance.ExitToRoot();
    }

    // 곰인형 속 기력 보충제 먹으면 스크립트 끝나면 바로 하트 2개 회복됨.
    public void EatEnergySupplement()
    {
        // 일단 현재 보이는 하트 지우고
        foreach (Transform child in heartParent.transform)
        {
            Destroy(child.gameObject);
        }

        // 하트 +2개 추가하고
        // 하트 다시 만들게 해서 하트가 2개 더 채워진 것처럼 보이게 함.

        isEatenEnergySupplement = true;

        CreateHearts();

        // 하트 배경지 바꿈
        ChangeHeartBackgroundImageExtended(true);
    }

    private bool isChoosingBrokenBearChoice = false;
    public void SetChoosingBrokenBearChoice(bool isChoosing)
    {
        isChoosingBrokenBearChoice = isChoosing;
    }

    public bool GetChoosingBrokenBearChoice()
    {
        return isChoosingBrokenBearChoice;
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

        if (nowDayNum < 5) 
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

    // 회복제 먹은 상태라면 배경지를 바꿈
    private void ChangeHeartBackgroundImageExtended(bool isExtended)
    {
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "Room2")
        {
            if (isExtended)
            {
                if (backgroundImageExtended.activeSelf) return;
                else
                {
                    if ((presentHeartIndex + 1) > 5)
                    {
                        backgroundImageDefault.SetActive(false);
                        backgroundImageExtended.SetActive(true);
                    }
                }
            }
            else
            {
                backgroundImageDefault.SetActive(true);
                backgroundImageExtended.SetActive(false);
            }
        }
    }



    //public void showchildCount()
    //{
    //    Debug.Log(heartParent.transform.childCount);
    //}
}
