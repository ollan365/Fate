using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

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

    [SerializeField] protected int maxDayNum;   // 방탈출에서 지내는 최대 일수
    [SerializeField] protected int nowDayNum;
    [SerializeField] protected int actionPointsPerDay;

    // 행동력 감소로 터질 하트 자리
    [SerializeField] protected int presentHeartIndex;

    // 설정한 actionPointsPerDay에 따라 달라지는 actionpoints배열
    protected int[,] actionPointsArray;

    // ************************* temporary members for Day Animation *************************
    [SerializeField] private float dayScalingTime = 2f;
    [SerializeField] private float dayAnimatingTime = 4f;

    public bool isDayAnimating = false;

    [SerializeField] private List<Vector3> DayAnimRotationValues;   // (50, 100, 90) (0, 0, 0)

    [SerializeField] private Vector3 DayAnimOriginalPosition;  // 기존 위치 (-832, 435.6, -100)
    [SerializeField] private Vector3 DayAnimMovedPosition;  // (0, 0, -100) 
    [SerializeField] private float DayAnimScaleOriginalValue = 0.385f;
    [SerializeField] private float DayAnimScaleZoomedValue = 1;

    public TextMeshProUGUI yesterDayNumText;
    public TextMeshProUGUI nowDayNumText;
    public RectTransform yesterDayRectTransform;
    public RectTransform DayAnimGroupRectTransform;

    [SerializeField] private GameObject GearGroup;
    [SerializeField] private GameObject MainGear;
    [SerializeField] private GameObject SubGear;
    [SerializeField] private GameObject GearHourHand;
    [SerializeField] private GameObject GearMinuteHand;

    private float elapsedTime = 0f;
    private bool isDayScaling = false;
    private bool hasChangedSibling = false;

    // ************************* temporary members for Day Gear Rotation and Alpha value *************************
    [SerializeField] private float mainGearSpeed = 90f;
    [SerializeField] private float subGearSpeed = 180f;

    [SerializeField] private float minuteRotationSpeed = 5f;
    [SerializeField] private float hourRotationSpeed = 15f;
    [SerializeField] private float clockSpeedMultiplier = 1.5f;

    public List<Image> GearImages;

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
    public abstract IEnumerator nextMorningDay();

    public void Awake()
    {
        heartParent = UIManager.Instance.heartParent;
        dayText = UIManager.Instance.dayTextTextMeshProUGUI;

        yesterDayNumText = UIManager.Instance.yesterDayNumTextTextMeshProUGUI;
        nowDayNumText = UIManager.Instance.nowDayNumTextTextMeshProUGUI;
        yesterDayRectTransform = UIManager.Instance.yesterDayRectTransform;
        DayAnimGroupRectTransform = UIManager.Instance.DayAnimGroupRectTransform;

        DayAnimRotationValues.Add(new Vector3(50, 100, 90));
        DayAnimRotationValues.Add(new Vector3(180, 180, 180));

        DayAnimOriginalPosition = DayAnimGroupRectTransform.anchoredPosition;  // 초기 위치 저장
        DayAnimMovedPosition = new Vector3(0, 0, -100);

        DayAnimScaleOriginalValue = 0.385f;
        DayAnimScaleZoomedValue = 1;

        GearGroup = UIManager.Instance.gearGroup;
        MainGear = UIManager.Instance.mainGear;
        SubGear = UIManager.Instance.subGear;
        GearHourHand = UIManager.Instance.gearHourHand;
        GearMinuteHand = UIManager.Instance.gearMinuteHand;

        GearImages.Add(MainGear.GetComponent<Image>());
        GearImages.Add(SubGear.GetComponent<Image>());
        GearImages.Add(GearHourHand.GetComponent<Image>());
        GearImages.Add(GearMinuteHand.GetComponent<Image>());
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

    // 침대에서 휴식하면 행동력 강제로 다음날로 넘어감
    // Day1에 하트 4개 남아있어도 Day2로 넘어가고 actionPointsPerDay 만큼 채워짐
    public IEnumerator TakeRest()
    {
        RoomManager.Instance.SetIsInvestigating(true);

        UIManager.Instance.SetUI("MemoButton", false);
        UIManager.Instance.SetUI("LeftButton", false);
        UIManager.Instance.SetUI("RightButton", false);

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

        RoomManager.Instance.SetIsInvestigating(false);
        SaveManager.Instance.SaveGameData();
    }

    protected void Warning()
    {
        int actionPoint = (int)GameManager.Instance.GetVariable("ActionPoint");
        if (actionPoint <= 0) // actionPoint가 0보다 클 때만 코루틴 실행
            return;
        
        StartCoroutine(UIManager.Instance.WarningCoroutine());
    }

    // ************************* temporary methods for day animation *************************

    // test
    public void StartNextDayUIAnimation(int nowDayNum)
    {
        StartCoroutine(NextDayUIAnimation(nowDayNum));
    }

    public void StartNextDayUIAnimationVer2(int nowDayNum)
    {
        StartCoroutine(NextDayUIAnimationVer2(nowDayNum));
    }

    protected IEnumerator NextDayUIAnimation(int nowDayNum)
    {
        if (isDayAnimating)
            yield break;
        //RoomManager.Instance.SetIsInvestigating(true);
        isDayAnimating = true;

        UIManager.Instance.SetUI("ActionPoints", false);

        UIManager.Instance.SetUI("DayAnimGameObject", true);
        UIManager.Instance.SetUI("YesterDayNumText", true);
        UIManager.Instance.SetUI("NowDayNumText", true);
        UIManager.Instance.SetUI("YesterDay", true);

        int yesterdayNum = nowDayNum - 1;
        float AnimatingHalfTime = dayAnimatingTime / 2;

        yesterDayNumText.text = $"Day {yesterdayNum}";
        nowDayNumText.text = $"Day {nowDayNum}";


        Quaternion startRotation = yesterDayRectTransform.rotation;
        Quaternion midRotation = startRotation * Quaternion.Euler(0,180,90);
        Quaternion endRotation = startRotation * Quaternion.Euler(180,180,180);

        elapsedTime = 0f;
        hasChangedSibling = false;

        Vector2 targetPosition = DayAnimMovedPosition;
        float afterScaleValue = DayAnimScaleZoomedValue;

        // 배경이 점점 어두워짐
        StartCoroutine(ScreenEffect.Instance.OnFade(null, 0, 1, dayScalingTime));

        // DayUI 확대되는 코루틴
        StartCoroutine(NextDayUIScaleAndMoving(targetPosition, afterScaleValue));

        // DayUI 확대가 끝날 때까지 대기
        yield return new WaitWhile(() => isDayScaling);

        // DayUI_Gear들과 시침과 분침이 돌기 시작하는 코루틴
        StartCoroutine(RotateGearsAndClockHands());

        elapsedTime = 0f;
        while (elapsedTime < dayAnimatingTime)
        {
            if (elapsedTime < AnimatingHalfTime)
            {
                float fraction = elapsedTime / AnimatingHalfTime;
                yesterDayRectTransform.rotation = Quaternion.Slerp(startRotation, midRotation, fraction);
            }
            else
            {
                if (!hasChangedSibling)
                {
                    yesterDayRectTransform.SetAsFirstSibling();
                    hasChangedSibling = true;
                }

                float fraction = (elapsedTime - AnimatingHalfTime) / AnimatingHalfTime;
                yesterDayRectTransform.rotation = Quaternion.Slerp(midRotation, endRotation, fraction);

            }

            float currentY = yesterDayRectTransform.rotation.eulerAngles.y;
            if (Mathf.Abs(currentY - 263f) < 1f)
            {
                //Debug.Log($" Rotation Y 값이 -97도에 가까움: {currentY}");
                UIManager.Instance.SetUI("YesterDayNumText", false);
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        yesterDayRectTransform.rotation = endRotation;
        UIManager.Instance.SetUI("YesterDayNumText", true);

        // 날짜 변경 후 순서 복구
        yesterDayNumText.text = $"Day {nowDayNum}";
        yesterDayRectTransform.SetAsLastSibling();

        // 배경이 점점 밝아짐
        StartCoroutine(ScreenEffect.Instance.OnFade(null, 1, 0, dayScalingTime));

        targetPosition = DayAnimOriginalPosition;
        afterScaleValue = DayAnimScaleOriginalValue;
        // DayUI 커지게 했던 거 작게 하고 제자리로 옮김
        StartCoroutine(NextDayUIScaleAndMoving(targetPosition, afterScaleValue));

        yield return new WaitForSeconds(dayScalingTime);

        UIManager.Instance.SetUI("ActionPoints", true);

        UIManager.Instance.SetUI("DayAnimGameObject", false);
        //RoomManager.Instance.SetIsInvestigating(false);

        isDayAnimating = false;
    }

    IEnumerator NextDayUIScaleAndMoving(Vector3 targetPosition, float endScaleValue)
    {
        isDayScaling = true;

        elapsedTime = 0f;

        Vector3 startingPosition = DayAnimGroupRectTransform.anchoredPosition;
        Vector3 startScale = DayAnimGroupRectTransform.localScale;
        Vector3 endScale = new Vector3(endScaleValue, endScaleValue, endScaleValue);

        while (elapsedTime < dayScalingTime)
        {
            float fraction = elapsedTime / dayScalingTime;
            DayAnimGroupRectTransform.anchoredPosition = Vector3.Lerp(startingPosition, targetPosition, fraction);
            DayAnimGroupRectTransform.localScale = Vector3.Lerp(startScale, endScale, fraction);

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        DayAnimGroupRectTransform.anchoredPosition = targetPosition;
        DayAnimGroupRectTransform.localScale = endScale;

        isDayScaling = false;
    }

    private IEnumerator RotateGearsAndClockHands()
    {
        UIManager.Instance.SetUI("GearGroup", true);
        UIManager.Instance.SetUI("MainGear", true);
        UIManager.Instance.SetUI("SubGear", true);
        UIManager.Instance.SetUI("GearHourHand", true);
        UIManager.Instance.SetUI("GearMinuteHand", true);

        float minuteRotationPerSecond = 360f / minuteRotationSpeed;
        float hourRotationPerSecond = 360f / hourRotationSpeed;

        minuteRotationPerSecond *= clockSpeedMultiplier; // 1.5배 속도 적용
        hourRotationPerSecond *= clockSpeedMultiplier;

        // alpha값 변경되는 시간 (투명해지거나 불투명해지는 시간)
        float alphaTime = 0.5f;

        float elapsedTime = 0f;
        float triggerTime = dayAnimatingTime - alphaTime;
        // dayAnimatingTime 시간이 끝나면 아예 오브젝트가 꺼지기 때문에 
        //  dayAnimatingTime에서 alphaTime을 뺀 triggerTime이 되면 그때부터 기어UI 오브젝트들이 투명해지게 코루틴 시작함.

        // 투명했던 기어UI 오브젝트들을 불투명하게 함
        StartCoroutine(ControlImagesAlphaValue(GearImages,true, alphaTime));

        while (elapsedTime < triggerTime)
        {
            // 기어 시계 방향 회전
            MainGear.transform.Rotate(0, 0, -mainGearSpeed * Time.deltaTime);
            SubGear.transform.Rotate(0, 0, -subGearSpeed * Time.deltaTime);

            GearMinuteHand.transform.Rotate(0, 0, -minuteRotationPerSecond * Time.deltaTime); // 분침 회전
            GearHourHand.transform.Rotate(0, 0, -hourRotationPerSecond * Time.deltaTime); // 시침 회전

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 불투명했던 기어UI 오브젝트들을 투명하게 함
        StartCoroutine(ControlImagesAlphaValue(GearImages, false, alphaTime));

        // 나머지 시간동안 마저 기어 회전하게 함.
        while (elapsedTime < dayAnimatingTime)
        {
            // 기어 시계 방향 회전
            MainGear.transform.Rotate(0, 0, -mainGearSpeed * Time.deltaTime);
            SubGear.transform.Rotate(0, 0, -subGearSpeed * Time.deltaTime);

            GearMinuteHand.transform.Rotate(0, 0, -minuteRotationPerSecond * Time.deltaTime); // 분침 회전
            GearHourHand.transform.Rotate(0, 0, -hourRotationPerSecond * Time.deltaTime); // 시침 회전

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        UIManager.Instance.SetUI("GearGroup", false);
        UIManager.Instance.SetUI("MainGear", false);
        UIManager.Instance.SetUI("SubGear", false);
        UIManager.Instance.SetUI("GearHourHand", false);
        UIManager.Instance.SetUI("GearMinuteHand", false);
    }

    // 기어 UI Image들의 알파값을 조정
    // isActive가 true이면 기어 UI Image들의 알파값을 0 -> 1로 만들고
    // false이면 기어 UI Image들의 알파값을 1 -> 0으로 만들음
    private IEnumerator ControlImagesAlphaValue(List<Image> imageList,bool isActive, float durationTime)
    {
        float startValue;
        float endValue;
        switch (isActive)
        {
            case true:
                startValue = 0f;
                endValue = 1f;
                break;

            case false:
                startValue = 1f;
                endValue = 0f;
                break;
        }

        float elapsedTime = 0f;

        List<Color> colors = new List<Color>();
        foreach (var img in imageList)
        {
            colors.Add(img.color);
        }

        while (elapsedTime < durationTime)
        {
            float alpha = Mathf.Lerp(startValue, endValue, elapsedTime / durationTime);

            // 모든 이미지의 알파값을 동시에 변경
            for (int i = 0; i < imageList.Count; i++)
            {
                if (imageList[i] != null)
                {
                    Color color = colors[i];
                    color.a = alpha;
                    imageList[i].color = color;
                }
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        for (int i = 0; i < imageList.Count; i++)
        {
            if (imageList[i] != null)
            {
                Color color = colors[i];
                color.a = endValue;
                imageList[i].color = color;
            }
        }
    }


    protected IEnumerator NextDayUIAnimationVer2(int nowDayNum)
    {
        if (isDayAnimating)
            yield break;

        isDayAnimating = true;

        //RoomManager.Instance.SetIsInvestigating(true);
        UIManager.Instance.SetUI("ActionPoints", false);

        UIManager.Instance.SetUI("DayAnimGameObject", true);
        UIManager.Instance.SetUI("YesterDayNumText", true);
        UIManager.Instance.SetUI("NowDayNumText", true);
        UIManager.Instance.SetUI("YesterDay", true);

        int yesterdayNum = nowDayNum - 1;
        float AnimatingHalfTime = dayAnimatingTime / 2;

        yesterDayNumText.text = $"Day {yesterdayNum}";
        nowDayNumText.text = $"Day {nowDayNum}";

        Vector3 startEuler = yesterDayRectTransform.rotation.eulerAngles; // 초기 회전값
        Vector3 midEuler = startEuler + new Vector3(0, 180, 90); // 중간 회전값
        Vector3 endEuler = startEuler + new Vector3(180, 180, 180); // 최종 회전값

        elapsedTime = 0f;
        hasChangedSibling = false;

        Vector2 targetPosition = DayAnimMovedPosition;
        float afterScaleValue = DayAnimScaleZoomedValue;

        // 배경이 점점 어두워짐
        //StartCoroutine(ScreenEffect.Instance.OnFade(null, 0, 1, dayScalingTime));

        // DayUI 확대되는 코루틴
        StartCoroutine(NextDayUIScaleAndMoving(targetPosition, afterScaleValue));

        // DayUI 확대가 끝날 때까지 대기
        yield return new WaitWhile(() => isDayScaling);

        // DayUI_Gear들과 시침과 분침이 돌기 시작하는 코루틴
        StartCoroutine(RotateGearsAndClockHands());

        elapsedTime = 0f;
        while (elapsedTime < dayAnimatingTime)
        {
            if (elapsedTime < AnimatingHalfTime)
            {
                float fraction = elapsedTime / AnimatingHalfTime;
                yesterDayRectTransform.rotation = Quaternion.Euler(Vector3.Lerp(startEuler, midEuler, fraction));
            }
            else
            {
                if (!hasChangedSibling)
                {
                    yesterDayRectTransform.SetAsFirstSibling();
                    hasChangedSibling = true;
                }

                float fraction = (elapsedTime - AnimatingHalfTime) / AnimatingHalfTime;

                yesterDayRectTransform.rotation = Quaternion.Euler(Vector3.Lerp(midEuler, endEuler, Mathf.Clamp01(fraction)));

            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        yesterDayRectTransform.rotation = Quaternion.Euler(endEuler);

        // 날짜 변경 후 순서 복구
        yesterDayNumText.text = $"Day {nowDayNum}";
        yesterDayRectTransform.SetAsLastSibling();

        // 배경이 점점 밝아짐
        //StartCoroutine(ScreenEffect.Instance.OnFade(null, 1, 0, dayScalingTime));

        targetPosition = DayAnimOriginalPosition;
        afterScaleValue = DayAnimScaleOriginalValue;
        // DayUI 커지게 했던 거 작게 하고 제자리로 옮김
        StartCoroutine(NextDayUIScaleAndMoving(targetPosition, afterScaleValue));

        yield return new WaitForSeconds(dayScalingTime);

        UIManager.Instance.SetUI("ActionPoints", true);

        UIManager.Instance.SetUI("DayAnimGameObject", false);
        //RoomManager.Instance.SetIsInvestigating(false);

        isDayAnimating = false;
    }
}
