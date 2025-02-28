using System.Collections;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using static Constants;

public class EndingManager : MonoBehaviour
{
    public static EndingManager Instance { get; private set; }
    
    [Header("배경")]
    [SerializeField] private Image background;
    [SerializeField] private Sprite background_room1;
    [SerializeField] private Sprite background_follow1;

    [Header("시계")]
    [SerializeField] private GameObject clock;
    [SerializeField] private Image backgroundClock;
    [SerializeField] private VideoPlayer effectVideo;
    [SerializeField] private Transform hourHand;
    [SerializeField] private Transform minuteHand;
    [SerializeField] private Sprite clockBackgroundAfterEffect;
    [SerializeField] private Sprite hourAfterEffect;
    [SerializeField] private Sprite minuteAfterEffect;
    [SerializeField] private float waitingTime;
    [SerializeField] private float value;

    [Header("미행 1 엔딩")]
    [SerializeField] private VideoClip[] videoClips;
    [SerializeField] private VideoPlayer followVideoPlayer;


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        StartCoroutine(StartEnding());
    }

    public IEnumerator StartEnding()
    {
        // Day UI와 방탈출 이동 버튼 UI off
        UIManager.Instance.SetUI("ActionPoints", false);
        UIManager.Instance.SetUI("MemoGauge", false);
        UIManager.Instance.SetUI("LeftButton", false);
        UIManager.Instance.SetUI("RightButton", false);

        StartCoroutine(ScreenEffect.Instance.OnFade(null, 1, 0, 1, false, 0, 0));
        MemoManager.Instance.HideMemoButton = true;
        MemoManager.Instance.isFollow = false;
        DialogueManager.Instance.dialogueType = DialogueType.ROOM_ACCIDY;

        yield return new WaitForSeconds(2.5f);

        // 배경 바꾸기
        if ((int)GameManager.Instance.GetVariable("CurrentScene") == SceneType.ROOM_1.ToInt())
        {
            background.sprite = background_room1;
            background.color = Color.white;
        }

        EventManager.Instance.CallEvent("EventEnding");
        GameManager.Instance.SetVariable("CurrentScene", SceneType.ENDING.ToInt());
    }

    public void EndEnding(EndingType endingType)
    {
        SaveManager.Instance.SaveEndingDataAndInitGameDataExceptEndingData(endingType);
        StartCoroutine(DelayLoadScene());
    }
    public void TestClock()
    {
        StartCoroutine(DelayLoadScene(true));
    }
    private IEnumerator DelayLoadScene(bool isTest = false)
    {
        StartCoroutine(ScreenEffect.Instance.OnFade(null, 0, 1, 0.5f, true, 0, 0.5f));
        yield return new WaitForSeconds(0.5f);
        clock.SetActive(true);
        StartCoroutine(ClockEffect());
        yield return new WaitForSeconds(15f);

        if (!isTest)
        {
            SceneManager.Instance.LoadScene(SceneType.START);
        }
        if (isTest) SceneManager.Instance.LoadScene(SceneType.ENDING);
    }
    private IEnumerator ClockEffect()
    {
        float acceleration = 0.2f;
        float minuteAngle = 0, hourAngle = 0;
        waitingTime = 0.3f;

        for (int i = 0; i < 5; i++)
        {
            minuteAngle -= 6;
            minuteHand.rotation = Quaternion.Euler(0, 0, 30 + minuteAngle);

            hourAngle -= 0.5f;
            hourHand.rotation = Quaternion.Euler(0, 0, 2.5f + hourAngle);

            waitingTime = waitingTime += acceleration;

            yield return new WaitForSeconds(waitingTime);
        }

        effectVideo.gameObject.SetActive(true);

        while (effectVideo.isPlaying) yield return null;
        acceleration = 0.1f;
        minuteAngle = 0;
        hourAngle = 0;
        waitingTime = 1;
        yield return new WaitForSeconds(3);

        effectVideo.gameObject.SetActive(false);
        backgroundClock.sprite = clockBackgroundAfterEffect;
        hourHand.GetComponent<Image>().sprite = hourAfterEffect;
        minuteHand.GetComponent<Image>().sprite = minuteAfterEffect;

        while (true)
        {
            minuteAngle += 6;
            minuteHand.rotation = Quaternion.Euler(0, 0, minuteAngle);

            hourAngle += 0.5f;
            hourHand.rotation = Quaternion.Euler(0, 0, hourAngle);

            waitingTime = Mathf.Max(Time.deltaTime, waitingTime - acceleration);

            if (waitingTime < 0.15f) acceleration = value;
            else acceleration = Mathf.Max(0.04f, acceleration - 0.007f);

            yield return new WaitForSeconds(waitingTime);
        }
    }
    public void Ending_Follow1_StreetVideo()
    {
        followVideoPlayer.gameObject.SetActive(true);
        followVideoPlayer.clip = videoClips[0];
        followVideoPlayer.Play();
        followVideoPlayer.loopPointReached += OnStreetVideoEnd;
    }
    public void OnStreetVideoEnd(VideoPlayer vp)
    {
        followVideoPlayer.gameObject.SetActive(false);
        EventManager.Instance.CallEvent("EventStreetVideoEnd");
    }
    public void Ending_Follow1()
    {
        if ((int)GameManager.Instance.GetVariable("AccidyGender") == 0) followVideoPlayer.clip = videoClips[1];
        else followVideoPlayer.clip = videoClips[2];

        background.sprite = background_follow1;
        background.color = Color.white;
        followVideoPlayer.loopPointReached += OnFollowFateAndAccidyVideoEnd;

        // 비디오 재생
        followVideoPlayer.gameObject.SetActive(true);
        followVideoPlayer.Play();
    }
    private void OnFollowFateAndAccidyVideoEnd(VideoPlayer vp)
    {
        followVideoPlayer.gameObject.SetActive(false);

        // 우연의 대사 시작
        EventManager.Instance.CallEvent("EventEndUnlockROOM_2");
    }
    public void ChoiceEnding()
    {
        // 배경 변경
        background.color = Color.black;
    }
}
