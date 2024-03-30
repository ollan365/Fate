
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Chair : EventObject, IResultExecutable
{
    private Vector3 originalPosition;
    private Vector3 movedPosition1; // 사이드 1번 방 의자 움직임
    [SerializeField] private Vector3 movedPosition2; // 사이드 2번 방 의자 움직임

    // targetPosition에 위의 origin 위치랑 moved 위치를 대입해서 거기까지 움직이게 함.
    private Vector3 targetPosition;
    private RectTransform buttonRectTransform;

    public float speed = 6f; // 이동 속도

    public bool isMoving = false; // 의자가 움직이는지 여부

    private void Awake()
    {
        ResultManager.Instance.RegisterExecutable($"Chair{sideNum}", this);
        
        buttonRectTransform = GetComponent<RectTransform>();

        switch (sideNum)
        {
            case 1:
                // 사이드 1번 방의 의자 위치
                movedPosition1 = originalPosition = buttonRectTransform.anchoredPosition;
                movedPosition1.x = -125f;
                break;
            case 2:
                // 사이드 2번 방의 의자 위치 
                movedPosition2 = originalPosition = buttonRectTransform.anchoredPosition;
                movedPosition2.x = 652f;
                movedPosition2.y = -497f;
                break;
        }
    }

    // 의자 천천히 움직이게 하기
    void Update()
    {
        if (isMoving)
        {
            StartCoroutine(MoveChairCoroutine());
        }
    }

    void OnEnable()
    {
        // 다른 사이드에서 이미 의자 움직였으면 해당 사이드에도 적용함
        StartCoroutine(readyChairCoroutine());
    }

    public new void OnMouseDown()
    {
        if (!isMoving)
        {
            base.OnMouseDown();
            
            switch (sideNum)
            {
                case 1:
                    GameManager.Instance.InverseVariable("ChairMoved1");
                    break;
                case 2:
                    GameManager.Instance.InverseVariable("ChairMoved2");
                    break;
            }
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

            switch (sideNum)
            {
                case 1:
                    targetPosition = ((bool)GameManager.Instance.GetVariable("ChairMoved1")) ? originalPosition : movedPosition1;
                    break;
                case 2:
                    targetPosition = ((bool)GameManager.Instance.GetVariable("ChairMoved2")) ? originalPosition : movedPosition2;
                    break;
            }
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

    private IEnumerator readyChairCoroutine()
    {
        if ((bool)GameManager.Instance.GetVariable("ChairMoved1") != (bool)GameManager.Instance.GetVariable("ChairMoved2"))
        {
            //Debug.Log(sideNum+" 의자 보정 전- 1번 의자 : "+ (bool)GameManager.Instance.GetVariable("ChairMoved1")
            //+" 2번 의자 : " + (bool)GameManager.Instance.GetVariable("ChairMoved2"));
            switch (sideNum)
            {
                case 1:
                    if ((bool)GameManager.Instance.GetVariable("ChairMoved2"))
                        buttonRectTransform.anchoredPosition = movedPosition1;
                    else
                        buttonRectTransform.anchoredPosition = originalPosition;
                    GameManager.Instance.InverseVariable("ChairMoved1");
                    break;
                case 2:
                    if ((bool)GameManager.Instance.GetVariable("ChairMoved1"))
                        buttonRectTransform.anchoredPosition = movedPosition2;
                    else
                        buttonRectTransform.anchoredPosition = originalPosition;
                    GameManager.Instance.InverseVariable("ChairMoved2");
                    break;
            }
            //Debug.Log(sideNum + " 의자 보정 후- 1번 의자 : " + (bool)GameManager.Instance.GetVariable("ChairMoved1")
            //+ " 2번 의자 : " + (bool)GameManager.Instance.GetVariable("ChairMoved2"));
        }
        yield return null;
    }
}