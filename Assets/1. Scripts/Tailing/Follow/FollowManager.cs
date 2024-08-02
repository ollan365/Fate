using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using static Constants;
public class FollowManager : MonoBehaviour
{
    // FollowManager를 싱글턴으로 생성
    public static FollowManager Instance { get; private set; }

    [Header("UI")]
    [SerializeField] private GameObject UICanvas;
    public GameObject blockingPanel;
    public GameObject eventButtonPrefab; // 특별한 오브젝트를 클릭했을 때 버튼 생성
    [SerializeField] private Image beaconImage;
    [SerializeField] private Sprite[] beaconSprites;
    public Slider memoGaugeSlider;

    [Header("Character")]
    [SerializeField] private Animator fate;
    [SerializeField] private Animator[] accidyGirl, accidyBoy;
    private Animator accidy;
    public Animator Accidy { get => accidy; }
    public Animator Fate { get => fate; }

    [Header("Another Follow Manager")]
    public FollowTutorial followTutorial;
    [SerializeField] private FollowEnd followEnd;
    [SerializeField] private FollowDialogueManager followDialogueManager;
    [SerializeField] private FollowGameManager followGameManager;


    [Header("Variables")]
    [SerializeField] private float accidyAnimatorSpeed;
    [SerializeField] private float fateAnimatorSpeed;
    public float totalFollowSpecialObjectCount = 10;
    
    public bool CanClick { get { return !followGameManager.IsFateHide && followGameManager.NowAccidyStatus != FollowGameManager.AccidyStatus.RED; } }
    public bool IsTutorial { set; get; } // 튜토리얼 중인지 아닌지
    public bool IsEnd { set; get; } // 현재 미행이 끝났는지 아닌지
    public bool IsDialogueOpen { set; get; } // 현재 대화창이 열려있는지

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        IsTutorial = false;
        IsEnd = false;
        IsDialogueOpen = false;
        SetCharcter(0);

        StartCoroutine(ChangeBeaconSprite());

        if (GameManager.Instance.skipTutorial) { StartFollow(); return; }
        if (SceneManager.Instance.CurrentScene == SceneType.FOLLOW_1) { StartCoroutine(followTutorial.StartTutorial()); }
    }
    public void StartFollow()
    {
        followGameManager.ChangeAnimStatusToStop(false);
        followGameManager.StartGame();
    }

    public void SetCharcter(int index)
    {
        if (accidy != null) accidy.gameObject.SetActive(false);

        // 우연의 성별에 따라 다른 애니메이터 작동
        if ((int)GameManager.Instance.GetVariable("AccidyGender") == 0) accidy = accidyGirl[index];
        else accidy = accidyBoy[index];

        fate.gameObject.SetActive(true);
        accidy.gameObject.SetActive(true);
    }

    // ==================== 미행 다이얼로그 ==================== //
    public bool ClickObject()
    {
        if (IsEnd || IsDialogueOpen || !CanClick) return false;

        accidyAnimatorSpeed = Accidy.GetComponent<Animator>().speed;
        fateAnimatorSpeed = Fate.GetComponent<Animator>().speed;

        Accidy.GetComponent<Animator>().speed = 0;
        Fate.GetComponent<Animator>().speed = 0;

        IsDialogueOpen = true; // 다른 오브젝트를 누를 수 없게 만든다
        followDialogueManager.ClickObject();

        followGameManager.ChangeAnimStatusToStop(true);
        return true;
    }
    public void EndScript()
    {
        Accidy.GetComponent<Animator>().speed = accidyAnimatorSpeed;
        Fate.GetComponent<Animator>().speed = fateAnimatorSpeed;

        IsDialogueOpen = false; // 다른 오브젝트를 누를 수 있게 만든다

        if (IsTutorial) // 튜토리얼 중에는 다르게 작동
        {
            followTutorial.NextStep();
            return;
        }
        else if (IsEnd)
        {
            blockingPanel.SetActive(false);
            return;
        }

        followDialogueManager.EndScript();
        followGameManager.ChangeAnimStatusToStop(false);
    }
    public void OpenExtraDialogue(string extraName)
    {
        followDialogueManager.OpenExtraDialogue(extraName);
    }
    public void EndExtraDialogue(bool dialogueEnd)
    {
        followDialogueManager.EndExtraDialogue(dialogueEnd);
    }


    // ==================== 미행 ==================== //

    public void ClickCat()
    {
        SoundPlayer.Instance.UISoundPlay(Sound_Cat);
    }
    private IEnumerator ChangeBeaconSprite()
    {
        // 신호등의 색을 3초마다 바꿔준다
        while (gameObject.activeSelf)
        {
            foreach (Sprite sprite in beaconSprites)
            {
                beaconImage.sprite = sprite;
                yield return new WaitForSeconds(3f);
            }
        }
    }

    public void CheckPosition(Vector3 position)
    {
        if (position.x < -39)
        {
            followGameManager.ChangeAnimStatusToStop(true);
            FollowEndLogicStart();
        }

        // 두번째 미행
        if (SceneManager.Instance.CurrentScene == SceneType.FOLLOW_2)
        {
            switch ((int)position.x)
            {
                case 0: followDialogueManager.ExtraAutoDialogue("Follow2_017"); break; // 호객 행위
                case -4: followDialogueManager.ExtraAutoDialogue("Follow2_020"); break; // 가출 청소년
            }
        }
    }
    public void FollowEndLogicStart()
    {
        IsEnd = true;

        followGameManager.ChangeAnimStatusToStop(true);
        SetCharcter(1);
        fate.GetComponent<Animator>().SetBool("Ending", true);

        UICanvas.SetActive(false);
        MemoManager.Instance.HideMemoButton = true;
        MemoManager.Instance.SetMemoButtons(false);

        followGameManager.moveSpeed *= -1.5f;

        StartCoroutine(followEnd.EndFollow());
    }
}
