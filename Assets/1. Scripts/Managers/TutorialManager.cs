using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    private int currentPhase;
    private string resultTutorialPhase1 = "Result_StartDialogue" + "Tutorial_000";
    private string resultTutorialPhase2 = "Result_StartDialogue" + "Tutorial_002_A";
    private bool[] seenSides;

    public void StartTutorial() {
        currentPhase = (int)GameManager.Instance.GetVariable("TutorialPhase");
        seenSides = new bool[] {false, false, false, false};
        
        MemoManager.Instance.SetShouldHideMemoButton(true); 
        UIManager.Instance.SetUI(eUIGameObjectName.MemoButton, false);
        UIManager.Instance.SetUI(eUIGameObjectName.BlockingPanelDefault, true);

        GameManager.Instance.SetVariable("isTutorial", true);
        ProceedToNextPhase();
    }
    
    public void SetSeenSide(int sideIndex) {
        if (currentPhase > 1)
            return;
        
        seenSides[sideIndex] = true;
        foreach (bool seen in seenSides)
            if (!seen)
                return;
        
        ProceedToNextPhase();
    }

    public void ProceedToNextPhase() {
        Debug.Log("Current tutorial phase: " + currentPhase);
        currentPhase++;
        GameManager.Instance.SetVariable("TutorialPhase", currentPhase);
        switch (currentPhase) {
            case 1:
                ResultManager.Instance.ExecuteResult(resultTutorialPhase1);
                break;

            case 2:
                UIManager.Instance.ToggleHighlightAnimationEffect(eUIGameObjectName.LeftButton, false);
                UIManager.Instance.ToggleHighlightAnimationEffect(eUIGameObjectName.RightButton, false);

                ResultManager.Instance.ExecuteResult(resultTutorialPhase2);
                break;
        }
    }

}
