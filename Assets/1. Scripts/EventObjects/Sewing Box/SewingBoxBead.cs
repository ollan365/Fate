using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SewingBoxBead : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public RectTransform[] dropZones; // 드롭할 수 있는 영역들
    [SerializeField] private bool canDrag;

    // 비즈의 행 위치
    [Header("비즈의 행 위치")]
    public int beadRow;

    [Header("현재 비즈 위치")]
    public int currentPositionNumber;  // 현재 비즈 위치 [1,X]의 X (열) 정보 저장
    [Header("비즈 고유 번호")]
    public int beadNameNumber;  // 비즈 고유 번호

    [SerializeField] private float moveDuration = 0.3f; // 이동에 걸리는 시간

    [SerializeField] private bool isConflict = false;

    private RectTransform originalParent;

    private RectTransform rectTransform;

    float fixedPositionXvalue;


    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();

        // 시작 시 위치 번호 저장
        if (transform.parent != null)
        {
            currentPositionNumber = ParseColumn(transform.parent.name);
        }

        fixedPositionXvalue = gameObject.transform.position.x;
        
        // (옵션) 드래그 감도 조정
        //EventSystem.current.pixelDragThreshold = 1;
    }


    public void OnBeginDrag(PointerEventData eventData)
    {
        // 비밀번호 무한 입력 시도 방지
        RoomManager.Instance.ProhibitInput();

        originalParent = (RectTransform)transform.parent;
        transform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            (RectTransform)transform.parent,
            eventData.position,
            eventData.pressEventCamera,
            out pos);

        pos.x = rectTransform.anchoredPosition.x; // x축 값 고정해서 비즈 위치 보정
        rectTransform.anchoredPosition = pos;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        RectTransform closestZone = GetClosestDropZone(eventData);
        RectTransform validZone = GetValidDropZone(closestZone);

        if (validZone != null)
        {
            // 부드럽게 이동
            StartCoroutine(SmoothMoveToParent(validZone, moveDuration));

            // 위치 값 갱신
            currentPositionNumber = ParseColumn(validZone.name);
        }
        else
        {
            StartCoroutine(SmoothMoveToParent(originalParent, moveDuration));
            currentPositionNumber = ParseColumn(originalParent.name);
        }
    }

    RectTransform GetClosestDropZone(PointerEventData eventData)
    {
        RectTransform bestDrop = null;
        float minDist = float.MaxValue;

        foreach (var zone in dropZones)
        {
            // 월드 좌표를 스크린 좌표로 변환
            Vector2 zoneScreenPos = RectTransformUtility.WorldToScreenPoint(eventData.pressEventCamera, zone.position);
            float dist = Vector2.Distance(zoneScreenPos, eventData.position);

            if (dist < minDist)
            {
                minDist = dist;
                bestDrop = zone;
            }
        }

        return bestDrop;
    }

    RectTransform GetValidDropZone(RectTransform target)
    {
        if (beadRow != 1) return target;

        int targetCol = ParseColumn(target.name);
        int otherBeadNumber = (beadNameNumber == 1) ? 2 : 1;
        int otherCol = FindBeadColumn(otherBeadNumber);

        if (otherCol == -1) return target;

        int allowedCol = -1;
        if (beadNameNumber == 1 && targetCol >= otherCol)
            allowedCol = otherCol - 1;
        else if (beadNameNumber == 2 && targetCol <= otherCol)
            allowedCol = otherCol + 1;

        if (allowedCol != -1)
        {
            foreach (var zone in dropZones)
            {
                if (ParseColumn(zone.name) == allowedCol)
                    return zone;
            }
            return null;
        }

        return target;
    }

    int ParseColumn(string name)
    {
        string[] parts = name.Split(',');
        return int.Parse(parts[1]);
    }

    int FindBeadColumn(int targetBeadNumber)
    {
        string targetName = "Bead" + targetBeadNumber;
        foreach (var zone in dropZones)
        {
            Transform child = zone.Find(targetName);
            if (child != null)
                return ParseColumn(zone.name);
        }
        return -1;
    }

    IEnumerator SmoothMoveToParent(RectTransform targetParent, float duration)
    {
        SoundPlayer.Instance.UISoundPlay(Constants.Sound_SewingBoxBall);

        Vector3 startPos = rectTransform.position;
        Vector3 endPos = targetParent.position;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            rectTransform.position = Vector3.Lerp(startPos, endPos, elapsed / duration);
            yield return null;
        }

        rectTransform.position = endPos;

        // 부모 재설정은 이동 끝난 후 마지막에 하기
        transform.SetParent(targetParent);
        rectTransform.localPosition = Vector3.zero;
    }
}
