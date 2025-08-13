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
            executableObjects[objectName] = executable;
    }

    public void InitializeExecutableObjects()
    {
        //Debug.Log("############### unregistered all executable objects ###############");

        executableObjects = new Dictionary<string, IResultExecutable>();
    }

    //public void executableObjectsKeyCheck(string objectName)
    //{
    //    if (executableObjects.ContainsKey(objectName))
    //        Debug.Log(objectName + " key exists");
    //    else
    //        Debug.Log(objectName + " key does not exist");
    //}

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
        if (GameManager.Instance.isDebug) 
            Debug.Log(resultID);
        
        string variableName;
        
        TutorialManager tutorialManager = null;
        if (RoomManager.Instance && RoomManager.Instance.tutorialManager)
            tutorialManager = RoomManager.Instance.tutorialManager;
        
        // ------------------------ 이곳에 모든 동작을 수동으로 추가 ------------------------
        switch (resultID)
        {
            case string when resultID.StartsWith("Result_RevealMemo"): // 메모 획득
                variableName = resultID["Result_RevealMemo".Length..];
                MemoManager.Instance.RevealMemo(variableName);
                break;

            case string when resultID.StartsWith("Result_StartDialogue"):  // 대사 시작
                variableName = resultID["Result_StartDialogue".Length..];
                DialogueManager.Instance.StartDialogue(variableName);
                break;
            
            case string when resultID.StartsWith("Result_Increment"):  // 값++
                variableName = resultID["Result_Increment".Length..];
                GameManager.Instance.IncrementVariable(variableName);
                break;

            case string when resultID.StartsWith("Result_Decrement"):  // 값--
                variableName = resultID["Result_Decrement".Length..];
                GameManager.Instance.DecrementVariable(variableName);
                break;

            case string when resultID.StartsWith("Result_Inverse"):  // !값
                variableName = resultID["Result_Inverse".Length..];
                GameManager.Instance.InverseVariable(variableName);
                break;

            case string when resultID.StartsWith("Result_IsFinished"):  // 조사 후 EventObject의 isFinished를 true로
                variableName = resultID["Result_isFinished".Length..];
                GameManager.Instance.SetEventFinished(variableName);
                break;

            case string when resultID.StartsWith("Result_IsUnFinished"):  
                // EventObject의 isFinished를 false로 (포스터 커터칼 있는채로 다시 조사하면 처음 조사 스크립트 나와야 해서 추가됨)
                variableName = resultID["Result_IsUnFinished".Length..];
                GameManager.Instance.SetEventUnFinished(variableName);
                break;

            case "Result_girl":  // 우연의 성별을 여자로 설정
                GameManager.Instance.SetVariable("AccidyGender", 0);
                DialogueManager.Instance.EndDialogue();
                break;
            
            case "Result_boy":  // 우연의 성별을 남자로 설정
                GameManager.Instance.SetVariable("AccidyGender", 1);
                DialogueManager.Instance.EndDialogue();
                break;

            case "ResultCloseEyes": // 눈 깜빡이는 효과
                StartCoroutine(UIManager.Instance.OnFade(null, 0, 1, 1, true, 0.5f, 0));
                break;

            case "Result_FadeOut":  // fade out
                float fadeOutTime = 3f;
                StartCoroutine(UIManager.Instance.OnFade(null, 0, 1, fadeOutTime));
                break;

            case "Result_FadeIn":  // fade out
                float fadeInTime = 3f;
                StartCoroutine(UIManager.Instance.OnFade(null, 1, 0, fadeInTime));
                break;

            case "ResultPrologueLimit":
                StartCoroutine(DialogueManager.Instance.StartDialogue("Prologue_000", 3));
                break;

            case "ResultCommonPrologueA":
                LobbyManager.Instance.backgroundImage.sprite = LobbyManager.Instance?.room1Side1BackgroundSprite;
                StartCoroutine(DialogueManager.Instance.StartDialogue("Prologue_002", 3));
                break;

            case "ResultName": // 이름 입력창
                LobbyManager.Instance?.OpenNamePanel();
                break;

            case "ResultBirth": // 생일 입력창
                LobbyManager.Instance?.OpenBirthPanel();
                break;

            case "ResultPrologueEnd":
                GameSceneManager.Instance.LoadScene(SceneType.ROOM_1);
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
                StartCoroutine(RoomManager.Instance.actionPointManager.nextMorningDay());
                break;

            case "ResultBlanketCheck": // 조사하기 버튼 누르면 침대 조사할 수 있게 함
                DialogueManager.Instance.EndDialogue();
                GameManager.Instance.SetVariable("isInquiry", true);
                GameManager.Instance.SetCurrentInquiryObjectId("EventBlanket");
                EventManager.Instance.CallEvent("Event_Inquiry");
                break;
            
            // 튜토리얼
            case "Result_nextTutorialPhase":  // 튜토리얼 다음 페이즈로 진행
                if (!tutorialManager)
                    break;
                
                tutorialManager.ProceedToNextPhase();
                break;

            case "Result_TutorialPhase1ForceMoveButtons":  // 방을 둘러보자 (이동버튼 강조)
                UIManager.Instance.ToggleHighlightAnimationEffect(eUIGameObjectName.LeftButton, true);
                UIManager.Instance.ToggleHighlightAnimationEffect(eUIGameObjectName.RightButton, true);
                break;

            case "Result_TutorialPhase2ForceCarpet":  // 닫힌 카펫 강조
                if (!tutorialManager)
                    break;

                UIManager.Instance.ToggleHighlightAnimationEffect(
                    tutorialManager.tutorialGameObjects[TutorialManager.eTutorialObjectName.CarpetClosed], true);
                UIManager.Instance.ToggleHighlightAnimationEffect(
                    tutorialManager.tutorialGameObjects[TutorialManager.eTutorialObjectName.Chair], false);
                tutorialManager.ToggleCollider(TutorialManager.eTutorialObjectName.CarpetClosed, true);
                tutorialManager.ToggleCollider(TutorialManager.eTutorialObjectName.Chair, false);
                break;
            
            case "Result_TutorialPhase2ForceChair":  // 의자 강조
                if (!tutorialManager)
                    break;
                
                UIManager.Instance.ToggleHighlightAnimationEffect(
                    tutorialManager.tutorialGameObjects[TutorialManager.eTutorialObjectName.CarpetClosed], false);
                UIManager.Instance.ToggleHighlightAnimationEffect(
                    tutorialManager.tutorialGameObjects[TutorialManager.eTutorialObjectName.Chair], true);
                tutorialManager.ToggleCollider(TutorialManager.eTutorialObjectName.CarpetClosed, false);
                tutorialManager.ToggleCollider(TutorialManager.eTutorialObjectName.Chair, true);
                break;
            
            case "Result_TutorialPhase3ForceLoR":  // 종이 강조
                if (!tutorialManager)
                    break;
                
                UIManager.Instance.ToggleHighlightAnimationEffect(
                    tutorialManager.tutorialGameObjects[TutorialManager.eTutorialObjectName.CarpetClosed], false);
                UIManager.Instance.ToggleHighlightAnimationEffect(
                    tutorialManager.tutorialGameObjects[TutorialManager.eTutorialObjectName.LoR], true);
                tutorialManager.ToggleCollider(TutorialManager.eTutorialObjectName.CarpetOpen, false);
                break;
            
            case "ResultHighlightHeartsOn":
                foreach (Transform heart in UIManager.Instance.GetUI(eUIGameObjectName.HeartParent).transform)
                    UIManager.Instance.ToggleHighlightAnimationEffect(heart.gameObject, true);
                break;
            
            case "ResultHighlightHeartsOff":
                foreach (Transform heart in UIManager.Instance.GetUI(eUIGameObjectName.HeartParent).transform)
                    UIManager.Instance.ToggleHighlightAnimationEffect(heart.gameObject, false);
                break;
            
            case "Result_TutorialPhase5Force":  // 열린 카펫 덮자 (이미지 강조는 X 검은 화면O)
                if (!tutorialManager)
                    break;
                
                tutorialManager.ToggleCollider(TutorialManager.eTutorialObjectName.CarpetOpen, true);
                UIManager.Instance.ToggleHighlightAnimationEffect(
                    tutorialManager.tutorialGameObjects[TutorialManager.eTutorialObjectName.CarpetOpen], true);
                break;
                
            case "ResultNewTeddyBearZoom":
                RoomManager.Instance.imageAndLockPanelManager.SetObjectImageGroup(true, "newTeddyBear");
                break;

            // 방탈출1
            case "ResultDrinkAndMedicineScript": // 술과 감기약에 대한 설명
                RoomManager.Instance.imageAndLockPanelManager.SetObjectImageGroup(true, "liquorAndPills");
                break;

            case "ResultPillowAmuletScript": // 베개 안에 부적을 발견
                RoomManager.Instance.imageAndLockPanelManager.SetObjectImageGroup(true, "amulet");
                break;
            
            case "ResultDiaryLockActivation": // 다이어리 잠금 장치 활성화됨
                RoomManager.Instance.imageAndLockPanelManager.SetLockObject(true, "diary");
                executableObjects["Diary"].ExecuteAction();
                break;
            
            case "ResultDiaryLockOpen": // 다이어리가 열림
                executableObjects["DiaryLock"].ExecuteAction();
                break;
            
            case "ResultDiaryLockFailedScript": // 다이어리 비밀번호를 틀렸다는 스크립트
                SoundPlayer.Instance.UISoundPlay(Sound_DiaryUnlock);
                break;
            
            case "Result_showClockImage":  // 시계 확대 이미지를 표시
                RoomManager.Instance.imageAndLockPanelManager.SetObjectImageGroup(true, "clock");
                break;
            

            case "ResultClockSystemActivation": // 시계 시스템 활성화
                RoomManager.Instance.imageAndLockPanelManager.SetLockObject(true, "clock");
                executableObjects["Clock"].ExecuteAction();
                break;

            case "ResultClockGetKey": // 열쇠를 획득
                SoundPlayer.Instance.UISoundPlay(Sound_Key);
                RoomManager.Instance.imageAndLockPanelManager.SetObjectImageGroup(true, "keys");
                break;

            case "ResultChairMoved": // 의자가 옆으로 이동함
                SoundPlayer.Instance.UISoundPlay(Sound_ChairMovement);
                executableObjects["Chair1"].ExecuteAction();
                executableObjects["Chair2"].ExecuteAction();
                executableObjects["Chair4"].ExecuteAction();
                GameManager.Instance.InverseVariable("ChairMoved");
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

            case "ResultLaptopLockFailedScript": // 노트북 비밀번호 틀림에 대한 설명
                SoundPlayer.Instance.UISoundPlay(Sound_Wrong);
                break;
            
            case "ResultCarpetOpen": // 카펫이 들쳐짐
                SoundPlayer.Instance.UISoundPlay(Sound_CarpetOpen);
                //executableObjectsKeyCheck("ClosedCarpet1");
                //executableObjectsKeyCheck("ClosedCarpet3");
                executableObjects["ClosedCarpet1"].ExecuteAction();
                executableObjects["ClosedCarpet3"].ExecuteAction();
                break;
            
            case "ResultCarpetDropOutLetterZoom": // 종이를 확대해주는 UI
                SoundPlayer.Instance.UISoundPlay(Sound_GrabPaper);
                RoomManager.Instance.imageAndLockPanelManager.SetObjectImageGroup(true, "letterOfResignation");
                break;
            
            case "ResultCarpetClosed": // 카펫이 원래대로 돌아감
                //executableObjectsKeyCheck("OpenCarpet1");
                //executableObjectsKeyCheck("OpenCarpet3");
                SoundPlayer.Instance.UISoundPlay(Sound_CarpetClose);
                executableObjects["OpenCarpet1"].ExecuteAction();
                executableObjects["OpenCarpet3"].ExecuteAction();
                break;
            
            case "ResultPosterScript": // 포스터에 대한 스크립트
                RoomManager.Instance.imageAndLockPanelManager.SetObjectImageGroup(true, "poster");
                break;

            case "ResultCutterKnifeDisappear": // 커터칼 집음
                //Debug.Log("ResultCutterKnifeDisappear executed");
                executableObjects["Knife0"].ExecuteAction();
                executableObjects["Knife1"].ExecuteAction();
                break;

            case "ResultCutterKnifeScript": // 커터칼에 대한 스크립트
                float knifeHideTime = 0.4f;
                StartCoroutine(DialogueManager.Instance.StartDialogue("RoomEscape_020", knifeHideTime));
                StartCoroutine(RoomManager.Instance.imageAndLockPanelManager.SetObjectImageGroupCoroutine(true, "knife", knifeHideTime));
                //DialogueManager.Instance.StartDialogue("RoomEscape_020");
                //RoomManager.Instance.imageAndLockPanelManager.SetObjectImageGroup(true, "knife");
                break;

            case "ResultPosterOversideImageAndSound": // 포스터 뒷장 이미지와 효과음
                RoomManager.Instance.imageAndLockPanelManager.SetObjectImageGroup(true, "posterOverside");
                SoundPlayer.Instance.UISoundPlay(Sound_Poster);
                break;

            case "ResultClosetDoorsOpen": // 옷장 문이 열림
                SoundPlayer.Instance.UISoundPlay(Sound_ClosetOpen);
                executableObjects["ClosedClosetDoors"].ExecuteAction();
                break;
            
            case "ResultClosetDoorsClosed": // 옷장 문이 닫힘
                SoundPlayer.Instance.UISoundPlay(Sound_ClosetClose);
                executableObjects["OpenClosetDoors"].ExecuteAction();
                break;
                
            case "ResultClosetBagScript": // 옷장 속 가방 클릭 스크립트
                RoomManager.Instance.imageAndLockPanelManager.SetObjectImageGroup(true, "cafePintInBagImage");
                break;

            case "ResultClosetBoxWithoutKeyScript": // 옷장 위 상자에 대한 설명(열쇠X 상태 일 때)
                SoundPlayer.Instance.UISoundPlay(Sound_LockerUnlock);
                break;

            case "ResultClosetBoxOpenScript": // 열린 상자 대한 스크립트
                float boxOpenTime = 1.5f;
                executableObjects["Box"].ExecuteAction();
                SoundPlayer.Instance.UISoundPlay(Sound_LockerKeyMovement);
                StartCoroutine(DialogueManager.Instance.StartDialogue("RoomEscape_025", boxOpenTime));
                StartCoroutine(RoomManager.Instance.imageAndLockPanelManager.SetObjectImageGroupCoroutine(true, "photoInsideBox", boxOpenTime));
                GameManager.Instance.SetVariable("BoxOpened", true);
                break;

            //case "ResultClosetBoxSound": // 상자 열리는 사운드
            //    SoundPlayer.Instance.UISoundPlay(Sound_LockerKeyMovement);
            //    executableObjects["Box"].ExecuteAction();
            //    break;

            //case "ResultClosetBoxPicturesUI": // 상자 안 사진들이 UI로 보임
            //    RoomManager.Instance.imageAndLockPanelManager.SetObjectImageGroup(true, "photoInsideBox");
            //    GameManager.Instance.SetVariable("BoxOpened", true);
            //    break;

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
                executableObjects["Closet Unzoomed-closed 3"].ExecuteAction();
                executableObjects["Closet Unzoomed-open 3"].ExecuteAction();
                break;
            
            case "ResultCabinetZoom": // 서랍장 확대 화면으로 전환
                executableObjects["Cabinet Unzoomed-closed 2"].ExecuteAction();
                executableObjects["Cabinet Unzoomed-open 2"].ExecuteAction();
                executableObjects["Cabinet Unzoomed-closed 3"].ExecuteAction();
                executableObjects["Cabinet Unzoomed-open 3"].ExecuteAction();
                executableObjects["Cabinet Unzoomed-closed 4"].ExecuteAction();
                executableObjects["Cabinet Unzoomed-open 4"].ExecuteAction();
                break;
            
            case "Result_showZoomedDesk": // 책상 화면으로 전환
                executableObjects["Desk Unzoomed-UnMovedChair 1"].ExecuteAction();
                executableObjects["Desk Unzoomed-MovedChair 1"].ExecuteAction();
                executableObjects["Desk Unzoomed 2"].ExecuteAction();
                break;
            
            case "Result_showZoomedPillow": // 배게와 침대 위 인형 확대 화면으로 전환
                executableObjects["Pillow Unzoomed 1"].ExecuteAction();
                executableObjects["Pillow Unzoomed 4"].ExecuteAction();
                break;
            
            case "Result_showZoomedDeskShelf": // 책상 위 선반 확대 화면으로 전환
                executableObjects["DeskShelf Unzoomed 1"].ExecuteAction();
                executableObjects["DeskShelf Unzoomed 2"].ExecuteAction();
                break;

            case "Result_showZoomedDrinkAndMedicine": // 술과 감기약 확대 화면으로 전환
                executableObjects["LiquorAndPills Unzoomed 1"].ExecuteAction();
                executableObjects["LiquorAndPills Unzoomed 4"].ExecuteAction();
                break;

            case "ResultEndRoom1": // Room1 끝
                GameSceneManager.Instance.LoadScene(SceneType.FOLLOW_1);
                break;

            // 방탈출1 귀가 스크립트
            case "Result_Room1HomeComing2":
                // 랜덤
                var randomHomeComing = new Random((uint)System.DateTime.Now.Ticks);  // choose a random dialogue ID
                string[] random1HomeComing2DialogueIDs = { "RoomEscapeS_001", "RoomEscapeS_003" };
                DialogueManager.Instance.StartDialogue(random1HomeComing2DialogueIDs[randomHomeComing.NextInt(0, 2)]);
                break;

            case "Result_Room1HomeComing5":
                // 랜덤
                randomHomeComing = new Random((uint)System.DateTime.Now.Ticks);  // choose a random dialogue ID
                string[] random1HomeComing5DialogueIDs = { "RoomEscapeS_012", "RoomEscapeS_013" };
                DialogueManager.Instance.StartDialogue(random1HomeComing5DialogueIDs[randomHomeComing.NextInt(0, 2)]);
                break;

            // 방탈출1 아침 스크립트
            case "Result_Room1Morning2":
                const float totalTime = 3f;
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

            case "ResultTinCaseSystemActivation": // 틴케이스 비밀번호 시스템 활성화
                RoomManager.Instance.imageAndLockPanelManager.SetLockObject(true, "tinCase");
                executableObjects["TinCase"].ExecuteAction();
                break;

            case "ResultTinCaseGetTicket": // 티켓을 획득
                SoundPlayer.Instance.UISoundPlay(Sound_TincaseOpen); // 틴케이스 열리는 소리
                RoomManager.Instance.imageAndLockPanelManager.SetObjectImageGroup(true, "ticket");
                break;

            case "Result_showSewingBoxImage":   // 반짇고리 상자 확대 이미지 표시
                RoomManager.Instance.imageAndLockPanelManager.SetObjectImageGroup(true, "sewingBox");
                break;

            case "ResultSewingBoxSystemActivation": // 반짇고리 퍼즐 시스템 활성화
                RoomManager.Instance.imageAndLockPanelManager.SetLockObject(true, "sewingBox");
                executableObjects["SewingBox"].ExecuteAction();
                break;

            case "ResultSewingBoxGetThreadAndNeedle":   // 실과 바늘을 획득
                SoundPlayer.Instance.UISoundPlay(Sound_SewingBoxOpen); // 반짇고리 상자 열리는 소리
                RoomManager.Instance.imageAndLockPanelManager.SetObjectImageGroup(true, "thread");  
                break;

            case "ResultFixTeddyBear": // 망가진 곰인형을 고침
                //Debug.Log("ResultFixTeddyBear executed");
                executableObjects["BrokenTeddyBear0"].ExecuteAction();
                executableObjects["BrokenTeddyBear1"].ExecuteAction();
                executableObjects["BrokenTeddyBear4"].ExecuteAction();
                GameManager.Instance.SetVariable("TeddyBearFixed", true);
                break;

            case "ResultBrokenTeddyBear2Yes": // 선택지 약을 먹음
                GameManager.Instance.SetVariable("RefillHeartsOrEndDay", false);
                DialogueManager.Instance.EndDialogue();
                break;

            case "ResultEatEnergySupplement":
                RoomManager.Instance.room2ActionPointManager.EatEnergySupplement();
                break;

            case "ResultBrokenTeddyBear2No": // 선택지 약을 안 먹음
                RoomManager.Instance.room2ActionPointManager.SetChoosingBrokenBearChoice(true);
                DialogueManager.Instance.EndDialogue();
                DialogueManager.Instance.StartDialogue("RoomEscape2_025");
                RoomManager.Instance.room2ActionPointManager.SetChoosingBrokenBearChoice(false);
                break;

            case "ResultDiary2LockActivation": // 다이어리 잠금 장치 활성화됨
                RoomManager.Instance.imageAndLockPanelManager.SetLockObject(true, "diary2");
                executableObjects["Diary2"].ExecuteAction();
                break;

            case "ResultDiary2LockOpen": // 다이어리가 열림
                executableObjects["Diary2Lock"].ExecuteAction();
                break;

            case "ResultDiary2LockFailedScript": // 다이어리 비밀번호를 틀렸다는 스크립트
                SoundPlayer.Instance.UISoundPlay(Sound_DiaryUnlock);
                break;

            // 방탈출2의 확대 화면 전환 result 
            case "Result_showZoomedBox": // 옷장 위 상자 확대 화면으로 전환
                if (GameSceneManager.Instance.GetActiveScene() == Constants.SceneType.ROOM_1) {
                    executableObjects["Box Unzoomed-open 2"].ExecuteAction();
                    executableObjects["Box Unzoomed-open 3"].ExecuteAction();
                }
                executableObjects["Box Unzoomed-closed 2"].ExecuteAction();
                executableObjects["Box Unzoomed-closed 3"].ExecuteAction();
                break;

            case "Result_showZoomedClosetUnder": // 옷장 아래 영역 확대 화면으로 전환
                executableObjects["ClosetUnder Unzoomed 2"].ExecuteAction();
                executableObjects["ClosetUnderDown Unzoomed-closed 2"].ExecuteAction();
                executableObjects["ClosetUnderDown Unzoomed-open 2"].ExecuteAction();
                executableObjects["ClosetUnderUp Unzoomed-closed 2"].ExecuteAction();
                executableObjects["ClosetUnderUp Unzoomed-open 2"].ExecuteAction();
                executableObjects["ClosetUnder Unzoomed 3"].ExecuteAction();
                executableObjects["ClosetUnderDown Unzoomed-closed 3"].ExecuteAction();
                executableObjects["ClosetUnderDown Unzoomed-open 3"].ExecuteAction();
                executableObjects["ClosetUnderUp Unzoomed-closed 3"].ExecuteAction();
                executableObjects["ClosetUnderUp Unzoomed-open 3"].ExecuteAction();
                break;

            case "Result_showZoomedMedicine2": // 약 확대 화면으로 전환
                executableObjects["Medicine2 Unzoomed 1"].ExecuteAction();
                executableObjects["Medicine2 Unzoomed 4"].ExecuteAction();
                break;

            case "Result_showZoomedShoppingBags": // 쇼핑백 확대 화면으로 전환
                executableObjects["ShoppingBags Unzoomed 2"].ExecuteAction();
                executableObjects["ShoppingBags Unzoomed 3"].ExecuteAction();
                executableObjects["ShoppingBags Unzoomed 4"].ExecuteAction();
                break;

            case "ResultMedicine2Zoom": // 선반 위 약 확대UI
                RoomManager.Instance.imageAndLockPanelManager.SetObjectImageGroup(true, "medicine2");
                break;

            case "ResultHospitalFlyer2Script": // 방탈출2 병원 전단지  스크립트
                SoundPlayer.Instance.UISoundPlay(Sound_GrabPaper);
                break;

            case "ResultHospitalFlyer2Zoom": // 방탈출2 병원 전단지 확대 UI
                RoomManager.Instance.imageAndLockPanelManager.SetObjectImageGroup(true, "hospitalPrint");
                break;

            case "ResultPoster2Zoom": // 방탈출2 바다 포스터 확대 UI
                RoomManager.Instance.imageAndLockPanelManager.SetObjectImageGroup(true, "poster2");
                break;

            case "ResultStarSticker2Zoom": // 방탈출2 별 스티커 확대 UI
                RoomManager.Instance.imageAndLockPanelManager.SetObjectImageGroup(true, "starSticker");
                break;

            case "ResultShoppingBag2Zoom": // 방탈출2 쇼핑백 확대 UI
                RoomManager.Instance.imageAndLockPanelManager.SetObjectImageGroup(true, "shoppingBag2");
                break;

            case "ResultWardrobeUpDrawersOpenScript": // 옷장 서랍장 위칸이 열리는 스크립트
                // 임시!!!
                DialogueManager.Instance.StartDialogue("RoomEscape_026");
                break;

            case "ResultWardrobeUpDrawersOpen": // 옷장 서랍장 위칸이 열림
                SoundPlayer.Instance.UISoundPlay(Sound_DrawerOpen);
                executableObjects["ClosedWardrobeUpDrawers"].ExecuteAction();
                break;

            case "ResultWardrobeUpDrawersClosed": // 옷장 서랍장 위칸이 닫힘
                SoundPlayer.Instance.UISoundPlay(Sound_DrawerClose);
                executableObjects["OpenWardrobeUpDrawers"].ExecuteAction();
                break;

            case "ResultWardrobeDownDrawersOpenScript": // 옷장 서랍장 아래칸이 열리는 스크립트
                // 임시!!!
                DialogueManager.Instance.StartDialogue("RoomEscape_026");
                break;

            case "ResultWardrobeDownDrawersOpen": // 옷장 서랍장 아래칸이 열림
                SoundPlayer.Instance.UISoundPlay(Sound_DrawerOpen);
                executableObjects["ClosedWardrobeDownDrawers"].ExecuteAction();
                break;

            case "ResultWardrobeDownDrawersClosed": // 옷장 서랍장 아래칸이 닫힘
                SoundPlayer.Instance.UISoundPlay(Sound_DrawerClose);
                executableObjects["OpenWardrobeDownDrawers"].ExecuteAction();
                break;

            case "Result_showBook2CoverImage": // 떨어진 책 한권 표지 확대 UI
                RoomManager.Instance.imageAndLockPanelManager.SetObjectImageGroup(true, "book2Cover");
                //Debug.Log("떨어진 책 한권 표지 확대 UI");
                break;

            case "ResultBook2SystemActivation": // 떨어진 책 읽기
                RoomManager.Instance.imageAndLockPanelManager.SetLockObject(true, "book2");
                executableObjects["Book2"].ExecuteAction();
                //Debug.Log("  떨어진 책 읽기");
                break;

            case "ResultClosetKey2Script": // 옷장 안 열쇠에 대한 스크립트
                SoundPlayer.Instance.UISoundPlay(Sound_Key);
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

            case "ResultDreamDiary2WithKeyScript": // 열쇠 발견 후 꿈일기 발견 스크립트
                SoundPlayer.Instance.UISoundPlay(Sound_LockerUnlock);
                break;

            case "ResultDreamDiary2SystemActivation": // 꿈일기 읽기
                RoomManager.Instance.imageAndLockPanelManager.SetLockObject(true, "dreamDiary");
                executableObjects["DreamDiary"].ExecuteAction();
                break;

            case "ResultEndRoom2": // Room2 끝, 스크립트 출력 및 미행2로 진행
                DialogueManager.Instance.StartDialogue("RoomEscape2S_016");
                GameSceneManager.Instance.LoadScene(SceneType.FOLLOW_2);
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
            case "ResultFollowTutorialNextStep":
                FollowManager.Instance.TutorialNextStep();
                break;

            case "ResultFollowEndLogic_1":
                FollowManager.Instance.FollowEndLogic_1();
                break;

            case "ResultFollowEndLogic_3":
                FollowManager.Instance.FollowEndLogic_3();
                break;

            case "ResultFollowEnd": // 미행 끝 (메모 개수 충분)
                GameSceneManager.Instance.LoadScene(SceneType.ROOM_2);
                break;

            case "ResultUnlockFOLLOW_1":
                SoundPlayer.Instance.ChangeBGM(BGM_ROOM1);
                break;

            case "ResultLockFOLLOW_1":
                SoundPlayer.Instance.ChangeBGM(BGM_ROOM1);
                break;

            case "ResultUnlockROOM_2":
                SoundPlayer.Instance.ChangeBGM(BGM_FOLLOW1);
                EndingManager.Instance.Ending_Follow_Video(1);
                break;

            case "ResultLockROOM_2":
                SoundPlayer.Instance.ChangeBGM(BGM_FOLLOW1);
                EndingManager.Instance.Ending_Follow_Video(0);
                break;

            case "ResultBadEndingB_01B":
                SoundPlayer.Instance.ChangeBGM(BGM_BAD_B);
                break;

            case "ResultUnlockHidden":
                SoundPlayer.Instance.ChangeBGM(BGM_FOLLOW1);
                EndingManager.Instance.Ending_Follow_Video(3);
                break;

            case "ResultUnlockTrue":
                SoundPlayer.Instance.ChangeBGM(BGM_FOLLOW1);
                EndingManager.Instance.Ending_Follow_Video(3);
                break;

            case "ResultLockTrueAndHidden":
                SoundPlayer.Instance.ChangeBGM(BGM_FOLLOW1);
                EndingManager.Instance.Ending_Follow_Video(3);
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
                Debug.LogWarning($"Result ID: {resultID} not found!");
                break;
        }
    }
}
