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

    // ������ �� ��ġ
    [Header("������ �� ��ġ")]
    public int beadRow;

    [Header("���� ���� ��ġ")]
    public int currentPositionNumber;  // ���� ���� ��ġ
    [Header("���� ���� ��ȣ")]
    public int beadNameNumber;  // ���� ���� ��ȣ

    [SerializeField] private float moveDuration = 0.3f; // �̵��� �ɸ��� �ð�

    public bool isMoveButtonVisible = false;

    //// ���� Ŭ���� �� ȣ��� �̺�Ʈ
    //public delegate void BeadClickedEventHandler(int currentBeadNumber);
    //public static event BeadClickedEventHandler OnBeadClicked;


    public void BeadClick()
    {
        // ���� ������ �� �Ʒ��� �˻����� �� ���� �Ʒ��� �ٸ� ��� �ִٸ� �ش� ���� ��ġ�� ���� �ϰ� �̵� ��ư ������ �ϱ�
        // ���� currentPositionNumber -1, �Ʒ��� currentPositionNumber +1
        // ���� ������ y��ġ�� 1�̸� down��ư��, 5�� up��ư�� ������ �ϱ�

        // ���� Ŭ���ϸ� �̵� ��ư ���Դٰ� �ٽ� Ŭ���ϸ� �̵� ��ư ��
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

            // ���� ��ġ �˻��ϰ�, 1�̳� 5�� down��ư��, up��ư�� ����.
            switch (currentPositionNumber)
            {
                case 1:
                    // down��ư�� ����
                    UpButton.SetActive(false);
                    DownButton.SetActive(true);
                    break;

                case 5:
                    // up��ư�� ����
                    UpButton.SetActive(true);
                    DownButton.SetActive(false);
                    break;

                default:    // 2~4 ��ġ
                    UpButton.SetActive(true);
                    DownButton.SetActive(true);
                    break;
            }
            
            // �׸��� ���� ���� �ֺ��� �ٸ� ���� ������(�׷��� �̰� beadRaw==1�� �͸� �˻��ϰ� ��)
            if (beadRow == 1)
            {
                // �� �Ʒ��� �ٸ� ���� ���� �˻��ϰ� �ش� ��ġ�� ���� ������ �� ��ġ�� �̵� ��ư �� ������ ��
                int upPositionNumber = currentPositionNumber - 1;
                int downPositionNumber = currentPositionNumber + 1;

                // positions[upPositionNumber - 1].childCount > 0 �̸� �ش� ���� �̵� ��ư X
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
        // nextPosition�� -1�̸� Up, 1�̸� Down.
        currentPositionNumber += nextPosition;
        //Debug.Log(currentPositionNumber);

        gameObject.transform.SetParent(positions[currentPositionNumber - 1].transform);

        Vector3 targetPosition = positions[currentPositionNumber - 1].transform.position;

        StartCoroutine(MoveBead(targetPosition));


        // �̵��ϰ� ���� �̵� ��ư �� ���̰� �ϱ�
        UpButton.SetActive(false);
        DownButton.SetActive(false);

        // �׸��� �̵���ưVisible ������ false�� �ٲ�.
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


