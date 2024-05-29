using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SewingBoxPuzzle : MonoBehaviour
{
    public static SewingBoxPuzzle Instance { get; private set; }

    private bool isBeadsCorrect = false;

    [SerializeField] private SewingBox sewingBoxA;

    // 다른 비즈 누르면 현재 켜져 있는 비즈의 이동 버튼 꺼지게 함
    public int focusedBeadNumber;

    [Header("비즈들")]
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
        //Debug.Log("이전 클릭된 비즈 넘버 : " + focusedBeadNumber);
        //Debug.Log("현재 클릭된 비즈 넘버 : " + clickedBeadNumber);
        // 퍼즐 맨 처음 상태라면
        if (focusedBeadNumber == 0)
        {
            // 바로 클릭된 비즈의 이동 버튼이 나오게 함
            // focusedBeadNumber를 클릭된 해당 비즈의 넘버로 업데이트 함
            focusedBeadNumber = clickedBeadNumber;
            return;
        }
        else
        {
            // 이전 클릭된 상태의 비즈의 이동버튼을 끔
            if (focusedBeadNumber != clickedBeadNumber && Beads[focusedBeadNumber - 1].isMoveButtonVisible)
            {
                Beads[focusedBeadNumber - 1].BeadClick();
            }
            focusedBeadNumber = clickedBeadNumber;
            return;
        }
    }


    // 하단 반짇고리 뚜껑 버튼 누르면 퍼즐 정답 비교하게 함
    public void CompareBeads()
    {
        if (isBeadsCorrect) return;

        // 비즈들 가져와서 검사
        if (CheckBeadsAnswer())
        {
            GameManager.Instance.SetVariable("SewingBoxCorrect", true);
            // 퍼즐 맞춘 이후에 반짇고리 다시 클릭하면 조사창 패스된거 다시 조사창 나오게 함.
            sewingBoxA.SetIsInquiry(true);
            isBeadsCorrect = true;

            Debug.Log("맞췄습니다!");
        }
        else
        {
            Debug.Log("틀렸습니다!");
        }

        EventManager.Instance.CallEvent("EventSewingBoxB");
    }

    private bool CheckBeadsAnswer()
    {
        for (int i = 0; i < 6; i++)
        {
            //Debug.Log((i + 1) + "번 비즈의 현재 위치 " + Beads[i].currentPositionNumber + "/ 정답 위치 : " + BeadsAnswer[i + 1]);
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
