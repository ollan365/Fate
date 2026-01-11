using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static Fate.Utilities.Constants;
using Fate.Managers;
using Fate.Utilities;

public class FollowGameManager : MonoBehaviour
{
    [Header("Gauges")]
    [SerializeField] private Slider overHeadDoubtGaugeSlider;
    [SerializeField] private Image[] overHeadDoubtGaugeSliderImages;
    private float endPositonOfMap = 48.5f;

    [SerializeField] private Q_Vignette_Single vignette;

    [SerializeField] private float accidyMoveSpeed;
    [SerializeField] float fateMoveSpeed;
    [SerializeField] int clickCountLimit;
    public float Distance { get => Accidy.transform.position.x - Fate.transform.position.x; }
    public bool StopAccidy { get; set; }

    private AccidyStatus accidyStatus = AccidyStatus.GREEN;
    public AccidyStatus NowAccidyStatus { get => accidyStatus; }

    private Animator Fate { get => FollowManager.Instance.Fate; }
    private Animator Accidy { get => FollowManager.Instance.Accidy; }
    public GameObject AccidyDialogueBox { get => FollowManager.Instance.AccidyDialogueBox; }

    private bool IsEnd { get => FollowManager.Instance.IsEnd; }
    private bool IsDialogueOpen { get => FollowManager.Instance.IsDialogueOpen; }
    public bool IsFateHide { get; private set; }
    private bool IsTutorial { get => FollowManager.Instance.IsTutorial; }
    private void Start()
    {
        UIManager.Instance.ChangeSliderValue(eUIGameObjectName.AccidyPositionSlider, Accidy.transform.position.x / endPositonOfMap, 0);
    }
    void Update()
    {
        if (!Accidy) return;

        if (!StopAccidy && !IsTutorial && !IsEnd) MoveAccidy();

        if (!IsEnd && !IsDialogueOpen) MoveFate();
        else SoundPlayer.Instance.UISoundPlay_LOOP(Sound_FootStep_Fate, false);

        if (!IsEnd) FollowManager.Instance.CheckPosition();
    }
    private void MoveAccidy()
    {
        Vector3 moveVector = Vector3.left * accidyMoveSpeed * Time.deltaTime;
        Accidy.transform.position -= moveVector;
        AccidyDialogueBox.transform.position -= moveVector;
        if (GameSceneManager.Instance.GetActiveScene() == SceneType.FOLLOW_1)
            UIManager.Instance.ChangeSliderValue(eUIGameObjectName.AccidyPositionSlider, Accidy.transform.position.x / endPositonOfMap, 0);
        else
            UIManager.Instance.ChangeSliderValue(eUIGameObjectName.AccidyPositionSlider_Night, Accidy.transform.position.x / endPositonOfMap, 0);
    }
    private void MoveFate()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !FollowManager.Instance.TutorialFateCantHide) FateHide(true);

        if (!IsFateHide && !FollowManager.Instance.TutorialFateNotMovable)
        {
            bool isFateMove = false;
            if (Input.GetKey(KeyCode.A))
            {
                // 나중에 아트 리소스 추가되면 Vector3.right를 Vector3.left로 변경
                if (Fate.transform.position.x > -1) Fate.transform.Translate(Vector3.left * fateMoveSpeed * Time.deltaTime);

                Fate.SetBool("Right", false);
                isFateMove = true;
            }
            if (Input.GetKey(KeyCode.D))
            {
                if (!IsTutorial || Fate.transform.position.x < 2) Fate.transform.Translate(Vector3.right * fateMoveSpeed * Time.deltaTime);
                Fate.SetBool("Right", true);
                isFateMove = true;
            }
            SoundPlayer.Instance.UISoundPlay_LOOP(Sound_FootStep_Fate, isFateMove);
            Fate.SetBool("Walking", isFateMove);

            if (GameSceneManager.Instance.GetActiveScene() == SceneType.FOLLOW_1)
                UIManager.Instance.ChangeSliderValue(eUIGameObjectName.FatePositionSlider,
                Fate.transform.position.x / endPositonOfMap,
                0);
            else
                UIManager.Instance.ChangeSliderValue(eUIGameObjectName.FatePositionSlider_Night,
                Fate.transform.position.x / endPositonOfMap,
                0);
        }

        if (Input.GetKeyUp(KeyCode.Space) && !FollowManager.Instance.TutorialFateCantHide) FateHide(false);
    }
    public void ChangeAnimStatusToStop(bool stop)
    {
        // 대화 중이거나, 만약 필연 또는 우연이 뒤돌아 있으면 다시 이동하지 않음
        if (!stop && !IsEnd)
        {
            if (IsDialogueOpen || accidyStatus != AccidyStatus.GREEN || IsTutorial) return;
        }
        else if (IsEnd)
        {
            stop = true;
        }

        StopAccidy = stop;

        // 이동 중에는 발자국 소리
        SoundPlayer.Instance.UISoundPlay_LOOP(Sound_FootStep_Accidy, !stop);

        // 애니메이션 변경
        Accidy.SetBool("Walking", !stop);
    }
    public void StartGame()
    {
        StopAllCoroutines();
        StartCoroutine(StartGameLogic());
    }
    private IEnumerator StartGameLogic()
    {
        StopAccidy = false;
        IsFateHide = false;

        if (GameSceneManager.Instance.GetActiveScene() == SceneType.FOLLOW_1)
            UIManager.Instance.ChangeSliderValue(eUIGameObjectName.DoubtGaugeSlider, 0, 0);
        else
            UIManager.Instance.ChangeSliderValue(eUIGameObjectName.DoubtGaugeSlider_Night, 0, 0);
        overHeadDoubtGaugeSlider.value = 0;

        // 우연의 움직임, 우연의 말풍선 애니메이션 시작
        StartCoroutine(CameraMove());
        StartCoroutine(AccidyLogic());
        StartCoroutine(AccidyDialogueBoxLogic());
        StartCoroutine(Warning());

        // 미행이 끝날 때까지 반복
        while (!IsEnd)
        {
            while (Time.timeScale == 0) yield return null;

            // 필연이 움직였고 우연이 뒤를 돌아본 상태가 중첩되면 의심 게이지 증가
            if (!IsFateHide && accidyStatus == AccidyStatus.RED)
            {
                ChangeGaugeAlpha(Time.deltaTime * 3);

                if (GameSceneManager.Instance.GetActiveScene() == SceneType.FOLLOW_1)
                    UIManager.Instance.ChangeSliderValue(eUIGameObjectName.DoubtGaugeSlider, 0, 0.001f);
                else
                    UIManager.Instance.ChangeSliderValue(eUIGameObjectName.DoubtGaugeSlider_Night, 0, 0.001f);
                
                overHeadDoubtGaugeSlider.value += 0.001f;
                if (!IsTutorial && Mathf.Approximately(overHeadDoubtGaugeSlider.value, 1)) 
                    FollowManager.Instance.FollowEndLogicStart();
            }
            else
                ChangeGaugeAlpha(-Time.deltaTime);

            yield return null;
        }
    }

    private void ChangeGaugeAlpha(float value)
    {
        Color color = overHeadDoubtGaugeSliderImages[0].color;
        color.a = Mathf.Clamp(color.a + value, 0, 1);
        foreach (Image image in overHeadDoubtGaugeSliderImages) image.color = color;
    }

    private void FateHide(bool hide)
    {
        IsFateHide = hide;
        Fate.SetBool("Hide", hide);
        if(hide) SoundPlayer.Instance.UISoundPlay_LOOP(Sound_FootStep_Fate, false);

        UIManager.Instance.ChangeCursor();
    }

    private IEnumerator AccidyLogic()
    {
        // 3초에서 6초 사이 랜덤한 시간 동안 우연이 움직임
        AccidyAction nextAccidyAction = AccidyAction.Stop;
        float waitingTimeForNextAction = WaitingTimeForNextAccidyAction(nextAccidyAction), currentTime = 0;
        while (!IsEnd)
        {
            if (IsTutorial)
            {
                while (!FollowManager.Instance.TutorialAccidyNextLogic) yield return null;
                FollowManager.Instance.TutorialAccidyNextLogic = false;
            }
            else
            {
                while (Time.timeScale == 0) yield return null;

                while (IsDialogueOpen || (!IsFateHide && accidyStatus == AccidyStatus.RED)) { yield return null; } // 대화창이 열려있음

                // if (nextAccidyAction == AccidyAction.Stop && FollowManager.Instance.ClickCount >= clickCountLimit) { FollowManager.Instance.ClickCount = 0; } // 우연이 걷고 있을 때 클릭을 다섯번 이상하면 우연이가 바로 뒤돌아 봄
                // else if 
                if (currentTime < waitingTimeForNextAction) { currentTime += Time.deltaTime; yield return null; continue; } // 아직 다음 행동을 할 만큼 시간이 흐르지 않음
            }

            switch (nextAccidyAction)
            {
                case AccidyAction.Move:
                    ChangeAnimStatusToStop(false);
                    nextAccidyAction = AccidyAction.Stop;
                    break;

                case AccidyAction.Stop: // 우연의 움직임을 멈춤
                    ChangeAnimStatusToStop(true);
                    accidyStatus = AccidyStatus.YELLOW;
                    nextAccidyAction = AccidyAction.Turn;
                    SoundPlayer.Instance.SetMuteBGM(true);
                    break;

                case AccidyAction.Turn:
                    accidyStatus = AccidyStatus.RED;
                    UIManager.Instance.ChangeCursor();
                    Accidy.SetBool("Back", true);
                    nextAccidyAction = AccidyAction.Inverse_Stop;
                    break;

                case AccidyAction.Inverse_Stop:
                    nextAccidyAction = AccidyAction.Inverse_Turn;
                    break;

                case AccidyAction.Inverse_Turn:
                    accidyStatus = AccidyStatus.GREEN;
                    if(FollowManager.Instance.CanClick) UIManager.Instance.ChangeCursor(false);
                    Accidy.SetBool("Back", false);
                    nextAccidyAction = AccidyAction.Move;
                    SoundPlayer.Instance.SetMuteBGM(false);
                    break;
            }

            currentTime = 0;
            waitingTimeForNextAction = WaitingTimeForNextAccidyAction(nextAccidyAction);
            yield return null;
        }
    }

    private IEnumerator AccidyDialogueBoxLogic()
    {
        TMP_Text text = AccidyDialogueBox.GetComponentInChildren<TextMeshProUGUI>();
        float currentTime = 0;
        while (!IsEnd)
        {
            while (!IsTutorial && IsDialogueOpen) { yield return null; } // 대화창이 열려있음

            if (accidyStatus == AccidyStatus.RED && IsFateHide) { text.text = "?"; currentTime = 0; }
            else if (accidyStatus == AccidyStatus.RED && !IsFateHide) { text.text = "!"; currentTime = 0; }
            else if (accidyStatus == AccidyStatus.YELLOW) text.text = "...?";
            else
            {
                if(text.text == "?" || text.text == "!" || text.text == "...?") text.text = "";

                currentTime += Time.deltaTime;
                if (currentTime > 1)
                {
                    if (text.text.Length < 3) text.text += ".";
                    else text.text = ".";

                    currentTime = 0;
                }
            }
            yield return null;
        }
        AccidyDialogueBox.SetActive(false);
    }
    private IEnumerator CameraMove()
    {
        while (!IsEnd)
        {
            // 평상시: Fate 따라가되 X>=0
            while (!IsEnd && accidyStatus != AccidyStatus.RED)
            {
                float cameraX = Mathf.Max(0, Fate.transform.position.x);
                CameraSmoother.Instance.SetTarget(new Vector3(cameraX, 0f, -10f), Camera.main.orthographicSize);
                yield return null;
            }

            // RED 진입: 두 캐릭터를 화면에 담는 타겟을 매 프레임 갱신
            while (!IsEnd && accidyStatus == AccidyStatus.RED)
            {
                float targetSize = Mathf.Clamp((Accidy.transform.position.x - Fate.transform.position.x) / 3f, 3f, 5f);
                Vector3 targetPos = new Vector3((Fate.transform.position.x + Accidy.transform.position.x) / 2f, targetSize - 5f, -10f);

                CameraSmoother.Instance.SetTarget(targetPos, targetSize);
                yield return null;
            }

            if (!IsEnd)
                yield return new WaitForSeconds(FollowManager.Instance.Zoom(FollowManager.Position.ZoomOut));
        }
    }
    private IEnumerator Warning()
    {
        // 부드럽게 변화시키기 위한 상태값
        float targetAlpha = 0f;
        float currentAlpha = 0f;
        float smoothVel = 0f;

        // 조절 파라미터 (기존 fadeTime=1과 유사하게 동작)
        float smoothTime = 0.2f;   // 값이 작을수록 더 빠르게 수렴
        float holdEpsilon = 0.02f;  // 목표 근접 판정 오차
        bool toOne = true;          // RED가 아닐 때 0↔1 토글용

        // 시작 알파 동기화
        currentAlpha = GetAlpha();
        targetAlpha = 0f;
        SetAlpha(currentAlpha);

        while (IsTutorial)
        {
            yield return null;
        }

        while (!IsEnd)
        {
            // 기존 조건: "트리거 전 대기"
            while (!IsEnd && (accidyStatus != AccidyStatus.RED && Distance > 6 && Distance < 9 || IsDialogueOpen))
                yield return null;

            // 기존 조건: "경고 상태 동작"
            while (!IsEnd && (Distance <= 6 || accidyStatus == AccidyStatus.RED || Distance >= 9) && !IsDialogueOpen)
            {
                // 목표에 충분히 근접하면 방향 전환
                if (Mathf.Abs(currentAlpha - targetAlpha) <= holdEpsilon)
                    toOne = !toOne;

                targetAlpha = toOne ? 1f : 0.1f;

                // 알파를 부드럽게 목표로 수렴
                currentAlpha = Mathf.SmoothDamp(currentAlpha, targetAlpha, ref smoothVel, smoothTime);
                SetAlpha(currentAlpha);

                // 즉시 빠져야 하는 종료/대화 열림 체크
                if (IsEnd || IsDialogueOpen) break;

                yield return null;
            }

            // 경고 상태가 끝났다면 알파를 부드럽게 0으로 수렴
            while (Mathf.Abs(currentAlpha - 0f) > holdEpsilon)
            {
                currentAlpha = Mathf.SmoothDamp(currentAlpha, 0f, ref smoothVel, smoothTime);
                SetAlpha(currentAlpha);
                yield return null;
            }
        }

        // 완전 종료 시 0으로 정리
        SetAlpha(0f);

        // 내부 헬퍼: 알파 읽기/쓰기 (Color는 struct라 재할당 방식 권장)
        float GetAlpha()
        {
            var c = vignette.mainColor;
            return c.a;
        }
        void SetAlpha(float a)
        {
            var c = vignette.mainColor;
            c.a = Mathf.Clamp01(a);
            vignette.mainColor = c;
        }
    }


    public enum AccidyStatus { RED, YELLOW, GREEN } // 빨강: 우연이 보고 있음, 노랑: 우연이 보기 직전, 초록: 우연이 안 봄
    private enum AccidyAction { Move, Stop, Turn, Inverse_Stop, Inverse_Turn }
    private float WaitingTimeForNextAccidyAction(AccidyAction nextAction)
    {
        switch (nextAction)
        {
            case AccidyAction.Move: return 0.5f;
            case AccidyAction.Stop: return Random.Range(5.5f, 7.5f);
            case AccidyAction.Turn: return 0.5f;
            case AccidyAction.Inverse_Stop: return 0.5f;
            case AccidyAction.Inverse_Turn: return 2;

            default: return 0;
        }
    }
}
