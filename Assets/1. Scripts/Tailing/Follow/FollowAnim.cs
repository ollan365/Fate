using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using static Constants;

public class FollowAnim : MonoBehaviour
{
    [SerializeField] private Transform backgroundPosition;
    [SerializeField] private Transform frontCanvasPosition;
    [SerializeField] private float moveSpeed;

    [SerializeField] private Image beaconImage;
    [SerializeField] private Sprite[] beaconSprites;

    [SerializeField] private Animator fate, accidyBoy, accidyGirl;
    private Animator accidy;

    [SerializeField] private TextMeshProUGUI stopButtonText; // 이동&멈춤 버튼의 글씨
    private bool isStop = true; // 현재 이동 중인지를 나타내는 변수
    public bool IsStop { get => isStop; }
    private float moveTime = 0;

    private void Start()
    {
        SetCharcter();
        StartCoroutine(ChangeBeaconSprite());
    }
    private void Update()
    {
        if (!isStop) // 배경 이동
        {
            if (FollowManager.Instance.isTutorial)
            {
                moveTime += Time.deltaTime;
                if (moveTime > 2) return;
            }

            Vector3 moveVector = Vector3.left * moveSpeed * Time.deltaTime;
            backgroundPosition.position += moveVector;
            frontCanvasPosition.position += moveVector;

            CheckPosition();
        }
    }
    public void ChangeAnimStatus()
    {
        // 이동을 멈춤 or 시작
        isStop = !isStop;

        // 이동 중에는 발자국 소리
        SoundPlayer.Instance.UISoundPlay_LOOP(Sound_FootStep_Accidy, !isStop);
        SoundPlayer.Instance.UISoundPlay_LOOP(Sound_FootStep_Fate, !isStop);

        if (isStop) stopButtonText.text = "이동";
        else stopButtonText.text = "정지";

        fate.SetBool("Walking", !isStop);
        accidy.SetBool("Walking", !isStop);
    }
    public void SetCharcter()
    {
        // 우연의 성별에 따라 다른 애니메이터 작동
        if ((int)GameManager.Instance.GetVariable("AccidyGender") == 0) accidy = accidyGirl;
        else accidy = accidyBoy;

        fate.gameObject.SetActive(true);
        accidy.gameObject.SetActive(true);
    }

    private IEnumerator ChangeBeaconSprite()
    {
        // 신호등의 색을 3초마다 바꿔준다
        while (gameObject.activeSelf)
        {
            foreach (Sprite sprite in beaconSprites)
            {
                beaconImage.sprite = sprite;
                yield return new WaitForSeconds(3f);
            }
        }
    }

    private void CheckPosition()
    {
        // 첫번째 미행
        if (SceneManager.Instance.CurrentScene == SceneType.FOLLOW_1 && backgroundPosition.position.x < -39)
        {
            ChangeAnimStatus();
            FollowManager.Instance.FollowEndLogicStart();

            // 다음 배경 이동 시에는 반대 방향으로 1.5배 속도
            moveSpeed *= -1.5f;
        }

        // 두번째 미행
        if (SceneManager.Instance.CurrentScene == SceneType.FOLLOW_2)
        {
            switch ((int)backgroundPosition.position.x)
            {
                case 0: ExtraAutoDialogue("Follow2_001"); break;
                case -4: ExtraAutoDialogue("Follow2_002"); break;
                case -12: ExtraAutoDialogue("Follow2_003"); break;
                case -19: ExtraAutoDialogue("Follow2_004"); break;
                case -22: ExtraAutoDialogue("Follow2_005"); break;
                case -31: ExtraAutoDialogue("Follow2_006"); break;
            }
        }
    }

