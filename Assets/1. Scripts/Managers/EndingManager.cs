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
        // 배경 바꾸기
        switch (SceneManager.Instance.CurrentScene)
        {
            case SceneType.ROOM_1:
                background.sprite = background_room1;
                background.color = Color.white;
                StartCoroutine(Ending_Room1());
                break;
        }
        blockingPanel.SetActive(true);
    }
    private IEnumerator Ending_Room1()
    {
        yield return new WaitForSeconds(2.5f);

        if (MemoManager.Instance.UnlockNextScene()) // 메모의 개수가 충분할 때
            DialogueManager.Instance.StartDialogue("FollowTutorial_001");
        else
            DialogueManager.Instance.StartDialogue("BadEndingA_ver1_01");
    }
    public void ChoiceEnding()
    {
        // 배경 변경
        background.color = Color.black;
    }
}