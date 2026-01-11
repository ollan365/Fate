using System.Collections.Generic;
using Fate.Data;
using Fate.Utilities;
using UnityEngine;
using static Fate.Utilities.Constants;
using Random = Unity.Mathematics.Random;

namespace Fate.Managers
{
    public class ResultManager : MonoBehaviour
{
    public static ResultManager Instance { get; private set; }

    // results: dictionary of "Results"s indexed by string "Result ID"
    public readonly Dictionary<string, Result> Results = new Dictionary<string, Result>();
    
    // 이벤트 오브젝트 참조
    private Dictionary<string, IResultExecutable> executableObjects = new Dictionary<string, IResultExecutable>();
    
    // Handler system
    private ResultHandlerRegistry handlerRegistry = new ResultHandlerRegistry();
    
    public void RegisterExecutable(string objectName, IResultExecutable executable) {
        //Debug.Log($"registered {objectName}");

        executableObjects.TryAdd(objectName, executable);
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
            Instance = this;
            DontDestroyOnLoad(gameObject);
            RegisterHandlers();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void RegisterHandlers()
    {
        // Register all handlers
        handlerRegistry.RegisterHandlers(new IResultHandler[]
        {
            // Variable manipulation handlers
            new RevealMemoHandler(),
            new StartDialogueHandler(),
            new IncrementVariableHandler(),
            new DecrementVariableHandler(),
            new InverseVariableHandler(),
            new SetEventFinishedHandler(),
            new SetEventUnFinishedHandler(),
            
            // Game state handlers
            new SetGenderGirlHandler(),
            new SetGenderBoyHandler(),
            new CloseEyesHandler(),
            new FadeOutHandler(),
            new FadeInHandler(),
            new PrologueLimitHandler(),
            new CommonPrologueAHandler(),
            new NamePanelHandler(),
            new BirthPanelHandler(),
            new PrologueEndHandler(),
            new TimePassHandler(),
            
            // Inquiry handlers
            new InquiryHandler(),
            new InquiryYesHandler(),
            new InquiryNoHandler(),
            new BlanketCheckHandler(),
            
            // Rest handlers
            new RestButtonHandler(),
            new RestYesHandler(),
            new RestNoHandler(),
            new StartHomecomingHandler(),
            new NextMorningDayHandler(),
            
            // Legacy handler for remaining cases (to be refactored gradually)
            new LegacyResultHandler()
        });
    }
    
    public void ParseResults()
    {
        TextAsset resultsCSV = Resources.Load<TextAsset>("Datas/results");
        if (resultsCSV == null || string.IsNullOrEmpty(resultsCSV.text))
        {
            Debug.LogError("ResultManager: Failed to load results CSV file from Resources/Datas/results");
            return;
        }

        string[] lines = resultsCSV.text.Split('\n');
        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i]))
                continue;

            string[] fields = lines[i].Split(',');
            
            if (fields.Length < 3)
            {
                Debug.LogWarning($"ResultManager: Invalid result line {i}: expected at least 3 fields, got {fields.Length}");
                continue;
            }
            
            if (fields[0] == "" && fields[1] == "")
                continue;
            
            string resultID = fields[0].Trim();
            if (string.IsNullOrEmpty(resultID))
            {
                Debug.LogWarning($"ResultManager: Result ID is empty at line {i}");
                continue;
            }

            if (Results.ContainsKey(resultID))
            {
                Debug.LogWarning($"ResultManager: Duplicate result ID '{resultID}' at line {i}, skipping");
                continue;
            }
            
            Result result = new Result(
                resultID,
                fields[1].Trim(),
                fields[2].Trim()
                );
            
            Results[result.ResultID] = result;
        }
    }

    public void ExecuteResult(string resultID)
    {
        if (string.IsNullOrEmpty(resultID))
        {
            Debug.LogWarning("ResultManager: ExecuteResult called with null or empty resultID");
            return;
        }

        if (GameManager.Instance != null && GameManager.Instance.isDebug) 
            Debug.Log(resultID);

        // Try to find and execute a handler
        IResultHandler handler = handlerRegistry.FindHandler(resultID);
        if (handler != null)
        {
            handler.Execute(resultID);
            return;
        }

        // Fallback: log warning if no handler found
        Debug.LogWarning($"ResultManager: No handler found for result ID: {resultID}");
    }

    public void ExecuteResultLegacy(string resultID)
    {
        if (string.IsNullOrEmpty(resultID))
        {
            Debug.LogWarning("ResultManager: ExecuteResultLegacy called with null or empty resultID");
            return;
        }

        if (GameManager.Instance != null && GameManager.Instance.isDebug) 
            Debug.Log(resultID);
        
        TutorialManager tutorialManager = null;
        if (RoomManager.Instance != null && RoomManager.Instance.tutorialManager != null)
            tutorialManager = RoomManager.Instance.tutorialManager;
        
        // Remaining cases that haven't been migrated to handlers yet
        switch (resultID)
        {
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
                RoomManager.Instance.imageAndLockPanelManager.SetLockObject(false, "clock");
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

            case "ResultClosetBoxPictures": // 상자 안 사진들이 바로 보임(중복조사 했을 경우)
                RoomManager.Instance.imageAndLockPanelManager.SetObjectImageGroup(true, "photoInsideBox");
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
                StartCoroutine(DialogueManager.Instance.StartDialogue("RoomEscapeS_016", totalTime));
                break;

            case "Result_Room1Morning5":
                StartCoroutine(DialogueManager.Instance.StartDialogue("RoomEscapeS_014", totalTime));
                break;
                
            //case "ResultChangeBGM_Dream":
            //    SoundPlayer.Instance.ChangeBGM(BGM_DREAM);
            //    break;

            // 방탈출 2

            case "Result_showTinCaseImage": // 틴케이스 확대 이미지 표시
                RoomManager.Instance.imageAndLockPanelManager.SetObjectImageGroup(true, "tinCase");
                break;

            case "ResultTinCaseSystemActivation": // 틴케이스 비밀번호 시스템 활성화
                RoomManager.Instance.imageAndLockPanelManager.SetLockObject(true, "tinCase");
                executableObjects["TinCase"].ExecuteAction();
                break;

            case "ResultTinCaseGetTicket": // 티켓을 획득
                RoomManager.Instance.imageAndLockPanelManager.SetLockObject(false, "tinCase");
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
                RoomManager.Instance.imageAndLockPanelManager.SetLockObject(false, "sewingBox");
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
                RoomManager.Instance.actionPointManager.refillHeartsOrEndDayState = false;
                break;

            case "ResultEatEnergySupplement":
                RoomManager.Instance.room2ActionPointManager.EatEnergySupplement();
                break;

            case "ResultBrokenTeddyBear2No": // 선택지 약을 안 먹음
                RoomManager.Instance.room2ActionPointManager.SetChoosingBrokenBearChoice(true);
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
                bool boxOpened = (bool)GameManager.Instance.GetVariable("BoxOpened");
                if (GameSceneManager.Instance.GetActiveScene() == SceneType.ROOM_1
                    && boxOpened)
                {
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
            case "Result_FollowTutorial_Lighting":
                FollowManager.Instance.TutorialNextStep("Light");
                break;

            case "Result_FollowTutorial_Move":
                FollowManager.Instance.TutorialNextStep("Move");
                break;

            case "Result_FollowTutorial_Hide":
                FollowManager.Instance.TutorialNextStep("Hide");
                break;

            case "Result_FollowTutorial_End":
                FollowManager.Instance.TutorialNextStep("End");
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
                SoundPlayer.Instance.ChangeBGM(BGM_EMERGENCYEXIT);
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
}
