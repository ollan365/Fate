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

    // 디버깅용
    [SerializeField] private TextMeshProUGUI variablesText;
    public bool isDebug = false;

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
        variables["FateBirthday"] = 0;  // 필연 생일
        
        variables["AccidyGender"] = 0;  // 우연 성별 {0: 여자, 1: 남자}
        
        variables["ActionPoint"] = 25;  // 행동력 
        
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
        variables["DiaryPassword"] = "0410";

        // 시계
        variables["ClockClick"] = 0;
        variables["ClockTimeCorrect"] = false;
        
        float[] clockPassword = { 180f, 180f };
        variables["ClockPassword"] = clockPassword;

        // 의자
        variables["ChairMoved"] = false;

        // 노트북
        variables["LaptopClick"] = 0;
        variables["LaptopPasswordCorrect"] = false;
        variables["LaptopPassword"] = "0410";

        // 카펫
        variables["CarpetClick"] = 0;

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
        variables["CalendarClick"] = 0;
        variables["CalendarCluesFound"] = 0;
        variables["CalendarMonth"] = false;

        // 달력
        // 필연 생일
        variables["FBDClick"] = 0;
        // 우연 생일
        variables["ABDClick"] = 0;
        // 10월 1일 클릭
        variables["_1001Click"] = 0;
        // 10월 31일 클릭
        variables["_1031Click"] = 0;

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
        // Debug.Log("variable: \"" + variableName + "\" does not exist!");
        return null;
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
            "ActionPoint",
        });
        
        foreach (var item in variables)
        {
            if (keysToShow.Contains(item.Key)) variablesText.text += $"{item.Key}: {item.Value}\n";
        }
    }
}