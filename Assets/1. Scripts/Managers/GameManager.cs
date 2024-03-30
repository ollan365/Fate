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

    // public TextMeshProUGUI variableValueText;

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
        
        // 2. 이벤트 오브젝트 관련 변수들
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
        
        float[] clockPassword = { 210f, 180f };
        variables["clockPassword"] = clockPassword;
        // 위의 상태로는 이상하게도 (float[])GameManager.Instance.GetVariable("ClockPassword")를 가져오면 빈 값으로 나옴.

        //Debug.Log("시계정답 : " + (float[])GameManager.Instance.GetVariable("ClockPassword"));
        //Debug.Log("시계정답 : " + clockPassword[0]+ " "+ clockPassword[1]);

        // 의자
        variables["ChairMoved1"] = false;
        variables["ChairMoved2"] = false;

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
        variables["PosterCorrect"] = false;

        // 커터칼
        variables["KnifeClick"] = 0;

        // 사이드 2번 
        // 옷장
        variables["ClosetOffClick"] = 0;
        variables["ClosetDoor"] = false;

        // 옷장 속 가방
        variables["BagClick"] = 0;
        variables["BagClue"] = false;

        // 옷장 위 상자
        variables["BoxClick"] = 0;
        variables["BoxCorrect"] = false;

        // 서랍장
        variables["CabinetOffClick"] = 0;
        variables["CabinetDoor"] = false;

        // 서랍장 속 달력
        variables["CalendarClick"] = 0;
        variables["CalendarClue"] = 0;
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
        // Debug.Log("variable: \"" + variableName + "\" does not exist!");
        return null;
    }

    public void IncrementVariable(string variableName)
    {
        int cnt = (int)GetVariable(variableName);
        cnt++;
        SetVariable(variableName, cnt);
    }
    
    public void DecrementVariable(string variableName)
    {
        int cnt = (int)GetVariable(variableName);
        cnt--;
        SetVariable(variableName, cnt);
    }

    public void InverseVariable(string variableName)
    {
        bool variableValue= (bool)GetVariable(variableName);
        variableValue = !variableValue;
        SetVariable(variableName, variableValue);
    }

    //void UpdateUI()
    //{
    //    variableValueText.text = "PhoneCalled: " + PhoneCalled + "\n" + "HasKey: " + HasKey;
    //}
}