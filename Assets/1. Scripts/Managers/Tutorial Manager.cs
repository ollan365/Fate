using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    private List<bool> seenSides = new List<bool>() { true, false, false };
    private int currentPhase = 1;

    void Start()
    {
        if (GameManager.Instance.skipTutorial) return;
        GameManager.Instance.SetVariable("isTutorial", true);
        GameManager.Instance.SetVariable("TutorialPhase", 1);
        RoomManager.Instance.imageAndLockPanelManager.SetBlockingPanel();
    }
    
    public void SetSeenSides(int seenSideIndex)
    {
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
        RoomManager.Instance.imageAndLockPanelManager.SetBlockingPanel();

        MemoManager.Instance.HideMemoButton(false);
        //// 휴식 버튼도 보이게 함
        //RoomManager.Instance.HideRestButton(false);
    }
    
}
