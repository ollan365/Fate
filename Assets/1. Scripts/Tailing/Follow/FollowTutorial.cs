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
        // �ٸ� ��ü�� ���� �� ������ �����
        FollowManager.Instance.isTutorial = true;
        FollowManager.Instance.canClick = false;
        frontCanvas.SetActive(false);
        moveButton.SetActive(false);

         // Ʃ�丮�� ĵ������ �Ҵ�
         followTutorialCanvas = DialogueManager.Instance.dialogueCanvas[Constants.DialogueType.FOLLOW_TUTORIAL.ToInt()].transform.parent.gameObject;
        
        StartCoroutine(ScreenEffect.Instance.OnFade(null, 1, 0, 1, false, 0, 0));
        yield return new WaitForSeconds(1.5f);

        // �޸� ��ư�� ȭ�鿡�� ������� �Ѵ�
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

        // ���̶���Ʈ �ǳ��� �Ҵ�
        highlightPanel.SetActive(true);

        // �ٸ� ��ü�� ���� �� �ֵ��� �����
        FollowManager.Instance.canClick = true;
    }

    // �̵� ��ư ����
    private void MoveButtonTutorial()
    {
        // �̵� ��ư Ȱ��ȭ
        followTutorialCanvas.SetActive(true);
        followTutorialCanvas.GetComponentInChildren<Button>().onClick.AddListener(() => followAnim.ChangeAnimStatus());
        followTutorialCanvas.GetComponentInChildren<Button>().onClick.AddListener(() =>
        {
            if (FollowManager.Instance.followAnim.IsStop)
                followTutorialCanvas.GetComponentInChildren<Button>().GetComponentInChildren<TextMeshProUGUI>().text = "�̵�";
            else
                followTutorialCanvas.GetComponentInChildren<Button>().GetComponentInChildren<TextMeshProUGUI>().text = "����";
        });
        followTutorialCanvas.GetComponentInChildren<Button>().interactable = true;

        // �ٸ� ��ü�� ���� �� ������ �����
        FollowManager.Instance.canClick = false;

        // �̵� ��ư�� ���� ����
        blockingPanel.SetActive(true);
        DialogueManager.Instance.StartDialogue("FollowTutorial_004");
    }

    private void AdditionalTutorialSet()
    {
        // ���ŷ �ǳ��� ���� ���� ������ �����ϴ� ��ư�� Ȱ��ȭ ��Ų��
        blockingPanel.SetActive(false);
        nextButton.SetActive(true);
    }
    private void AdditionalTutorial()
    {
        // ���� ������ �����ϴ� ��ư�� ���� ���ŷ �ǳ��� �Ѱ� ���� ���� ����
        nextButton.SetActive(false);

        blockingPanel.SetActive(true);
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
        followTutorialCanvas.SetActive(false);

        MemoManager.Instance.HideMemoButton(false);
        FollowManager.Instance.isTutorial = false;
        FollowManager.Instance.canClick = true;
        frontCanvas.SetActive(true);
        nextButton.SetActive(false);
        moveButton.SetActive(true);
    }
}
