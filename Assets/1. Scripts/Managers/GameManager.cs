using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.PlayerLoop;
using Random = Unity.Mathematics.Random;

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

    // 조사시스템 테스트
    private string currentInquiryObjectId = "";
    
    // ************************* temporary members for action points *************************
    public GameObject heartPrefab;
    public GameObject heartParent;
    public TextMeshProUGUI dayText;
    public int actionPointsPerDay = 5;

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

        variables["EndingCollect"] = 0; // 본 엔딩의 개수
        variables["LastEnding"] = "NONE";

        variables["Language"] = 1;  // 시스템 언어
        
        variables["FateName"] = "필연";  // 필연 이름
        variables["FateGender"] = 0;  // 필연 성별 {0: 여자, 1: 남자}
        variables["FateBirthday"] = "0516";  // 필연 생일
        
        variables["AccidyGender"] = 0;  // 우연 성별 {0: 여자, 1: 남자}
        variables["AccidyBirthday"] = "0616";  // 우연 생일
        
        variables["ActionPoint"] = 25;  // 행동력 

        variables["isInquiry"] = false; // 조사 시스템에서 예 누르면 true되고 계속 조사 가능.

        variables["currentSideIndex"] = 0; // 방탈출 현재 사이드 번호

        variables["RefillHeartsOrEndDay"] = false;

        // 2 - 0. 튜토리얼 관련 변수들 - 첫번째 방탈출
        variables["isTutorial"] = false;
        variables["TutorialPhase"] = 0;

        // 2 - 1. 이벤트 오브젝트 관련 변수들 - 첫번째 방탈출
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

        // 달력

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


        // 옷장 서랍장
        // 위칸
        variables["WardrobeUpDrawersClosed"] = true;
        variables["ClosedWardrobeUpDrawersClick"] = 0;
        // 아래칸
        variables["WardrobeDownDrawersClosed"] = true;
        variables["ClosedWardrobeDownDrawersClick"] = 0;

        // 망가진 곰인형
        variables["BrokenTeddyBearClick"] = 0;
        variables["TeddyBearFixed"] = false;

        // 실과 바늘
        variables["GetThreadAndNeedle"] = false;

        // 2 - 2. 이벤트 오브젝트 관련 변수들 - 첫번째 미행
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
        
        if (isDebug) ShowVariables();
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
    
        if (isDebug) ShowVariables();
    }
    
    public void DecrementVariable(string variableName)
    {
        int cnt = (int)GetVariable(variableName);
        cnt--;
        SetVariable(variableName, cnt);

        if (isDebug) ShowVariables();
    }

    public void DecrementVariable(string variableName, int count)
    {
        int cnt = (int)GetVariable(variableName);
        cnt -= count;
        SetVariable(variableName, cnt);

        if (isDebug) ShowVariables();
    }

    public void InverseVariable(string variableName)
    {
        bool variableValue = (bool)GetVariable(variableName);
        variableValue = !variableValue;
        SetVariable(variableName, variableValue);
        
        if (isDebug) ShowVariables();
    }

    // 디버깅 용
    private void ShowVariables()
    {
        variablesText.text = "";  // 텍스트 초기화

        // 화면에 표시하고 싶은 변수명 추가
        List<string> keysToShow = new List<string>(new string[]
        {
         //   "FateBirthday"
         // "isTutorial",
         // "TutorialPhase"
         "ActionPoint"
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
    
    // ************************* temporary methods for action points *************************
    // create 5 hearts on screen on room start
    public void CreateHearts()
    {
        int actionPoint = (int)GetVariable("ActionPoint");
        // 25 action points -> 5 hearts, 24 action points -> 4 hearts, so on...
        int heartCount = actionPoint % actionPointsPerDay;
        if (heartCount == 0) heartCount = actionPointsPerDay;
        for (int i = 0; i < heartCount; i++) 
        {
            // create heart on screen by creating instances of heart prefab under heart parent
            Instantiate(heartPrefab, heartParent.transform);
        }
        
        // change Day text on screen
        dayText.text = $"Day {actionPointsPerDay - (actionPoint - 1) / actionPointsPerDay}";
    }
    
    public void DecrementActionPoint()
    {
        if ((bool)GetVariable("TeddyBearFixed")) actionPointsPerDay = 7;

        DecrementVariable("ActionPoint");
        int actionPoint = (int)GetVariable("ActionPoint");
        // pop heart on screen
        GameObject heart = heartParent.transform.GetChild(actionPoint % actionPointsPerDay).gameObject;
        // animate heart by triggering "break" animation
        heart.GetComponent<Animator>().SetTrigger("Break");
        
        // deactivate heart after animation
        StartCoroutine(DeactivateHeart(heart));
        
        if (actionPoint % actionPointsPerDay == 0)
        {
            bool isDialogueActive = DialogueManager.Instance.isDialogueActive;
            if (!isDialogueActive) RefillHeartsOrEndDay();
            else SetVariable("RefillHeartsOrEndDay", true);
        }
    }
    
    public void RefillHeartsOrEndDay()
    {
        // if all action points are used, load "Follow 1" scene
        int actionPoint = (int)GetVariable("ActionPoint");
        if (actionPoint == 0)
        {
            SceneManager.Instance.LoadScene(Constants.SceneType.ENDING);
            return;
        }

        const float totalTime = 3f;
        StartCoroutine(ScreenEffect.Instance.DayPass(totalTime));  // fade in/out effect

        var random = new Random((uint)System.DateTime.Now.Ticks);  // choose a random dialogue ID
        string[] randomDialogueIDs = { "RoomEscapeS_001", "RoomEscapeS_003" };
        var randomDialogueID = randomDialogueIDs[random.NextInt(0, 2)];
        StartCoroutine(DialogueManager.Instance.StartDialogue(randomDialogueID, totalTime));

        StartCoroutine(RefillHearts(totalTime/2));
        SetVariable("RefillHeartsOrEndDay", false);
    } 
    
    private static IEnumerator DeactivateHeart(Object heart)
    {
        yield return new WaitForSeconds(0.5f);
        Destroy(heart);
    }
    
    private IEnumerator RefillHearts(float totalTime)
    {
        yield return new WaitForSeconds(totalTime);
        CreateHearts();
        // turn off all ImageAndLockPanel objects and zoom out
        RoomManager.Instance.ExitToRoot();
    }
    
}