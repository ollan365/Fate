using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] private bool isTesting;

    [Header("미행 1 엔딩")]
    [SerializeField] private GameObject fate;
    [SerializeField] private GameObject[] accidys;
    private GameObject accidy;


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

        if (isTesting) ScreenEffect.Instance.coverPanel.gameObject.SetActive(false);

        StartCoroutine(StartEnding());
    }

    public IEnumerator StartEnding()
    {
        StartCoroutine(ScreenEffect.Instance.OnFade(null, 1, 0, 1, false, 0, 0));
        MemoManager.Instance.HideMemoButton = true;
        MemoManager.Instance.isFollow = false;
        DialogueManager.Instance.dialogueType = DialogueType.ROOM;

        yield return new WaitForSeconds(2.5f);

        // 배경 바꾸기
        if ((int)GameManager.Instance.GetVariable("CurrentScene") == SceneType.ROOM_1.ToInt())
        {
            background.sprite = background_room1;
            background.color = Color.white;
        }

        EventManager.Instance.CallEvent("EventEnding");
    }

    public void EndEnding(EndingType endingType)
    {
        SaveManager.Instance.InitGameData();
        SaveManager.Instance.SaveEndingData(endingType);
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
            SaveManager.Instance.LoadGameData();
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
    public IEnumerator Ending_Follow1()
    {
        background.sprite = background_follow1;
        background.color = Color.white;

        if ((int)GameManager.Instance.GetVariable("AccidyGender") == 0) accidy = accidys[0];
        else accidy = accidys[1];

        // 필연이 앞으로 걸어나옴
        while (true)
        {
            fate.transform.position += Vector3.up * Time.deltaTime * 5;
            if (fate.transform.position.y >= 2.3f)
            {
                fate.transform.position = new(fate.transform.position.x, 2.3f, fate.transform.position.z);
                break;
            }
            yield return null;
        }
        yield return new WaitForSeconds(1.5f);

        // 우연이 앞으로 걸어나옴
        while (true)
        {
            accidy.transform.position += Vector3.up * Time.deltaTime;
            if (accidy.transform.position.y >= 0)
            {
                accidy.transform.position = new(accidy.transform.position.x, 0, accidy.transform.position.z);
                break;
            }
            yield return null;
        }
        yield return new WaitForSeconds(0.5f);

        // 우연의 대사 시작
        EventManager.Instance.CallEvent("EventEndUnlockROOM_2");
    }
    public void ChoiceEnding()
    {
        // 배경 변경
        background.color = Color.black;
    }
}
