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
        StartCoroutine(ScreenEffect.Instance.OnFade(null, 0, 1, 1, false, 0, 0));

        int sceneIndex = -1, bgmIndex = -1;
        switch (sceneType)
        {
            case SceneType.START:
                sceneIndex = 0;
                break;
            case SceneType.ROOM_1:
                sceneIndex = 1;
                bgmIndex = BGM_ROOM;
                break;
            case SceneType.FOLLOW_1:
                sceneIndex = 2;
                bgmIndex = BGM_FOLLOW;
                break;
            case SceneType.ROOM_2:
                sceneIndex = 3;
                break;
            case SceneType.FOLLOW_2:
                sceneIndex = 4;
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
                MemoManager.Instance.MemoButtonChange();
                DialogueManager.Instance.dialogueType = DialogueType.ROOM;
                // RoomManager.Instance.currentSideIndex = roomSideIndex;
                break;
            case SceneType.FOLLOW_1:
            case SceneType.FOLLOW_2:
                MemoManager.Instance.isFollow = true;
                MemoManager.Instance.MemoButtonChange();
                DialogueManager.Instance.dialogueType = DialogueType.FOLLOW;
                break;
        }

        // 배경음과 페이드 효과
        SoundPlayer.Instance.ChangeBGM(bgmIndex, true);
        StartCoroutine(ScreenEffect.Instance.OnFade(null, 1, 0, 1, false, 0, 0));
    }
}

