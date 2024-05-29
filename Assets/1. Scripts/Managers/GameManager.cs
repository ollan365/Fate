using System.Collections.Generic;
using TMPro;
using UnityEditor.UI;
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

    // 행동력 변수 변경될 때 호출될 이벤트
    public delegate void ActionPointChangedEventHandler(int newActionPointValue);
    public static event ActionPointChangedEventHandler OnActionPointChanged;

    // 조사시스템 테스트
    private string currentInquiryObjectId = "";

    public void setCurrentInquiryObjectId(string objectId)
    {
        currentInquiryObjectId = objectId;
    }

    public string getCurrentInquiryObjectId()
    {
        if (currentInquiryObjectId == null)
        {
            Debug.Log("currentInquiryObjectId is NULL!");
            return null;
        }
        else
        {
            return currentInquiryObjectId;
        }
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
        
        variables["AccidyGender"] = 0;  // 우연 성별 {0: 여자, 1: 남자}
        variables["AccidyBirthday"] = "0616";  // 우연 생일
        
        variables["ActionPoint"] = 25;  // 행동력 

        variables["isInquiry"] = false; // 조사 시스템에서 예 누르면 true되고 계속 조사 가능.

        variables["currentSideIndex"] = 0; // 방탈출 현재 사이드 번호

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


        // 반짇고리
        variables["SewingBoxClick"] = 0;
        variables["SewingBoxCorrect"] = false;


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
        // 클럽-
        variables["ClubClick2"] = 0;

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

        // 행동력 감소 시 행동력 감소함을 이벤트로 알림
        if (variableName == "ActionPoint")
            OnActionPointChanged?.Invoke(cnt);

        if (isDebug) ShowVariables();
    }

    public void DecrementVariable(string variableName, int count)
    {
        int cnt = (int)GetVariable(variableName);
        cnt -= count;
        SetVariable(variableName, cnt);

        // 행동력 감소 시 행동력 감소함을 이벤트로 알림
        if (variableName == "ActionPoint")
            OnActionPointChanged?.Invoke(cnt);

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

        bool isBusy = isDialogueActive || isInvestigating || isTutorialPhase1;

        return isBusy;
    }
}