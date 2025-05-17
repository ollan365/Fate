using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Constants;

public class SceneManager : MonoBehaviour
{
    public static SceneManager Instance { get; private set; }
    // public int roomSideIndex = 0;
    
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
    public void GoTitle()
    {
        LoadScene(SceneType.START);
    }
    
    public void LoadScene(SceneType loadSceneType)
    {
        if (loadSceneType == SceneType.ENDING)
            StartCoroutine(LoadEnding());
        else
            StartCoroutine(ChangeScene(loadSceneType));
    }
    
    private IEnumerator LoadEnding()
    {
        // 대사 출력 중이면 기다리기
        while (DialogueManager.Instance.isDialogueActive)
            yield return null;

        MemoManager.Instance.SetMemoButtons(false);
        SoundPlayer.Instance.ChangeBGM(BGM_STOP);
        StartCoroutine(UIManager.Instance.OnFade(null, 0, 1, 1, false, 0, 0));

        yield return new WaitForSeconds(1f);

        UnityEngine.SceneManagement.SceneManager.LoadScene(SceneType.ENDING.ToInt());
    }
    
    public int GetActiveScene()
    {
        Scene activeScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        return activeScene.buildIndex;
    }

    private IEnumerator ChangeScene(SceneType loadSceneType)
    {
        // 대사 출력 중이면 기다리기
        while (DialogueManager.Instance.isDialogueActive)
            yield return null;

        MemoManager.Instance.SetMemoButtons(false);
        SoundPlayer.Instance.ChangeBGM(BGM_STOP);
        StartCoroutine(UIManager.Instance.OnFade(null, 0, 1, 1, false, 0, 0));

        switch (loadSceneType)
        {
            case SceneType.START:
                UIManager.Instance.TextOnFade("Prologue");
                break;
            case SceneType.ROOM_1:
                GameManager.Instance.SetVariable("CurrentScene", SceneType.ROOM_1.ToInt());
                UIManager.Instance.TextOnFade("Chapter I");
                break;
            case SceneType.FOLLOW_1:
                GameManager.Instance.SetVariable("CurrentScene", SceneType.FOLLOW_1.ToInt());
                UIManager.Instance.TextOnFade("Chapter II");
                break;
            case SceneType.ROOM_2:
                GameManager.Instance.SetVariable("CurrentScene", SceneType.ROOM_2.ToInt());
                UIManager.Instance.TextOnFade("Chapter III");
                break;
            case SceneType.FOLLOW_2:
                GameManager.Instance.SetVariable("CurrentScene", SceneType.FOLLOW_2.ToInt());
                UIManager.Instance.TextOnFade("Chapter IV");
                break;
        }

        yield return new WaitForSeconds(1f);

        // 씬 로드
        UnityEngine.SceneManagement.SceneManager.LoadScene((int)GameManager.Instance.GetVariable("CurrentScene"));
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
                DialogueManager.Instance.dialogueType = DialogueType.ROOM_ACCIDY;
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
        StartCoroutine(UIManager.Instance.OnFade(null, 1, 0, 1, false, 0, 0));
    }

    public void NotClearThisScene()
    {
        string currentScene = ((int)GameManager.Instance.GetVariable("CurrentScene")).ToEnum().ToString();
        GameManager.Instance.SetVariable($"MemoCount_{currentScene}", 0);

        LoadScene(SceneType.ENDING);
    }
    
    public void ClearThisScene()
    {
        string currentScene = ((int)GameManager.Instance.GetVariable("CurrentScene")).ToEnum().ToString();
        GameManager.Instance.SetVariable($"MemoCount_{currentScene}", 9);

        LoadScene(SceneType.ENDING);
    }
}

