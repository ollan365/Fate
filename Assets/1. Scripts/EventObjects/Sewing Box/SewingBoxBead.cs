using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SewingBoxBead : MonoBehaviour, IDragHandler, IEndDragHandler
{
    public RectTransform[] dropZones; // ����� �� �ִ� ������
    private Vector3 offset;
    private float zCoord;
    private int newParentIndex;
    public BoxCollider2D boxCollider;
    private bool canDrag;

    // ������ �� ��ġ
    [Header("������ �� ��ġ")]
    public int beadRow;

    [Header("���� ���� ��ġ")]
    public int currentPositionNumber;  // ���� ���� ��ġ
    [Header("���� ���� ��ȣ")]
    public int beadNameNumber;  // ���� ���� ��ȣ

    [SerializeField] private float moveDuration = 0.3f; // �̵��� �ɸ��� �ð�

    // 1�� ���� �ٸ� ����
    public GameObject anotherBead;


    private void Start()
    {
        newParentIndex = -1; // �ʱⰪ ����, ��ȿ���� ���� �ε���

        boxCollider = gameObject.GetComponent<BoxCollider2D>();
        if (boxCollider == null)
        {
            boxCollider = gameObject.AddComponent<BoxCollider2D>();
        }

        if (beadRow == 1)
        {
            if (beadNameNumber == 1)
            {
                anotherBead = GameObject.Find("Bead2");
            }
            else if (beadNameNumber == 2)
            {
                anotherBead = GameObject.Find("Bead1");
            }
        }
    }

    private void OnMouseDown()
    {
        zCoord = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;
        offset = gameObject.transform.position - GetMouseAsWorldPoint();
    }

    private Vector3 GetMouseAsWorldPoint()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = zCoord;
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        canDrag = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (canDrag)
        {
            Vector3 targetPos = GetMouseAsWorldPoint() + offset;
            float newY = transform.position.y;

            int currentParentIndex = currentPositionNumber - 1;

            // ���ѵ� ���� �̵� ���
            if (currentParentIndex > 0 && currentParentIndex < dropZones.Length - 1)
            {
                newY = targetPos.y;
            }
            else if (currentParentIndex == 0 && targetPos.y < transform.position.y)
            {
                newY = targetPos.y;
            }
            else if (currentParentIndex == dropZones.Length - 1 && targetPos.y > transform.position.y)
            {
                newY = targetPos.y;
            }

            // �浹 ����
            if (CheckCollision(new Vector3(transform.position.x, newY, transform.position.z)))
            {
                return;
            }

            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canDrag = false;

        RectTransform newParent = null;
        newParentIndex = -1;

        for (int i = 0; i < dropZones.Length; i++)
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(dropZones[i], Input.mousePosition, Camera.main))
            {
                newParent = dropZones[i];
                newParentIndex = i;
                break;
            }
        }

        if (newParent != null && IsDropAllowed(newParentIndex))
        {
            StartCoroutine(MoveBead(newParent.position, newParent));
            currentPositionNumber = newParentIndex + 1;
        }
        else
        {
            if (beadRow == 1)
            {
                if (isConflict)
                {
                    int anotherBeadIndex = anotherBead.GetComponent<SewingBoxBead>().currentPositionNumber - 1;
                    switch (beadNameNumber)
                    {
                        case 1:
                            int newPositionIndex = anotherBeadIndex - 1;
                            if (newPositionIndex < 1) newPositionIndex = 1;

                            newParent = dropZones[newPositionIndex];
                            StartCoroutine(MoveBead(newParent.position, newParent));

                            currentPositionNumber = newPositionIndex + 1;
                            break;

                        case 2:
                            newPositionIndex = anotherBeadIndex + 1;
                            if (newPositionIndex > 4) newPositionIndex = 4;

                            newParent = dropZones[newPositionIndex];
                            StartCoroutine(MoveBead(newParent.position, newParent));

                            currentPositionNumber = newPositionIndex + 1;
                            break;

                        default:
                            return;
                    }

                }

            } 
            else return;
        }

    }

    private bool isConflict = false;

    private bool IsDropAllowed(int targetIndex)
    {
        int currentParentIndex = currentPositionNumber - 1;

        if (beadNameNumber == 1)
        {
            if (targetIndex == 4)
            {
                isConflict = true;
                return false;
            }
            if ((anotherBead.GetComponent<SewingBoxBead>().currentPositionNumber - 1) < targetIndex)
            {
                isConflict = true;
                return false;
            }
        }

        if (beadNameNumber == 2)
        {
            if (targetIndex == 0)
            {
                isConflict = true;
                return false;
            }
            if ((anotherBead.GetComponent<SewingBoxBead>().currentPositionNumber - 1) > targetIndex)
            {
                isConflict = true;
                return false;
            }
        }

        if (targetIndex < 0 || targetIndex >= dropZones.Length) return false;
        if (targetIndex == currentParentIndex) return false;

        // ��ǥ ��ġ�� �ٸ� �ڽ��� �ִ��� Ȯ��
        return dropZones[targetIndex].childCount == 0;
    }

    private bool CheckCollision(Vector3 targetPosition)
    {
        foreach (RectTransform dropZone in dropZones)
        {
            if (dropZone == transform.parent) continue;

            foreach (Transform child in dropZone)
            {
                if (child != transform && child.GetComponent<RectTransform>().rect.Contains(targetPosition))
                {
                    return true;
                }
            }
        }

        return false;
    }


    IEnumerator MoveBead(Vector3 targetPosition, RectTransform rect)
    {
        float elapsedTime = 0;
        Vector3 startingPosition = transform.position;

        while (elapsedTime < moveDuration)
        {
            transform.position = Vector3.Lerp(startingPosition, targetPosition, (elapsedTime / moveDuration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.SetParent(rect);
        transform.localPosition = Vector3.zero;
    }
}
