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
                ObjectTutorial(); // ���� ���̶���Ʈ
                break;
            case 3:
                MoveButtonTutorial(); // �̵� ��ư ����
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

    // ���� ���̶���Ʈ
    private void ObjectTutorial()
    {
        blockingPanel.SetActive(false);
        highlightPanel.SetActive(true);
    }

    // �̵� ��ư ����
    private void MoveButtonTutorial()
    {
        // �̵� ��ư�� ���̰� �Ѵ�
        moveButton.SetActive(true);

        // �̵� ��ư�� ���� ����
        DialogueManager.Instance.StartDialogue("FollowTutorial_004");
    }

    private void AdditionalTutorialSet()
    {
        // �̵� ��ư�� ���� ���ŷ �ǳ��� ���� ���� ������ �����ϴ� ��ư�� Ȱ��ȭ ��Ų��
        moveButton.SetActive(false);
        blockingPanel.SetActive(false);
        nextButton.SetActive(true);
    }
    private void AdditionalTutorial()
    {
        // ���� ������ �����ϴ� ��ư�� ���� ���ŷ �ǳ��� �Ѱ� ���� ���� ����
        nextButton.SetActive(false);
        blockingPanel.SetActive(false);
        DialogueManager.Instance.StartDialogue("FollowTutorial_006");
    }
    private void EndTutorialSet()
    {
        blockingPanel.SetActive(false);

        // ���� �ܰ踦 �����ϴ� ��ư Ȱ��ȭ
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
