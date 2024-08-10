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
    [SerializeField] private GameObject[] UI_OffAtEnd;
    public GameObject blockingPanel;
    [SerializeField] private Image beaconImage;
    [SerializeField] private Sprite[] beaconSprites;
    public Slider memoGaugeSlider;

    [Header("Character")]
    [SerializeField] private Animator fate;
    [SerializeField] private Animator accidyGirl, accidyBoy;
    private Animator accidy;
    public Animator Accidy { get => accidy; }
    public Animator Fate { get => fate; }

    [Header("Another Follow Manager")]
    public FollowTutorial followTutorial;
    [SerializeField] private FollowEnd followEnd;
    [SerializeField] private FollowDialogueManager followDialogueManager;
    [SerializeField] private FollowGameManager followGameManager;

    [Header("Zoom")]
    private Camera mainCam;
    private float zoomTime = 1.5f;
    public enum Position { Fate, Accidy, ZoomOut }

    [Header("Variables")]
    [SerializeField] private float accidyAnimatorSpeed;
    [SerializeField] private float fateAnimatorSpeed;
    public float totalFollowSpecialObjectCount = 10;
    
    public int ClickCount { get; set; }
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
        ClickCount = 0;
        mainCam = Camera.main;
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
        if ((int)GameManager.Instance.GetVariable("AccidyGender") == 0) accidy = accidyGirl;
        else accidy = accidyBoy;

        accidy.gameObject.SetActive(true);

        if (index == 0) accidy.transform.parent.SetAsFirstSibling();
        if (index == 1)
        {
            blockingPanel.transform.SetAsLastSibling();
            accidy.transform.parent.SetAsLastSibling();
        }
    }

    // ==================== 미행 다이얼로그 ==================== //
    public bool ClickObject()
    {
        if (IsEnd || IsDialogueOpen || !CanClick) return false;

        accidyAnimatorSpeed = Accidy.speed;
        fateAnimatorSpeed = Fate.speed;

        Accidy.speed = 0;
        Fate.speed = 0;

        IsDialogueOpen = true; // 다른 오브젝트를 누를 수 없게 만든다
        followDialogueManager.ClickObject();

        followGameManager.ChangeAnimStatusToStop(true);
        return true;
    }
    public void EndScript()
    {
        Accidy.speed = accidyAnimatorSpeed;
        Fate.speed = fateAnimatorSpeed;

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
        IsDialogueOpen = true;
        followDialogueManager.OpenExtraDialogue(extraName);
    }
    public void EndExtraDialogue(bool dialogueEnd)
    {
        followDialogueManager.EndExtraDialogue(dialogueEnd);
        IsDialogueOpen = !dialogueEnd;
    }
    public void ClickSpecialObject(FollowObject followObject)
    {
        followDialogueManager.ClickSpecialObject(followObject);
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
    public float Zoom(Position type)
    {
        StartCoroutine(ZoomIn(type));
        return zoomTime;
    }
    private IEnumerator ZoomIn(Position type)
    {
        Vector3 targetPosition = new(Fate.transform.position.x, 0, -10);
        float targetSize = 5;

        Vector3 originPosition = mainCam.transform.position;
        float originSize = mainCam.orthographicSize;

        float elapsedTime = 0f;

        while (elapsedTime < zoomTime)
        {
            switch (type)
            {
                case Position.Fate:
                    targetPosition = new(Fate.transform.position.x, -2, -10);
                    targetSize = 3;
                    break;

                case Position.Accidy:
                    targetPosition = new(Accidy.transform.position.x, -2, -10);
                    targetSize = 3;
                    break;

                case Position.ZoomOut:
                    targetPosition = new(Fate.transform.position.x, 0, -10);
                    targetSize = 5;
                    break;
            }

            // 보간하여 카메라 위치와 크기를 변경
            mainCam.transform.position = Vector3.Lerp(originPosition, targetPosition, elapsedTime / zoomTime);
            mainCam.orthographicSize = Mathf.Lerp(originSize, targetSize, elapsedTime / zoomTime);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 변경이 완료된 후 최종 목표값으로 설정
        mainCam.orthographicSize = targetSize;
    }
    public void CheckPosition()
    {
        if (Accidy.transform.position.x > 49)
        {
            FollowEndLogicStart();
        }

        if(followGameManager.Distance > 11 || followGameManager.Distance < 4)
        {
            FollowEndLogicStart();
        }

        // 두번째 미행
        if (SceneManager.Instance.CurrentScene == SceneType.FOLLOW_2)
        {
            switch ((int)Accidy.transform.position.x)
            {
                case 8: followDialogueManager.ExtraAutoDialogue("Follow2_017"); break; // 호객 행위
                case 12: followDialogueManager.ExtraAutoDialogue("Follow2_020"); break; // 가출 청소년
            }
        }
    }
    public void FollowEndLogicStart()
    {
        IsEnd = true;

        followGameManager.ChangeAnimStatusToStop(true);

        // 필연과 우연의 방향 조정
        Fate.SetBool("Hide", false);
        Fate.SetBool("Right", true);
        Fate.SetBool("Walking", false);
        Accidy.SetBool("Walking", false);
        Accidy.SetBool("Back", false);

        foreach(GameObject ui in UI_OffAtEnd) ui.SetActive(false);

        MemoManager.Instance.HideMemoButton = true;
        MemoManager.Instance.SetMemoButtons(false);

        StartCoroutine(followEnd.EndFollow());
    }
}
