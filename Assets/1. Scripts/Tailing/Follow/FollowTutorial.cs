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
    [SerializeField] private GameObject moveButton;
    [SerializeField] private Sprite moveButtonSprite, stopButtonSprite;
    [SerializeField] private Image arrow;
    [SerializeField] private TextMeshProUGUI startText;

    private int moveButtonClickCount = 0; // 이동 버튼을 누른 횟수
    private int tutorialStep = 0;

    public IEnumerator StartTutorial()
    {
        // 다른 물체를 누를 수 없도록 만든다
        FollowManager.Instance.IsTutorial = true;
        frontCanvas.SetActive(false);
        moveButton.SetActive(false);

        // 튜토리얼 중에 메모 버튼이 켜지지 않도록 설정
        MemoManager.Instance.HideMemoButton = true;
        MemoManager.Instance.SetMemoButtons(false);

        StartCoroutine(ScreenEffect.Instance.OnFade(null, 1, 0, 1, false, 0, 0));
        yield return new WaitForSeconds(1.5f);

        // 메모 버튼을 화면에서 사라지게 한다
        MemoManager.Instance.SetMemoButtons(false);
        highlightPanel.SetActive(false);

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
            case 7:
                EndTutorialSet();
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
        // FollowManager.Instance.canClick = true;
    }

    // 이동 버튼 설명
    private void MoveButtonTutorial()
    {
        // 다른 물체를 누를 수 없도록 만든다
        // FollowManager.Instance.canClick = false;

        // 이동 버튼에 대한 설명
        blockingPanel.SetActive(true);
        DialogueManager.Instance.StartDialogue("FollowTutorial_004");
    }

    private void AdditionalTutorialSet()
    {
        StartCoroutine(AdditionalTutorial());
    }
    private IEnumerator AdditionalTutorial()
    {
        while(moveButtonClickCount == 0) // 이동 버튼을 한번이라도 누를 때까지 깜빡인다
        {
            StartCoroutine(ScreenEffect.Instance.OnFade(arrow, 1, 0, 0.5f, true, 0.1f, 0));
            yield return new WaitForSeconds(1.2f);
        }
        arrow.gameObject.SetActive(false);
        DialogueManager.Instance.StartDialogue("FollowTutorial_006");
    }
    private void EndTutorialSet()
    {
        StartCoroutine(EndTutorial());
    }
    private IEnumerator EndTutorial()
    {
        startText.gameObject.SetActive(true);
        float current = 0, fadeTime = 1;
        while (current < fadeTime)
        {
            current += Time.deltaTime;

            startText.color = new Color(1, 1, 1, Mathf.Lerp(1, 0, current / fadeTime));
            yield return null;
        }
        startText.gameObject.SetActive(false);
        yield return new WaitForSeconds(0.5f);

        blockingPanel.SetActive(false);

        MemoManager.Instance.HideMemoButton = false;
        MemoManager.Instance.SetMemoButtons(true);

        FollowManager.Instance.IsTutorial = false;
        // FollowManager.Instance.canClick = true;
        frontCanvas.SetActive(true);
        moveButton.SetActive(true);
        
        MemoManager.Instance.SetMemoButtons(true);
    }
}