
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Chair : EventObject, IResultExecutable
{
    // 의자 위치
    private Vector2 originalPosition;  // 기존 위치 
    private List<Vector2> movedPositions = new List<Vector2> { Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero};  // 사이드별 이동한 위치

    // targetPosition에 위의 origin 위치랑 moved 위치를 대입해서 거기까지 움직이게 함.
    private RectTransform rectTransform;

    [SerializeField] private float moveDuration = 0.3f; // 이동에 걸리는 시간

    private bool isMoving = false; // 의자가 움직이는 중인지 여부

    private void Awake()
    {
        ResultManager.Instance.RegisterExecutable($"Chair{sideNum}", this);
        rectTransform = GetComponent<RectTransform>();

        originalPosition = rectTransform.anchoredPosition;  // 초기 위치 저장
        switch (sideNum)  // 사이드별 이동한 의자 위치
        {
            case 1:
                movedPositions[1] = new Vector2(-125f, originalPosition.y);
                break;
            case 2:
                movedPositions[2] = new Vector2(652f, -497f);
                break;
        }
    }

    public new void OnMouseDown()
    {
        bool isBusy = GameManager.Instance.GetIsBusy(); 
        
        if (isMoving | isBusy) return;
        
        base.OnMouseDown();
    }
    
    public void ExecuteAction()
    {
        bool chairMoved = (bool)GameManager.Instance.GetVariable("ChairMoved");
        Vector2 targetPosition = chairMoved ? originalPosition : movedPositions[sideNum];
        if (isActiveAndEnabled) StartCoroutine(MoveChair(targetPosition));
    }
    
    IEnumerator MoveChair(Vector2 targetPosition)
    {
        isMoving = true;

        float elapsedTime = 0;
        Vector2 startingPosition = rectTransform.anchoredPosition;

        while (elapsedTime < moveDuration)
        {
            rectTransform.anchoredPosition = Vector2.Lerp(startingPosition, targetPosition, (elapsedTime / moveDuration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        rectTransform.anchoredPosition = targetPosition;
        
        isMoving = false;
    }

    private void OnEnable()
    {
        bool chairMoved = (bool)GameManager.Instance.GetVariable("ChairMoved");
        rectTransform.anchoredPosition = chairMoved ? movedPositions[sideNum] : originalPosition;
    }
}