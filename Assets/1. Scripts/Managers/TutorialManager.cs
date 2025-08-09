using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public enum eTutorialObjectName
    {
        CarpetClosed,
        CarpetOpen,
        Chair,
        LoR
    }
    
    private int currentPhase;
    private static readonly string resultStartDialogue = "Result_StartDialogue";
    private readonly string resultTutorialPhase1 = resultStartDialogue + "Tutorial_000";
    private readonly string resultTutorialPhase2A = resultStartDialogue + "Tutorial_002_A";
    private readonly string resultTutorialPhase4A = resultStartDialogue + "Tutorial_004_A";
    private readonly string resultTutorialPhase6 = resultStartDialogue + "Tutorial_006";
    private bool[] seenSides;

    [SerializeField] private GameObject carpetClosedGameObject;
    [SerializeField] private GameObject carpetOpenGameObject;
    [SerializeField] private GameObject chairGameObject;
    [SerializeField] private GameObject letterOfResignationGameObject;
    
    public Dictionary<eTutorialObjectName, GameObject> tutorialGameObjects = new();

    public void StartTutorial() {
        tutorialGameObjects.Add(eTutorialObjectName.CarpetClosed, carpetClosedGameObject);
        tutorialGameObjects.Add(eTutorialObjectName.CarpetOpen, carpetOpenGameObject);
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
            
            case 4:
                ResultManager.Instance.ExecuteResult(resultTutorialPhase4A);
                break;
                
            case 5:
                UIManager.Instance.ToggleHighlightAnimationEffect(tutorialGameObjects[eTutorialObjectName.LoR], false);
                ToggleCollider(eTutorialObjectName.LoR, false);
                break;
            
            case 6:
                MemoManager.Instance.SetShouldHideMemoButton(false);
                UIManager.Instance.ToggleHighlightAnimationEffect(eUIGameObjectName.MemoButton, true);
                ResultManager.Instance.ExecuteResult(resultTutorialPhase6);
                
                CompleteTutorial();
                break;
        }
    }

    private void CompleteTutorial() {
        GameManager.Instance.SetVariable("isTutorial", false);
        UIManager.Instance.SetUI(eUIGameObjectName.BlockingPanelDefault, false);
    }
    
    public void ToggleCollider(eTutorialObjectName objectName, bool isOn)
    {
        foreach (Collider2D collider in tutorialGameObjects[objectName].GetComponents<Collider2D>())
            collider.enabled = isOn;
    }
}
