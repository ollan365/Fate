using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SewingBoxBead : MonoBehaviour
{
    [Header("비즈 위 아래 이동 버튼")]
    [SerializeField] GameObject UpButton;
    [SerializeField] GameObject DownButton;

    [Header("비즈 움직일 수 있는 위치")]
    public List<GameObject> positions;  // 움직일 수 있는 위치들

    // 비즈의 행 위치
    [Header("비즈의 행 위치")]
    public int beadRow;

    [Header("현재 비즈 위치")]
    public int currentPositionNumber;  // 현재 비즈 위치
    [Header("비즈 고유 번호")]
    public int beadNameNumber;  // 비즈 고유 번호

    [SerializeField] private float moveDuration = 0.3f; // 이동에 걸리는 시간

    public bool isMoveButtonVisible = false;

    //// 비즈 클릭될 때 호출될 이벤트
    //public delegate void BeadClickedEventHandler(int currentBeadNumber);
    //public static event BeadClickedEventHandler OnBeadClicked;


    public void BeadClick()
    {
        // 현재 비즈의 위 아래를 검사했을 때 위나 아래에 다른 비즈가 있다면 해당 비즈 위치를 제외 하고 이동 버튼 나오게 하기
        // 위는 currentPositionNumber -1, 아래는 currentPositionNumber +1
        // 현재 비즈의 y위치가 1이면 down버튼만, 5면 up버튼만 나오게 하기

        // 비즈 클릭하면 이동 버튼 나왔다가 다시 클릭하면 이동 버튼 끔
        if (UpButton.activeSelf || DownButton.activeSelf)
        {
            UpButton.SetActive(false);
            DownButton.SetActive(false);

            isMoveButtonVisible = false;
        }
        else if (!UpButton.activeSelf && !DownButton.activeSelf)
        {
            isMoveButtonVisible = true;

            // 
            SewingBoxPuzzle.Instance.FocusClickedBead(beadNameNumber);

            // 현재 위치 검사하고, 1이나 5면 down버튼만, up버튼만 나옴.
            switch (currentPositionNumber)
            {
                case 1:
                    // down버튼만 나옴
                    UpButton.SetActive(false);
                    DownButton.SetActive(true);
                    break;

                case 5:
                    // up버튼만 나옴
                    UpButton.SetActive(true);
                    DownButton.SetActive(false);
                    break;

                default:    // 2~4 위치
                    UpButton.SetActive(true);
                    DownButton.SetActive(true);
                    break;
            }
            
            // 그리고 만일 비즈 주변에 다른 비즈 있으면(그런데 이건 beadRaw==1인 것만 검사하게 함)
            if (beadRow == 1)
            {
                // 위 아래에 다른 비즈 없나 검사하고 해당 위치에 비즈 있으면 그 위치는 이동 버튼 안 나오게 함
                int upPositionNumber = currentPositionNumber - 1;
                int downPositionNumber = currentPositionNumber + 1;

                // positions[upPositionNumber - 1].childCount > 0 이면 해당 방향 이동 버튼 X
                if (upPositionNumber > 0 && positions[upPositionNumber - 1].transform.childCount > 0)
                {
                    UpButton.SetActive(false);
                    return;
                }
                else if (downPositionNumber < 5 && positions[downPositionNumber - 1].transform.childCount > 0)
                {
                    DownButton.SetActive(false);
                    return;
                }
                return;
            }

        }


    }


    public void MoveBeadButton(int nextPosition)
    {
        // nextPosition이 -1이면 Up, 1이면 Down.
        currentPositionNumber += nextPosition;
        //Debug.Log(currentPositionNumber);

        gameObject.transform.SetParent(positions[currentPositionNumber - 1].transform);

        Vector3 targetPosition = positions[currentPositionNumber - 1].transform.position;

        StartCoroutine(MoveBead(targetPosition));


        // 이동하고 나면 이동 버튼 안 보이게 하기
        UpButton.SetActive(false);
        DownButton.SetActive(false);

        // 그리고 이동버튼Visible 변수도 false로 바꿈.
        isMoveButtonVisible = false;
    }

    IEnumerator MoveBead(Vector3 targetPosition)
    {
        float elapsedTime = 0;
        Vector3 startingPosition = transform.position;

        while (elapsedTime < moveDuration)
        {
            transform.position = Vector3.Lerp(startingPosition, targetPosition, (elapsedTime / moveDuration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;
    }
}


