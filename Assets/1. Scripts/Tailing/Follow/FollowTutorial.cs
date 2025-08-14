using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class FollowTutorial : MonoBehaviour
{
    [SerializeField] private GameObject highlightPanel;
    [SerializeField] private GameObject beacon;
    [SerializeField] private GameObject tutorialBlockingPanel;
    [SerializeField] private GameObject[] moveButtons;
    [SerializeField] private GameObject hideButton;
    [SerializeField] private GameObject startText;
    [SerializeField] private Image startBlockingPanel;
    [SerializeField] private GameObject fate;

    public bool accidyNextLogic = false;
    public bool fateMovable = false;
    public bool fateCanHide = false;

    private GameObject accidy;

    public IEnumerator StartTutorial()
    {
        accidy = FollowManager.Instance.Accidy.gameObject;

        // 다른 물체를 누를 수 없도록 만든다
        FollowManager.Instance.IsTutorial = true;
        if(tutorialBlockingPanel) tutorialBlockingPanel.SetActive(true);

        // 튜토리얼 중에 메모 버튼이 켜지지 않도록 설정
        MemoManager.Instance.SetShouldHideMemoButton(true);
        MemoManager.Instance.SetMemoButtons(false);

        StartCoroutine(UIManager.Instance.OnFade(null, 1, 0, 1, false, 0, 0));
        if (highlightPanel) highlightPanel.SetActive(false);
        yield return new WaitForSeconds(1.5f);

        FollowManager.Instance.StartFollow();
        EventManager.Instance.CallEvent("EventFollowTutorial");
    }
    
    public void NextStep()
    {
        Debug.Log(GameManager.Instance.GetVariable("FollowTutorialPhase"));

        if (GameSceneManager.Instance.GetActiveScene() == Constants.SceneType.FOLLOW_2)
        {
            StartCoroutine(EndTutorial());
            return;
        }

        switch ((int)GameManager.Instance.GetVariable("FollowTutorialPhase"))
        {
            case 1: // 신호등 판넬 켜기
                highlightPanel.SetActive(true);
                UIManager.Instance.ToggleHighlightAnimationEffect(beacon, true);
                break;
            case 2: // 신호등을 눌렀을 때
                highlightPanel.SetActive(false);
                UIManager.Instance.ToggleHighlightAnimationEffect(beacon, false);
                break;
            case 3: // 이동 가능
                StartCoroutine(MoveLogicTutorial());
                break;
            case 5: // 이동 가능
                MoveEnd();
                break;
            case 6: // 우연이 뒤를 돌았다가 다시 앞을 볼 때까지 숨기
                StartCoroutine(HideLogicTutorial());
                break;
            case 8: // 튜토리얼 끝
                StartCoroutine(EndTutorial());
                break;
        }
    }
    private IEnumerator MoveLogicTutorial()
    {
        moveButtons[0].SetActive(true);
        moveButtons[1].SetActive(true);
        UIManager.Instance.ToggleHighlightAnimationEffect(moveButtons[1], true);

        fateMovable = true;

        // 필연이 일정 거리 이상 앞으로 이동할 때까지 대기
        while (fate.transform.position.x < 2 || (int)GameManager.Instance.GetVariable("FollowTutorialPhase") == 3) yield return null;

        EventManager.Instance.CallEvent("EventFollowTutorialNextStep");
    }
    private void MoveEnd()
    {
        fateCanHide = true;

        moveButtons[0].SetActive(false);
        moveButtons[1].SetActive(false);
        UIManager.Instance.ToggleHighlightAnimationEffect(moveButtons[1], false);

        // 우연이 뒤돌아보기 직전
        accidy.transform.parent.SetAsLastSibling();

        accidyNextLogic = true;

        hideButton.SetActive(true);
        UIManager.Instance.ToggleHighlightAnimationEffect(hideButton, true);
    }

    private IEnumerator HideLogicTutorial()
    {
        accidyNextLogic = true;
        yield return new WaitForSeconds(0.5f);
        accidyNextLogic = true;
        yield return new WaitForSeconds(0.5f);

        float spaceBarClickTime = 3;
        while (spaceBarClickTime > 0)
        {
            while (Input.GetKey(KeyCode.Space) && spaceBarClickTime > 0)
            {
                spaceBarClickTime -= Time.deltaTime;
                yield return null;
            }
            
            if (spaceBarClickTime <= 0) break;
            else yield return null;
        }
        hideButton.SetActive(false);
        UIManager.Instance.ToggleHighlightAnimationEffect(hideButton, false);

        accidy.transform.parent.SetAsFirstSibling();
        accidyNextLogic = true;
        yield return new WaitForSeconds(1f);

        EventManager.Instance.CallEvent("EventFollowTutorialNextStep");
    }

    //private void AdditionalTutorialSet()
    //{
    //    StartCoroutine(AdditionalTutorial());
    //}
    //private IEnumerator AdditionalTutorial()
    //{
    //    clickSpaceBar = false;

    //    StartCoroutine(WaitingSpaceBarClick());

    //    while(!clickSpaceBar) // 이동 버튼을 한번이라도 누를 때까지 깜빡인다
    //    {
    //        foreach (Image button in moveButtons)
    //            StartCoroutine(ScreenEffect.Instance.OnFade(button, 1, 0, 0.5f, true, 0.1f, 0));

    //        yield return new WaitForSeconds(1.2f);
    //    }
    //    foreach (Image button in moveButtons)
    //        button.gameObject.SetActive(false);
    //}

    
    private IEnumerator EndTutorial()
    {
        fateMovable = false;

        
        startText.gameObject.SetActive(true);
        //startBlockingPanel.gameObject.SetActive(true);

        float current = 0f;
        float fadeTime = 0.5f; // 한쪽 방향 페이드 시간 (총 깜빡임은 *2)

        // === Fade In (0 → 1) ===
        while (current < fadeTime)
        {
            current += Time.deltaTime;
            float t = Mathf.Clamp01(current / fadeTime);

            startText.GetComponent<Image>().color = new Color(1, 1, 1, Mathf.Lerp(1, 0, t)); // 텍스트는 기존처럼 1→0
            //startBlockingPanel.color = new Color(0, 0, 0, Mathf.Lerp(0, 1, t)); // 0→1
            yield return null;
        }

        // === Fade Out (1 → 0) ===
        current = 0f;
        while (current < fadeTime)
        {
            current += Time.deltaTime;
            float t = Mathf.Clamp01(current / fadeTime);

            startText.GetComponent<Image>().color = new Color(1, 1, 1, Mathf.Lerp(0, 1, t)); // 텍스트는 다시 0→1 (원하면 생략 가능)
            //startBlockingPanel.color = new Color(0, 0, 0, Mathf.Lerp(1, 0, t)); // 1→0
            yield return null;
        }

        startText.gameObject.SetActive(false);
        //startBlockingPanel.gameObject.SetActive(false);
        yield return new WaitForSeconds(0.5f);
        if (tutorialBlockingPanel) tutorialBlockingPanel.SetActive(false);

        MemoManager.Instance.SetShouldHideMemoButton(false);
        MemoManager.Instance.SetMemoButtons(true);

        GameManager.Instance.SetVariable("EndTutorial_FOLLOW_1", true);

        FollowManager.Instance.IsTutorial = false;
        FollowManager.Instance.StartFollow();
    }
}