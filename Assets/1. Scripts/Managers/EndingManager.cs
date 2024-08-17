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
    [SerializeField] private GameObject blockingPanel;
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

        StartEnding();
    }

    public void StartEnding()
    {
        MemoManager.Instance.HideMemoButton = true;
        MemoManager.Instance.isFollow = false;
        DialogueManager.Instance.dialogueType = DialogueType.ROOM;
        blockingPanel.SetActive(true);

        // 배경 바꾸기
        switch (SceneManager.Instance.CurrentScene)
        {
            case SceneType.ROOM_1:
                background.sprite = background_room1;
                background.color = Color.white;
                StartCoroutine(Ending_Room1());
                break;
            case SceneType.FOLLOW_1:
                if (MemoManager.Instance.UnlockNextScene())
                {
                    blockingPanel.SetActive(false);
                    background.sprite = background_follow1;
                    background.color = Color.white;
                }
                StartCoroutine(Ending_Follow1());
                break;
            case SceneType.FOLLOW_2:
                StartCoroutine(Ending_Follow2());
                break;
        }
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

        if (!isTest) SaveManager.Instance.LoadGameData();
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
    private IEnumerator Ending_Room1()
    {
        yield return new WaitForSeconds(2.5f);

        if (MemoManager.Instance.UnlockNextScene()) // 메모의 개수가 충분할 때
            DialogueManager.Instance.StartDialogue("FollowTutorial_001");
        else
        {
            SoundPlayer.Instance.ChangeBGM(BGM_BAD);
            DialogueManager.Instance.StartDialogue("BadEndingA_ver1_01");
        }
    }
    private IEnumerator Ending_Follow1()
    {
        yield return new WaitForSeconds(2.5f);

        if (MemoManager.Instance.UnlockNextScene())
        {
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
            blockingPanel.SetActive(true);
            DialogueManager.Instance.StartDialogue("Follow1Final_003");
        }
        else
        {
            SoundPlayer.Instance.ChangeBGM(BGM_BAD);
            DialogueManager.Instance.StartDialogue("BadEndingA_ver2_01");
        }
    }
    private IEnumerator Ending_Follow2()
    {
        yield return new WaitForSeconds(2.5f);

        if (MemoManager.Instance.UnlockNextScene() && MemoManager.Instance.UnlockNextScene(true))
        {
            SoundPlayer.Instance.ChangeBGM(BGM_HIDDEN);
            DialogueManager.Instance.StartDialogue("HiddenEnding_00");
        }
        else if (MemoManager.Instance.UnlockNextScene() || MemoManager.Instance.UnlockNextScene(true))
        {
            SoundPlayer.Instance.ChangeBGM(BGM_TRUE);
            DialogueManager.Instance.StartDialogue("TrueEnding_01");
        }
        else
        {
            SoundPlayer.Instance.ChangeBGM(BGM_BAD);
            DialogueManager.Instance.StartDialogue("BadEndingA_ver2_01");
        }
    }
    public void ChoiceEnding()
    {
        // 배경 변경
        background.color = Color.black;
    }
}
