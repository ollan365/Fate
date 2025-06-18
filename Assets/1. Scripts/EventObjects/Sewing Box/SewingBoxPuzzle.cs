using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SewingBoxPuzzle : MonoBehaviour
{
    public static SewingBoxPuzzle Instance { get; private set; }

    private bool isBeadsCorrect = false;

    [SerializeField] private SewingBox sewingBoxA;

    [Header("�����")]
    public List<SewingBoxBead> Beads;

    protected Dictionary<int, int> BeadsAnswer = new Dictionary<int, int>();

    private void Awake()
    {
        if (Instance == null) {
            Instance = this;

            settingBeadsAnswer();
        } else
            Destroy(gameObject);
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

    // �ϴ� ������ �Ѳ� ��ư ������ ���� ���� ���ϰ� ��
    public void CompareBeads()
    {
        // ��й�ȣ ���� �Է� �õ� ����
        RoomManager.Instance.ProhibitInput();

        if (isBeadsCorrect) 
            return;

        // ����� �����ͼ� �˻�
        if (CheckBeadsAnswer())
        {
            GameManager.Instance.SetVariable("SewingBoxCorrect", true);
            // ���� ���� ���Ŀ� ������ �ٽ� Ŭ���ϸ� ����â �н��Ȱ� �ٽ� ����â ������ ��.
            sewingBoxA.SetIsInquiry(true);
            isBeadsCorrect = true;

            //Debug.Log("������ϴ�!");
        }
        else
        {
            //Debug.Log("Ʋ�Ƚ��ϴ�!");
        }

        EventManager.Instance.CallEvent("EventSewingBoxB");
    }

    private bool CheckBeadsAnswer()
    {
        for (int i = 0; i < 6; i++)
        {
            //Debug.Log((i + 1) + "�� ������ ���� ��ġ " + Beads[i].currentPositionNumber + "/ ���� ��ġ : " + BeadsAnswer[i + 1]);
            if (Beads[i].currentPositionNumber == BeadsAnswer[i + 1])
                continue;

            return false;
        }
        return true;
    }
    
}
