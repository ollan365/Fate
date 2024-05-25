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
    // 나중에 Awake에서 비즈 이름에 따라서 저 positions에 위치 게임오브젝트들 넣게 수정하기 
    // Lock 시스템 만들어서 그건 아예 Board에 붙이고 거기선 2차원 배열 만들어서 해당 위치에 있는 비즈 가져오고 올바른 위치면 맞게 하기..?

    public int currentPosition;  // 현재 비즈 위치

    public int beadRaw;
    public string beadName;

    [SerializeField] private float moveDuration = 0.3f; // 이동에 걸리는 시간

    private RectTransform rectTransform;
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void BeadClick()
    {
        // 현재 비즈의 위 아래를 검사했을 때 위나 아래에 다른 비즈가 있다면 해당 비즈 위치를 제외 하고 이동 버튼 나오게 하기
        // 현재 비즈의 y위치가 1이면 down버튼만, 5면 up버튼만 나오게 하기


        if (UpButton.activeSelf || DownButton.activeSelf)
        {
            UpButton.SetActive(false);
            DownButton.SetActive(false);
        }
        else if (!UpButton.activeSelf && !DownButton.activeSelf)
        {
            UpButton.SetActive(true);
            DownButton.SetActive(true);
        }

    }

    public void MoveBeadButton(int nextPosition)
    {
        // nextPosition이 -1이면 Up, 1이면 Down.
        currentPosition += nextPosition;
        Debug.Log(currentPosition);

        gameObject.transform.SetParent(positions[currentPosition - 1].transform);

        Vector3 targetPosition = positions[currentPosition - 1].transform.position;

        StartCoroutine(MoveBead(targetPosition));


        // 이동하고 나면 이동 버튼 안 보이게 하기
        UpButton.SetActive(false);
        DownButton.SetActive(false);
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
