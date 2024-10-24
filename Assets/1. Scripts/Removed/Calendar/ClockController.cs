using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClockController : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler
{
    // 드래그 중인지 여부를 나타내는 플래그
    private bool isDragging = false;

    // 드래그 시작 시 바늘의 초기 회전 각도
    private float startAngle;

    // 바늘이 회전할 수 있는 최대 각도
    public float maxAngle = 360f;

    // 바늘이 회전하는 속도
    public float rotationSpeed = 5f;

    // 한 번에 회전할 각도 (한 칸)
    public float stepAngle = 30f;


    public Transform hourHand;
    public Transform minuteHand;

    private Vector3 prevMousePosition;

    // 분침의 누적 회전 각도
    private float accumulatedMinuteAngle = 0f;
    private float min = 0f;

    // 분침 바늘이 한 바퀴를 회전했는지 여부를 나타내는 변수
    private bool hasRotatedOnce = false;

    // 현재 시간을 저장하는 변수
    private int currentHour = 3;

    // 한 바퀴를 돌 때 호출될 함수
    private void OnClockHandRotated()
    {
        // 한 바퀴 회전 시 min 값 업데이트
        min += 360f;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // 드래그 중일 때만 실행
        if (isDragging)
        {
            // 현재 마우스 위치에서 시작 마우스 위치로의 벡터를 계산
            Vector2 currentPosition = eventData.position;
            Vector2 startPosition = eventData.pressPosition;
            Vector2 dragVector = currentPosition - startPosition;

            // 드래그 벡터의 각도를 계산
            float angle = Mathf.Atan2(dragVector.y, dragVector.x) * Mathf.Rad2Deg;

            // 바늘의 회전 각도를 설정
            float newAngle = Mathf.Clamp(angle - startAngle, -maxAngle, maxAngle);
            minuteHand.rotation = Quaternion.Euler(0f, 0f, newAngle);

            // 한 바퀴를 돌았는지 확인하고 한 번만 호출
            if (Mathf.Abs(minuteHand.rotation.eulerAngles.z) >= 300f && !hasRotatedOnce)
            {
                OnClockHandRotated();
                hasRotatedOnce = true; // 한 번의 회전만 인식하도록 플래그 업데이트
            }
            else if (Mathf.Abs(minuteHand.rotation.eulerAngles.z) < 300f)
            {
                hasRotatedOnce = false; // 회전이 300도 미만이면 플래그 재설정
            }

            // 분침의 현재 z축 값 업데이트
            accumulatedMinuteAngle = 360f - minuteHand.rotation.eulerAngles.z;

            // 시침도 함께 움직이도록 설정
            if (hourHand != null)
            {
                // 시침의 누적 회전 각도 업데이트
                float hourHandAngle = ((accumulatedMinuteAngle + min) / 360f) * 30f;
                hourHand.rotation = Quaternion.Euler(0f, 0f, -hourHandAngle);
                currentHour = (int)((accumulatedMinuteAngle + min) / 360f);
                if (currentHour == 0)
                    currentHour = 12;
                else if (currentHour > 12)
                    currentHour = currentHour % 12;
            }
        }
    }


    // 바늘이 원하는 위치에 멈추도록 보정하는 함수
    private void SnapToStepAngle()
    {
        // 현재 회전 각도
        float currentAngle = minuteHand.eulerAngles.z;

        // 바늘이 한 번에 회전할 각도에 가까운 각도로 조정
        float snappedAngle = Mathf.Round(currentAngle / stepAngle) * stepAngle;

        // 바늘의 회전 각도를 설정
        minuteHand.rotation = Quaternion.Euler(0f, 0f, snappedAngle);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // 드래그 종료 시 플래그를 false로 설정
        isDragging = false;

        // 바늘이 원하는 위치에 멈추도록 보정
        SnapToStepAngle();

        gameObject.GetComponent<ClockPuzzle>().TryPassword(currentHour);
        //Debug.Log("TryPassword 호출됨");
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // 드래그 시작 시 플래그를 true로 설정하고, 시작 각도를 계산
        isDragging = true;
        Vector2 dragVector = eventData.position - (Vector2)minuteHand.position;
        startAngle = Mathf.Atan2(dragVector.y, dragVector.x) * Mathf.Rad2Deg;
    }
  
}
