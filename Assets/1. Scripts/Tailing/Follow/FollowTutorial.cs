using UnityEngine;
using System.Collections;

public class FollowTutorial : MonoBehaviour
{
    [SerializeField] private GameObject tutorialCanvas, followCanvas;
    [SerializeField] private GameObject moveButton, highlightPanel, blockingPanel;
    [SerializeField] private GameObject nextButton;

    [SerializeField] private int tutorialStep = 0;

    public IEnumerator StartTutorial()
    {
        blockingPanel.SetActive(false);
        StartCoroutine(ScreenEffect.Instance.OnFade(null, 1, 0, 1, false, 0, 0));
        yield return new WaitForSeconds(0.25f);

        FollowManager.Instance.isTutorial = true;

        followCanvas.SetActive(false);
        tutorialCanvas.SetActive(true);

        moveButton.SetActive(false);
        MemoManager.Instance.HideMemoButton(true);
        highlightPanel.SetActive(false);
        nextButton.SetActive(false);

        blockingPanel.SetActive(true);
        DialogueManager.Instance.StartDialogue("FollowTutorial_002");
    }
    
    public void NextStep()
    {
        tutorialStep++;

        Debug.Log($"{tutorialStep}");
        switch (tutorialStep)
        {
            case 2:
                ObjectTutorial(); // 빌딩 하이라이트
                break;
            case 3:
                MoveButtonTutorial(); // 이동 버튼 설명
                break;
            case 5:
                AdditionalTutorialSet();
                break;
            case 6:
                AdditionalTutorial();
                break;
            case 8:
                EndTutorialSet();
                break;
            case 9:
                EndTutorial();
                break;
        }
    }

    // 빌딩 하이라이트
    private void ObjectTutorial()
    {
        blockingPanel.SetActive(false);
        highlightPanel.SetActive(true);
    }

    // 이동 버튼 설명
    private void MoveButtonTutorial()
    {
        // 이동 버튼을 보이게 한다
        moveButton.SetActive(true);

        // 이동 버튼에 대한 설명
        DialogueManager.Instance.StartDialogue("FollowTutorial_004");
    }

    private void AdditionalTutorialSet()
    {
        // 이동 버튼을 끄고 블로킹 판넬을 끄고 다음 설명을 진행하는 버튼을 활성화 시킨다
        moveButton.SetActive(false);
        blockingPanel.SetActive(false);
        nextButton.SetActive(true);
    }
    private void AdditionalTutorial()
    {
        // 다음 설명을 진행하는 버튼을 끄고 블로킹 판넬을 켜고 다음 설명 진행
        nextButton.SetActive(false);
        blockingPanel.SetActive(false);
        DialogueManager.Instance.StartDialogue("FollowTutorial_006");
    }
    private void EndTutorialSet()
    {
        blockingPanel.SetActive(false);

        // 다음 단계를 진행하는 버튼 활성화
        nextButton.SetActive(true);
    }
    private void EndTutorial()
    {
        tutorialCanvas.SetActive(false);
        followCanvas.SetActive(true);

        MemoManager.Instance.HideMemoButton(false);
        FollowManager.Instance.isTutorial = false;
    }
}
