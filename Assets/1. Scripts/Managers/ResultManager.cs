using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using static Constants;

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
        if (!executableObjects.ContainsKey(objectName))
        {
            executableObjects[objectName] = executable;
        }
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
        
        // ------------------------ 이곳에 모든 동작을 수동으로 추가 ------------------------
        switch (resultID)
        {
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
            
            case "Result_001": // 눈 뜨는 효과
                executableObjects["StartLogic"].ExecuteAction();
                break;
            
            case "Result_002": // 설명창 띄우기 - ### 추후 구현 필요 ###
                SceneManager.LoadScene(1);
                break;
            
            case "Result_003": // 행동력 감소 (행동력이 감소할 때마다 게임 저장)
                GameManager.Instance.DecrementVariable("ActionPoint");
                SaveManager.Instance.SaveGameData();
                break;
            
            case "Result_004": // 조사 완료 스크립트
                DialogueManager.Instance.StartDialogue("RoomEscape_008");
                break;

            // 휴식 시스템
            case "Result_restButton": // 휴식에서 예 버튼
                DialogueManager.Instance.StartDialogue("RoomEscape_034");
                break;

            case "Result_restYes": // 휴식에서 예 버튼
                DialogueManager.Instance.EndDialogue();
                // 눈깜빡
                ScreenEffect.Instance.RestButtonEffect();
                // 행동력 5감소
                GameManager.Instance.DecrementVariable("ActionPoint", 5);
                // 대사 출력 지연 시킴
                StartCoroutine(DialogueManager.Instance.StartDialogue("RoomEscape_035", 5f));
                break;

            case "Result_restNo": // 휴식에서 아니오 버튼
                DialogueManager.Instance.EndDialogue();
                break;


            // 튜토리얼
            case "Result_nextTutorialPhase":  // 튜토리얼 다음 페이즈로 진행
                RoomManager.Instance.tutorialManager.ProceedToNextPhase();
                break;
            
            case "Result_005": // 곰인형에 대한 설명
                DialogueManager.Instance.StartDialogue("RoomEscape_001");
                break;

            case "Result_006": // 이불에 대한 설명
                DialogueManager.Instance.StartDialogue("RoomEscape_002");
                break;
            
            case "Result_007": // 술과 감기약에 대한 설명
                RoomManager.Instance.imageAndLockPanelManager.SetObjectImageGroup(true, "liquorAndPills");
                DialogueManager.Instance.StartDialogue("RoomEscape_003");
                break;
            
            case "Result_008": // 책상 위 스탠드에 대한 설명
                DialogueManager.Instance.StartDialogue("RoomEscape_004");
                break;
            
            case "Result_009": // 인형 수납장에 대한 설명
                DialogueManager.Instance.StartDialogue("RoomEscape_005");
                break;

            case "Result_010": // 베개에 대한 설명
                DialogueManager.Instance.StartDialogue("RoomEscape_006");
                break;
            
            case "Result_011": // 베개 안에 부적을 발견
                RoomManager.Instance.imageAndLockPanelManager.SetObjectImageGroup(true, "amulet");
                DialogueManager.Instance.StartDialogue("RoomEscape_007");
                break;
            
            case "Result_012": // 부적에 대한 메모 작성
                MemoManager.Instance.AddMemo("R1Memo_001");
                break;

            case "Result_013": // 다이어리에 대한 설명
                DialogueManager.Instance.StartDialogue("RoomEscape_009");
                break;
            
            case "Result_014": // 다이어리 잠금 장치 활성화됨
                RoomManager.Instance.imageAndLockPanelManager.SetLockObject(true, "diary");
                executableObjects["Diary"].ExecuteAction();
                break;
            
            case "Result_015": // 다이어리가 열림
                executableObjects["DiaryLock"].ExecuteAction();
                break;
            
            case "Result_016": // 다이어리에 대한 메모가 작성됨
                MemoManager.Instance.AddMemo("R1Memo_002");
                break;
            
            case "Result_017": // 다이어리에 대한 스크립트
                DialogueManager.Instance.StartDialogue("RoomEscape_010");
                break;
            
            case "Result_018": // 다이어리 비밀번호를 틀렸다는 스크립트
                SoundPlayer.Instance.UISoundPlay(Sound_DiaryUnlock);
                DialogueManager.Instance.StartDialogue("RoomEscape_011");
                break;
            
            case "Result_showClockImage":  // 시계 확대 이미지를 표시
                RoomManager.Instance.imageAndLockPanelManager.SetObjectImageGroup(true, "clock");
                break;
            
            case "Result_019": // 시계에 대한 설명
                DialogueManager.Instance.StartDialogue("RoomEscape_012");
                break;
            
            case "Result_020": // 시계 시스템 활성화
                RoomManager.Instance.imageAndLockPanelManager.SetLockObject(true, "clock");
                executableObjects["Clock"].ExecuteAction();
                break;

            case "Result_021": // 열쇠를 획득
                RoomManager.Instance.imageAndLockPanelManager.SetObjectImageGroup(true, "keys");
                break;

            case "Result_022": // 열쇠를 획득 후 스크립트
                DialogueManager.Instance.StartDialogue("RoomEscape_013");
                break;

            case "Result_023": // 열쇠 획득 후 메모
                MemoManager.Instance.AddMemo("R1Memo_003");
                break;

            case "Result_024": // 의자가 옆으로 이동함
                SoundPlayer.Instance.UISoundPlay(Sound_ChairMovement);
                executableObjects["Chair1"].ExecuteAction();
                executableObjects["Chair2"].ExecuteAction();
                GameManager.Instance.InverseVariable("ChairMoved");
                break;

            case "Result_025": // 노트북에 대한 설명
                DialogueManager.Instance.StartDialogue("RoomEscape_014");
                break;

            case "Result_026": // 노트북 비밀번호 화면 활성화
                SoundPlayer.Instance.UISoundPlay(Sound_LaptopBoot);
                RoomManager.Instance.imageAndLockPanelManager.SetLockObject(true, "laptop");
                executableObjects["Laptop"].ExecuteAction();
                break;

            case "Result_027": // 노트북이 열림
                SoundPlayer.Instance.UISoundPlay(Sound_Correct);
                executableObjects["LaptopLock"].ExecuteAction();
                break;

            case "Result_028": // 노트북에 대한 설명이 메모에 기록됨
                MemoManager.Instance.AddMemo("R1Memo_004");
                break;

            case "Result_029": // 노트북에 대한 스크립트 출력
                DialogueManager.Instance.StartDialogue("RoomEscape_015");
                break;

            case "Result_030": // 노트북 비밀번호 틀림에 대한 설명
                SoundPlayer.Instance.UISoundPlay(Sound_Wrong);
                DialogueManager.Instance.StartDialogue("RoomEscape_016");
                break;
            
            case "Result_031": // 카펫이 들쳐짐
                SoundPlayer.Instance.UISoundPlay(Sound_CarpetOpen);
                executableObjects["ClosedCarpet"].ExecuteAction();
                break;
            
            case "Result_032": // 종이를 확대해주는 UI
                RoomManager.Instance.imageAndLockPanelManager.SetObjectImageGroup(true, "letterOfResignation");
                break;
            
            case "Result_033": // 종이에 대한 스크립트
                DialogueManager.Instance.StartDialogue("RoomEscape_017");
                break;
            
            case "Result_034": // 카펫이 원래대로 돌아감
                SoundPlayer.Instance.UISoundPlay(Sound_CarpetClose);
                executableObjects["OpenCarpet"].ExecuteAction();
                DialogueManager.Instance.StartDialogue("RoomEscape_018");
                break;
            
            case "Result_035": // 포스터에 대한 스크립트
                RoomManager.Instance.imageAndLockPanelManager.SetObjectImageGroup(true, "poster");
                DialogueManager.Instance.StartDialogue("RoomEscape_019");
                break;

            case "Result_036": // 커터칼에 대한 스크립트
                RoomManager.Instance.imageAndLockPanelManager.SetObjectImageGroup(true, "knife");
                DialogueManager.Instance.StartDialogue("RoomEscape_020");
                break;

            case "Result_037": // 커터칼 사라짐
                Debug.Log("result 37 executed");
                executableObjects["Knife0"].ExecuteAction();
                executableObjects["Knife1"].ExecuteAction();
                break;

            case "Result_038": // 포스터 뒷장에 대한 설명
                SoundPlayer.Instance.UISoundPlay(Sound_Poster);
                RoomManager.Instance.imageAndLockPanelManager.SetObjectImageGroup(true, "poster");
                DialogueManager.Instance.StartDialogue("RoomEscape_021");
                GameManager.Instance.SetVariable("PosterOpened", true);
                break;

            case "Result_039": // 포스터에 대한 메모
                MemoManager.Instance.AddMemo("R1Memo_005");
                break;

            case "Result_040": // 옷장 문이 열림
                SoundPlayer.Instance.UISoundPlay(Sound_ClosetOpen);
                executableObjects["ClosedClosetDoors"].ExecuteAction();
                break;
            
            case "Result_041": // 옷장 문이 닫힘
                SoundPlayer.Instance.UISoundPlay(Sound_ClosetClose);
                executableObjects["OpenClosetDoors"].ExecuteAction();
                break;
                
            case "Result_042": // 옷장 처음 상호작용했을 때 스크립트
                DialogueManager.Instance.StartDialogue("RoomEscape_022");
                break;

            case "Result_043": // 옷장 속 가방 클릭 스크립트
                RoomManager.Instance.imageAndLockPanelManager.SetObjectImageGroup(true, "cafePintInBagImage");
                DialogueManager.Instance.StartDialogue("RoomEscape_023");
                break;

            case "Result_044": // 옷장 속 가방 내에 전단지에 대한 메모
                MemoManager.Instance.AddMemo("R1Memo_006");
                break;

            case "Result_045": // 옷장 위 상자에 대한 설명(열쇠X 상태 일 때)
                SoundPlayer.Instance.UISoundPlay(Sound_LockerUnlock);
                DialogueManager.Instance.StartDialogue("RoomEscape_024");
                break;

            case "Result_046": // 상자가 열리는 스크립트
                DialogueManager.Instance.StartDialogue("RoomEscape_025");
                break;

            case "Result_047": // 상자 열리는 사운드 - ### 추후 구현 필요 ###
                SoundPlayer.Instance.UISoundPlay(Sound_LockerKeyMovement);
                break;

            case "Result_048": // 상자 안 사진들이 UI로 보임
                RoomManager.Instance.imageAndLockPanelManager.SetObjectImageGroup(true, "photoInsideBox");
                GameManager.Instance.SetVariable("BoxOpened", true);
                break;

            case "Result_090": // 상자 안 사진들에 대한 메모
                MemoManager.Instance.AddMemo("R1Memo_011");
                break;

            case "Result_093": // 쇼핑백에 대한 스크립트
                DialogueManager.Instance.StartDialogue("RoomEscape_031");
                break;

            case "Result_094": // 평범 포스터에 대한 스크립트
                DialogueManager.Instance.StartDialogue("RoomEscape_032");
                break;

            case "Result_095": // 서랍장 속 인형에 대한 스크립트
                DialogueManager.Instance.StartDialogue("RoomEscape_033");
                break;

            case "Result_049": // 서랍장이 열리는 스크립트
                DialogueManager.Instance.StartDialogue("RoomEscape_026");
                break;

            case "Result_050": // 서랍장이 열림
                SoundPlayer.Instance.UISoundPlay(Sound_StorageOpen);
                executableObjects["ClosedCabinetDoors"].ExecuteAction();
                break;

            case "Result_051": // 서랍장이 닫힘
                SoundPlayer.Instance.UISoundPlay(Sound_StorageClose);
                executableObjects["OpenCabinetDoors"].ExecuteAction();
                break;

            case "Result_052": // 달력이 열림
                RoomManager.Instance.imageAndLockPanelManager.SetLockObject(true, "calendar");
                executableObjects["Calendar"].ExecuteAction();
                break;

            case "Result_053": // 필연 생일에 대한 스크립트 - ### 추후 구현 필요 ###
                DialogueManager.Instance.StartDialogue("RoomEscape_027");
                break;

            case "Result_054": // 우연 생일에 대한 스크립트 - ### 추후 구현 필요 ###
                DialogueManager.Instance.StartDialogue("RoomEscape_028");
                break;

            case "Result_055": // 10월 31일에 대한 스크립트
                DialogueManager.Instance.StartDialogue("RoomEscape_029");
                break;

            case "Result_056": // 10월 1일에 대한 스크립트
                DialogueManager.Instance.StartDialogue("RoomEscape_030");
                break;

            case "Result_057": // 필연 생일 메모
                MemoManager.Instance.AddMemo("R1Memo_007");
                break;

            case "Result_058": // 우연 생일 메모
                MemoManager.Instance.AddMemo("R1Memo_010");
                break;

            case "Result_059": // 10월 31일 메모
                MemoManager.Instance.AddMemo("R1Memo_008");
                break;

            case "Result_060": // 10월 1일 메모
                MemoManager.Instance.AddMemo("R1Memo_009");
                break;

            case "Result_061": // 옷장 확대 화면으로 전환
                executableObjects["Closet Unzoomed 2"].ExecuteAction();
                break;
            
            case "Result_062": // 서랍장 확대 화면으로 전환
                // executableObjects["Cabinet Unzoomed 2"].ExecuteAction();
                executableObjects["Cabinet Unzoomed 3"].ExecuteAction();
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
                MemoManager.Instance.AddMemo("F1Memo_001");
                break;

            case "ResultCafeMemo2": // 카페에 대한 메모 2
                MemoManager.Instance.AddMemo("F1Memo_002");
                break;

            case "ResultReceiptScript": // 영수증에 대한 스크립트
                DialogueManager.Instance.StartDialogue("Follow_009");
                break;

            case "ResultReceiptMemo": // 영수증에 대한 메모
                MemoManager.Instance.AddMemo("F1Memo_003");
                break;

            case "ResultLightScript": // 신호등에 대한 스크립트
                DialogueManager.Instance.StartDialogue("Follow_010");
                break;

            case "ResultLightMemo": // 신호등에 대한 메모
                MemoManager.Instance.AddMemo("F1Memo_004");
                break;

            case "ResultLatteScript": // 음료에 대한 스크립트
                DialogueManager.Instance.StartDialogue("Follow_011");
                break;

            case "ResultLatteMemo": // 음료에 대한 메모
                MemoManager.Instance.AddMemo("F1Memo_005");
                break;

            case "Result1FClothesScript": // 1층 옷가게에 대한 스크립트
                DialogueManager.Instance.StartDialogue("Follow_012");
                break;

            case "Result1FClothesMemo": // 1층 옷가게에 대한 메모
                MemoManager.Instance.AddMemo("F1Memo_006");
                break;

            case "Result2FClothesScript": // 2층 옷가게에 대한 스크립트
                DialogueManager.Instance.StartDialogue("Follow_013");
                break;

            case "Result2FClothesMemo": // 2층 옷가게에 대한 메모
                MemoManager.Instance.AddMemo("F1Memo_007");
                break;

            case "ResultConstructionScript": // 공사장에 대한 스크립트
                DialogueManager.Instance.StartDialogue("Follow_014");
                break;

            case "ResultConstructionMemo": // 공사장에 대한 메모
                MemoManager.Instance.AddMemo("F1Memo_008");
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
                MemoManager.Instance.AddMemo("F1Memo_009");
                break;

            case "ResultWreathScript": // 쓰러진 화환에 스크립트
                DialogueManager.Instance.StartDialogue("Follow_023");
                break;

            case "ResultWreathMemo": // 쓰러진 화환에 대한 메모
                MemoManager.Instance.AddMemo("F1Memo_010");
                break;
        }
    }
}
