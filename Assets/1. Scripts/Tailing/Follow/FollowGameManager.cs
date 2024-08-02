using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static Constants;

public class FollowGameManager : MonoBehaviour
{
    [SerializeField] private Slider[] doubtGaugeSliders;
    [SerializeField] private Image[] overHeadDoubtGaugeSliderImages;
    [SerializeField] private GameObject accidyDialogueBox;

    [SerializeField] private Transform backgroundPosition;
    [SerializeField] private Transform frontCanvasPosition;
    public float moveSpeed;
    private float tutorialMoveTime = 0;

    private AccidyStatus accidyStatus = AccidyStatus.GREEN;
    public AccidyStatus NowAccidyStatus { get => accidyStatus; }

    private Animator Fate { get => FollowManager.Instance.Fate; }
    private Animator Accidy { get => FollowManager.Instance.Accidy; }

    private bool IsTutorial { get => FollowManager.Instance.IsTutorial; }
    private bool IsEnd { get; set; }
    private bool IsDialogueOpen { get => FollowManager.Instance.IsDialogueOpen; }
    public bool IsFateHide { get; private set; }

    void Update()
    {
        // 배경 이동
        if (!IsFateHide && accidyStatus == AccidyStatus.GREEN && !IsDialogueOpen) MoveBackground();

        // 스페이스바를 눌렀을 때
        if (!IsEnd && !IsTutorial && Input.GetKeyDown(KeyCode.Space)) FateHide(true);

        // 스페이스바를 뗐을 때
        if (!IsEnd && !IsTutorial && Input.GetKeyUp(KeyCode.Space)) FateHide(false);
    }
    private void MoveBackground()
    {
        if (FollowManager.Instance.IsTutorial)
        {
            tutorialMoveTime += Time.deltaTime;
            if (tutorialMoveTime > 2) return;
        }

        Vector3 moveVector = Vector3.left * moveSpeed * Time.deltaTime;
        backgroundPosition.position += moveVector;
        frontCanvasPosition.position += moveVector;

        FollowManager.Instance.CheckPosition(backgroundPosition.position);
    }
    public void ChangeAnimStatusToStop(bool stop)
    {
        // 대화 중이거나, 만약 필연 또는 우연이 뒤돌아 있으면 다시 이동하지 않음
        if (!stop)
        {
            if (IsDialogueOpen || IsFateHide || accidyStatus != AccidyStatus.GREEN) return;
        }

        // 이동 중에는 발자국 소리
        SoundPlayer.Instance.UISoundPlay_LOOP(Sound_FootStep_Accidy, !stop);
        SoundPlayer.Instance.UISoundPlay_LOOP(Sound_FootStep_Fate, !stop);

        // 애니메이션 변경
        Accidy.SetBool("Walking", !stop);
        Fate.SetBool("Walking", !stop);
    }
    public void StartGame()
    {
        IsFateHide = false;

        StartCoroutine(StartGameLogic());
    }
    private IEnumerator StartGameLogic()
    {
        // 우연의 움직임, 우연의 말풍선 애니메이션 시작
        StartCoroutine(AccidyLogic());
        StartCoroutine(AccidyDialogueBoxLogic());

        // 미행이 끝날 때까지 반복
        while (!IsEnd)
        {
            // 필연이 움직였고 우연이 뒤를 돌아본 상태가 중첩되면 의심 게이지 증가
            if (!IsFateHide && accidyStatus == AccidyStatus.RED)
            {
                ChangeGaugeAlpha(Time.deltaTime * 3);
                doubtGaugeSliders[0].value += 0.001f;
                doubtGaugeSliders[1].value = doubtGaugeSliders[0].value;
                if (doubtGaugeSliders[0].value == 1) FollowManager.Instance.FollowEndLogicStart();
            }
            else ChangeGaugeAlpha(-Time.deltaTime);

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

        ChangeAnimStatusToStop(hide);

        CursorManager.Instance.ChangeCursorInFollow();
    }

    private IEnumerator AccidyLogic()
    {
        // 3초에서 6초 사이 랜덤한 시간 동안 우연이 움직임
        AccidyAction nextAccidyAction = AccidyAction.Stop;
        float waitingTimeForNextAction = WaitingTimeForNextAccidyAction(nextAccidyAction), currentTime = 0;
        while (!IsEnd)
        {
            while (IsDialogueOpen) { yield return null; continue; } // 대화창이 열려있음
            if (currentTime < waitingTimeForNextAction) { currentTime += Time.deltaTime; yield return null; continue; } // 아직 다음 행동을 할 만큼 시간이 흐르지 않음

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
                    break;

                case AccidyAction.Turn:
                    accidyStatus = AccidyStatus.RED;
                    CursorManager.Instance.ChangeCursorInFollow();
                    Accidy.SetTrigger("Turn");
                    nextAccidyAction = AccidyAction.Inverse_Stop;
                    break;

                case AccidyAction.Inverse_Stop:
                    nextAccidyAction = AccidyAction.Inverse_Turn;
                    break;

                case AccidyAction.Inverse_Turn:
                    accidyStatus = AccidyStatus.GREEN;
                    CursorManager.Instance.ChangeCursorInFollow();
                    Accidy.SetTrigger("Turn");
                    nextAccidyAction = AccidyAction.Move;
                    break;
            }

            currentTime = 0;
            waitingTimeForNextAction = WaitingTimeForNextAccidyAction(nextAccidyAction);
            yield return null;
        }
    }

    private IEnumerator AccidyDialogueBoxLogic()
    {
        TMP_Text text = accidyDialogueBox.GetComponentInChildren<TextMeshProUGUI>();
        float currentTime = 0;
        while (!IsEnd)
        {
            while (IsDialogueOpen) { yield return null; } // 대화창이 열려있음

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
        accidyDialogueBox.SetActive(false);
    }


    public enum AccidyStatus { RED, YELLOW, GREEN } // 빨강: 우연이 보고 있음, 노랑: 우연이 보기 직전, 초록: 우연이 안 봄
    private enum AccidyAction { Move, Stop, Turn, Inverse_Stop, Inverse_Turn }
    private float WaitingTimeForNextAccidyAction(AccidyAction nextAction)
    {
        switch (nextAction)
        {
            case AccidyAction.Move: return 0.5f;
            case AccidyAction.Stop: return Random.Range(2.5f, 5.5f);
            case AccidyAction.Turn: return 0.5f;
            case AccidyAction.Inverse_Stop: return 0.5f;
            case AccidyAction.Inverse_Turn: return 2;

            default: return 0;
        }
    }
}
