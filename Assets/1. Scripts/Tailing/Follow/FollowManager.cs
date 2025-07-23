using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using TMPro;
using static Constants;
public class FollowManager : MonoBehaviour
{
    // FollowManager를 싱글턴으로 생성
    public static FollowManager Instance { get; private set; }

    [Header("UI")]
    [SerializeField] private GameObject[] UI_OffAtEnd;
    [SerializeField] private Image beaconImage;
    [SerializeField] private Sprite[] beaconSprites;

    [Header("Character")]
    [SerializeField] private Animator fate;
    [SerializeField] private Animator accidyGirl, accidyBoy;
    [SerializeField] private GameObject accidyDialogueBox;
    private Animator accidy;
    public Animator Accidy { get => accidy; }
    public Animator Fate { get => fate; }
    public GameObject AccidyDialogueBox { get => accidyDialogueBox; }

    [Header("Another Follow Manager")]
    [SerializeField] private FollowTutorial followTutorial;
    [SerializeField] private FollowEnd followEnd;
    [SerializeField] private FollowDialogueManager followDialogueManager;
    [SerializeField] private FollowGameManager followGameManager;

    [Header("Zoom")]
    [SerializeField] private Camera cameraAfterBlur;
    private float zoomTime = 1.5f;
    public enum Position { Fate, Accidy, ZoomOut }
    public Camera CameraAfterBlur { get => cameraAfterBlur; }

    [Header("Variables")]
    public float totalFollowSpecialObjectCount = 10;
    public Action EndScriptAction;


    public int ClickCount { get; set; }
    public bool CanClick { get { return !followGameManager.IsFateHide && followGameManager.NowAccidyStatus != FollowGameManager.AccidyStatus.RED; } }
    public bool IsEnd { set; get; } // 현재 미행이 끝났는지 아닌지
    public bool IsDialogueOpen { set; get; } // 현재 대화창이 열려있는지

    // 튜토리얼 관련 변수들
    public bool IsTutorial { set; get; }
    public bool TutorialFateNotMovable { get => IsTutorial && !followTutorial.fateMovable; }
    public bool TutorialFateCantHide { get => IsTutorial && !followTutorial.fateCanHide; }
    public bool TutorialAccidyNextLogic
    {
        set => followTutorial.accidyNextLogic = value;
        get => followTutorial.accidyNextLogic;
    }

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        SceneManager.Instance.ChangeSceneEffect();

        IsTutorial = false;
        IsEnd = false;
        IsDialogueOpen = false;
        ClickCount = 0;
        SetCharcter();

        SetUI();

        StartCoroutine(ChangeBeaconSprite());

