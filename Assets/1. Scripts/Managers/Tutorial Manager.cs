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
                || SaveManager.Instance.EndingData.allEndingCollectCount != 0
                || (bool)GameManager.Instance.GetVariable("EndTutorial_ROOM_1"))
            return;

        GameManager.Instance.SetVariable("isTutorial", true);
        GameManager.Instance.SetVariable("TutorialPhase", 1);
        RoomManager.Instance.imageAndLockPanelManager.SetBlockingPanel();
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
        RoomManager.Instance.imageAndLockPanelManager.SetBlockingPanel();

        MemoManager.Instance.SetMemoButtons(true);
    }
    
}
