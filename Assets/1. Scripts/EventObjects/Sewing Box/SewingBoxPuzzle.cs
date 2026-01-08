using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SewingBoxPuzzle : MonoBehaviour
{
    private bool isBeadsCorrect = false;

    [SerializeField] private SewingBox sewingBoxA;

    [Header("비즈들")]
    public List<SewingBoxBead> Beads;

    protected Dictionary<int, int> BeadsAnswer = new Dictionary<int, int>();

    private void Start()
    {
        settingBeadsAnswer();
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

    // 하단 반짇고리 뚜껑 버튼 누르면 퍼즐 정답 비교하게 함
    public void CompareBeads()
    {
        // 비밀번호 무한 입력 시도 방지
        RoomManager.Instance.ProhibitInput();

        if (isBeadsCorrect) 
            return;

        // 비즈들 가져와서 검사
        if (CheckBeadsAnswer())
        {
            GameManager.Instance.SetVariable("SewingBoxCorrect", true);
            // 퍼즐 맞춘 이후에 반짇고리 다시 클릭하면 조사창 패스된거 다시 조사창 나오게 함.
            sewingBoxA.SetIsInquiry(true);
            isBeadsCorrect = true;
        }
        else
        {
            GameManager.Instance.SetVariable("SewingBoxCorrect", false);
        }

        EventManager.Instance.CallEvent("EventSewingBoxB");
    }

    private bool CheckBeadsAnswer()
    {
        for (int i = 0; i < 6; i++)
        {
            //Debug.Log((i + 1) + "번 비즈의 현재 위치 " + Beads[i].currentPositionNumber + "/ 정답 위치 : " + BeadsAnswer[i + 1]);
            if (Beads[i].currentPositionNumber == BeadsAnswer[i + 1])
                continue;

            return false;
        }
        return true;
    }
}
