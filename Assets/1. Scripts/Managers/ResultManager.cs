using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Constants;
using Random = Unity.Mathematics.Random;

public class ResultManager : MonoBehaviour
{
    public static ResultManager Instance { get; private set; }

    private TextAsset resultsCSV;
    
    // results: dictionary of "Results"s indexed by string "Result ID"
    public Dictionary<string, Result> results = new Dictionary<string, Result>();
    
    // 이벤트 오브젝트 참조
    private Dictionary<string, IResultExecutable> executableObjects = new Dictionary<string, IResultExecutable>();
    
    public void RegisterExecutable(string objectName, IResultExecutable executable)
    {
        //Debug.Log($"registered {objectName}");

        if (!executableObjects.ContainsKey(objectName))
        {
            executableObjects[objectName] = executable;
        }
    }

    public void InitializeExecutableObjects()
    {
        //Debug.Log("############### unregistered all executable objects ###############");

        executableObjects = new Dictionary<string, IResultExecutable>();
    }

    public void executableObjectsKeyCheck(string objectName)
    {
        if (executableObjects.ContainsKey(objectName))
            Debug.Log(objectName + " key exists");
        else
            Debug.Log(objectName + " key does not exist");
    }

    void Awake()
    {
        if (Instance == null)
        {
            resultsCSV = Resources.Load<TextAsset>("Datas/results");
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public void ParseResults()
    {
        string[] lines = resultsCSV.text.Split('\n');
        for (int i = 1; i < lines.Length; i++)
        {
            string[] fields = lines[i].Split(',');
            
            if ((string.IsNullOrWhiteSpace(lines[i])) || (fields[0] == "" && fields[1] == "")) continue;
            
            Result result = new Result(
                fields[0].Trim(),
                fields[1].Trim(),
                fields[2].Trim()
                );
            
            results[result.ResultID] = result;
        }
    }

    public void ExecuteResult(string resultID)
    {
        if (GameManager.Instance.isDebug) Debug.Log(resultID);
        string variableName;

        // ------------------------ 이곳에 모든 동작을 수동으로 추가 ------------------------
        switch (resultID)
        {
            case string temp when temp.StartsWith("Result_Increment"):  // 값++
                variableName = temp.Substring("Result_Increment".Length);
                GameManager.Instance.IncrementVariable(variableName);
                break;

            case string temp when temp.StartsWith("Result_Decrement"):  // 값--
                variableName = temp.Substring("Result_Decrement".Length);
                GameManager.Instance.DecrementVariable(variableName);
                break;

            case string temp when temp.StartsWith("Result_Inverse"):  // !값
                variableName = temp.Substring("Result_Inverse".Length);
                GameManager.Instance.InverseVariable(variableName);
                break;

            case string temp when temp.StartsWith("Result_IsFinished"):  // 조사 후 EventObject의 isFinished를 true로
                variableName = temp.Substring("Result_isFinished".Length);
                GameManager.Instance.SetEventFinished(variableName);
                break;

            case string temp when temp.StartsWith("Result_IsUnFinished"):  
                // EventObject의 isFinished를 false로 (포스터 커터칼 있는채로 다시 조사하면 처음 조사 스크립트 나와야 해서 추가됨)
                variableName = temp.Substring("Result_IsUnFinished".Length);
                GameManager.Instance.SetEventUnFinished(variableName);
                break;

            case "Result_girl":  // 우연의 성별을 여자로 설정
                GameManager.Instance.SetVariable("AccidyGender", 0);
                DialogueManager.Instance.EndDialogue();
                DialogueManager.Instance.StartDialogue("Prologue_006");
                break;
            
            case "Result_boy":  // 우연의 성별을 남자로 설정
                GameManager.Instance.SetVariable("AccidyGender", 1);
                DialogueManager.Instance.EndDialogue();
                DialogueManager.Instance.StartDialogue("Prologue_007");
                break;

            case "ResultCloseEyes": // 눈 깜빡이는 효과
                StartCoroutine(ScreenEffect.Instance.OnFade(null, 0, 1, 1, true, 0.5f, 0));
                break;

            case "Result_FadeOut":  // fade out
                float fadeOutTime = 3f;
                StartCoroutine(ScreenEffect.Instance.OnFade(null, 0, 1, fadeOutTime));
                break;

            case "Result_FadeIn":  // fade out
                float fadeInTime = 3f;
                StartCoroutine(ScreenEffect.Instance.OnFade(null, 1, 0, fadeInTime));
                break;

            case "ResultPrologueLimit":
                StartLogic.Instance.DarkBackground(true);
                StartCoroutine(DialogueManager.Instance.StartDialogue("Prologue_000", 3));
                break;

            case "ResultCommonPrologueA":
                StartLogic.Instance.DarkBackground(false);
                StartCoroutine(DialogueManager.Instance.StartDialogue("Prologue_002", 3));
                break;

            case "ResultName": // 이름 입력창
                StartLogic.Instance.OpenNamePanel();
                break;

            case "ResultBirth": // 생일 입력창
                StartLogic.Instance.OpenBirthPanel();
                break;

            case "ResultNameSetting":
                DialogueManager.Instance.StartDialogue("Prologue_Birth");
                break;

            case "ResultBirthSetting":
                DialogueManager.Instance.StartDialogue("Prologue_008_B");
                break;

            case "ResultPrologueQuestions":
                DialogueManager.Instance.StartDialogue("Prologue_Name");
                break;

            case "ResultCommonPrologueB":
                DialogueManager.Instance.StartDialogue("Prologue_008_C");
                break;

            case "Result_002": // 설명창 띄우기 - ### 추후 구현 필요 ###
                break;

            case "ResultPrologueEnd":
                SceneManager.Instance.LoadScene(SceneType.ROOM_1);
                break;

            case "ResultTimePass": // 행동력 감소 (행동력이 감소할 때마다 게임 저장)
                SoundPlayer.Instance.UISoundPlay(Sound_HeartPop);
                RoomManager.Instance.actionPointManager.DecrementActionPoint();
                break;

            // 조사 시스템
            case "ResultInquiry": // 조사 선택 묻기
                //Debug.Log("현재 오브젝트 : "+ GameManager.Instance.GetCurrentInquiryObjectId()
                //    +" : "+ GameManager.Instance.GetEventStatus(GameManager.Instance.GetCurrentInquiryObjectId()));
                if (!GameManager.Instance.GetEventStatus(GameManager.Instance.GetCurrentInquiryObjectId()))
                {
                    DialogueManager.Instance.EndDialogue();

                    EventManager.Instance.CallEvent(GameManager.Instance.GetCurrentInquiryObjectId());
                    GameManager.Instance.SetVariable("isInquiry", false);
                }
                else
                {
                    //Debug.Log("중복조사 발생");
                    DialogueManager.Instance.StartDialogue("RoomEscape_Inquiry2");
                }
                break;

            case "ResultInquiryYes": // 조사 예 선택
                GameManager.Instance.SetVariable("isInquiry", true);
                DialogueManager.Instance.EndDialogue();

                EventManager.Instance.CallEvent(GameManager.Instance.GetCurrentInquiryObjectId());
                GameManager.Instance.SetVariable("isInquiry", false);
                break;

            case "ResultInquiryNo": // 조사 아니오 선택
                GameManager.Instance.SetVariable("isInquiry",false);
                DialogueManager.Instance.EndDialogue();
                break;
            
            // 휴식 시스템
            case "Result_restButton":
                DialogueManager.Instance.EndDialogue();
                DialogueManager.Instance.StartDialogue("RoomEscape_034");
                break;

            case "Result_restYes": // 휴식에서 예 버튼
                SoundPlayer.Instance.UISoundPlay(Sound_HeartPop);
                DialogueManager.Instance.EndDialogue();
                // 휴식취함(다음날로 넘어가는 만큼 행동력 감소, 날짜와 하트 업데이트)
                // fade in, fade out 이후 휴식 대사 출력되고 우연 랜덤 대사 출력됨
                StartCoroutine(RoomManager.Instance.actionPointManager.TakeRest());
                break;

            case "Result_restNo": // 휴식에서 아니오 버튼
                DialogueManager.Instance.EndDialogue();
                break;

            case "Result_StartHomecoming":
                // 휴식 대사 스크립트 끝난 다음 귀가 대사 스크립트 출력되게 RefillHeartsOrEndDay 호출.
                DialogueManager.Instance.EndDialogue();
                RoomManager.Instance.actionPointManager.RefillHeartsOrEndDay();
                break;

            case "Result_NextMorningDay":    // 휴식 대사 스크립트 마지막인 Next에서 호출됨.
                // fade in/out effect 실행 후 아침 대사 출력하는 메소드 호출
                DialogueManager.Instance.EndDialogue();
                RoomManager.Instance.actionPointManager.nextMorningDay();
                break;

            // 침대 시스템
            case "ResultBedChoice": // 침대 선택지 나오게 함
                DialogueManager.Instance.StartDialogue("RoomEscape_BedClick");
                break;

            case "ResultBlanketCheck": // 조사하기 버튼 누르면 침대 조사할 수 있게 함
                DialogueManager.Instance.EndDialogue();
                GameManager.Instance.SetVariable("isInquiry", true);
                GameManager.Instance.SetCurrentInquiryObjectId("EventBlanket");
                EventManager.Instance.CallEvent("Event_Inquiry");
                break;
                

            // 튜토리얼
            case "Result_nextTutorialPhase":  // 튜토리얼 다음 페이즈로 진행
                RoomManager.Instance.tutorialManager.ProceedToNextPhase();
                break;

            case "Result_TutorialPhase1Force":  // 방을 둘러보자 (이동버튼 강조)
                RoomManager.Instance.imageAndLockPanelManager.SetTutorialImageObject(true, "TutorialMoveButton");
                DialogueManager.Instance.StartDialogue("TutorialForce_001");
                break;

            case "Result_TutorialPhase2ChairStateCheck": // 의자가 밀고 있는 상태인지 체크
                StartCoroutine(RoomManager.Instance.tutorialManager.CheckChairMovement());
                break;

            case "Result_TutorialPhase2ForceSide1":  // 의자 밀어보자 (의자 강조)
                RoomManager.Instance.imageAndLockPanelManager.SetTutorialImageObject(true, "TutorialChair");
                DialogueManager.Instance.StartDialogue("TutorialForce_002");
                break;

            case "Result_TutorialPhase2Force":  // 의자 밀어보자 (이미지 강조는 X 검은 화면O)
                RoomManager.Instance.imageAndLockPanelManager.SetTutorialBlockingPanel(true);
                DialogueManager.Instance.StartDialogue("TutorialForce_002");
                break;

            case "Result_TutorialPhase3ForceSide1":  // 카펫 들어보자 (덮인 카펫 강조)
                RoomManager.Instance.imageAndLockPanelManager.SetTutorialImageObject(true, "TutorialCarpet");
                DialogueManager.Instance.StartDialogue("TutorialForce_003");
                break;

            case "Result_TutorialPhase3Force":  // 카펫 들어보자 (이미지 강조는 X 검은 화면O)
                RoomManager.Instance.imageAndLockPanelManager.SetTutorialBlockingPanel(true);
                DialogueManager.Instance.StartDialogue("TutorialForce_003");
                break;

            case "Result_TutorialPhase4ForceSide1":  // 종이 조사해보자 (카펫 밑 종이 강조)
                RoomManager.Instance.imageAndLockPanelManager.SetTutorialImageObject(true, "TutorialCarpetPaper");
                DialogueManager.Instance.StartDialogue("TutorialForce_004");
                break;

            case "Result_TutorialPhase4Force":  // 종이 조사해보자 (이미지 강조는 X 검은 화면O)
                RoomManager.Instance.imageAndLockPanelManager.SetTutorialBlockingPanel(true);
                DialogueManager.Instance.StartDialogue("TutorialForce_004");
                break;

            case "Result_TutorialPhase5ForceSide1":  // 열린 카펫 덮자 (열린 카펫 강조)
                RoomManager.Instance.imageAndLockPanelManager.SetTutorialImageObject(true, "TutorialCarpetOpen");
                DialogueManager.Instance.StartDialogue("TutorialForce_005");
                break;

            case "Result_TutorialPhase5Force":  // 열린 카펫 덮자 (이미지 강조는 X 검은 화면O)
                RoomManager.Instance.imageAndLockPanelManager.SetTutorialBlockingPanel(true);
                DialogueManager.Instance.StartDialogue("TutorialForce_005");
                break;

            case "Result_DayPass":  // fade in/out 후 대사 출력
                const float totalTime = 3f;
                StartCoroutine(ScreenEffect.Instance.DayPass(totalTime));  // fade in/out effect
                StartCoroutine(DialogueManager.Instance.StartDialogue("RoomEscapeS_004", totalTime));
                break;

            // 다회차 곰인형
            case "ResultNewTeddyBearScript":
                DialogueManager.Instance.StartDialogue("RoomEscape2nd_001");
                break;

            case "ResultNewTeddyBearMemo":
                MemoManager.Instance.RevealMemo("R1Memo_014");
                break;

            case "ResultNewTeddyBearZoom":
                RoomManager.Instance.imageAndLockPanelManager.SetObjectImageGroup(true, "newTeddyBear");
                break;


            // 방탈출1

            case "ResultTeddyBearScript": // 곰인형에 대한 설명
                DialogueManager.Instance.StartDialogue("RoomEscape_001");
                break;

            case "ResultBlanketScript": // 이불에 대한 설명
                DialogueManager.Instance.StartDialogue("RoomEscape_002");
                break;
            
            case "ResultDrinkAndMedicineScript": // 술과 감기약에 대한 설명
                RoomManager.Instance.imageAndLockPanelManager.SetObjectImageGroup(true, "liquorAndPills");
                DialogueManager.Instance.StartDialogue("RoomEscape_003");
                break;

            case "ResultDrinkAndMedicinMemo": // 술과 감기약에 대한 메모 작성
                MemoManager.Instance.RevealMemo("R1Memo_012");
                break;

            case "ResultStandLightScript": // 책상 위 스탠드에 대한 설명
                DialogueManager.Instance.StartDialogue("RoomEscape_004");
                break;
            
            case "ResultBearStorageClosetyScript": // 인형 수납장에 대한 설명
                DialogueManager.Instance.StartDialogue("RoomEscape_005");
                break;

            case "ResultPillowScript": // 베개에 대한 설명
                DialogueManager.Instance.StartDialogue("RoomEscape_006");
                break;
            
            case "ResultPillowAmuletScript": // 베개 안에 부적을 발견
                RoomManager.Instance.imageAndLockPanelManager.SetObjectImageGroup(true, "amulet");
                DialogueManager.Instance.StartDialogue("RoomEscape_007");
                break;
            
            case "ResultPillowAmuletMemo": // 부적에 대한 메모 작성
                MemoManager.Instance.RevealMemo("R1Memo_001");
                break;

            case "ResultDiaryScript": // 다이어리에 대한 설명
                DialogueManager.Instance.StartDialogue("RoomEscape_009");
                break;
            
            case "ResultDiaryLockActivation": // 다이어리 잠금 장치 활성화됨
                RoomManager.Instance.imageAndLockPanelManager.SetLockObject(true, "diary");
                executableObjects["Diary"].ExecuteAction();
                break;
            
            case "ResultDiaryLockOpen": // 다이어리가 열림
                executableObjects["DiaryLock"].ExecuteAction();
                break;
            
            case "ResultDiaryMemo": // 다이어리에 대한 메모가 작성됨
                MemoManager.Instance.RevealMemo("R1Memo_002");
                break;
            
            case "ResultDiaryContentScript": // 다이어리에 대한 스크립트
                DialogueManager.Instance.StartDialogue("RoomEscape_010");
                break;
            
            case "ResultDiaryLockFailedScript": // 다이어리 비밀번호를 틀렸다는 스크립트
                SoundPlayer.Instance.UISoundPlay(Sound_DiaryUnlock);
                DialogueManager.Instance.StartDialogue("RoomEscape_011");
                break;
            
            case "Result_showClockImage":  // 시계 확대 이미지를 표시
                RoomManager.Instance.imageAndLockPanelManager.SetObjectImageGroup(true, "clock");
                break;
            
            case "ResultClockScript": // 시계에 대한 설명
                DialogueManager.Instance.StartDialogue("RoomEscape_012");
                break;

            case "ResultClockSystemActivartion": // 시계 시스템 활성화
                RoomManager.Instance.imageAndLockPanelManager.SetLockObject(true, "clock");
                executableObjects["Clock"].ExecuteAction();
                break;

            case "ResultClockGetKey": // 열쇠를 획득
                SoundPlayer.Instance.UISoundPlay(Sound_Key);
                RoomManager.Instance.imageAndLockPanelManager.SetObjectImageGroup(true, "keys");
                break;

            case "ResultClockGetKeyScript": // 열쇠를 획득 후 스크립트
                DialogueManager.Instance.StartDialogue("RoomEscape_013");
                break;

            case "ResultClockGetKeyMemo": // 열쇠 획득 후 메모
                MemoManager.Instance.RevealMemo("R1Memo_003");
                break;

            case "ResultChairMoved": // 의자가 옆으로 이동함
                SoundPlayer.Instance.UISoundPlay(Sound_ChairMovement);
                executableObjects["Chair1"].ExecuteAction();
                executableObjects["Chair2"].ExecuteAction();
                executableObjects["Chair3"].ExecuteAction();
                GameManager.Instance.InverseVariable("ChairMoved");
                break;

            case "ResultLaptopScript": // 노트북에 대한 설명
                DialogueManager.Instance.StartDialogue("RoomEscape_014");
                break;

            case "ResultLaptopLockActivation": // 노트북 비밀번호 화면 활성화
                SoundPlayer.Instance.UISoundPlay(Sound_LaptopBoot);
                RoomManager.Instance.imageAndLockPanelManager.SetLockObject(true, "laptop");
                executableObjects["Laptop"].ExecuteAction();
                break;

            case "ResultLaptopLockOpen": // 노트북이 열림
                SoundPlayer.Instance.UISoundPlay(Sound_Correct);
                executableObjects["LaptopLock"].ExecuteAction();
                break;

            case "ResultLaptopMemo": // 노트북에 대한 설명이 메모에 기록됨
                MemoManager.Instance.RevealMemo("R1Memo_004");
                break;

            case "ResultLaptopContentScript": // 노트북에 대한 스크립트 출력
                DialogueManager.Instance.StartDialogue("RoomEscape_015");
                break;

            case "ResultLaptopLockFailedScript": // 노트북 비밀번호 틀림에 대한 설명
                SoundPlayer.Instance.UISoundPlay(Sound_Wrong);
                DialogueManager.Instance.StartDialogue("RoomEscape_016");
                break;
            
            case "ResultCarpetOpen": // 카펫이 들쳐짐
                SoundPlayer.Instance.UISoundPlay(Sound_CarpetOpen);
                executableObjects["ClosedCarpet"].ExecuteAction();
                break;
            
            case "ResultCarpetDropOutLetterZoom": // 종이를 확대해주는 UI
                SoundPlayer.Instance.UISoundPlay(Sound_GrabPaper);
                RoomManager.Instance.imageAndLockPanelManager.SetObjectImageGroup(true, "letterOfResignation");
                break;
            
            case "ResultCarpetDropOutLetterScript": // 종이에 대한 스크립트
                DialogueManager.Instance.StartDialogue("RoomEscape_017");
                break;

            case "ResultCarpetDropOutLetterMemo": // 종이에 대한 메모
                MemoManager.Instance.RevealMemo("R1Memo_006");
                break;

            case "ResultCarpetClosed": // 카펫이 원래대로 돌아감
                SoundPlayer.Instance.UISoundPlay(Sound_CarpetClose);
                executableObjects["OpenCarpet"].ExecuteAction();
                DialogueManager.Instance.StartDialogue("RoomEscape_018");
                break;
            
            case "ResultPosterScript": // 포스터에 대한 스크립트
                RoomManager.Instance.imageAndLockPanelManager.SetObjectImageGroup(true, "poster");
                DialogueManager.Instance.StartDialogue("RoomEscape_019");
                break;

            case "ResultCutterKnifeScript": // 커터칼에 대한 스크립트
                RoomManager.Instance.imageAndLockPanelManager.SetObjectImageGroup(true, "knife");
                DialogueManager.Instance.StartDialogue("RoomEscape_020");
                break;

            case "ResultCutterKnifeDisappear": // 커터칼 사라짐
                //Debug.Log("ResultCutterKnifeDisappear executed");
                executableObjects["Knife0"].ExecuteAction();
                executableObjects["Knife1"].ExecuteAction();
                break;

            case "ResultPosterWithKnifeScript": // 포스터 뒷장에 대한 설명
                SoundPlayer.Instance.UISoundPlay(Sound_Poster);
                RoomManager.Instance.imageAndLockPanelManager.SetObjectImageGroup(true, "poster");
                DialogueManager.Instance.StartDialogue("RoomEscape_021");
                break;

            case "ResultPosterMemo": // 포스터에 대한 메모
                MemoManager.Instance.RevealMemo("R1Memo_005");
                break;

            case "ResultClosetDoorsOpen": // 옷장 문이 열림
                SoundPlayer.Instance.UISoundPlay(Sound_ClosetOpen);
                executableObjects["ClosedClosetDoors"].ExecuteAction();
                break;
            
            case "ResultClosetDoorsClosed": // 옷장 문이 닫힘
                SoundPlayer.Instance.UISoundPlay(Sound_ClosetClose);
                executableObjects["OpenClosetDoors"].ExecuteAction();
                break;
                
            case "ResultCloseFirstClickScript": // 옷장 처음 상호작용했을 때 스크립트
                DialogueManager.Instance.StartDialogue("RoomEscape_022");
                break;

            case "ResultClosetBagScript": // 옷장 속 가방 클릭 스크립트
                RoomManager.Instance.imageAndLockPanelManager.SetObjectImageGroup(true, "cafePintInBagImage");
                DialogueManager.Instance.StartDialogue("RoomEscape_023");
                break;

            case "ResultClosetBagMemo": // 옷장 속 가방 내에 전단지에 대한 메모
                MemoManager.Instance.RevealMemo("R1Memo_006");
                break;

            case "ResultClosetBoxWithoutKeyScript": // 옷장 위 상자에 대한 설명(열쇠X 상태 일 때)
                SoundPlayer.Instance.UISoundPlay(Sound_LockerUnlock);
                DialogueManager.Instance.StartDialogue("RoomEscape_024");
                break;

            case "ResultClosetBoxScript": // 상자가 열리는 스크립트
                DialogueManager.Instance.StartDialogue("RoomEscape_025");
                break;

            case "ResultClosetBoxSound": // 상자 열리는 사운드
                SoundPlayer.Instance.UISoundPlay(Sound_LockerKeyMovement);
                break;

            case "ResultClosetBoxPicturesUI": // 상자 안 사진들이 UI로 보임
                RoomManager.Instance.imageAndLockPanelManager.SetObjectImageGroup(true, "photoInsideBox");
                GameManager.Instance.SetVariable("BoxOpened", true);
                break;

            case "ResultClosetBoxMemo": // 상자 안 사진들에 대한 메모
                MemoManager.Instance.RevealMemo("R1Memo_011");
                break;

            case "ResultShoppingBagScript": // 쇼핑백에 대한 스크립트
                DialogueManager.Instance.StartDialogue("RoomEscape_031");
                break;

            case "ResultNomalPoasterScript": // 평범 포스터에 대한 스크립트
                DialogueManager.Instance.StartDialogue("RoomEscape_032");
                break;

            case "ResultStorageTeddyBeaScript": // 서랍장 속 인형에 대한 스크립트
                DialogueManager.Instance.StartDialogue("RoomEscape_033");
                break;

            case "ResultCabinetDoorsOpenScript": // 서랍장이 열리는 스크립트
                DialogueManager.Instance.StartDialogue("RoomEscape_026");
                break;

            case "ResultCabinetDoorsOpen": // 서랍장이 열림
                SoundPlayer.Instance.UISoundPlay(Sound_StorageOpen);
                executableObjects["ClosedCabinetDoors"].ExecuteAction();
                break;

            case "ResultCabinetDoorsClosed": // 서랍장이 닫힘
                SoundPlayer.Instance.UISoundPlay(Sound_StorageClose);
                executableObjects["OpenCabinetDoors"].ExecuteAction();
                break;

            case "ResultCalendarOpen": // 달력이 열림
                RoomManager.Instance.imageAndLockPanelManager.SetLockObject(true, "calendar");
                executableObjects["Calendar"].ExecuteAction();
                break;
            
            case "Result_IncrementCalendarCluesFound": // 달력 관련 단서 찾은 수 1 증가
                GameManager.Instance.IncrementVariable("CalendarCluesFound");
                break;

            case "ResultCalendarFBDScirpt": // 필연 생일에 대한 스크립트
                DialogueManager.Instance.StartDialogue("RoomEscape_027");
                break;

            case "ResultCalendarABDScript": // 우연 생일에 대한 스크립트
                DialogueManager.Instance.StartDialogue("RoomEscape_028");
                break;

            case "ResultCalendar1031Script": // 10월 31일에 대한 스크립트
                DialogueManager.Instance.StartDialogue("RoomEscape_029");
                break;

            case "ResultCalendar1001Script": // 10월 1일에 대한 스크립트
                DialogueManager.Instance.StartDialogue("RoomEscape_030");
                break;

            case "ResultCalendarFBDMemo": // 필연 생일 메모
                MemoManager.Instance.RevealMemo("R1Memo_007");
                break;

            case "ResultCalendarABDMemo": // 우연 생일 메모
                MemoManager.Instance.RevealMemo("R1Memo_010");
                break;

            case "ResultCalendar1031Memo": // 10월 31일 메모
                MemoManager.Instance.RevealMemo("R1Memo_008");
                break;

            case "ResultCalendar1001Memo": // 10월 1일 메모
                MemoManager.Instance.RevealMemo("R1Memo_009");
                break;
            
            case "Result_IncrementFateBirthday":  // 필연 클릭 횟수 1 증가
                GameManager.Instance.IncrementVariable("FateBirthdayClick");
                break;
            
            case "Result_IncrementAccidyBirthday":  // 우연 클릭 횟수 1 증가
                GameManager.Instance.IncrementVariable("AccidyBirthdayClick");
                break;
            
            case "Result_IncrementSpecialDateA":  // 1031 클릭 횟수 1 증가 
                GameManager.Instance.IncrementVariable("SpecialDateAClick");
                break;

            case "Result_IncrementSpecialDateB":  // 1001 클릭 횟수 1 증가 
                GameManager.Instance.IncrementVariable("SpecialDateBClick");
                break;

            case "ResultClueCompleted": // 달력 조사 다 완료함

                break;


            case "ResultClosetZoom": // 옷장 확대 화면으로 전환
                //executableObjectsKeyCheck("Closet Unzoomed-closed 2");
                //executableObjectsKeyCheck("Closet Unzoomed-open 2");
                executableObjects["Closet Unzoomed-closed 2"].ExecuteAction();
                executableObjects["Closet Unzoomed-open 2"].ExecuteAction();
                executableObjects["Closet Unzoomed-open back image 2"].ExecuteAction();
                break;
            
            case "ResultCabinetZoom": // 서랍장 확대 화면으로 전환
                executableObjects["Cabinet Unzoomed-closed 2"].ExecuteAction();
                executableObjects["Cabinet Unzoomed-open 2"].ExecuteAction();
                executableObjects["Cabinet Unzoomed-closed 3"].ExecuteAction();
                executableObjects["Cabinet Unzoomed-open 3"].ExecuteAction();
                break;
            
            case "Result_showZoomedDesk": // 책상 화면으로 전환
                executableObjects["Desk Unzoomed 1"].ExecuteAction();
                executableObjects["Desk Unzoomed 2"].ExecuteAction();
                break;
            
            case "Result_showZoomedPillow": // 배게와 침대 위 인형 확대 화면으로 전환
                executableObjects["Pillow Unzoomed 1"].ExecuteAction();
                executableObjects["Pillow Unzoomed 3"].ExecuteAction();
                break;
            
            case "Result_showZoomedDeskShelf": // 책상 위 선반 확대 화면으로 전환
                executableObjects["DeskShelf Unzoomed 1"].ExecuteAction();
                executableObjects["DeskShelf Unzoomed 2"].ExecuteAction();
                break;

            case "ResultBooksScript": // 다이어리 옆 책 스크립트
                DialogueManager.Instance.StartDialogue("RoomEscape_036");
                break;

            case "ResultShelfBearScript": // 다이어리 선반위 곰인형 스크립트
                DialogueManager.Instance.StartDialogue("RoomEscape_037");
                break;

            case "ResultClosetUnderScript": // 옷장 밑 부분 스크립트
                DialogueManager.Instance.StartDialogue("RoomEscape_038");
                break;

            case "ResultDoorBearScript": // 문 앞 곰인형 장식 스크립트
                DialogueManager.Instance.StartDialogue("RoomEscape_039");
                break;

            case "ResultDoorScript": // 문 스크립트
                DialogueManager.Instance.StartDialogue("RoomEscape_040");
                break;


            case "ResultEndRoom1": // Room1 끝
                SceneManager.Instance.LoadScene(SceneType.FOLLOW_1);
                break;

            // 방탈출1 귀가 스크립트
            case "Result_Room1HomeComing2":
                // 랜덤
                var randomHomeComing = new Random((uint)System.DateTime.Now.Ticks);  // choose a random dialogue ID
                string[] random1HomeComing2DialogueIDs = { "RoomEscapeS_001", "RoomEscapeS_003" };
                DialogueManager.Instance.StartDialogue(random1HomeComing2DialogueIDs[randomHomeComing.NextInt(0, 2)]);
                break;

            case "Result_Room1HomeComing3":
                DialogueManager.Instance.StartDialogue("RoomEscapeS_005");
                break;

            case "Result_Room1HomeComing4":
                DialogueManager.Instance.StartDialogue("RoomEscapeS_008");
                break;

            case "Result_Room1HomeComing5":
                // 랜덤
                randomHomeComing = new Random((uint)System.DateTime.Now.Ticks);  // choose a random dialogue ID
                string[] random1HomeComing5DialogueIDs = { "RoomEscapeS_012", "RoomEscapeS_013" };
                DialogueManager.Instance.StartDialogue(random1HomeComing5DialogueIDs[randomHomeComing.NextInt(0, 2)]);
                break;

            // 방탈출1 아침 스크립트
            case "Result_Room1Morning2":
                StartCoroutine(DialogueManager.Instance.StartDialogue("RoomEscapeS_004", totalTime));
                break;

            case "Result_Room1Morning3":
            case "Result_Room1Morning4":
                StartCoroutine(DialogueManager.Instance.StartDialogue("RoomEscapeS_015", totalTime));
                break;

            case "Result_Room1Morning5":
                StartCoroutine(DialogueManager.Instance.StartDialogue("RoomEscapeS_014", totalTime));
                break;


            // 방탈출 2

            case "Result_showTinCaseImage": // 틴케이스 확대 이미지 표시
                RoomManager.Instance.imageAndLockPanelManager.SetObjectImageGroup(true, "tinCase");
                break;

            case "ResultTinCaseScript": // 틴케이스에 대한 설명
                DialogueManager.Instance.StartDialogue("RoomEscape2_020");
                break;

            case "ResultTinCaseSystemActivartion": // 틴케이스 비밀번호 시스템 활성화
                RoomManager.Instance.imageAndLockPanelManager.SetLockObject(true, "tinCase");
                executableObjects["TinCase"].ExecuteAction();
                break;

            case "ResultTinCaseFailedScript": // 틴케이스 비밀번호 틀림에 대한 설명
                //DialogueManager.Instance.StartDialogue("RoomEscape2_020");
                break;

            case "ResultTinCaseGetTicket": // 티켓을 획득
                SoundPlayer.Instance.UISoundPlay(Sound_TincaseOpen); // 틴케이스 열리는 소리
                RoomManager.Instance.imageAndLockPanelManager.SetObjectImageGroup(true, "ticket");
                break;

            case "ResultTinCaseGetTicketScript": // 티켓을 획득 후 스크립트
                DialogueManager.Instance.StartDialogue("RoomEscape2_021");
                break;

            case "ResultTinCaseGetTicketMemo": // 티켓 획득 후 메모
                MemoManager.Instance.RevealMemo("R2Memo_007");
                break;


            case "Result_showSewingBoxImage":   // 반짇고리 상자 확대 이미지 표시
                RoomManager.Instance.imageAndLockPanelManager.SetObjectImageGroup(true, "sewingBox");
                break;

            case "ResultSewingBoxScript":   // 반짇고리에 대한 설명
                DialogueManager.Instance.StartDialogue("RoomEscape2_018");
                break;

            case "ResultSewingBoxSystemActivartion": // 반짇고리 퍼즐 시스템 활성화
                RoomManager.Instance.imageAndLockPanelManager.SetLockObject(true, "sewingBox");
                executableObjects["SewingBox"].ExecuteAction();
                break;

            case "ResultSewingBoxPuzzleFailedScript":   // 반짇고리 퍼즐 틀림에 대한 설명
                //DialogueManager.Instance.StartDialogue("");

                Debug.Log("반짇고리 퍼즐 틀림에 대한 설명");
                break;

            case "ResultSewingBoxGetThreadAndNeedle":   // 실과 바늘을 획득
                SoundPlayer.Instance.UISoundPlay(Sound_SewingBoxOpen); // 반짇고리 상자 열리는 소리
                RoomManager.Instance.imageAndLockPanelManager.SetObjectImageGroup(true, "thread");  
                break;

            case "ResultSewingBoxGetThreadAndNeedleScript":   // 실과 바늘을 획득 후 스크립트
                DialogueManager.Instance.StartDialogue("RoomEscape2_019");
                break;

            case "ResultSewingBoxGetThreadAndNeedleMemo":   // 실과 바늘 획득 후 메모
                MemoManager.Instance.RevealMemo("R2Memo_006");
                break;

            case "ResultBrokenTeddyBear2NoThreadAndNeedleScript": // 실바늘 없을때 곰인형에 대한 설명
                DialogueManager.Instance.StartDialogue("RoomEscape2_013");
                break;

            case "ResultBrokenTeddyBear2YesThreadAndNeedleScript": // 실바늘 있을때 곰인형에 대한 설명
                DialogueManager.Instance.StartDialogue("RoomEscape2_023");
                break;

            case "ResultFixTeddyBear": // 망가진 곰인형을 고침
                //Debug.Log("ResultFixTeddyBear executed");
                executableObjects["BrokenTeddyBear0"].ExecuteAction();
                executableObjects["BrokenTeddyBear1"].ExecuteAction();
                executableObjects["BrokenTeddyBear3"].ExecuteAction();
                GameManager.Instance.SetVariable("TeddyBearFixed", true);
                break;

            case "ResultBrokenTeddyBear2Yes": // 선택지 약을 먹음
                GameManager.Instance.SetVariable("RefillHeartsOrEndDay", false);
                DialogueManager.Instance.EndDialogue();
                DialogueManager.Instance.StartDialogue("RoomEscape2_024");
                break;

            case "ResultEatEnergySupplement":
                RoomManager.Instance.Room2ActionPointManager.EatEnergySupplement();
                break;

            case "ResultBrokenTeddyBear2No": // 선택지 약을 안 먹음
                RoomManager.Instance.Room2ActionPointManager.SetChoosingBrokenBearChoice(true);
                DialogueManager.Instance.EndDialogue();
                DialogueManager.Instance.StartDialogue("RoomEscape2_025");
                RoomManager.Instance.Room2ActionPointManager.SetChoosingBrokenBearChoice(false);
                break;

            case "ResultDiary2Script": // 다이어리에 대한 설명
                DialogueManager.Instance.StartDialogue("RoomEscape2_014");
                break;

            case "ResultDiary2LockActivation": // 다이어리 잠금 장치 활성화됨
                RoomManager.Instance.imageAndLockPanelManager.SetLockObject(true, "diary2");
                executableObjects["Diary2"].ExecuteAction();
                break;

            case "ResultDiary2LockOpen": // 다이어리가 열림
                executableObjects["Diary2Lock"].ExecuteAction();
                break;

            case "ResultDiary2Memo": // 다이어리에 대한 메모가 작성됨
                MemoManager.Instance.RevealMemo("R2Memo_003");
                break;

            case "ResultDiary2ContentScript": // 다이어리에 대한 스크립트
                DialogueManager.Instance.StartDialogue("RoomEscape2_016");
                break;

            case "ResultDiary2LockFailedScript": // 다이어리 비밀번호를 틀렸다는 스크립트
                SoundPlayer.Instance.UISoundPlay(Sound_DiaryUnlock);
                DialogueManager.Instance.StartDialogue("RoomEscape_011");
                break;

            // 방탈출2의 확대 화면 전환 result 
            case "Result_showZoomedBox": // 옷장 위 상자 확대 화면으로 전환
                executableObjects["Box Unzoomed 2"].ExecuteAction();
                break;

            case "Result_showZoomedClosetUnder": // 옷장 아래 영역 확대 화면으로 전환
                executableObjects["ClosetUnder Unzoomed 2"].ExecuteAction();
                executableObjects["ClosetUnderDown Unzoomed-closed 2"].ExecuteAction();
                executableObjects["ClosetUnderDown Unzoomed-open 2"].ExecuteAction();
                executableObjects["ClosetUnderUp Unzoomed-closed 2"].ExecuteAction();
                executableObjects["ClosetUnderUp Unzoomed-open 2"].ExecuteAction();
                break;

            case "Result_showZoomedMedicine2": // 약 확대 화면으로 전환
                executableObjects["Medicine2 Unzoomed 1"].ExecuteAction();
                executableObjects["Medicine2 Unzoomed 3"].ExecuteAction();
                break;

            case "Result_showZoomedShoppingBags": // 쇼핑백 확대 화면으로 전환
                executableObjects["ShoppingBags Unzoomed 3"].ExecuteAction();
                break;

            case "ResultBedTeddy2Script": // 침대 위 곰인형에 대한 설명
                DialogueManager.Instance.StartDialogue("RoomEscape2_001");
                break;

            case "ResultStandLight2Script": // 스탠드에 대한 설명
                DialogueManager.Instance.StartDialogue("RoomEscape2_002");
                break;

            case "ResultUpDeskBook2Script": // 책상 위 책 무더기에 대한 설명
                DialogueManager.Instance.StartDialogue("RoomEscape2_003");
                break;

            case "ResultUnderDeskBook2Script": // 책상 아래 책에 대한 설명
                DialogueManager.Instance.StartDialogue("RoomEscape2_004");
                break;

            case "ResultShelfBook2Script": // 책장 위 책에 대한 설명
                DialogueManager.Instance.StartDialogue("RoomEscape2_005");
                break;

            case "ResultShelfTeddyBear2Script": // 책장 위 곰인형에 대한 설명
                DialogueManager.Instance.StartDialogue("RoomEscape2_006");
                break;

            case "ResultClock2Script": // 책장 위 시계에 대한 설명
                DialogueManager.Instance.StartDialogue("RoomEscape2_007");
                break;

            case "ResultNormalPoster2Script": // 평범한 포스터에 대한 설명
                DialogueManager.Instance.StartDialogue("RoomEscape2_008");
                break;

            case "ResultClosetBox2Script": // 옷장 위 상자에 대한 설명
                DialogueManager.Instance.StartDialogue("RoomEscape2_009");
                break;

            case "ResultUnderPhoto2Script": // 옷장 아래 사진에 대한 설명
                DialogueManager.Instance.StartDialogue("RoomEscape2_010");
                break;

            case "ResultMedicine2Script": // 선반 위 약 스크립트
                DialogueManager.Instance.StartDialogue("RoomEscape2_026");
                break;

            case "ResultMedicine2Memo": // 선반 위 약 메모
                MemoManager.Instance.RevealMemo("R2Memo_010");
                break;

            case "ResultMedicine2Zoom": // 선반 위 약 확대UI
                RoomManager.Instance.imageAndLockPanelManager.SetObjectImageGroup(true, "medicine2");
                break;

            case "ResultHospitalFlyer2Script": // 방탈출2 병원 전단지  스크립트
                SoundPlayer.Instance.UISoundPlay(Sound_GrabPaper);
                DialogueManager.Instance.StartDialogue("RoomEscape2_011");
                break;

            case "ResultHospitalFlyer2Memo": // 방탈출2 병원 전단지 메모
                MemoManager.Instance.RevealMemo("R2Memo_001");
                break;

            case "ResultHospitalFlyer2Zoom": // 방탈출2 병원 전단지 확대 UI
                RoomManager.Instance.imageAndLockPanelManager.SetObjectImageGroup(true, "hospitalPrint");
                break;

            case "ResultPoster2Script": // 방탈출2 바다 포스터 스크립트
                DialogueManager.Instance.StartDialogue("RoomEscape2_012");
                break;

            case "ResultPoster2Memo": // 방탈출2 바다 포스터 메모
                MemoManager.Instance.RevealMemo("R2Memo_002");
                break;

            case "ResultPoster2Zoom": // 방탈출2 바다 포스터 확대 UI
                RoomManager.Instance.imageAndLockPanelManager.SetObjectImageGroup(true, "poster2");
                break;

            case "ResultStarSticker2Script": // 방탈출2 별 스티커 스크립트
                DialogueManager.Instance.StartDialogue("RoomEscape2_017");
                break;

            case "ResultStarSticker2Memo": // 방탈출2 별 스티커 메모
                MemoManager.Instance.RevealMemo("R2Memo_005");
                break;

            case "ResultStarSticker2Zoom": // 방탈출2 별 스티커 확대 UI
                RoomManager.Instance.imageAndLockPanelManager.SetObjectImageGroup(true, "starSticker");
                break;

            case "ResultShoppingBag2Script": // 방탈출2 쇼핑백 스크립트
                DialogueManager.Instance.StartDialogue("RoomEscape2_022");
                break;

            case "ResultShoppingBag2Memo": // 방탈출2 쇼핑백 메모
                MemoManager.Instance.RevealMemo("R2Memo_008");
                break;

            case "ResultShoppingBag2Zoom": // 방탈출2 쇼핑백 확대 UI
                RoomManager.Instance.imageAndLockPanelManager.SetObjectImageGroup(true, "shoppingBag2");
                break;

            case "ResultClosetBag2Script": // 옷장 안 가방 스크립트
                DialogueManager.Instance.StartDialogue("RoomEscape2_029");
                break;

            case "ResultClosetNamecard2Script": // 옷장 안 명함 스크립트
                DialogueManager.Instance.StartDialogue("RoomEscape2_030");
                break;

            case "ResultClosetClothAndBag2Script": // 옷장 안 정리 안 된 옷 스크립트
                DialogueManager.Instance.StartDialogue("RoomEscape2_031");
                break;

            case "ResultUnderClosetClothes2Script": // 옷장 아래 정리 안 된 옷 스크립트
                DialogueManager.Instance.StartDialogue("RoomEscape2_033");
                break;


            case "ResultWardrobeUpDrawersOpenScript": // 옷장 서랍장 위칸이 열리는 스크립트
                //DialogueManager.Instance.StartDialogue("RoomEscape_026");
                break;

            case "ResultWardrobeUpDrawersOpen": // 옷장 서랍장 위칸이 열림
                SoundPlayer.Instance.UISoundPlay(Sound_StorageOpen);
                executableObjects["ClosedWardrobeUpDrawers"].ExecuteAction();
                break;

            case "ResultWardrobeUpDrawersClosed": // 옷장 서랍장 위칸이 닫힘
                SoundPlayer.Instance.UISoundPlay(Sound_StorageClose);
                executableObjects["OpenWardrobeUpDrawers"].ExecuteAction();
                break;

            case "ResultWardrobeDownDrawersOpenScript": // 옷장 서랍장 아래칸이 열리는 스크립트
                //DialogueManager.Instance.StartDialogue("RoomEscape_026");
                break;

            case "ResultWardrobeDownDrawersOpen": // 옷장 서랍장 아래칸이 열림
                SoundPlayer.Instance.UISoundPlay(Sound_StorageOpen);
                executableObjects["ClosedWardrobeDownDrawers"].ExecuteAction();
                break;

            case "ResultWardrobeDownDrawersClosed": // 옷장 서랍장 아래칸이 닫힘
                SoundPlayer.Instance.UISoundPlay(Sound_StorageClose);
                executableObjects["OpenWardrobeDownDrawers"].ExecuteAction();
                break;

            case "Result_showBook2CoverImage": // 떨어진 책 한권 표지 확대 UI
                RoomManager.Instance.imageAndLockPanelManager.SetObjectImageGroup(true, "book2Cover");
                Debug.Log("떨어진 책 한권 표지 확대 UI");
                break;

            case "ResultBook2Script": // 떨어진 책 한권 스크립트
                DialogueManager.Instance.StartDialogue("RoomEscape2_015");
                //Debug.Log(" 떨어진 책 한권 스크립트");
                break;

            case "ResultBook2Memo": // 책에 대한 메모
                MemoManager.Instance.RevealMemo("R2Memo_004");
                break;

            case "ResultBook2SystemActivartion": // 떨어진 책 읽기
                RoomManager.Instance.imageAndLockPanelManager.SetLockObject(true, "book2");
                executableObjects["Book2"].ExecuteAction();
                //Debug.Log("  떨어진 책 읽기");
                break;

            case "ResultClosetKey2Script": // 옷장 안 열쇠에 대한 스크립트
                SoundPlayer.Instance.UISoundPlay(Sound_Key);
                DialogueManager.Instance.StartDialogue("RoomEscape2_028");
                break;

            case "ResultClosetKey2Zoom": // 옷장 안 열쇠 줌
                RoomManager.Instance.imageAndLockPanelManager.SetObjectImageGroup(true, "closetKey2");
                break;

            case "ResultClosetKey2Disappear": // 열쇠 사라짐
                //Debug.Log("ResultClosetKey2Disappear executed");
                executableObjects["ClosetKey2_0"].ExecuteAction();
                executableObjects["ClosetKey2_2"].ExecuteAction();
                // ClosetKey2_2 는 Side2의 옷장 문 열어 뒀을 때 보이는 옷장 속 클릭 안 되는 열쇠
                break;

            case "ResultDreamDiary2Script": // 옷장 꿈일기 발견 스크립트
                DialogueManager.Instance.StartDialogue("RoomEscape2_034");
                break;

            case "ResultDreamDiary2WithKeyScript": // 열쇠 발견 후 꿈일기 발견 스크립트
                SoundPlayer.Instance.UISoundPlay(Sound_LockerUnlock);
                DialogueManager.Instance.StartDialogue("RoomEscape2_035");
                break;

            case "ResultDreamDiary2Memo": // 꿈일기에 대한 메모
                MemoManager.Instance.RevealMemo("R2Memo_009");
                break;

            case "ResultDreamDiary2SystemActivartion": // 꿈일기 읽기
                RoomManager.Instance.imageAndLockPanelManager.SetLockObject(true, "dreamDiary");
                executableObjects["DreamDiary"].ExecuteAction();
                break;


            // 방탈출2 귀가 스크립트
            case "Result_Room2HomeComing2":
                DialogueManager.Instance.StartDialogue("RoomEscape2S_004");
                break;

            case "Result_Room2HomeComing3":
                DialogueManager.Instance.StartDialogue("RoomEscape2S_006");
                break;

            case "Result_Room2HomeComing4":
                DialogueManager.Instance.StartDialogue("RoomEscape2S_009");
                break;

            case "Result_Room2HomeComing5":
                DialogueManager.Instance.StartDialogue("RoomEscape2S_012");
                break;

            // 방탈출2 아침 스크립트
            case "Result_Room2Morning2":
                StartCoroutine(DialogueManager.Instance.StartDialogue("RoomEscape2S_005", totalTime));
                break;

            case "Result_Room2Morning3":
                StartCoroutine(DialogueManager.Instance.StartDialogue("RoomEscape2S_008", totalTime));
                break;

            case "Result_Room2Morning4":
                StartCoroutine(DialogueManager.Instance.StartDialogue("RoomEscape2S_011", totalTime));
                break;

            case "Result_Room2Morning5":
                StartCoroutine(DialogueManager.Instance.StartDialogue("RoomEscape2S_014", totalTime));
                break;


            // 미행 1
            case "ResultFollow1Tutorial":
                DialogueManager.Instance.StartDialogue("FollowTutorial_002");
                break;

            case "ResultFollow2Tutorial":
                DialogueManager.Instance.StartDialogue("Follow2S_01");
                break;

            case "ResultFollowTutorialNextStep_0":
                FollowManager.Instance.TutorialNextStep();
                break;

            case "ResultFollowTutorialNextStep_1":
                FollowManager.Instance.TutorialNextStep();
                DialogueManager.Instance.StartDialogue("FollowTutorial_004");
                break;

            case "ResultFollowTutorialNextStep_2":
                FollowManager.Instance.TutorialNextStep();
                DialogueManager.Instance.StartDialogue("FollowTutorial_005");
                break;

            case "ResultFollowTutorialNextStep_3":
                FollowManager.Instance.TutorialNextStep();
                break;

            case "ResultFollowTutorialNextStep_4":
                FollowManager.Instance.TutorialNextStep();
                DialogueManager.Instance.StartDialogue("FollowTutorial_006");
                break;

            case "ResultFollowTutorialNextStep_5":
                FollowManager.Instance.TutorialNextStep();
                break;

            case "ResultFollowTutorialNextStep_6":
                FollowManager.Instance.TutorialNextStep();
                DialogueManager.Instance.StartDialogue("FollowTutorial_007");
                break;

            case "ResultFollowTutorialNextStep_7":
                FollowManager.Instance.TutorialNextStep();
                break;

            case "ResultVillaScript": // 빌라에 대한 스크립트
                DialogueManager.Instance.StartDialogue("Follow_002");
                break;

            case "ResultBreadScript": // 빵집에 대한 스크립트
                DialogueManager.Instance.StartDialogue("Follow_003");
                break;

            case "ResultConScript": // 편의점에 대한 스크립트
                DialogueManager.Instance.StartDialogue("Follow_004");
                break;

            case "ResultBarScript": // 바에 대한 스크립트
                DialogueManager.Instance.StartDialogue("Follow_005");
                break;

            case "ResultIzakawaScript": // 이자카야에 대한 스크립트
                DialogueManager.Instance.StartDialogue("Follow_006");
                break;

            case "ResultCatScript": // 고양이에 대한 스크립트
                DialogueManager.Instance.StartDialogue("Follow_007");
                break;

            case "ResultCafeScript": // 카페에 대한 스크립트
                DialogueManager.Instance.StartDialogue("Follow_008");
                break;

            case "ResultCafeMemo1": // 카페에 대한 메모 1
                MemoManager.Instance.RevealMemo("F1Memo_001");
                break;

            case "ResultCafeMemo2": // 카페에 대한 메모 2
                MemoManager.Instance.RevealMemo("F1Memo_002");
                break;

            case "ResultReceiptScript": // 영수증에 대한 스크립트
                DialogueManager.Instance.StartDialogue("Follow_009");
                break;

            case "ResultReceiptMemo": // 영수증에 대한 메모
                MemoManager.Instance.RevealMemo("F1Memo_003");
                break;

            case "ResultLightScript": // 신호등에 대한 스크립트
                DialogueManager.Instance.StartDialogue("Follow_010");
                break;

            case "ResultLightMemo": // 신호등에 대한 메모
                MemoManager.Instance.RevealMemo("F1Memo_004");
                break;

            case "ResultLatteScript": // 음료에 대한 스크립트
                DialogueManager.Instance.StartDialogue("Follow_011");
                break;

            case "ResultLatteMemo": // 음료에 대한 메모
                MemoManager.Instance.RevealMemo("F1Memo_005");
                break;

            case "Result1FClothesScript": // 1층 옷가게에 대한 스크립트
                DialogueManager.Instance.StartDialogue("Follow_012");
                break;

            case "Result1FClothesMemo": // 1층 옷가게에 대한 메모
                MemoManager.Instance.RevealMemo("F1Memo_006");
                break;

            case "Result2FClothesScript": // 2층 옷가게에 대한 스크립트
                DialogueManager.Instance.StartDialogue("Follow_013");
                break;

            case "Result2FClothesMemo": // 2층 옷가게에 대한 메모
                MemoManager.Instance.RevealMemo("F1Memo_007");
                break;

            case "ResultConstructionScript": // 공사장에 대한 스크립트
                DialogueManager.Instance.StartDialogue("Follow_014");
                break;

            case "ResultConstructionMemo": // 공사장에 대한 메모
                MemoManager.Instance.RevealMemo("F1Memo_008");
                break;

            case "ResultRestaurantScript": // 식당에 대한 스크립트
                DialogueManager.Instance.StartDialogue("Follow_015");
                break;

            case "ResultCocktailBarScript": // 칵테일바에 대한 스크립트
                DialogueManager.Instance.StartDialogue("Follow_016");
                break;

            case "ResultCosemeticScript": // 화장품 가게에 대한 스크립트
                DialogueManager.Instance.StartDialogue("Follow_017");
                break;

            case "ResultMusicBarScript": // 뮤직바에 대한 스크립트
                DialogueManager.Instance.StartDialogue("Follow_018");
                break;

            case "ResultClubScript": // 클럽에 대한 스크립트
                DialogueManager.Instance.StartDialogue("Follow_019");
                break;

            case "ResultPubScript": // 술집에 대한 스크립트
                DialogueManager.Instance.StartDialogue("Follow_020");
                break;

            case "ResultAngryScript": // 화난 사람의 대화 스크립트
                DialogueManager.Instance.StartDialogue("Follow_021");
                break;

            case "ResultAngryFateScript": // 화난 사람을 본 필연의 스크립트
                DialogueManager.Instance.StartDialogue("Follow_022");
                break;

            case "ResultAngryMemo": // 화난 사람에 대한 메모
                MemoManager.Instance.RevealMemo("F1Memo_009");
                break;

            case "ResultWreathScript": // 쓰러진 화환에 스크립트
                DialogueManager.Instance.StartDialogue("Follow_023");
                break;

            case "ResultWreathMemo": // 쓰러진 화환에 대한 메모
                MemoManager.Instance.RevealMemo("F1Memo_010");
                break;

            case "ResultFollowEndNextStep":
                // FollowManager.Instance.FollowTutorialNextStep();
                break;

            case "ResultFollowEnd": // 미행 끝 (메모 개수 충분)
                SceneManager.Instance.LoadScene(SceneType.ROOM_2);
                break;

            case "ResultVillaScript2": // 미행2 빌라에 대한 스크립트
                DialogueManager.Instance.StartDialogue("Follow2_001");
                break;

            case "ResultBarScript2": // 미행2 편의점 건물 3층 칵테일바 스크립트
                DialogueManager.Instance.StartDialogue("Follow2_002");
                break;

            case "ResultBreadScript2": // 미행2 빵집 스크립트
                DialogueManager.Instance.StartDialogue("Follow2_003");
                break;

            case "ResultBeerAScript2": // 미행2 카페 3층 맥주전문점 스크립트
                DialogueManager.Instance.StartDialogue("Follow2_004");
                break;

            case "ResultIzakayaScript2": // 미행2 이자카야 스크립트
                DialogueManager.Instance.StartDialogue("Follow2_005");
                break;

            case "ResultCafeScript2": // 미행2 카페 스크립트
                DialogueManager.Instance.StartDialogue("Follow2_006");
                break;

            case "Result3FClothesScript2": // 미행2 3층 옷가게 스크립트
                DialogueManager.Instance.StartDialogue("Follow2_007");
                break;

            case "Result2FClothesScript2": // 미행2 2층 옷가게 스크립트
                DialogueManager.Instance.StartDialogue("Follow2_008");
                break;

            case "ResultDrinkScript2": // 미행2 술집 스크립트
                DialogueManager.Instance.StartDialogue("Follow2_009");
                break;

            case "ResultCocktailBarScript2": // 미행2 칵테일바 스크립트
                DialogueManager.Instance.StartDialogue("Follow2_010");
                break;

            case "ResultRestaurantScript2": // 미행2 식당 스크립트
                DialogueManager.Instance.StartDialogue("Follow2_011");
                break;

            case "ResultMusicBarScript2": // 미행2 뮤직바 스크립트
                DialogueManager.Instance.StartDialogue("Follow2_012");
                break;

            case "ResultCosemeticScript2": // 미행2 화장품 가게 스크립트
                DialogueManager.Instance.StartDialogue("Follow2_013");
                break;

            case "ResultBeerBScript2": // 미행2 클럽 건물 맥주가게 스크립트
                DialogueManager.Instance.StartDialogue("Follow2_014");
                break;

            case "ResultClubScript2": // 미행2 클럽 스크립트
                DialogueManager.Instance.StartDialogue("Follow2_015");
                break;

            case "ResultLightScript2": // 미행2 신호등 스크립트
                DialogueManager.Instance.StartDialogue("Follow2_016");
                break;

            case "ResultLightMemo2": // 미행2 신호등 메모 -> 아직 스크립트 없음
                MemoManager.Instance.RevealMemo("F1Memo_008");
                break;

            case "ResultSolicitationScript2": // 미행2 호객행위 스크립트 출력
                DialogueManager.Instance.StartDialogue("Follow2_018");
                break;

            case "ResultSolicitationMemo2": // 미행2 호객행위 메모 -> 아직 스크립트 없음
                MemoManager.Instance.RevealMemo("F1Memo_008");
                break;

            case "ResultConScript2": // 미행2 편의점 스크립트 출력
                DialogueManager.Instance.StartDialogue("Follow2_019");
                break;

            case "ResultConMemo2": // 미행2 편의점 메모 -> 아직 스크립트 없음
                MemoManager.Instance.RevealMemo("F1Memo_008");
                break;

            case "ResultTeenageScript2": // 미행2 가출 청소년 스크립트 출력
                DialogueManager.Instance.StartDialogue("Follow2_021");
                break;

            case "ResultTeenageMemo2": // 미행2 가출 청소년 메모 -> 아직 스크립트 없음
                MemoManager.Instance.RevealMemo("F1Memo_008");
                break;

            case "Result1FClothesScript2": // 미행2 가출 1층 옷가게 스크립트 출력
                DialogueManager.Instance.StartDialogue("Follow2_022");
                break;

            case "Result1FClothesMemo2": // 미행2 가출 1층 옷가게 메모 -> 아직 스크립트 없음
                MemoManager.Instance.RevealMemo("F1Memo_008");
                break;

            case "ResultPoliceScript2": // 미행2 경찰 스크립트
                DialogueManager.Instance.StartDialogue("Follow2_027");
                break;

            case "ResultPoliceMemo2": // 미행2 경찰 메모 -> 아직 스크립트 없음
                MemoManager.Instance.RevealMemo("F1Memo_008");
                break;

            case "ResultCigaretteScript2": // 미행2 담배피는 사람 스크립트
                DialogueManager.Instance.StartDialogue("Follow2_028");
                break;

            case "ResultCigaretteMemo2": // 미행2 담배피는 사람 메모 -> 아직 스크립트 없음
                MemoManager.Instance.RevealMemo("F1Memo_008");
                break;

            case "ResultGuardScript2": // 미행2 클럽가드 스크립트
                DialogueManager.Instance.StartDialogue("Follow2_029");
                break;

            case "ResultGuardMemo2": // 미행2 클럽가드 메모 -> 아직 스크립트 없음
                MemoManager.Instance.RevealMemo("F1Memo_008");
                break;

            case "ResultUnlockFOLLOW_1":
                SoundPlayer.Instance.ChangeBGM(BGM_ROOM1);
                DialogueManager.Instance.StartDialogue("FollowTutorial_001");
                break;

            case "ResultLockFOLLOW_1":
                SoundPlayer.Instance.ChangeBGM(BGM_ROOM1);
                DialogueManager.Instance.StartDialogue("BadEndingA_ver1_01");
                break;

            case "ResultUnlockROOM_2":
                SoundPlayer.Instance.ChangeBGM(BGM_FOLLOW1);
                EndingManager.Instance.Ending_Follow1_StreetVideo();
                break;

            case "ResultStartDialogue_Follow1Final":
                DialogueManager.Instance.StartDialogue("Follow1Final_003");
                break;

            case "ResultLockROOM_2":
                SoundPlayer.Instance.ChangeBGM(BGM_FOLLOW1);
                EndingManager.Instance.Ending_Follow1_StreetVideo();
                break;

            case "ResultStreetVideo_NotClear":
                DialogueManager.Instance.StartDialogue("BadEndingA_ver2_01");
                break;

            case "ResultStreetVideo_Clear":
                EndingManager.Instance.Ending_Follow1();
                break;

            case "ResultBadEndingB_01B":
                SoundPlayer.Instance.ChangeBGM(BGM_BAD_B);
                DialogueManager.Instance.StartDialogue("BadEndingB_ver1_01B");
                break;

            case "ResultUnlockHidden":
                SoundPlayer.Instance.ChangeBGM(BGM_FOLLOW1);
                DialogueManager.Instance.StartDialogue("HiddenEnding_00");
                break;

            case "ResultUnlockTrue":
                SoundPlayer.Instance.ChangeBGM(BGM_FOLLOW1);
                DialogueManager.Instance.StartDialogue("TrueEnding_01");
                break;

            case "ResultLockTrueAndHidden":
                SoundPlayer.Instance.ChangeBGM(BGM_FOLLOW1);
                DialogueManager.Instance.StartDialogue("BadEndingA_ver2_01");
                break;

            case "ResultChangeBGM_BadA":
                SoundPlayer.Instance.ChangeBGM(BGM_BAD_A);
                break;

            case "ResultChangeBGM_True":
                SoundPlayer.Instance.ChangeBGM(BGM_TRUE);
                break;

            case "ResultChangeBGM_Hidden":
                SoundPlayer.Instance.ChangeBGM(BGM_HIDDEN);
                break;
                
            case "ResultBadA_End":
                EndingManager.Instance.EndEnding(EndingType.BAD_A);
                break;

            case "ResultBadB_End":
                EndingManager.Instance.EndEnding(EndingType.BAD_B);
                break;

            case "ResultTrue_End":
                EndingManager.Instance.EndEnding(EndingType.TRUE);
                break;

            case "ResultHidden_End":
                EndingManager.Instance.EndEnding(EndingType.HIDDEN);
                break;


            default:
                Debug.Log($"Result ID: {resultID} not found!");
                break;
        }
    }
}