    // === 첫번째 미행 === //
    public void ChangeAnimStatusOnEnd(int num)
    {
        if (num == 0)
        {
            accidy.SetBool("Back", true);
        }
        else if (num == 1)
        {
            fate.speed = 0.6f;
            fate.SetBool("Walking", true);
        }
        else if (num == 2)
        {
            fate.SetBool("Walking", false);
            fate.SetTrigger("Turn");

            // 이동을 시작
            isStop = false;
        }
    }
    public IEnumerator MoveFate()
    {
        accidy.gameObject.SetActive(false); // 꺼두지 않으면 카메라 원상 복귀 때 화면에 잡힘

        Vector3 originPosition = fate.transform.position;
        Vector3 targetPosition = originPosition + new Vector3(-0.3f, 0, 0);

        float elapsedTime = 0f;

        while (elapsedTime < 1.2f)
        {
            fate.transform.position = Vector3.Lerp(originPosition, targetPosition, elapsedTime / 1.2f);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        fate.transform.position = targetPosition;
    }

    // ===== 엑스트라 대화창 ===== //
    private List<string> alreadyType = new();

    public void ExtraAutoDialogue(string dialogueID)
    {
        foreach (string id in alreadyType) // 이미 출력한 대사라면 다시 출력하지 않는다
            if (id == dialogueID) return;

        alreadyType.Add(dialogueID);

        // 테스트용
        int speakerIndex = 0;
        switch (dialogueID)
        {
            case "Follow2_001": speakerIndex = 0; break;
            case "Follow2_002": speakerIndex = 1; break;
            case "Follow2_003": speakerIndex = 3; break;
            case "Follow2_004": speakerIndex = 4; break;
            case "Follow2_005": speakerIndex = 5; break;
            case "Follow2_006": speakerIndex = 7; break;
        }
        // 테스트용

        StartCoroutine(TypeSentence(dialogueID, speakerIndex));
    }
    private IEnumerator TypeSentence(string dialogueID, int speakerIndex)
    {
        DialogueLine dialogueLine;
        string sentence;
        int currentDialogueLineIndex = 0;

        while (FollowManager.Instance.canClick)
        {
            DialogueManager.Instance.dialogues[dialogueID].SetCurrentLineIndex(currentDialogueLineIndex);
            dialogueLine = DialogueManager.Instance.dialogues[dialogueID].Lines[currentDialogueLineIndex];
            sentence = DialogueManager.Instance.scripts[dialogueLine.ScriptID].GetScript();

            // 스피커가 누구인지 확인 본래는 여기서 speakerIndex 구해야함 -> 테스트를 위해 막아둠
            //switch (DialogueManager.Instance.scripts[dialogueLine.SpeakerID].GetScript())
            //{
            //    case "EMPLOYEE": speaker = FollowExtra.Employee; break; // 화자에 따라 다른 캔버스가 켜지도록 만들면 될듯
            //}

            // 대사가 출력된 판넬과 텍스트 받아오기
            FollowManager.Instance.extraCanvas[speakerIndex].SetActive(true);
            FollowManager.Instance.extraDialogueText[speakerIndex].text = "";

            foreach (char letter in sentence.ToCharArray())
            {
                // 다른 물체가 클릭되었을 시
                if (!FollowManager.Instance.canClick)
                {
                    FollowManager.Instance.extraDialogueText[speakerIndex].text = "";
                    break;
                }

                FollowManager.Instance.extraDialogueText[speakerIndex].text += letter;
                SoundPlayer.Instance.UISoundPlay(Sound_Typing); // 타자 소리 한번씩만
                yield return new WaitForSeconds(0.1f);
            }

            yield return new WaitForSeconds(0.25f);

            // 대사가 끝난 후
            string next = DialogueManager.Instance.dialogues[dialogueID].Lines[currentDialogueLineIndex].Next;

            if (DialogueManager.Instance.dialogues.ContainsKey(next))  // Dialogue인 경우 -> 반복문 종료
            {
                FollowManager.Instance.extraCanvas[speakerIndex].SetActive(false);
                ExtraAutoDialogue(next);
                break;
            }
            else if (string.IsNullOrWhiteSpace(next))  // 빈칸인 경우 다음 줄(대사)로 이동
            {
                currentDialogueLineIndex++;

                if (currentDialogueLineIndex >= DialogueManager.Instance.dialogues[dialogueID].Lines.Count)
                {
                    FollowManager.Instance.extraCanvas[speakerIndex].SetActive(false);
                    break; // 더 이상 DialogueLine이 존재하지 않으면 반복문 종료
                }
            }
        }
    }
    
}
