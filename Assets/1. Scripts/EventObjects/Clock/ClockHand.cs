using System.Collections;
using UnityEngine;

public class ClockHand : MonoBehaviour
{
    public Transform minuteHand;
    public Transform hourHand;

    private float lastAngle;
    private float minuteAngle = 120f;
    private float hourAngle = -170f; // 초기 위치에 맞게 설정 (5시 40분): 1분=0.5도 => (5*60+40)*0.5=170

    [SerializeField] private bool dragging;
    [SerializeField] private bool isSnapping;
    [SerializeField] private float smoothFollowSpeed = 3.0f;
    private readonly float smoothSnapSpeed = 50f;  // 달라붙는 속도

    private Coroutine snapCoroutine;

    private int correctMinute;
    private int correctHour;
    private bool isClockTimeCorrect;
    private int beforeTime;

    [SerializeField] private Clock clockA;
    private Camera mainCamera;

    private void Start() {
        mainCamera = Camera.main;
    }

    private void Awake() {
        correctMinute = (int)GameManager.Instance.GetVariable("ClockPasswordMinute");
        correctHour = (int)GameManager.Instance.GetVariable("ClockPasswordHour");
    }

    private void OnDisable() {
        StopSnapImmediately(); // 스냅 정리
        if (dragging == false) 
            return;
        
        dragging = false;
        SoundPlayer.Instance.UISoundPlay_LOOP(Constants.Sound_ClockMovement, false);
    }

    private void Update() {
        if (isSnapping || RoomManager.Instance.imageAndLockPanelManager.isImageActive) 
            return;
        
        if (Input.GetMouseButtonDown(0)) {
            RoomManager.Instance.ProhibitInput(); // 비밀번호 무한 입력 시도 방지

            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 handTipPosition = minuteHand.position + minuteHand.up * 5.7f;
            if (Vector2.Distance(mousePosition, handTipPosition) < 2f) { // 분침의 끝부분과 일정 거리 이하에서 클릭해야 작동
                SoundPlayer.Instance.UISoundPlay_LOOP(Constants.Sound_ClockMovement, true);
                dragging = true;
                lastAngle = Mathf.Atan2(mousePosition.y - minuteHand.position.y, mousePosition.x - minuteHand.position.x) * Mathf.Rad2Deg;
                beforeTime = CalculateHourFromAngle(hourAngle) * 60 + CalculateMinuteFromAngle(minuteAngle);
            }
        }

        if (dragging && Input.GetMouseButton(0)) {
            if (mainCamera) {
                Vector2 direction = mainCamera.ScreenToWorldPoint(Input.mousePosition) - minuteHand.position;
                float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                float smoothedAngle = Mathf.MoveTowardsAngle(lastAngle, targetAngle, smoothFollowSpeed);
                float delta = Mathf.DeltaAngle(lastAngle, smoothedAngle);
                RotateMinuteHand(delta);
                RotateHourHand(delta);
                lastAngle = smoothedAngle;
            }
        }
        
        if (dragging && Input.GetMouseButtonUp(0)) {
            SoundPlayer.Instance.UISoundPlay_LOOP(Constants.Sound_ClockMovement, false);
            dragging = false;
            if (DialogueManager.Instance.isDialogueActive) 
                return;
            
            if (snapCoroutine == null) {
                snapCoroutine = StartCoroutine(SnapHandsToNearestTick(beforeTime));
            } else {
                StopCoroutine(snapCoroutine); 
                snapCoroutine = null;
            }
        }
    }

    private void StopSnapImmediately() {
        if (isSnapping == false) 
            return;
        
        isSnapping = false;          // 상태 먼저 정리
        if (snapCoroutine == null)
            return;
        
        StopCoroutine(snapCoroutine);   // 다음 프레임부터 코루틴 중단
        snapCoroutine = null;
    }

    private void RotateMinuteHand(float delta) {
        minuteAngle += delta;
        minuteHand.rotation = Quaternion.Euler(0, 0, minuteAngle);
    }
    
    private void RotateHourHand(float delta) {
        hourAngle += delta / 12;
        hourHand.rotation = Quaternion.Euler(0, 0, hourAngle);
    }
    
    private IEnumerator SnapHandsToNearestTick(int timeBefore) {
        isSnapping = true;  // 달라붙고 있는 도중에는 조작 불가능 
        float targetMinuteRotation = Mathf.Round(minuteAngle / 30) * 30;
        float targetHourRotation = Mathf.Round(hourAngle / 2.5f) * 2.5f;
        while (true) {
            bool isTargetMinuteReached = Mathf.Abs(Mathf.DeltaAngle(minuteHand.eulerAngles.z, targetMinuteRotation)) <= 0.1f;
            bool isTargetHourReached = Mathf.Abs(Mathf.DeltaAngle(hourHand.eulerAngles.z, targetHourRotation)) <= 0.1f;
            if (isTargetMinuteReached && isTargetHourReached)
                break;
            
            if (isTargetMinuteReached == false) {
                float newMinuteAngle = Mathf.MoveTowardsAngle(minuteHand.eulerAngles.z, targetMinuteRotation,
                    smoothSnapSpeed * Time.deltaTime);
                minuteHand.rotation = Quaternion.Euler(0, 0, newMinuteAngle);
                minuteAngle = newMinuteAngle;
            }
            
            if (isTargetHourReached == false) {
                float newHourAngle = Mathf.MoveTowardsAngle(hourHand.eulerAngles.z, targetHourRotation,
                    smoothSnapSpeed * Time.deltaTime);
                hourHand.rotation = Quaternion.Euler(0, 0, newHourAngle);
                hourAngle = newHourAngle;
            }
            yield return null;
        }

        isSnapping = false;
        snapCoroutine = null;
        var timeAfter = CalculateHourFromAngle(hourAngle) * 60 + CalculateMinuteFromAngle(minuteAngle);
        if (timeBefore != timeAfter)
            CompareClockHands();
    }
    
    private void CompareClockHands() {
        if (isClockTimeCorrect) 
            return;

        if (CalculateHourFromAngle(hourAngle) == correctHour && CalculateMinuteFromAngle(minuteAngle) == correctMinute) {
            GameManager.Instance.SetVariable("ClockTimeCorrect", true);
            clockA.SetIsInquiry(true); // 시계 맞춘 이후에 시계 다시 클릭하면 조사창 패스된거 다시 조사창 나오게 함.
            clockA.SwapAfterImage();
            isClockTimeCorrect = true;
        }

        EventManager.Instance.CallEvent("EventClockB");
    }
    
    private static int CalculateHourFromAngle(float angle) {
        return 12 - (int)Mathf.Ceil(((angle + 360) % 360 == 0 ? 360 : (angle + 360) % 360) / 30);
    }

    private static int CalculateMinuteFromAngle(float angle) {
        return 5 * (12 - (int)((Mathf.Round(angle / 30) * 30 + 360) % 360) / 30) % 60;
    }
}