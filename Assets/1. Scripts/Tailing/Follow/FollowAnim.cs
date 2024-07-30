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
    [SerializeField] public float moveSpeed;

    [SerializeField] private Image beaconImage;
    [SerializeField] private Sprite[] beaconSprites;

    [SerializeField] private Animator fate;
    [SerializeField] private Animator[] accidyBoy, accidyGirl;
    private Animator accidy;

    private bool isStop = true; // 현재 이동 중인지를 나타내는 변수
    public bool IsStop { get => isStop; }
    private float moveTime = 0;
    

    private void Start()
    {
        StartCoroutine(ChangeBeaconSprite());
    }
    private void Update()
    {
        if (!isStop) // 배경 이동
        {
            if (FollowManager.Instance.IsTutorial)
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
    public void ChangeAnimStatusToStop(bool stop)
    {
        // 대화 중이거나, 만약 필연 또는 우연이 뒤돌아 있으면 다시 이동하지 않음
        if (!stop)
        {
            if (FollowManager.Instance.IsDialogueOpen
                || FollowManager.Instance.IsFateHide
                || FollowManager.Instance.NowAccidyStatus != FollowManager.AccidyStatus.GREEN)

                return;
        }

        // 이동을 멈춤 or 시작
        isStop = stop;

        // 이동 중에는 발자국 소리
        SoundPlayer.Instance.UISoundPlay_LOOP(Sound_FootStep_Accidy, !isStop);
        SoundPlayer.Instance.UISoundPlay_LOOP(Sound_FootStep_Fate, !isStop);

        // 애니메이션 변경
        accidy.SetBool("Walking", !isStop);
        fate.SetBool("Walking", !isStop);
    }
    public void SetCharcter(int index)
    {
        if (accidy != null) accidy.gameObject.SetActive(false);

        // 우연의 성별에 따라 다른 애니메이터 작동
        if ((int)GameManager.Instance.GetVariable("AccidyGender") == 0) accidy = accidyGirl[index];
        else accidy = accidyBoy[index];

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
        if (backgroundPosition.position.x < -39)
        {
            ChangeAnimStatusToStop(true);
            FollowManager.Instance.FollowEndLogicStart();
        }


        // 두번째 미행
        if (SceneManager.Instance.CurrentScene == SceneType.FOLLOW_2)
        {
            switch ((int)backgroundPosition.position.x)
            {
                case 0: ExtraAutoDialogue("Follow2_017"); break; // 호객 행위
                case -4: ExtraAutoDialogue("Follow2_020"); break; // 가출 청소년
            }
        }
    }

    // === 미행이 끝났을 때 === //
    public void ChangeAnimStatusOnEnd(int num)
    {
        if (num == 0)
        {
            accidy.SetTrigger("Turn");
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

        StartCoroutine(TypeSentence(dialogueID));
    }
    private IEnumerator TypeSentence(string dialogueID)
    {
        int currentDialogueLineIndex = 0;

        while (!FollowManager.Instance.IsEnd)
        {
            DialogueManager.Instance.dialogues[dialogueID].SetCurrentLineIndex(currentDialogueLineIndex);
            DialogueLine dialogueLine = DialogueManager.Instance.dialogues[dialogueID].Lines[currentDialogueLineIndex];
            string sentence = DialogueManager.Instance.scripts[dialogueLine.ScriptID].GetScript();
            int speakerIndex = FollowManager.Instance.Int(FollowManager.Instance.ToEnum(dialogueLine.SpeakerID));
            
            // 대사가 출력된 판넬과 텍스트 받아오기
            FollowManager.Instance.extraCanvas[speakerIndex].SetActive(true);
            FollowManager.Instance.extraDialogueText[speakerIndex].text = "";

            foreach (char letter in sentence.ToCharArray())
            {
                // 다른 물체가 클릭되었을 시
                if (FollowManager.Instance.IsDialogueOpen)
                {
                    if (FollowManager.Instance.IsEnd) // 미행이 끝났으면 즉시 종료
                    {
                        FollowManager.Instance.extraCanvas[speakerIndex].SetActive(false);
                        break;
                    }
                    else
                    {
                        // 기존에 출력한 대사까지 저장 후 대화창을 비운다
                        string saveText = FollowManager.Instance.extraDialogueText[speakerIndex].text;
                        FollowManager.Instance.extraDialogueText[speakerIndex].text = "";

                        // 다른 물체의 스크립트가 끝나기를 기다린다
                        while (!FollowManager.Instance.IsDialogueOpen) yield return null;

                        // 다른 물체의 스크립트가 끝나면 다시 이어서 출력
                        FollowManager.Instance.extraCanvas[speakerIndex].SetActive(true);
                        FollowManager.Instance.extraDialogueText[speakerIndex].text = saveText;
                    }
                }

                FollowManager.Instance.extraDialogueText[speakerIndex].text += letter;
                SoundPlayer.Instance.UISoundPlay(Sound_Typing); // 타자 소리 한번씩만
                yield return new WaitForSeconds(0.1f);
            }

            yield return new WaitForSeconds(0.25f);

            // 대사가 끝난 후
            FollowManager.Instance.extraDialogueText[speakerIndex].text = "";
            FollowManager.Instance.extraCanvas[speakerIndex].SetActive(false);

            currentDialogueLineIndex++;

            // 더 이상 DialogueLine이 존재하지 않으면 반복문 종료
            if (currentDialogueLineIndex >= DialogueManager.Instance.dialogues[dialogueID].Lines.Count) break;
            
        }
    }
    
}
