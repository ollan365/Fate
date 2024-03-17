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

    // 변수 초기값 설정
    //int PhoneCalled = 0;
    //bool HasKey = false;

    // public TextMeshProUGUI variableValueText;

    //// 다이어리
    //public bool DiaryPasswordCorrect = false;
    //public int DiaryClick = 0;

    void Start()
    {
        // variables에 변수 추가
        // 다이어리
        SetVariable("DiaryPasswordCorrect", false);
        SetVariable("DiaryClick", 0);
        // 노트북
        SetVariable("LaptopPasswordCorrect", false);
        SetVariable("LaptopClick", 0);
        // 시계
        SetVariable("ClockTimeCorrect", false);
        SetVariable("ClockClick", 0);

        // UpdateUI();
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

    public void IncrementObjectClick(string variableName)
    {
        int cnt = (int)GetVariable(variableName);
        cnt++;
        SetVariable(variableName, cnt);
    }




    //public void IncrementPhoneCalled()
    //{
    //    PhoneCalled++;
    //    SetVariable("PhoneCalled", PhoneCalled);
    //    UpdateUI();
    //}

    //public void InverseHasKey()
    //{
    //    HasKey = !HasKey;
    //    SetVariable("HasKey", HasKey);
    //    UpdateUI();
    //}

    //void UpdateUI()
    //{
    //    variableValueText.text = "PhoneCalled: " + PhoneCalled + "\n" + "HasKey: " + HasKey;
    //}
}