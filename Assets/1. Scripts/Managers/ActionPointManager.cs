using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;
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

    // 귀가 이벤트 실행 여부
    public bool refillHeartsOrEndDayState = false;

    // ************************* temporary members for Day Animation *************************
    int MidRotationIndex = 0;
    int EndRotationIndex = 1;

    [SerializeField] private float dayScalingTime = 2f;
    [SerializeField] private float dayTurningBackTime = 4f;

    public bool isDayChanging = false;

    [SerializeField] private List<Vector3> TurningDayBackRotationValues;

    [SerializeField] private Vector3 ChangeDayOriginalPosition;  // 기존 위치 (-832, 435.6, -100)
    [SerializeField] private Vector3 ChangeDayMovedPosition;  // (0, 0, -100) 
    [SerializeField] private float ChangeDayScaleOriginalValue = 0.385f;
    [SerializeField] private float ChangeDayScaleZoomedValue = 1;

    public TextMeshProUGUI yesterDayNumText;
    public TextMeshProUGUI nowDayNumText;
    public RectTransform yesterDayRectTransform;
    public RectTransform DayChangingGroupRectTransform;

    [SerializeField] private GameObject MainGear;
    [SerializeField] private GameObject SubGear;
    [SerializeField] private GameObject GearHourHand;
    [SerializeField] private GameObject GearMinuteHand;

    private bool hasChangedSibling = false;
    private bool _isTurningBack;   // Day Animation 재진입 가드용

    // ************************* temporary members for Day Gear Rotation and Alpha value *************************
    [SerializeField] private float mainGearSpeed = 90f;
    [SerializeField] private float subGearSpeed = 180f;

    [SerializeField] private float minuteRotationSpeed = 5f;
    [SerializeField] private float hourRotationSpeed = 15f;
    [SerializeField] private float clockSpeedMultiplier = 1.5f;

    [SerializeField] float minuteRotationPerSecond;
    [SerializeField] float hourRotationPerSecond;

    public List<Image> GearImages;

    public const int StartDayUIChange = 1,
        FinishDayUIChange = 2,
        StartGearsAndClockHandsRotate = 3,
        FinishGearsAndClockHandsRotate = 4,
        StartDayChangeBGM = 5,
        MiddleDayChangeBGM = 6,
        EndDayChangeBGM = 7,
        FinishDayChangeBGM = 8; 

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

    protected virtual void Awake()
    {
        heartParent = UIManager.Instance.heartParent;
        dayText = UIManager.Instance.dayTextTextMeshProUGUI;

        loadDayChangeVariables();
    }

    private void Start()
    {
        refillHeartsOrEndDayState = false;
    }

    private void loadDayChangeVariables()
    {
        yesterDayNumText = UIManager.Instance.yesterdayNumTextTextMeshProUGUI;
        nowDayNumText = UIManager.Instance.todayNumTextTextMeshProUGUI;
        yesterDayRectTransform = UIManager.Instance.yesterdayRectTransform;
        DayChangingGroupRectTransform = UIManager.Instance.dayChangingGroupRectTransform;

        TurningDayBackRotationValues.Add(new Vector3(0, 180, 90));
        TurningDayBackRotationValues.Add(new Vector3(180, 180, 180));

        ChangeDayOriginalPosition = DayChangingGroupRectTransform.anchoredPosition;  // 초기 위치 저장
        ChangeDayMovedPosition = new Vector3(0, 0, -100);

        ChangeDayScaleOriginalValue = 0.385f;
        ChangeDayScaleZoomedValue = 1;

        MainGear = UIManager.Instance.mainGear;
        SubGear = UIManager.Instance.subGear;
        GearHourHand = UIManager.Instance.gearHourHand;
        GearMinuteHand = UIManager.Instance.gearMinuteHand;

        GearImages.Add(MainGear.GetComponent<Image>());
        GearImages.Add(SubGear.GetComponent<Image>());
        GearImages.Add(GearHourHand.GetComponent<Image>());
        GearImages.Add(GearMinuteHand.GetComponent<Image>());

        minuteRotationPerSecond = (360f / minuteRotationSpeed)* clockSpeedMultiplier;
        hourRotationPerSecond = (360f / hourRotationSpeed) * clockSpeedMultiplier;
    }


    protected static IEnumerator DeactivateHeart(Object heart)
    {
        yield return new WaitForSeconds(0.5f);
        UIManager.Instance.RemoveUIToCheck(heart.GetComponent<RectTransform>());
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

        UIManager.Instance.SetUI(eUIGameObjectName.MemoButton, false);
        UIManager.Instance.SetUI(eUIGameObjectName.LeftButton, false);
        UIManager.Instance.SetUI(eUIGameObjectName.RightButton, false);

        float totalTime = 1.5f;

        // 페이드 인
        yield return StartCoroutine(UIManager.Instance.OnFade(null, 0, 1, totalTime));

        // 휴식하면 그날 하루에 남아있는 행동력 다 사용되기에 현재 있는 하트들 삭제
        foreach (Transform child in heartParent.transform)
        {
            UIManager.Instance.RemoveUIToCheck(child.GetComponent<RectTransform>());
            Destroy(child.gameObject);
        }

        // 페이드 아웃
        yield return StartCoroutine(UIManager.Instance.OnFade(null, 1, 0, totalTime));

        // 대사 출력 전 조사 오브젝트 클릭 막기용
        UIManager.Instance.loadingScreen.SetActive(true);

        // 휴식 대사 출력. 
        DialogueManager.Instance.StartDialogue("RoomEscape_035");

        int actionPoint;
        if (nowDayNum < maxDayNum) {
            nowDayNum += 1;
            presentHeartIndex = (int)GameManager.Instance.GetVariable("ActionPointsPerDay") - 1;

            GameManager.Instance.SetVariable("NowDayNum", nowDayNum);
            GameManager.Instance.SetVariable("PresentHeartIndex", presentHeartIndex);

            actionPoint = actionPointsArray[nowDayNum - 1, presentHeartIndex];
        }
        else
            actionPoint = 0; // 마지막 날인 5일에 휴식했을 경우: 행동력이 0이 된 상태

        GameManager.Instance.SetVariable("ActionPoint", actionPoint);

        UIManager.Instance.loadingScreen.SetActive(false);
        
        RoomManager.Instance.SetIsInvestigating(false);
        SaveManager.Instance.SaveGameData();
    }

    protected void Warning() {
        if ((int)GameManager.Instance.GetVariable("ActionPoint") <= 0) // actionPoint가 0보다 클 때만 코루틴 실행
            return;
        
        StartCoroutine(UIManager.Instance.WarningCoroutine());
    }

    // ************************* temporary methods for day animation *************************
    protected IEnumerator StartNextDayUIChange(int nowDayNum) {
        isDayChanging = true;

        // 브금 변경 (Start_Daychange)
        // 아이콘 내려오는 브금
        SetDayChangeBGM(StartDayChangeBGM);

        SetChangingDayUI(StartDayUIChange);

        int yesterdayNum = nowDayNum - 1;
        yesterDayNumText.text = $"Day {yesterdayNum}";
        nowDayNumText.text = $"Day {nowDayNum}";
        dayText.text = $"Day {nowDayNum}";

        // 배경 어두워지는 코루틴 실행
        StartCoroutine(UIManager.Instance.OnFade(null, 0, 1, dayScalingTime));

        // DayUI 확대 코루틴 실행
        StartCoroutine(ScaleAndMovingNextDayUI(ChangeDayMovedPosition, ChangeDayScaleZoomedValue));

        yield return new WaitForSeconds(dayScalingTime);

        // 한장 넘기는 브금
        SetDayChangeBGM(MiddleDayChangeBGM);

        // DayUI 기어와 시침 분침 돌리는 코루틴 실행
        StartCoroutine(StartRotateGearsAndClockHands());

        // NextDayUI 뒤집는 애니메이션이 실행됨.
        StartCoroutine(TurnNextDayUIBack());

        yield return new WaitForSeconds(dayTurningBackTime);

        // 다시 원위치 브금
        SetDayChangeBGM(EndDayChangeBGM);

        yesterDayNumText.text = $"Day {nowDayNum}";
        yesterDayRectTransform.SetAsLastSibling();

        // 배경 밝아지는 코루틴 실행
        StartCoroutine(UIManager.Instance.OnFade(null, 1, 0, dayScalingTime));

        // DayUI 축소 코루틴 실행
        StartCoroutine(ScaleAndMovingNextDayUI(ChangeDayOriginalPosition, ChangeDayScaleOriginalValue));

        yield return new WaitForSeconds(dayScalingTime);

        SetChangingDayUI(FinishDayUIChange);

        // 브금 변경
        SetDayChangeBGM(FinishDayChangeBGM);

        isDayChanging = false;
    }

    // DayUI 뒤로 넘김
    protected IEnumerator TurnNextDayUIBack()
    {
        if (_isTurningBack) yield break;
        _isTurningBack = true;

        // dayui 넘어가게 할 start, mid, end rotation 
        Quaternion startRotation = yesterDayRectTransform.rotation;
        Quaternion midRotation = startRotation * Quaternion.Euler(TurningDayBackRotationValues[MidRotationIndex]);
        Quaternion endRotation = startRotation * Quaternion.Euler(TurningDayBackRotationValues[EndRotationIndex]);

        float elapsedTime = 0f;
        hasChangedSibling = false;

        float AnimatingHalfTime = dayTurningBackTime / 2;
        const float MidRoation_Y_value = 263f;

        while (elapsedTime < dayTurningBackTime)
        {
            switch(elapsedTime < AnimatingHalfTime)
            {
                case true:
                    float fraction = elapsedTime / AnimatingHalfTime;
                    yesterDayRectTransform.rotation = Quaternion.Slerp(startRotation, midRotation, fraction);
                    break;

                case false:
                    if (!hasChangedSibling)
                    {
                        yesterDayRectTransform.SetAsFirstSibling();
                        hasChangedSibling = true;
                    }
                    fraction = (elapsedTime - AnimatingHalfTime) / AnimatingHalfTime;
                    yesterDayRectTransform.rotation = Quaternion.Slerp(midRotation, endRotation, fraction);
                    break;
            }

            float currentY = yesterDayRectTransform.rotation.eulerAngles.y;
            if (Mathf.Abs(currentY - MidRoation_Y_value) < 1f)
            {
                UIManager.Instance.SetUI(eUIGameObjectName.YesterdayNumText, false);
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        yesterDayRectTransform.rotation = endRotation;
        UIManager.Instance.SetUI(eUIGameObjectName.YesterdayNumText, true);
        _isTurningBack = false;
    }

    // DayUI 크기, 위치 가운데로 조정
    IEnumerator ScaleAndMovingNextDayUI(Vector3 targetPosition, float endScaleValue)
    {
        float elapsedTime = 0f;

        Vector3 startingPosition = DayChangingGroupRectTransform.anchoredPosition;
        Vector3 startScale = DayChangingGroupRectTransform.localScale;
        Vector3 endScale = new Vector3(endScaleValue, endScaleValue, endScaleValue);

        while (elapsedTime < dayScalingTime)
        {
            float fraction = elapsedTime / dayScalingTime;
            DayChangingGroupRectTransform.anchoredPosition = Vector3.Lerp(startingPosition, targetPosition, fraction);
            DayChangingGroupRectTransform.localScale = Vector3.Lerp(startScale, endScale, fraction);

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        DayChangingGroupRectTransform.anchoredPosition = targetPosition;
        DayChangingGroupRectTransform.localScale = endScale;
    }

    // 기어, 시계 시침분침 회전 시작하는 코루틴
    private IEnumerator StartRotateGearsAndClockHands()
    {
        SetChangingDayUI(StartGearsAndClockHandsRotate);

        // alpha값 변경되는 시간 (투명해지거나 불투명해지는 시간)
        float alphaTime = 0.5f;

        float elapsedTime = 0f;
        float triggerTime = dayTurningBackTime - alphaTime;
        // dayAnimatingTime 시간이 끝나면 아예 오브젝트가 꺼지기 때문에 
        //  dayAnimatingTime에서 alphaTime을 뺀 triggerTime이 되면 그때부터 기어UI 오브젝트들이 투명해지게 코루틴 시작함.

        // 투명했던 기어UI 오브젝트들을 불투명하게 함
        StartCoroutine(ControlImagesAlpha(GearImages,true, alphaTime));

        StartCoroutine(RotateGearsAndClockHands(elapsedTime, triggerTime));

        yield return new WaitForSeconds(triggerTime);

        // 불투명했던 기어UI 오브젝트들을 투명하게 함
        StartCoroutine(ControlImagesAlpha(GearImages, false, alphaTime));

        // 나머지 시간동안 마저 기어 회전하게 함.
        StartCoroutine(RotateGearsAndClockHands(elapsedTime, dayTurningBackTime));

        yield return new WaitForSeconds(dayTurningBackTime);

        SetChangingDayUI(FinishGearsAndClockHandsRotate);
    }

    // 기어, 시계 시침분침 회전
    private IEnumerator RotateGearsAndClockHands(float elapsedTime, float turningTime)
    {
        while (elapsedTime < turningTime)
        {
            // 기어 시계 방향 회전
            MainGear.transform.Rotate(0, 0, -mainGearSpeed * Time.deltaTime);
            SubGear.transform.Rotate(0, 0, -subGearSpeed * Time.deltaTime);

            GearMinuteHand.transform.Rotate(0, 0, -minuteRotationPerSecond * Time.deltaTime); // 분침 회전
            GearHourHand.transform.Rotate(0, 0, -hourRotationPerSecond * Time.deltaTime); // 시침 회전

            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    // 기어 UI Image들의 알파값을 조정
    // isActive가 true이면 기어 UI Image들의 알파값을 0 -> 1로 만들고
    // false이면 기어 UI Image들의 알파값을 1 -> 0으로 만들음
    private IEnumerator ControlImagesAlpha(List<Image> imageList, bool isActive, float durationTime)
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

            // 이미지의 알파값을 동시에 변경
            SetImagesAlphaValue(imageList, colors, alpha);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        SetImagesAlphaValue(imageList, colors, endValue);
    }

    // 리스트에 있는 모든 이미지의 알파값을 동시에 변경
    private void SetImagesAlphaValue(List<Image> imageList, List<Color> colors, float alpha)
    {
        for (int i = 0; i < imageList.Count; i++) {
            if (imageList[i]) {
                Color color = colors[i];
                color.a = alpha;
                imageList[i].color = color;
            }
        }
    }

    // UIManager로 필요한 UI를 키고 끔
    private void SetChangingDayUI(int num)
    {
        switch (num)
        {
            case StartDayUIChange:
                UIManager.Instance.SetUI(eUIGameObjectName.ActionPoints, false);
                UIManager.Instance.SetUI(eUIGameObjectName.DayChangingGameObject, true);
                UIManager.Instance.SetUI(eUIGameObjectName.YesterdayNumText, true);
                UIManager.Instance.SetUI(eUIGameObjectName.TodayNumText, true);
                UIManager.Instance.SetUI(eUIGameObjectName.Yesterday, true);
                break;
            
            case FinishDayUIChange:
                UIManager.Instance.SetUI(eUIGameObjectName.ActionPoints, true);
                UIManager.Instance.SetUI(eUIGameObjectName.DayChangingGameObject, false);
                break;

            case StartGearsAndClockHandsRotate:
                UIManager.Instance.SetUI(eUIGameObjectName.MainGear, true);
                UIManager.Instance.SetUI(eUIGameObjectName.SubGear, true);
                UIManager.Instance.SetUI(eUIGameObjectName.GearHourHand, true);
                UIManager.Instance.SetUI(eUIGameObjectName.GearMinuteHand, true);
                break;
            
            case FinishGearsAndClockHandsRotate:
                UIManager.Instance.SetUI(eUIGameObjectName.MainGear, false);
                UIManager.Instance.SetUI(eUIGameObjectName.SubGear, false);
                UIManager.Instance.SetUI(eUIGameObjectName.GearHourHand, false);
                UIManager.Instance.SetUI(eUIGameObjectName.GearMinuteHand, false);
                break;
        }
    }

    private void SetDayChangeBGM(int num)
    {
        switch (num)
        {
            case StartDayChangeBGM:
                // BGM 정지 및 Daychange_start bgm 재생 (아이콘이 내려왔다가)
                SoundPlayer.Instance.ChangeBGM(Constants.BGM_STOP);
                SoundPlayer.Instance.UISoundPlay(Constants.Sound_Daychange_start, SoundPlayer.SfxPriority.High);
                break;

            case MiddleDayChangeBGM:
                // Daychange_middle bgm 재생 (한 장 넘어가고)
                SoundPlayer.Instance.UISoundPlay(Constants.Sound_Daychange_middle, SoundPlayer.SfxPriority.High);
                break;

            case EndDayChangeBGM:
                // Daychange_end bgm 재생 (다시 원위치)
                SoundPlayer.Instance.UISoundPlay(Constants.Sound_Daychange_end, SoundPlayer.SfxPriority.High);
                break;

            case FinishDayChangeBGM:
                // Room Bgm 다시 재생
                SoundPlayer.Instance.ChangeBGM(GameSceneManager.Instance.GetActiveScene() == Constants.SceneType.ROOM_1
                    ? Constants.BGM_ROOM1
                    : Constants.BGM_ROOM2);
                break;
        }    
    }
}
