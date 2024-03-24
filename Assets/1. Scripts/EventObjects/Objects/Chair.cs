
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Chair : EventObject, IResultExecutable
{
    private Vector3 originalPosition;
    private Vector3 movedPosition;
    // targetPosition에 위의 origin 위치랑 moved 위치를 대입해서 거기까지 움직이게 함.
    private Vector3 targetPosition;
    private RectTransform buttonRectTransform;

    public float speed = 6f; // 이동 속도

    private bool isMoving = false; // 의자가 움직이는지 여부

    private void Start()
    {
        ResultManager.Instance.RegisterExecutable("Chair", this);
        
        buttonRectTransform = GetComponent<RectTransform>();

        movedPosition = originalPosition = buttonRectTransform.anchoredPosition;
        movedPosition.x = -125f;
    }

    // 의자 천천히 움직이게 하기
    void Update()
    {
        if (isMoving)
        {
            StartCoroutine(MoveChairCoroutine());
        }
    }

    public new void OnMouseDown()
    {
        if (!isMoving)
        {
            base.OnMouseDown();
            GameManager.Instance.InverseVariable("ChairMoved");
        }
    }
    
    public void ExecuteAction()
    {
        MoveChair();
    }
    
    public void MoveChair()
    {
        // 의자의 현재 위치와 목표 위치가 다르면 이동 시작
        if (!isMoving)
        {
            isMoving = true;

            targetPosition = ((bool)GameManager.Instance.GetVariable("ChairMoved")) ? originalPosition : movedPosition;
        }
    }

    private IEnumerator MoveChairCoroutine()
    {
        while (isMoving)
        {
            buttonRectTransform.anchoredPosition = Vector3.Lerp(buttonRectTransform.anchoredPosition, targetPosition, Time.deltaTime * speed);

            // 목표 위치에 도달했는지 확인
            if (Vector3.Distance(buttonRectTransform.anchoredPosition, targetPosition) < 0.1f)
            {
                isMoving = false;
            }

            yield return null;
        }
    }

}