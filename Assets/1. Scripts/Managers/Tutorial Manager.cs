using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    private List<bool> seenSides = new List<bool>() { true, false, false };
    private int currentPhase = 1;

    void Start()
    {
        if (GameManager.Instance.skipTutorial
                || (int)GameManager.Instance.GetVariable("ReplayCount") != 0
                || (bool)GameManager.Instance.GetVariable("EndTutorial_ROOM_1"))
            return;

        GameManager.Instance.SetVariable("isTutorial", true);
        GameManager.Instance.SetVariable("TutorialPhase", 1);
    }
    
    public void SetSeenSides(int seenSideIndex)
    {
        if (GameManager.Instance.skipTutorial) return;
        switch (currentPhase)
        {
            case 1:
                seenSides[seenSideIndex] = true;
                CheckAllSidesSeen();
                break;
            
            case 2:
                if (seenSideIndex == 0)  // 사이드 1로 왔을 때
                {
                    DialogueManager.Instance.StartDialogue("Tutorial_002_B");
                }

                break;
        }
    }

    private void CheckAllSidesSeen()
    {
        foreach (bool seenSide in seenSides)
        {
            if (!seenSide) return;
        }

        ProceedToNextPhase();
    }

    public void ProceedToNextPhase()
    {
        if (GameManager.Instance.skipTutorial) return;
        switch (currentPhase)
        {
            case 1:
                DialogueManager.Instance.StartDialogue("Tutorial_002_A");
                break;
            
            case 2:
                DialogueManager.Instance.StartDialogue("Tutorial_003");
                break;

            case 3:
                DialogueManager.Instance.StartDialogue("Tutorial_003_B");
                break;

            case 4:
                DialogueManager.Instance.StartDialogue("Tutorial_003_C");
                //메모 버튼 반짝반짝 효과
                RoomManager.Instance.imageAndLockPanelManager.SetTutorialImageObject(true, "TutorialMemoButton");
                break;

            case 5:
                DialogueManager.Instance.StartDialogue("Tutorial_004");
                EndTutorial();
                Debug.Log("튜토리얼끝");
                break;
        }
        
        IncrementCurrentPhase();
    }

    private void IncrementCurrentPhase()
    {
        currentPhase++;
        GameManager.Instance.SetVariable("TutorialPhase", currentPhase);
    }
    
    private void EndTutorial()
    {
        GameManager.Instance.SetVariable("isTutorial", false);
        GameManager.Instance.SetVariable("EndTutorial_ROOM_1", true);

        MemoManager.Instance.SetMemoButtons(true);
    }

    public IEnumerator CheckChairMovement()
    {
        // isChairMoving이 true일 때 대기
        while ((bool)GameManager.Instance.GetVariable("isChairMoving"))
        {
            yield return null; // 다음 프레임까지 대기
        }

        // isChairMoving이 false가 되면 의자 이미지 강조 실행
        ResultManager.Instance.ExecuteResult("Result_TutorialPhase2ForceSide1");
    }

    // 이동 버튼 강조 관련
    public int getSeenSideStateFalse()
    {
        int falseCount = 0; // false 값의 개수 카운트
        int falseIndex = -1; // false 값을 가진 인덱스를 저장 (-1은 초기값)

        for (int index = 0; index < seenSides.Count; index++)
        {
            if (!seenSides[index])
            {
                falseCount++;
                falseIndex = index;

                if (falseCount > 1)
                {
                    return 0; // falseCount가 2개 이상이면 아직 방 이동 버튼 안 누른 시점.
                }
            }
        }

        return falseCount == 1 ? falseIndex : 0;
    }

}
