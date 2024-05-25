using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SewingBoxBead : MonoBehaviour
{
    [Header("���� �� �Ʒ� �̵� ��ư")]
    [SerializeField] GameObject UpButton;
    [SerializeField] GameObject DownButton;

    [Header("���� ������ �� �ִ� ��ġ")]
    public List<GameObject> positions;  // ������ �� �ִ� ��ġ��
    // ���߿� Awake���� ���� �̸��� ���� �� positions�� ��ġ ���ӿ�����Ʈ�� �ְ� �����ϱ� 
    // Lock �ý��� ���� �װ� �ƿ� Board�� ���̰� �ű⼱ 2���� �迭 ���� �ش� ��ġ�� �ִ� ���� �������� �ùٸ� ��ġ�� �°� �ϱ�..?

    public int currentPosition;  // ���� ���� ��ġ

    public int beadRaw;
    public string beadName;

    [SerializeField] private float moveDuration = 0.3f; // �̵��� �ɸ��� �ð�

    private RectTransform rectTransform;
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void BeadClick()
    {
        // ���� ������ �� �Ʒ��� �˻����� �� ���� �Ʒ��� �ٸ� ��� �ִٸ� �ش� ���� ��ġ�� ���� �ϰ� �̵� ��ư ������ �ϱ�
        // ���� ������ y��ġ�� 1�̸� down��ư��, 5�� up��ư�� ������ �ϱ�


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
        // nextPosition�� -1�̸� Up, 1�̸� Down.
        currentPosition += nextPosition;
        Debug.Log(currentPosition);

        gameObject.transform.SetParent(positions[currentPosition - 1].transform);

        Vector3 targetPosition = positions[currentPosition - 1].transform.position;

        StartCoroutine(MoveBead(targetPosition));


        // �̵��ϰ� ���� �̵� ��ư �� ���̰� �ϱ�
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
