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

        // 배경 바꾸기
        switch (SceneManager.Instance.CurrentScene)
        {
            case SceneType.ROOM_1:
                background.sprite = background_room1;
                background.color = Color.white;
                StartCoroutine(Ending_Room1());
                break;
            case SceneType.FOLLOW_1:
                StartCoroutine(Ending_Follow1());
                break;
            case SceneType.FOLLOW_2:
                StartCoroutine(Ending_Follow2());
                break;
        }
        blockingPanel.SetActive(true);
    }
    public void EndEnding(EndingType endingType)
    {
        SaveManager.Instance.InitGameData();
        SaveManager.Instance.SaveEndingData(endingType);
        SaveManager.Instance.LoadGameData();
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

        if (MemoManager.Instance.UnlockNextScene()) // 메모의 개수가 충분할 때
            SceneManager.Instance.LoadScene(SceneType.ROOM_2);
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
