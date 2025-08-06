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
    [SerializeField] private Sprite background_follow2;

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

    [Header("미행 비디오")]
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
        UIManager.Instance.SetUI(eUIGameObjectName.ActionPoints, false);
        UIManager.Instance.SetUI(eUIGameObjectName.MemoGauge, false);
        UIManager.Instance.SetUI(eUIGameObjectName.LeftButton, false);
        UIManager.Instance.SetUI(eUIGameObjectName.RightButton, false);

        StartCoroutine(UIManager.Instance.OnFade(null, 1, 0, 1, false, 0, 0));
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
        StartCoroutine(UIManager.Instance.OnFade(null, 0, 1, 0.5f, true, 0, 0.5f));
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
    // num = 0 : 미행 1 실패 | num = 1 : 미행 1 성공 | num = 3 : 미행 2 엔딩
    public void Ending_Follow_Video(int num)
    {
        followVideoPlayer.gameObject.SetActive(true);

        // 우연 성별에 따라 영상 변경
        if(num == 1 && (int)GameManager.Instance.GetVariable("AccidyGender") == 1) num = 2;

        followVideoPlayer.clip = videoClips[num];
        followVideoPlayer.Play();

        if(num == 3) followVideoPlayer.loopPointReached += OnStreetVideoEnd_2;
        else followVideoPlayer.loopPointReached += OnStreetVideoEnd_1;
    }
    public void OnStreetVideoEnd_1(VideoPlayer vp)
    {
        followVideoPlayer.gameObject.SetActive(false);

        background.gameObject.SetActive(true);
        background.color = Color.white;
        background.sprite = background_follow1;

        EventManager.Instance.CallEvent("EventStreetVideoEnd_1");
    }
    public void OnStreetVideoEnd_2(VideoPlayer vp)
    {
        followVideoPlayer.gameObject.SetActive(false);

        background.gameObject.SetActive(true);
        background.color = Color.white;
        background.sprite = background_follow2;

        EventManager.Instance.CallEvent("EventStreetVideoEnd_2");
    }
    public void ChoiceEnding()
    {
        // 배경 변경
        background.color = Color.black;
    }
}
