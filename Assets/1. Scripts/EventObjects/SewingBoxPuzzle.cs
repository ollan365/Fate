using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SewingBoxPuzzle : MonoBehaviour
{
    public static SewingBoxPuzzle Instance { get; private set; }

    private bool isBeadsCorrect = false;

    [SerializeField] private SewingBox sewingBoxA;

    // �ٸ� ���� ������ ���� ���� �ִ� ������ �̵� ��ư ������ ��
    public int focusedBeadNumber;

    [Header("�����")]
    public List<SewingBoxBead> Beads;

    protected Dictionary<int, int> BeadsAnswer = new Dictionary<int, int>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            settingBeadsAnswer();
        }
        else
        {
            Destroy(gameObject);
        }
    }


    private void settingBeadsAnswer()
    {
        BeadsAnswer.Add(1,1);
        BeadsAnswer.Add(2,3);
        BeadsAnswer.Add(3,2);
        BeadsAnswer.Add(4,4);
        BeadsAnswer.Add(5,3);
        BeadsAnswer.Add(6,5);
    }

    public void FocusClickedBead(int clickedBeadNumber)
    {
        //Debug.Log("���� Ŭ���� ���� �ѹ� : " + focusedBeadNumber);
        //Debug.Log("���� Ŭ���� ���� �ѹ� : " + clickedBeadNumber);
        // ���� �� ó�� ���¶��
        if (focusedBeadNumber == 0)
        {
            // �ٷ� Ŭ���� ������ �̵� ��ư�� ������ ��
            // focusedBeadNumber�� Ŭ���� �ش� ������ �ѹ��� ������Ʈ ��
            focusedBeadNumber = clickedBeadNumber;
            return;
        }
        else
        {
            // ���� Ŭ���� ������ ������ �̵���ư�� ��
            if (focusedBeadNumber != clickedBeadNumber && Beads[focusedBeadNumber - 1].isMoveButtonVisible)
            {
                Beads[focusedBeadNumber - 1].BeadClick();
            }
            focusedBeadNumber = clickedBeadNumber;
            return;
        }
    }


    // �ϴ� ������ �Ѳ� ��ư ������ ���� ���� ���ϰ� ��
    public void CompareBeads()
    {
        if (isBeadsCorrect) return;

        // ����� �����ͼ� �˻�
        if (CheckBeadsAnswer())
        {
            GameManager.Instance.SetVariable("SewingBoxCorrect", true);
            // ���� ���� ���Ŀ� ������ �ٽ� Ŭ���ϸ� ����â �н��Ȱ� �ٽ� ����â ������ ��.
            sewingBoxA.SetIsInquiry(true);
            isBeadsCorrect = true;

            Debug.Log("������ϴ�!");
        }
        else
        {
            Debug.Log("Ʋ�Ƚ��ϴ�!");
        }

        EventManager.Instance.CallEvent("EventSewingBoxB");
    }

    private bool CheckBeadsAnswer()
    {
        for (int i = 0; i < 6; i++)
        {
            //Debug.Log((i + 1) + "�� ������ ���� ��ġ " + Beads[i].currentPositionNumber + "/ ���� ��ġ : " + BeadsAnswer[i + 1]);
            if (Beads[i].currentPositionNumber == BeadsAnswer[i + 1])
            {
                continue;
            }
            else
            {
                return false;
            }
        }
        return true;
    }
    
}
