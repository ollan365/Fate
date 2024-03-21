using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClockHand : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler
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

    [SerializeField]
    private GameObject clockPuzzle;

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
            transform.rotation = Quaternion.Euler(0f, 0f, newAngle);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // 드래그 종료 시 플래그를 false로 설정
        isDragging = false;

        // 바늘이 원하는 위치에 멈추도록 보정
        SnapToStepAngle();

        //Debug.Log(this.gameObject.name + "의 위치 " + transform.rotation.z);

        // 분침이 멈출 때마다 ClockPassword.cs의 loginPW() 호출
        if (this.gameObject.name=="minute hand")
            clockPuzzle.GetComponent<ClockPuzzle>().TryPassword();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // 드래그 시작 시 플래그를 true로 설정하고, 시작 각도를 계산
        isDragging = true;
        Vector2 dragVector = eventData.position - (Vector2)transform.position;
        startAngle = Mathf.Atan2(dragVector.y, dragVector.x) * Mathf.Rad2Deg;
    }

    // 바늘이 원하는 위치에 멈추도록 보정하는 함수
    private void SnapToStepAngle()
    {
        // 현재 회전 각도
        float currentAngle = transform.eulerAngles.z;

        // 바늘이 한 번에 회전할 각도에 가까운 각도로 조정
        float snappedAngle = Mathf.Round(currentAngle / stepAngle) * stepAngle;

        // 바늘의 회전 각도를 설정
        transform.rotation = Quaternion.Euler(0f, 0f, snappedAngle);
    }
}