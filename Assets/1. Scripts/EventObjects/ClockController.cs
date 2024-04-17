using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClockController : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler
{
    // �巡�� ������ ���θ� ��Ÿ���� �÷���
    private bool isDragging = false;

    // �巡�� ���� �� �ٴ��� �ʱ� ȸ�� ����
    private float startAngle;

    // �ٴ��� ȸ���� �� �ִ� �ִ� ����
    public float maxAngle = 360f;

    // �ٴ��� ȸ���ϴ� �ӵ�
    public float rotationSpeed = 5f;

    // �� ���� ȸ���� ���� (�� ĭ)
    public float stepAngle = 30f;


    public Transform hourHand;
    public Transform minuteHand;

    private Vector3 prevMousePosition;

    // ��ħ�� ���� ȸ�� ����
    private float accumulatedMinuteAngle = 0f;
    private float min = 0f;

    // ��ħ �ٴ��� �� ������ ȸ���ߴ��� ���θ� ��Ÿ���� ����
    private bool hasRotatedOnce = false;

    // ���� �ð��� �����ϴ� ����
    private int currentHour = 3;

    // �� ������ �� �� ȣ��� �Լ�
    private void OnClockHandRotated()
    {
        // �� ���� ȸ�� �� min �� ������Ʈ
        min += 360f;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // �巡�� ���� ���� ����
        if (isDragging)
        {
            // ���� ���콺 ��ġ���� ���� ���콺 ��ġ���� ���͸� ���
            Vector2 currentPosition = eventData.position;
            Vector2 startPosition = eventData.pressPosition;
            Vector2 dragVector = currentPosition - startPosition;

            // �巡�� ������ ������ ���
            float angle = Mathf.Atan2(dragVector.y, dragVector.x) * Mathf.Rad2Deg;

            // �ٴ��� ȸ�� ������ ����
            float newAngle = Mathf.Clamp(angle - startAngle, -maxAngle, maxAngle);
            minuteHand.rotation = Quaternion.Euler(0f, 0f, newAngle);

            // �� ������ ���Ҵ��� Ȯ���ϰ� �� ���� ȣ��
            if (Mathf.Abs(minuteHand.rotation.eulerAngles.z) >= 300f && !hasRotatedOnce)
            {
                OnClockHandRotated();
                hasRotatedOnce = true; // �� ���� ȸ���� �ν��ϵ��� �÷��� ������Ʈ
            }
            else if (Mathf.Abs(minuteHand.rotation.eulerAngles.z) < 300f)
            {
                hasRotatedOnce = false; // ȸ���� 300�� �̸��̸� �÷��� �缳��
            }

            // ��ħ�� ���� z�� �� ������Ʈ
            accumulatedMinuteAngle = 360f - minuteHand.rotation.eulerAngles.z;

            // ��ħ�� �Բ� �����̵��� ����
            if (hourHand != null)
            {
                // ��ħ�� ���� ȸ�� ���� ������Ʈ
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


    // �ٴ��� ���ϴ� ��ġ�� ���ߵ��� �����ϴ� �Լ�
    private void SnapToStepAngle()
    {
        // ���� ȸ�� ����
        float currentAngle = minuteHand.eulerAngles.z;

        // �ٴ��� �� ���� ȸ���� ������ ����� ������ ����
        float snappedAngle = Mathf.Round(currentAngle / stepAngle) * stepAngle;

        // �ٴ��� ȸ�� ������ ����
        minuteHand.rotation = Quaternion.Euler(0f, 0f, snappedAngle);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // �巡�� ���� �� �÷��׸� false�� ����
        isDragging = false;

        // �ٴ��� ���ϴ� ��ġ�� ���ߵ��� ����
        SnapToStepAngle();

        gameObject.GetComponent<ClockPuzzle>().TryPassword(currentHour);
        //Debug.Log("TryPassword ȣ���");
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // �巡�� ���� �� �÷��׸� true�� �����ϰ�, ���� ������ ���
        isDragging = true;
        Vector2 dragVector = eventData.position - (Vector2)minuteHand.position;
        startAngle = Mathf.Atan2(dragVector.y, dragVector.x) * Mathf.Rad2Deg;
    }
  
}