        if (GameManager.Instance.skipTutorial) { StartFollow(); }
        else if ((int)GameManager.Instance.GetVariable("CurrentScene") == SceneType.FOLLOW_1.ToInt())
        {
            if((int)GameManager.Instance.GetVariable("ReplayCount") > 0 || (bool)GameManager.Instance.GetVariable("EndTutorial_FOLLOW_1")) { StartFollow(); }
            else StartCoroutine(followTutorial.StartTutorial());
        }
        else StartCoroutine(followTutorial.StartTutorial());
    }
    private void SetUI()
    {
        if ((int)GameManager.Instance.GetVariable("CurrentScene") == SceneType.FOLLOW_1.ToInt())
        {
            UIManager.Instance.SetUI(eUIGameObjectName.FollowUI, true);
            UIManager.Instance.SetUI(eUIGameObjectName.FollowMemoGauge, true);
            MemoManager.Instance.SetMemoGauge(UIManager.Instance.GetUI(eUIGameObjectName.FollowMemoGauge));
            UIManager.Instance.SetUI(eUIGameObjectName.FollowUIBackground, true);
            UIManager.Instance.SetUI(eUIGameObjectName.DoubtGaugeSlider, true);
            UIManager.Instance.SetUI(eUIGameObjectName.FatePositionSlider, true);
            UIManager.Instance.SetUI(eUIGameObjectName.AccidyPositionSlider, true);
        }
        else
        {
            UIManager.Instance.SetUI(eUIGameObjectName.FollowUI_Night, true);
            UIManager.Instance.SetUI(eUIGameObjectName.FollowMemoGauge_Night, true);
            MemoManager.Instance.SetMemoGauge(UIManager.Instance.GetUI(eUIGameObjectName.FollowMemoGauge_Night));
            UIManager.Instance.SetUI(eUIGameObjectName.FollowUIBackground_Night, true);
            UIManager.Instance.SetUI(eUIGameObjectName.DoubtGaugeSlider_Night, true);
            UIManager.Instance.SetUI(eUIGameObjectName.FatePositionSlider_Night, true);
            UIManager.Instance.SetUI(eUIGameObjectName.AccidyPositionSlider_Night, true);
        }
    }
    public void TutorialNextStep()
    {
        followTutorial.NextStep();
    }
    public void StartFollow()
    {
        followGameManager.ChangeAnimStatusToStop(false);
        followGameManager.StartGame();
    }

    public void SetCharcter()
    {
        if (accidy != null) accidy.gameObject.SetActive(false);

        // 우연의 성별에 따라 다른 애니메이터 작동
        if ((int)GameManager.Instance.GetVariable("AccidyGender") == 0) accidy = accidyGirl;
        else accidy = accidyBoy;

        accidy.gameObject.SetActive(true);
    }


    // ==================== 미행 다이얼로그 ==================== //
    public bool ClickObject()
    {
        if (!CanClick) return false;

        followGameManager.ChangeAnimStatusToStop(true);
        followDialogueManager.ClickObject();
        if (!IsEnd)
        {
            accidyDialogueBox.SetActive(false);
        }

        if (IsTutorial || IsDialogueOpen || IsEnd) return false;
        IsDialogueOpen = true; // 다른 오브젝트를 누를 수 없게 만든다
        
        return true;
    }
    public void EndScript()
    {
        IsDialogueOpen = false; // 다른 오브젝트를 누를 수 있게 만든다

        followDialogueManager.EndScript();
        if (!IsEnd)
        {
            Accidy.GetComponent<Image>().color = new Color(1, 1, 1);
            accidyDialogueBox.SetActive(true);
        }
        EndScriptAction?.Invoke();

        if (IsTutorial) return;

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

        Vector3 originPosition = Camera.main.transform.position;
        float originSize = Camera.main.orthographicSize;

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
            Camera.main.transform.position = Vector3.Lerp(originPosition, targetPosition, elapsedTime / zoomTime);
            Camera.main.orthographicSize = Mathf.Lerp(originSize, targetSize, elapsedTime / zoomTime);
            
            cameraAfterBlur.orthographicSize = Camera.main.orthographicSize;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 변경이 완료된 후 최종 목표값으로 설정
        Camera.main.orthographicSize = targetSize;
        cameraAfterBlur.orthographicSize = Camera.main.orthographicSize;
    }
    public void CheckPosition()
    {
        if (Accidy.transform.position.x > 48)
        {
            FollowEndLogicStart();
        }

        if(followGameManager.Distance > 11 || followGameManager.Distance < 4)
        {
            FollowEndLogicStart();
        }

        // 두번째 미행
        if (!IsTutorial && (int)GameManager.Instance.GetVariable("CurrentScene") == SceneType.FOLLOW_2.ToInt())
        {
            switch ((int)Accidy.transform.position.x)
            {
                case 8: followDialogueManager.ExtraAutoDialogue("Follow2_017"); break; // 호객 행위
                case 13: followDialogueManager.ExtraAutoDialogue("Follow2_020"); break; // 가출 청소년
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

        UIManager.Instance.SetUI(eUIGameObjectName.FollowUI, false);
        UIManager.Instance.SetUI(eUIGameObjectName.FollowUI_Night, false);

        StartCoroutine(followEnd.EndFollowLogic_0());
    }
    public void FollowEndLogic_1()
    {
        StartCoroutine(followEnd.EndFollowLogic_1());
    }
    public void FollowEndLogic_3()
    {
        StartCoroutine(followEnd.EndFollowLogic_3());
    }
}
