using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Constants;

public class SceneManager : MonoBehaviour
{
    public static SceneManager Instance { get; private set; }
    private SceneType sceneType;
    public SceneType CurrentScene { get => sceneType; }
    public int roomSideIndex = 0;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadScene(SceneType sceneType)
    {
        StartCoroutine(ChangeScene(sceneType));
    }
    private IEnumerator ChangeScene(SceneType sceneType)
    {
        // 씬이 변경되는 동안 메모 버튼을 누르지 못하도록 꺼둔다
        MemoManager.Instance.HideMemoButton(true);

        StartCoroutine(ScreenEffect.Instance.OnFade(null, 0, 1, 1, false, 0, 0));

        int sceneIndex = -1, bgmIndex = -1;
        switch (sceneType)
        {
            case SceneType.START:
                sceneIndex = 0;
                sceneType = SceneType.START;
                break;
            case SceneType.ROOM_1:
                sceneIndex = 1;
                bgmIndex = BGM_ROOM;
                sceneType = SceneType.ROOM_1;
                break;
            case SceneType.FOLLOW_1:
                sceneIndex = 2;
                bgmIndex = BGM_FOLLOW;
                sceneType = SceneType.FOLLOW_1;
                break;
            case SceneType.ROOM_2:
                sceneIndex = 3;
                sceneType = SceneType.ROOM_2;
                break;
            case SceneType.FOLLOW_2:
                sceneIndex = 3; // 병합 후 4로 바꾸기!
                sceneType = SceneType.FOLLOW_2;
                break;
        }
        yield return new WaitForSeconds(1f);

        // 씬 로드
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneIndex);

        // 방탈출 씬인지 미행 씬인지에 따라 메모 버튼 변경, 대화창의 종류 변경, 방이면 방의 화면 변경
        switch (sceneType)
        {
            case SceneType.ROOM_1:
            case SceneType.ROOM_2:
                MemoManager.Instance.isFollow = false;
                DialogueManager.Instance.dialogueType = DialogueType.ROOM;
                // RoomManager.Instance.currentSideIndex = roomSideIndex;
                break;
            case SceneType.FOLLOW_1:
            case SceneType.FOLLOW_2:
                MemoManager.Instance.isFollow = true;
                DialogueManager.Instance.dialogueType = DialogueType.FOLLOW;
                break;
        }

        MemoManager.Instance.MemoButtonChange();
        MemoManager.Instance.HideMemoButton(false);

        // 배경음과 페이드 효과
        SoundPlayer.Instance.ChangeBGM(bgmIndex, true);
        StartCoroutine(ScreenEffect.Instance.OnFade(null, 1, 0, 1, false, 0, 0));
    }
}
