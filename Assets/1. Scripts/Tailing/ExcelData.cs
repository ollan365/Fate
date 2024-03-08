using UnityEngine;

[System.Serializable]
public class Dialogue
{
    [Tooltip("아이디")]
    public string id;

    [Tooltip("캐릭터 이름")]
    public string characterName;

    [Tooltip("대사 내용")]
    public string localizations;

    [Tooltip("이벤트 번호")]
    public string eventID;

    [Tooltip("스킵라인")]
    public string skipLine;

}

[System.Serializable]
public class Choice
{
    [Tooltip("이벤트 번호")]
    public string eventID;

    [Header("선택지")]
    public string choice_A;
    public string choice_B;

    [Header("결과")]
    public string result_A;
    public string result_B;
}