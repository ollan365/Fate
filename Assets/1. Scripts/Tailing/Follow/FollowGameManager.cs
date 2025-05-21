using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static Constants;

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
    private bool IsFateMove { get; set; }
    private bool IsTutorial { get => FollowManager.Instance.IsTutorial; }
    void Update()
    {
        if (!Accidy) return;

        if (!StopAccidy && !IsTutorial && !IsEnd) MoveAccidy();

        if (!IsEnd && !IsDialogueOpen || IsTutorial) MoveFate();

        if (!IsEnd) FollowManager.Instance.CheckPosition();
    }
    private void MoveAccidy()
    {
        Vector3 moveVector = Vector3.left * accidyMoveSpeed * Time.deltaTime;
        Accidy.transform.position -= moveVector;
        AccidyDialogueBox.transform.position -= moveVector;
        UIManager.Instance.ChangeSliderValue(eUIGameObjectName.AccidyPositionSlider, Accidy.transform.position.x / endPositonOfMap, 0);
    }
    private void MoveFate()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !FollowManager.Instance.TutorialFateCantHide) FateHide(true);

        if (!IsFateHide && !FollowManager.Instance.TutorialFateNotMovable)
        {
            IsFateMove = false;
            if (Input.GetKey(KeyCode.A))
            {
                if (Fate.transform.position.x > -1) Fate.transform.Translate(Vector3.left * fateMoveSpeed * Time.deltaTime);

                Fate.SetBool("Right", false);
                IsFateMove = true;
            }
            if (Input.GetKey(KeyCode.D))
            {
                if (!IsTutorial || Fate.transform.position.x < 2) Fate.transform.Translate(Vector3.right * fateMoveSpeed * Time.deltaTime);
                Fate.SetBool("Right", true);
                IsFateMove = true;
            }
            SoundPlayer.Instance.UISoundPlay_LOOP(Sound_FootStep_Accidy, IsFateMove);
            Fate.SetBool("Walking", IsFateMove);
            UIManager.Instance.ChangeSliderValue(eUIGameObjectName.FatePositionSlider,
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
        IsFateMove = false;
        IsFateHide = false;
        UIManager.Instance.ChangeSliderValue(eUIGameObjectName.DoubtGaugeSlider, 0, 0);
        overHeadDoubtGaugeSlider.value = 0;

        // 우연의 움직임, 우연의 말풍선 애니메이션 시작
        StartCoroutine(CameraMove());
        StartCoroutine(AccidyLogic());
        StartCoroutine(AccidyDialogueBoxLogic());
        StartCoroutine(Warning());

        // 미행이 끝날 때까지 반복
        while (!IsEnd)
        {
            // 필연이 움직였고 우연이 뒤를 돌아본 상태가 중첩되면 의심 게이지 증가
            if (!IsFateHide && accidyStatus == AccidyStatus.RED)
            {
                ChangeGaugeAlpha(Time.deltaTime * 3);
                UIManager.Instance.ChangeSliderValue(eUIGameObjectName.DoubtGaugeSlider, 0, 0.001f); 
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
                    SoundPlayer.Instance.ChangeBGM(BGM_STOP);
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
                    SoundPlayer.Instance.ChangeBGM(BGM_FOLLOW1);
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

            if (accidyStatus == AccidyStatus.RED && IsFateHide) { text.text = "  ?  "; currentTime = 0; }
            else if (accidyStatus == AccidyStatus.RED && !IsFateHide) { text.text = "  !  "; currentTime = 0; }
            else if (accidyStatus == AccidyStatus.YELLOW) text.text = "...?";
            else
            {
                currentTime += Time.deltaTime;
                if (currentTime > 1)
                {
                    if (text.text.Length < 3) text.text += ".";
                    else text.text = "";

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
            while(!IsEnd && accidyStatus != AccidyStatus.RED)
            {
                float cameraX = Mathf.Max(0, Fate.transform.position.x);
                Camera.main.transform.position = new(cameraX, 0, -10);
                yield return null;
            }

            Vector3 originPosition = Camera.main.transform.position;
            float originSize = Camera.main.orthographicSize;

            float elapsedTime = 0f, zoomTime = 1.5f;

            while (!IsEnd && elapsedTime < zoomTime)
            {
                float targetSize = Mathf.Clamp((Accidy.transform.position.x - Fate.transform.position.x) / 3, 3, 5);
                Vector3 targetPosition = new((Fate.transform.position.x + Accidy.transform.position.x) / 2, targetSize - 5, -10);

                Camera.main.transform.position = Vector3.Lerp(originPosition, targetPosition, elapsedTime / zoomTime);
                Camera.main.orthographicSize = Mathf.Lerp(originSize, targetSize, elapsedTime / zoomTime);
                FollowManager.Instance.CameraAfterBlur.orthographicSize = Camera.main.orthographicSize;

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            while (!IsEnd && accidyStatus == AccidyStatus.RED)
            {
                float targetSize = Mathf.Clamp((Accidy.transform.position.x - Fate.transform.position.x) / 3, 3, 5);
                Vector3 targetPosition = new((Fate.transform.position.x + Accidy.transform.position.x) / 2, targetSize - 5, -10);

                Camera.main.transform.position = targetPosition;
                Camera.main.orthographicSize = targetSize;
                FollowManager.Instance.CameraAfterBlur.orthographicSize = Camera.main.orthographicSize;

                yield return null;
            }

            if (!IsEnd) yield return new WaitForSeconds(FollowManager.Instance.Zoom(FollowManager.Position.ZoomOut));
        }
    }
    private IEnumerator Warning()
    {
        while (!IsEnd)
        {
            while (!IsEnd && (accidyStatus != AccidyStatus.RED && Distance > 6 && Distance < 9 || IsDialogueOpen))
                yield return null;

            float start = 0, end = 1, fadeTime = 1;
            float current = 0, percent = 0;

            while (!IsEnd && (Distance <= 6 || accidyStatus == AccidyStatus.RED || Distance >= 9) && !IsDialogueOpen)
            {
                while (percent < 1 && fadeTime != 0)
                {
                    current += Time.deltaTime;
                    percent = current / fadeTime;

                    vignette.mainColor.a = Mathf.Lerp(start, end, percent);

                    if (IsEnd || IsDialogueOpen) break;

                    yield return null;
                }

                while (!IsFateHide && accidyStatus == AccidyStatus.RED)
                {
                    vignette.mainColor.a = 1;

                    if (IsEnd || IsDialogueOpen) break;

                    yield return null;
                }

                vignette.mainColor.a = end;
                end = start;
                start = vignette.mainColor.a;

                current = 0;
                percent = 0;
            }

            while (percent < 1 && fadeTime != 0)
            {
                current += Time.deltaTime * 2;
                percent = current / fadeTime;

                vignette.mainColor.a = Mathf.Lerp(vignette.mainColor.a, 0, percent);

                yield return null;
            }
        }

        vignette.mainColor.a = 0;
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
