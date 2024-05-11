using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class FollowTutorial : MonoBehaviour
{
    [SerializeField] private FollowAnim followAnim;

    [SerializeField] private GameObject frontCanvas;
    [SerializeField] private GameObject highlightPanel;
    [SerializeField] private GameObject blockingPanel;
    [SerializeField] private GameObject nextButton;
    [SerializeField] private GameObject moveButton;

    private GameObject followTutorialCanvas;
    private int tutorialStep = 0;

    public IEnumerator StartTutorial()
    {
        // 다른 물체를 누를 수 없도록 만든다
        FollowManager.Instance.isTutorial = true;
        FollowManager.Instance.canClick = false;
        frontCanvas.SetActive(false);
        moveButton.SetActive(false);

         // 튜토리얼 캔버스를 할당
         followTutorialCanvas = DialogueManager.Instance.dialogueCanvas[Constants.DialogueType.FOLLOW_TUTORIAL.ToInt()].transform.parent.gameObject;
        
        StartCoroutine(ScreenEffect.Instance.OnFade(null, 1, 0, 1, false, 0, 0));
        yield return new WaitForSeconds(1.5f);

        // 메모 버튼을 화면에서 사라지게 한다
        MemoManager.Instance.HideMemoButton(true);
        highlightPanel.SetActive(false);
        nextButton.SetActive(false);

        blockingPanel.SetActive(true);
        DialogueManager.Instance.StartDialogue("FollowTutorial_002");
    }
    
    public void NextStep()
    {
        tutorialStep++;

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

        // 하이라이트 판넬을 켠다
        highlightPanel.SetActive(true);

        // 다른 물체를 누를 수 있도록 만든다
        FollowManager.Instance.canClick = true;
    }

    // 이동 버튼 설명
    private void MoveButtonTutorial()
    {
        // 이동 버튼 활성화
        followTutorialCanvas.SetActive(true);
        followTutorialCanvas.GetComponentInChildren<Button>().onClick.AddListener(() => followAnim.ChangeAnimStatus());
        followTutorialCanvas.GetComponentInChildren<Button>().onClick.AddListener(() =>
        {
            if (FollowManager.Instance.followAnim.IsStop)
                followTutorialCanvas.GetComponentInChildren<Button>().GetComponentInChildren<TextMeshProUGUI>().text = "이동";
            else
                followTutorialCanvas.GetComponentInChildren<Button>().GetComponentInChildren<TextMeshProUGUI>().text = "멈춤";
        });
        followTutorialCanvas.GetComponentInChildren<Button>().interactable = true;

        // 다른 물체를 누를 수 없도록 만든다
        FollowManager.Instance.canClick = false;

        // 이동 버튼에 대한 설명
        blockingPanel.SetActive(true);
        DialogueManager.Instance.StartDialogue("FollowTutorial_004");
    }

    private void AdditionalTutorialSet()
    {
        // 블로킹 판넬을 끄고 다음 설명을 진행하는 버튼을 활성화 시킨다
        blockingPanel.SetActive(false);
        nextButton.SetActive(true);
    }
    private void AdditionalTutorial()
    {
        // 다음 설명을 진행하는 버튼을 끄고 블로킹 판넬을 켜고 다음 설명 진행
        nextButton.SetActive(false);

        blockingPanel.SetActive(true);
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
        followTutorialCanvas.SetActive(false);

        MemoManager.Instance.HideMemoButton(false);
        FollowManager.Instance.isTutorial = false;
        FollowManager.Instance.canClick = true;
        frontCanvas.SetActive(true);
        nextButton.SetActive(false);
        moveButton.SetActive(true);
    }
}
