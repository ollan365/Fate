using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
    [SerializeField] private Transform hourHand;
    [SerializeField] private Transform minuteHand;
    [SerializeField] private float acceleration;

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
        StartCoroutine(ScreenEffect.Instance.OnFade(null, 0, 1, 1f, true, 1, 0.5f));
        yield return new WaitForSeconds(1);

        StartCoroutine(InverseClock());
        yield return new WaitForSeconds(7.5f);

        if (!isTest) SaveManager.Instance.LoadGameData();
        if (isTest) SceneManager.Instance.LoadScene(SceneType.START);
    }
    private IEnumerator InverseClock()
    {
        int hour = 0; float speed = 0;
        clock.SetActive(true);
        while (true)
        {
            for (float t = 0; t < 60f; t += Time.deltaTime * speed)
            {
                float minuteAngle = Mathf.Lerp(0, 360, t / 60);
                minuteHand.rotation = Quaternion.Euler(0, 0, 180 + minuteAngle);

                float hourAngle = (hour % 12) * 30f + (minuteAngle / 12f);
                hourHand.rotation = Quaternion.Euler(0, 0, 180 + hourAngle);

                speed += acceleration;
                yield return null;
            }

            minuteHand.rotation = Quaternion.Euler(0, 0, 180);
            hourHand.rotation = Quaternion.Euler(0, 0, 180 + hour % 12 * 30f);
            hour++;
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
