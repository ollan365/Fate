using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    private List<bool> seenSides = new List<bool>() { true, false, false };
    private int currentPhase = 1;

    void Start()
    {
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
    }
    
}
