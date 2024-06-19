using Unity.VisualScripting;
using UnityEngine;
public class ClockHand : MonoBehaviour
{
    public Transform minuteHand;
    public Transform hourHand;

    private float lastAngle = 0;
    private float minuteAngle = 90f;
    private float hourAngle = -112.5f;

    private bool dragging = false;
    private bool isSnapping = false;
    private float smoothSnapSpeed = 40f;  // 달라붙는 속도

    private int correctMinute;
    private int correctHour;
    private bool isClockTimeCorrect = false;

    [SerializeField] private ImageAndLockPanelManager imageAndLockPanelManager;

    [SerializeField] private Clock clockA;

    private void Awake()
    {
        correctMinute = (int)GameManager.Instance.GetVariable("ClockPasswordMinute");
        correctHour = (int)GameManager.Instance.GetVariable("ClockPasswordHour");
    }

    private void Update()
    {
        bool isImageActive = imageAndLockPanelManager.isImageActive;
        if (isSnapping || isImageActive) return;
        if (Input.GetMouseButtonDown(0) && !isSnapping)
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 handTipPosition = minuteHand.position + minuteHand.up * 5.7f;

            if (Vector2.Distance(mousePosition, handTipPosition) < 2f)  // 분침의 끝부분과 일정 거리 이하에서 클릭해야 작동
            {
                lastAngle = Mathf.Atan2(mousePosition.y - minuteHand.position.y, mousePosition.x - minuteHand.position.x) * Mathf.Rad2Deg;
                dragging = true;
                SoundPlayer.Instance.UISoundPlay_LOOP(Constants.Sound_ClockMovement, true);
            }
        }
        if (dragging && Input.GetMouseButton(0))
        {
            Vector2 direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - minuteHand.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            float delta = Mathf.DeltaAngle(lastAngle, angle);
            RotateMinuteHand(delta);
            RotateHourHand(delta);
            lastAngle = angle;
        }
        if (dragging && Input.GetMouseButtonUp(0))
        {
            SoundPlayer.Instance.UISoundPlay_LOOP(Constants.Sound_ClockMovement, false);
            dragging = false;
            StartCoroutine(SnapHandsToNearestTick());
        }
    }
    private void RotateMinuteHand(float delta)
    {
        minuteAngle += delta;
        minuteHand.rotation = Quaternion.Euler(0, 0, minuteAngle);
    }
    private void RotateHourHand(float delta)
    {
        hourAngle += delta / 12;
        hourHand.rotation = Quaternion.Euler(0, 0, hourAngle);
    }
    private System.Collections.IEnumerator SnapHandsToNearestTick()
    {
        isSnapping = true;  // 달라붙고 있는 도중에는 조작 불가능 
        float targetMinuteRotation = Mathf.Round(minuteAngle / 30) * 30;
        float targetHourRotation = Mathf.Round(hourAngle / 2.5f) * 2.5f;
        while (Mathf.Abs(Mathf.DeltaAngle(minuteHand.eulerAngles.z, targetMinuteRotation)) > 0.1f ||
               Mathf.Abs(Mathf.DeltaAngle(hourHand.eulerAngles.z, targetHourRotation)) > 0.1f)
        {
            if (Mathf.Abs(Mathf.DeltaAngle(minuteHand.eulerAngles.z, targetMinuteRotation)) > 0.1f)
            {
                float minuteAngle = Mathf.MoveTowardsAngle(minuteHand.eulerAngles.z, targetMinuteRotation, smoothSnapSpeed * Time.deltaTime);
                minuteHand.rotation = Quaternion.Euler(0, 0, minuteAngle);
                this.minuteAngle = minuteAngle;
            }
            if (Mathf.Abs(Mathf.DeltaAngle(hourHand.eulerAngles.z, targetHourRotation)) > 0.1f)
            {
                float hourAngle = Mathf.MoveTowardsAngle(hourHand.eulerAngles.z, targetHourRotation, smoothSnapSpeed * Time.deltaTime);
                hourHand.rotation = Quaternion.Euler(0, 0, hourAngle);
                this.hourAngle = hourAngle;
            }
            yield return null;
        }
        isSnapping = false;
        CompareClockHands();
    }
    private void CompareClockHands()
    {
        if (isClockTimeCorrect) return;

        int currentHour = CalculateHourFromAngle(hourAngle);
        int currentMinute = CalculateMinuteFromAngle(minuteAngle);

        // Debug.Log($"current time: {currentHour}:{currentMinute}");

        if (currentHour == correctHour && currentMinute == correctMinute)
        {
            GameManager.Instance.SetVariable("ClockTimeCorrect", true);
            // 시계 맞춘 이후에 시계 다시 클릭하면 조사창 패스된거 다시 조사창 나오게 함.
            clockA.SetIsInquiry(true);
            isClockTimeCorrect = true;
        }

        EventManager.Instance.CallEvent("EventClockB");
    }
    private int CalculateHourFromAngle(float angle)
    {
        angle = (angle + 360) % 360;
        if (angle == 0) angle = 360;
        int hour = 12 - (int)Mathf.Ceil(angle / 30);

        return hour;
    }

    private int CalculateMinuteFromAngle(float angle)
    {
        angle = (Mathf.Round(angle / 30) * 30 + 360) % 360;
        int minute = (5 * (12 - (int)angle / 30)) % 60;

        return minute;
    }
}