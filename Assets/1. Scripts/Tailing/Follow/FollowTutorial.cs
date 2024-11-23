using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class FollowTutorial : MonoBehaviour
{
    [SerializeField] private GameObject highlightPanel;
    [SerializeField] private Animator beaconAnimator;
    [SerializeField] private GameObject tutorialBlockingPanel;
    [SerializeField] private GameObject moveButtons;
    [SerializeField] private GameObject hideButtons;
    [SerializeField] private TextMeshProUGUI startText;
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
        tutorialBlockingPanel.SetActive(true);

        // 튜토리얼 중에 메모 버튼이 켜지지 않도록 설정
        MemoManager.Instance.HideMemoButton = true;
        MemoManager.Instance.SetMemoButtons(false);

        StartCoroutine(ScreenEffect.Instance.OnFade(null, 1, 0, 1, false, 0, 0));
        if (highlightPanel) highlightPanel.SetActive(false);
        yield return new WaitForSeconds(1.5f);

        FollowManager.Instance.StartFollow();
        FollowManager.Instance.SetBlockingPanel(true);
        EventManager.Instance.CallEvent("EventFollowTutorial");
    }
    
    public void NextStep()
    {
        Debug.Log(GameManager.Instance.GetVariable("FollowTutorialPhase"));

        if ((int)GameManager.Instance.GetVariable("CurrentScene") == Constants.SceneType.FOLLOW_2.ToInt())
        {
            StartCoroutine(EndTutorial());
            return;
        }

        switch ((int)GameManager.Instance.GetVariable("FollowTutorialPhase"))
        {
            case 0: // 신호등 판넬 켜기
                FollowManager.Instance.SetBlockingPanel(false);
                highlightPanel.SetActive(true);
                beaconAnimator.SetBool(nameof(Light), true);
                break;
            case 1: // 신호등을 눌렀을 때
                FollowManager.Instance.SetBlockingPanel(true);
                highlightPanel.SetActive(false);
                beaconAnimator.SetBool(nameof(Light), false);
                break;
            case 2: // 이동 가능
                StartCoroutine(MoveLogicTutorial());
                break;
            case 4: // 이동 가능
                MoveEnd();
                break;
            case 5: // 우연이 뒤를 돌았다가 다시 앞을 볼 때까지 숨기
                StartCoroutine(HideLogicTutorial());
                break;
            case 6:
                FollowManager.Instance.SetBlockingPanel(true);
                break;
            case 7: // 튜토리얼 끝
                StartCoroutine(EndTutorial());
                break;
        }
        GameManager.Instance.IncrementVariable("FollowTutorialPhase");
    }
    private IEnumerator MoveLogicTutorial()
    {
        moveButtons.SetActive(true);
        fateMovable = true;
        FollowManager.Instance.SetBlockingPanel(true);

        // 필연이 일정 거리 이상 앞으로 이동할 때까지 대기
        while (fate.transform.position.x < 2 || (int)GameManager.Instance.GetVariable("FollowTutorialPhase") == 3) yield return null;

        EventManager.Instance.CallEvent("EventFollowTutorialNextStep");
    }
    private void MoveEnd()
    {
        fateCanHide = true;

        moveButtons.SetActive(false);

        // 우연이 뒤돌아보기 직전
        accidy.transform.parent.SetAsLastSibling();

        accidyNextLogic = true;

        FollowManager.Instance.SetBlockingPanel(true);

        hideButtons.SetActive(true);
    }

    private IEnumerator HideLogicTutorial()
    {
        FollowManager.Instance.SetBlockingPanel(false);

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
        hideButtons.SetActive(false);

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
        startBlockingPanel.gameObject.SetActive(true);
        float current = 0, fadeTime = 1;
        while (current < fadeTime)
        {
            current += Time.deltaTime;

            startText.color = new Color(1, 1, 1, Mathf.Lerp(1, 0, current / fadeTime));
            startBlockingPanel.color = new Color(0, 0, 0, Mathf.Lerp(1, 0, current / fadeTime));
            yield return null;
        }
        startText.gameObject.SetActive(false);
        startBlockingPanel.gameObject.SetActive(false);
        yield return new WaitForSeconds(0.5f);
        FollowManager.Instance.SetBlockingPanel(false);
        tutorialBlockingPanel.SetActive(false);

        MemoManager.Instance.HideMemoButton = false;
        MemoManager.Instance.SetMemoButtons(true);

        GameManager.Instance.SetVariable("EndTutorial_FOLLOW_1", true);

        FollowManager.Instance.IsTutorial = false;
        FollowManager.Instance.StartFollow();
    }
}