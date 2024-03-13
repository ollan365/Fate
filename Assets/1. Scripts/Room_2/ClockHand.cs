using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClockHand : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler
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

    private GameObject clockPw;

    void Start()
    {
        clockPw = GameObject.Find("Clock_bg");
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
            transform.rotation = Quaternion.Euler(0f, 0f, newAngle);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // �巡�� ���� �� �÷��׸� false�� ����
        isDragging = false;

        // �ٴ��� ���ϴ� ��ġ�� ���ߵ��� ����
        SnapToStepAngle();

        //Debug.Log(this.gameObject.name + "�� ��ġ " + transform.rotation.z);

        // ��ħ�� ���� ������ ClockPassword.cs�� loginPW() ȣ��
        if (this.gameObject.name=="minute_hand")
            clockPw.GetComponent<ClockPassword>().loginPW();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // �巡�� ���� �� �÷��׸� true�� �����ϰ�, ���� ������ ���
        isDragging = true;
        Vector2 dragVector = eventData.position - (Vector2)transform.position;
        startAngle = Mathf.Atan2(dragVector.y, dragVector.x) * Mathf.Rad2Deg;
    }

    // �ٴ��� ���ϴ� ��ġ�� ���ߵ��� �����ϴ� �Լ�
    private void SnapToStepAngle()
    {
        // ���� ȸ�� ����
        float currentAngle = transform.eulerAngles.z;

        // �ٴ��� �� ���� ȸ���� ������ ����� ������ ����
        float snappedAngle = Mathf.Round(currentAngle / stepAngle) * stepAngle;

        // �ٴ��� ȸ�� ������ ����
        transform.rotation = Quaternion.Euler(0f, 0f, snappedAngle);
    }
}