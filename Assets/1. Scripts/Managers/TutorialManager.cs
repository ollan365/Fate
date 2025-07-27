using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public enum eTutorialObjectName
    {
        Carpet,
        Chair,
        LoR
    }
    
    private int currentPhase;
    private readonly string resultTutorialPhase1 = "Result_StartDialogue" + "Tutorial_000";
    private readonly string resultTutorialPhase2A = "Result_StartDialogue" + "Tutorial_002_A";
    private bool[] seenSides;

    [SerializeField] private GameObject carpetGameObject;
    [SerializeField] private GameObject chairGameObject;
    [SerializeField] private GameObject letterOfResignationGameObject;
    
    public Dictionary<eTutorialObjectName, GameObject> tutorialGameObjects = new();

    public void StartTutorial() {
        tutorialGameObjects.Add(eTutorialObjectName.Carpet, carpetGameObject);
        tutorialGameObjects.Add(eTutorialObjectName.Chair, chairGameObject);
        tutorialGameObjects[eTutorialObjectName.Chair].GetComponent<CapsuleCollider2D>().enabled = false;  // 의자 클릭 안되게 함
        tutorialGameObjects.Add(eTutorialObjectName.LoR, letterOfResignationGameObject);
        
        GameManager.Instance.SetVariable("isTutorial", true);
        currentPhase = (int)GameManager.Instance.GetVariable("TutorialPhase");
        
        seenSides = new bool[] {false, false, false, false};
        seenSides[0] = true;

        MemoManager.Instance.SetShouldHideMemoButton(true);
        UIManager.Instance.SetUI(eUIGameObjectName.MemoButton, false);
        UIManager.Instance.SetUI(eUIGameObjectName.BlockingPanelDefault, true);

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
        currentPhase++;
        GameManager.Instance.SetVariable("TutorialPhase", currentPhase);
        switch (currentPhase) {
            case 1:
                ResultManager.Instance.ExecuteResult(resultTutorialPhase1);
                break;

            case 2:
                UIManager.Instance.ToggleHighlightAnimationEffect(eUIGameObjectName.LeftButton, false);
                UIManager.Instance.ToggleHighlightAnimationEffect(eUIGameObjectName.RightButton, false);

                ResultManager.Instance.ExecuteResult(resultTutorialPhase2A);
                break;
        }
    }

    private void completeTutorial() {
        GameManager.Instance.SetVariable("isTutorial", false);
    }
}
