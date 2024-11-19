using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class GameManager : MonoBehaviour
{
    // GameManager를 싱글턴으로 생성
    public static GameManager Instance { get; private set; }
    
    // 이벤트의 실행 조건을 확인하기 위한 변수를 모두 이곳에서 관리
    // 변수의 타입은 int 또는 bool
    private Dictionary<string, object> variables = new Dictionary<string, object>();
    public Dictionary<string, object> Variables // 데이터 저장을 위해 작성
    {
        get => variables;
        set => variables = value;
    }

    // 디버깅용
    [SerializeField] private TextMeshProUGUI variablesText;
    public bool isDebug = false;
    public bool skipTutorial = false;
    public bool skipInquiry = false;
    public int accidyGender = 0;  // 우연 성별 {0: 여자, 1: 남자}

    // 조사 시스템에서 현재 조사하고 있는 오브젝트의 evnetId를 저장함
    private string currentInquiryObjectId = "";

    // 중복조사 관련 딕셔너리
    public Dictionary<string, bool> eventObjectsStatusDict = new Dictionary<string, bool>();

    public void SetCurrentInquiryObjectId(string objectId)
    {
        currentInquiryObjectId = objectId;
    }

    public string GetCurrentInquiryObjectId()
    {
        if (currentInquiryObjectId == null)
        {
            Debug.Log("currentInquiryObjectId is NULL!");
            return null;
        }
        else return currentInquiryObjectId;
    }

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
    
    void Start()
    {
        // ------------------------- 변수 초기화 -------------------------
        // 1. 시스템 관련 변수들
        variables["Language"] = 1;  // 시스템 언어
        
        variables["FateName"] = "필연";  // 필연 이름
        variables["FateGender"] = 0;  // 필연 성별 {0: 여자, 1: 남자}
        variables["FateBirthday"] = "0516";  // 필연 생일
        
        variables["AccidyGender"] = accidyGender;  // 우연 성별 {0: 여자, 1: 남자}
        variables["AccidyBirthday"] = "0616";  // 우연 생일

        variables["isInquiry"] = false; // 조사 시스템에서 예 누르면 true되고 계속 조사 가능.

        variables["currentSideIndex"] = 0; // 방탈출 현재 사이드 번호

        variables["RefillHeartsOrEndDay"] = false;

        variables["CurrentScene"] = Constants.SceneType.START.ToInt();

        variables["MemoCount_ROOM_1"] = 0;
        variables["MemoCount_FOLLOW_1"] = 0;
        variables["MemoCount_ROOM_2"] = 0;
        variables["MemoCount_FOLLOW_2"] = 0;

        for (int i = 1; i <= 4; i++)
        {
            int cutLine = int.Parse(ConditionManager.Instance.conditions[$"ConditionMemoClear_{i.ToEnum()}"].Value);
            variables[$"CutLine_{i.ToEnum()}"] = cutLine;
        }

        // 1 - 1. 방탈출 ActionPoint 관련 변수들
        variables["ActionPoint"] = 25;  // 행동력 

        variables["ActionPointsPerDay"] = 5;

        variables["PresentHeartIndex"] = (int)GetVariable("ActionPointsPerDay") - 1; // 행동력 감소로 터질 하트 자리
        // presentHeartIndex가 쓰이는 곳은 배열이기에 0부터 시작해서 5 - 1 한 값으로 초기화.

        variables["IsEatenEnergySupplement"] = false;   // 회복제 먹은 상태 변수

        variables["NowDayNum"] = 1; // 현재 날짜

        variables["MaxDayNum"] = 5; // 방탈출에서 지내는 최대 날짜수

        // 2 - 0. 튜토리얼 관련 변수들 - 첫번째 방탈출
        variables["isTutorial"] = false;
        variables["TutorialPhase"] = 0;

        // 2 - 1. 이벤트 오브젝트 관련 변수들 - 첫번째 방탈출

        // 다회차 추가 곰인형
        variables["NewTeddyBearClick"] = 0;


        // 침대 위 곰인형
        variables["DollsClick"] = 0;

        // 이불
        variables["BlanketsClick"] = 0;

        // 술과 감기약
        variables["LiquorAndPillsClick"] = 0;

        // 스탠드
        variables["LampClick"] = 0;

        // 인형 수납장
        variables["ShelvesClick"] = 0;

        // 배게
        variables["PillowClick"] = 0;

        // 다이어리
        variables["DiaryClick"] = 0;
        variables["DiaryPasswordCorrect"] = false;
        variables["Diary2PresentPageNumber"] = 0;

        // 시계
        variables["ClockClick"] = 0;
        variables["ClockTimeCorrect"] = false;

        variables["ClockPasswordHour"] = 6;
        variables["ClockPasswordMinute"] = 30;

        // 의자
        variables["ChairMoved"] = false;

        // 노트북
        variables["LaptopClick"] = 0;
        variables["LaptopPasswordCorrect"] = false;
        variables["LaptopPassword"] = "04551";

        // 카펫
        variables["ClosedCarpetClick"] = 0;
        variables["CarpetClosed"] = true;
        // 카펫 아래 종이
        variables["PaperClick"] = 0;

        // 포스터
        variables["PosterClick"] = 0;
        variables["PosterOpened"] = false;

        // 커터칼
        variables["HasKnife"] = false;

        // 옷장
        variables["ClosetDoorsClosed"] = true;
        variables["ClosedClosetDoorsClick"] = 0;

        // 옷장 속 가방
        variables["BagClick"] = 0;

        // 옷장 위 상자
        variables["BoxClick"] = 0;
        variables["BoxOpened"] = false;

        // 서랍장
        variables["CabinetDoorsClosed"] = true;
        variables["ClosedCabinetDoorsClick"] = 0;

        // 서랍장 속 달력
        variables["CalendarCluesFound"] = 0;
        variables["CalendarMonth"] = 0;

        variables["FateBirthdayClick"] = 0;
        variables["AccidyBirthdayClick"] = 0;
        variables["SpecialDateAClick"] = 0;
        variables["SpecialDateBClick"] = 0;

        // 쇼핑백
        variables["ShoppingBagClick"] = 0;

        // 침대 옆 일반 포스터
        variables["NormalPosterClick"] = 0;

        // 서랍장 내부 곰인형
        variables["StorageTeddyBearClick"] = 0;

        // 선반 위 책들
        variables["BooksClick"] = 0;

        // 선반 위 곰인형들
        variables["ShelfBearClick"] = 0;

        // 옷장 아래 서랍
        variables["ClosetUnderClick"] = 0;

        // 문 곰돌이 장식물
        variables["DoorBearClick"] = 0;

        // 문
        variables["DoorClick"] = 0;


        // 두번째 방탈출

        // 틴 케이스
        variables["TinCaseClick"] = 0;
        variables["TinCaseCorrect"] = false;

        // 반짇고리
        variables["SewingBoxClick"] = 0;
        variables["SewingBoxCorrect"] = false;

        // 침대 위 곰인형
        variables["BedTeddyClick2"] = 0;

        // 책상 위 스탠드
        variables["LampClick2"] = 0;

        // 책상 위 책 무더기
        variables["UpDeskBookClick2"] = 0;

        // 책상 밑 평범한 책
        variables["UnderDeskBookClick2"] = 0;

        // 책장 위 무너진 책들
        variables["ShelfBookClick2"] = 0;

        // 책장 위 곰인형들
        variables["ShelfTeddyBearClick2"] = 0;

        // 책장 위 깨진 시계
        variables["ClockClick2"] = 0;

        // 벽에 있는 평범한 포스터들
        variables["NormalPosterClick2"] = 0;

        // 옷장 위에 있는 상자
        variables["ClosetBoxClick2"] = 0;

        // 옷장 밑에 있는 사진들
        variables["UnderPhotoClick2"] = 0;

        // 병원 전단지
        variables["HospitalFlyerClick2"] = 0;

        // 바다 포스터
        variables["PosterClick2"] = 0;

        // 별 스티커
        variables["StarStickerClick2"] = 0;

        // 쇼핑백
        variables["ShoppingBagClick2"] = 0;

        // 옷장
        // 옷장 안 열쇠
        variables["ClosetKey2Click"] = 0;
        // 옷장 안 가방
        variables["ClosetBag2Click"] = 0;
        // 옷장 안 명함
        variables["Namecard2Click"] = 0;
        // 옷장 안 정리 안 된 옷과 쇼핑백
        variables["ClosetClothAndBag2Click"] = 0;

        // 옷장 서랍장
        // 위칸
        variables["WardrobeUpDrawersClosed"] = true;
        variables["ClosedWardrobeUpDrawersClick"] = 0;
        // 아래칸
        variables["WardrobeDownDrawersClosed"] = true;
        variables["ClosedWardrobeDownDrawersClick"] = 0;
        // 옷장 밑 수납장 속 헝클어진 옷
        variables["UnderClosetClothes2Click"] = 0;

        // 망가진 곰인형
        variables["BrokenTeddyBearClick"] = 0;
        variables["TeddyBearFixed"] = false;

        // 실과 바늘
        variables["GetThreadAndNeedle"] = false;

        // 책장 다이어리
        variables["Diary2Click"] = 0;
        variables["Diary2PasswordCorrect"] = false;

        // 선반 위의 약
        variables["Medicine2Click"] = 0;

        // 화려한 꿈관련 책
        variables["Book2Click"] = 0;

        // 옷장 안 열쇠
        variables["HasClosetKey2"] = false;

        // 꿈일기
        variables["DreamDiary2Click"] = 0;
        variables["DreamDiary2Opened"] = false;


        // 2 - 2. 이벤트 오브젝트 관련 변수들 - 첫번째 미행
        // 미행 튜토리얼 단계
        variables["FollowTutorialPhase"] = 0;

        // 빌라
        variables["VillaClick"] = 0;
        // 2층 빵집
        variables["BreadClick"] = 0;
        // 1층 편의점
        variables["ConClick"] = 0;
        // 3층 바
        variables["BarClick"] = 0;
        // 이자카야
        variables["IzakawaClick"] = 0;
        // 고양이
        variables["CatClick"] = 0;
        // 카페
        variables["CafeClick"] = 0;
        // 카페 영수증
        variables["ReceiptClick"] = 0;
        // 신호등
        variables["LightClick"] = 0;
        // 음료
        variables["LatteClcik"] = 0;
        // 1층 옷가게
        variables["1FClothesClick"] = 0;
        // 2층 옷가게
        variables["2FClothesClick"] = 0;
        // 공사장
        variables["ConstructionClick"] = 0;
        // 식당
        variables["RestaurantClick"] = 0;
        // 칵테일 바
        variables["CocktailBarClick"] = 0;
        // 화장품 가게
        variables["CosemeticClick"] = 0;
        // 뮤직바
        variables["MusicBarClick"] = 0;
        // 클럽
        variables["ClubCliclk"] = 0;
        // 술집
        variables["BeerClick"] = 0;
        // 화환
        variables["WreathClick"] = 0;
        // 화난 사람
        variables["AngryClick"] = 0;

        // 2 - 3. 이벤트 오브젝트 관련 변수들 - 두번째 미행
        // 빌라
        variables["VillaClick2"] = 0;
        // 3층 바
        variables["BarClick2"] = 0;
        // 빵집
        variables["BreadClick2"] = 0;
        // 3층 맥주 전문점
        variables["BeerAClick2"] = 0;
        // 이자카야
        variables["IzakawaClick2"] = 0;
        // 카페
        variables["CafeClick2"] = 0;
        // 3층 옷가게
        variables["3FClothesClick2"] = 0;
        // 2층 옷가게
        variables["2FClothesClick2"] = 0;
        // 3층 술집
        variables["DrinkAClick2"] = 0;
        // 칵테일 바
        variables["CocktailBarClick2"] = 0;
        // 식당
        variables["RestaurantClick2"] = 0;
        // 3층 술집
        variables["DrinkBClick2"] = 0;
        // 뮤직바
        variables["MusicBarClick2"] = 0;
        // 화장품 가게
        variables["CosemeticClick2"] = 0;
        // 3층 술집
        variables["DrinkCClick2"] = 0;
        // 2층 맥주 가게
        variables["BeerBClick2"] = 0;
        // 클럽
        variables["ClubClick2"] = 0;
        // 신호등
        variables["LightClick2"] = 0;
        // 호객 행위 하는 사람
        variables["SolicitationClick2"] = 0;
        // 편의점
        variables["ConClick2"] = 0;
        // 가출 청소년
        variables["TeenageClick2"] = 0;
        // 1층 옷가게
        variables["1FClothesClick2"] = 0;
        // 경찰
        variables["PoliceClick2"] = 0;
        // 담패 피는 사람
        variables["CigaretteClick2"] = 0;
        // 클럽 가드
        variables["GuardClick2"] = 0;

        // 3. 엔딩 관련 변수들
        variables["EndingCollect"] = 0; // 본 엔딩의 개수
        variables["LastEnding"] = "NONE"; // 마지막으로 본 엔딩
        variables["BadACollect"] = 0; // 배드A 엔딩을 본 횟수
        variables["BadBCollect"] = 0; // 배드B 엔딩을 본 횟수
        variables["TrueCollect"] = 0; // 트루 엔딩을 본 횟수
        variables["HiddenCollect"] = 0; // 히든 엔딩을 본 횟수
        variables["BadEndingCollect"] = 0; // 배드 엔딩을 본 횟수

        if (isDebug) ShowVariables();

    }

    public void SetVariable(string variableName, object value)
    {
        if (variables.ContainsKey(variableName))
        {
            variables[variableName] = value;
        }
        else
        {
            Debug.Log($"variable \"{variableName}\" does not exist!");
        }
    }

    public object GetVariable(string variableName)
    {
        if (variables.ContainsKey(variableName))
        {
            return variables[variableName];
        }
        else
        {
            Debug.Log($"variable: \"{variableName}\" does not exist!");
            return null;
        }
    }

    public void IncrementVariable(string variableName)
    {
        int cnt = (int)GetVariable(variableName);
        cnt++;
        SetVariable(variableName, cnt);
    }

    public void IncrementVariable(string variableName, int count)
    {
        int cnt = (int)GetVariable(variableName);
        cnt += count;
        SetVariable(variableName, cnt);
    }

    public void DecrementVariable(string variableName)
    {
        int cnt = (int)GetVariable(variableName);
        cnt--;
        SetVariable(variableName, cnt);
    }

    public void DecrementVariable(string variableName, int count)
    {
        int cnt = (int)GetVariable(variableName);
        cnt -= count;
        SetVariable(variableName, cnt);
    }

    public void InverseVariable(string variableName)
    {
        bool variableValue = (bool)GetVariable(variableName);
        variableValue = !variableValue;
        SetVariable(variableName, variableValue);
    }

    // 디버깅 용
    private void Update()
    {
        if (isDebug) ShowVariables();
    }

    private void ShowVariables()
    {
        variablesText.text = "";  // 텍스트 초기화

        // 화면에 표시하고 싶은 변수명 추가
        List<string> keysToShow = new List<string>(new string[]
        {
         "FateBirthday",
         "AccidyBirthday",
         // "isTutorial",
         // "TutorialPhase"
         "ActionPoint",
         "Diary2PresentPageNumber"
        });

        foreach (var item in variables)
        {
            if (keysToShow.Contains(item.Key)) variablesText.text += $"{item.Key}: {item.Value}\n";
        }
    }

    public bool GetIsBusy()  // 클릭을 막아야 하는 상황들
    {
        bool isDialogueActive = DialogueManager.Instance.isDialogueActive;
        bool isInvestigating = RoomManager.Instance.isInvestigating;
        bool isTutorialPhase1 = (int)variables["TutorialPhase"] == 1;
        bool isMemoOpen = MemoManager.Instance.isMemoOpen;

        bool isBusy = isDialogueActive || isInvestigating || isTutorialPhase1 || isMemoOpen;

        return isBusy;
    }

    // 원래 EventObjectManager의 별로 없었던 기능들 GameManager에 옮김

    public void AddEventObject(EventObject eventObject)
    {
        if (!eventObjectsStatusDict.ContainsKey(eventObject.GetEventId()))
        {
            eventObjectsStatusDict.Add(eventObject.GetEventId(), false);
        }
    }

    public void AddEventObject(string eventObjectId)
    {
        if (!eventObjectsStatusDict.ContainsKey(eventObjectId))
        {
            eventObjectsStatusDict.Add(eventObjectId, false);
        }
    }

    public void SetEventFinished(string eventId)
    {
        if (eventObjectsStatusDict.ContainsKey(eventId))
        {
            eventObjectsStatusDict[eventId] = true;
        }
        else
        {
            Debug.Log(eventId+ " is not existed!");
        }
    }

    public void SetEventUnFinished(string eventId)
    {
        if (eventObjectsStatusDict.ContainsKey(eventId))
        {
            eventObjectsStatusDict[eventId] = false;
        }
    }

    public bool GetEventStatus(string eventId)
    {
        return eventObjectsStatusDict.ContainsKey(eventId) && eventObjectsStatusDict[eventId];
    }
}