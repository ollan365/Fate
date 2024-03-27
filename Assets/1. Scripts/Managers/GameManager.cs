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
        SetVariable("ActionPoint", 25);
        
        // 2. 이벤트 오브젝트 관련 변수들
        // 침대 위 곰인형
        SetVariable("DollsClick", 0);
        
        // 이불
        SetVariable("BlanketsClick", 0);
        
        // 술과 감기약
        SetVariable("LiquorAndPillsClick", 0);
        
        // 스탠드
        SetVariable("LampClick", 0);
        
        // 인형 수납장
        SetVariable("ShelvesClick", 0);
        
        // 배게
        SetVariable("PillowClick", 0);
        
        // 다이어리
        SetVariable("DiaryClick", 0);
        SetVariable("DiaryPasswordCorrect", false);
        SetVariable("DiaryPassword", "0410");
        
        // 시계
        SetVariable("ClockClick", 0);
        SetVariable("ClockTimeCorrect", false);
        float[] clockPassword = { 210f, 180f };
        SetVariable("ClockPassword", clockPassword);
        
        // 의자
        SetVariable("ChairMoved", false);
        
        // 노트북
        SetVariable("LaptopClick", 0);
        SetVariable("LaptopPasswordCorrect", false);
        SetVariable("LaptopPassword", "0410");
        
        // 카펫
        SetVariable("CarpetClick", 0);

        // 카펫 아래 종이
        SetVariable("PaperClick", 0);

        // 포스터
        SetVariable("PosterClick", 0);
        SetVariable("PosterCorrect", false);

        // 커터칼
        SetVariable("KnifeClick", 0);

    }
    
    public void SetVariable(string variableName, object value)
    {
        variables[variableName] = value;
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