using System.Collections;
using UnityEngine;
using static Constants;

public class SceneManager : MonoBehaviour
{
    public static SceneManager Instance { get; private set; }
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

    public void LoadScene(SceneType loadSceneType)
    {
        StartCoroutine(ChangeScene(loadSceneType));
    }

    private IEnumerator ChangeScene(SceneType loadSceneType)
    {
        // 씬이 변경되는 동안 메모 버튼을 누르지 못하도록 꺼둔다
        MemoManager.Instance.SetMemoButtons(false);

        // 대사 출력 중이면 기다리기
        while (DialogueManager.Instance.isDialogueActive)
            yield return null;

        SoundPlayer.Instance.ChangeBGM(BGM_STOP);
        StartCoroutine(ScreenEffect.Instance.OnFade(null, 0, 1, 1, false, 0, 0));

        int sceneIndex = -1;
        switch (loadSceneType)
        {
            case SceneType.START:
                sceneIndex = 0;
                GameManager.Instance.SetVariable("CurrentScene", SceneType.START.ToInt());
                ScreenEffect.Instance.TextOnFade("Prologue");
                break;
            case SceneType.ROOM_1:
                sceneIndex = 1;
                GameManager.Instance.SetVariable("CurrentScene", SceneType.ROOM_1.ToInt());
                ScreenEffect.Instance.TextOnFade("Chapter I");
                break;
            case SceneType.FOLLOW_1:
                sceneIndex = 2;
                GameManager.Instance.SetVariable("CurrentScene", SceneType.FOLLOW_1.ToInt());
                ScreenEffect.Instance.TextOnFade("Chapter II");
                break;
            case SceneType.ROOM_2:
                sceneIndex = 3;
                GameManager.Instance.SetVariable("CurrentScene", SceneType.ROOM_2.ToInt());
                ScreenEffect.Instance.TextOnFade("Chapter III");
                break;
            case SceneType.FOLLOW_2:
                sceneIndex = 4;
                GameManager.Instance.SetVariable("CurrentScene", SceneType.FOLLOW_2.ToInt());
                ScreenEffect.Instance.TextOnFade("Chapter IV");
                break;
            case SceneType.ENDING:
                sceneIndex = 5;
                break;
        }

        yield return new WaitForSeconds(1f);

        // 씬 로드
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneIndex);
    }

    public void ChangeSceneEffect()
    {
        // 방탈출 씬인지 미행 씬인지에 따라 메모 버튼 변경, 대화창의 종류 변경, 방이면 방의 화면 변경
        switch (((int)GameManager.Instance.GetVariable("CurrentScene")).ToEnum())
        {
            case SceneType.START:
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

        int bgmIndex = -1;
        switch (((int)GameManager.Instance.GetVariable("CurrentScene")).ToEnum())
        {
            case SceneType.START:
                bgmIndex = BGM_OPENING;
                SoundPlayer.Instance.ChangeBGM(bgmIndex);
                return;
            case SceneType.ROOM_1:
                bgmIndex = BGM_ROOM1;
                break;
            case SceneType.FOLLOW_1:
                bgmIndex = BGM_FOLLOW1;
                break;
            case SceneType.ROOM_2:
                bgmIndex = BGM_ROOM2;
                break;
            case SceneType.FOLLOW_2:
                bgmIndex = BGM_FOLLOW1;
                break;
        }

        MemoManager.Instance.SetMemoCurrentPageAndFlags();
        MemoManager.Instance.HideMemoButton = false;
        MemoManager.Instance.SetMemoButtons(true);

        SoundPlayer.Instance.ChangeBGM(bgmIndex);
        StartCoroutine(ScreenEffect.Instance.OnFade(null, 1, 0, 1, false, 0, 0));
    }
}

