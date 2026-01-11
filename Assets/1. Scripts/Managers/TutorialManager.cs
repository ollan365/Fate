using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Fate.Managers
{
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

        public void AddTutorialObjects() {
            tutorialGameObjects.Add(eTutorialObjectName.CarpetClosed, carpetClosedGameObject);
            tutorialGameObjects.Add(eTutorialObjectName.CarpetOpen, carpetOpenGameObject);
            tutorialGameObjects.Add(eTutorialObjectName.Chair, chairGameObject);
            tutorialGameObjects[eTutorialObjectName.Chair].GetComponent<CapsuleCollider2D>().enabled = false;  // 의자 클릭 안되게 함
            tutorialGameObjects.Add(eTutorialObjectName.LoR, letterOfResignationGameObject);
        }
    
        public void StartTutorial() {
            GameManager.Instance.SetVariable("isTutorial", true);
            currentPhase = (int)GameManager.Instance.GetVariable("TutorialPhase");
        
            seenSides = new bool[] {false, false, false, false};
            seenSides[0] = true;

            MemoManager.Instance.SetShouldHideMemoButton(true);
            UIManager.Instance.SetUI(eUIGameObjectName.MemoButton, false);
            UIManager.Instance.SetUI(eUIGameObjectName.TutorialBlockingPanel, true);

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
                    ToggleCollider(eTutorialObjectName.Chair, false);
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
                    if ((int)GameManager.Instance.GetVariable("ReplayCount") == 0)
                        UIManager.Instance.ToggleHighlightAnimationEffect(eUIGameObjectName.MemoButton, true);
                    UIManager.Instance.ToggleHighlightAnimationEffect(tutorialGameObjects[eTutorialObjectName.CarpetOpen], false);
                    ToggleCollider(eTutorialObjectName.Chair, true);
                    ResultManager.Instance.ExecuteResult(resultTutorialPhase6);
                
                    CompleteTutorial();
                    break;
            }
        }

        private void CompleteTutorial() {
            GameManager.Instance.SetVariable("isTutorial", false);
            GameManager.Instance.SetVariable("EndTutorial_ROOM_1", true);
            UIManager.Instance.SetUI(eUIGameObjectName.TutorialBlockingPanel, false);
        }
    
        public void ToggleCollider(eTutorialObjectName objectName, bool isOn)
        {
            foreach (Collider2D collider in tutorialGameObjects[objectName].GetComponents<Collider2D>())
                collider.enabled = isOn;
        }
    }
}
