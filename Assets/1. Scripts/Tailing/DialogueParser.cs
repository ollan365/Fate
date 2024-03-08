using System.Collections.Generic;
using UnityEngine;
using static Constants;
public class DialgoueParser : MonoBehaviour
{
    [SerializeField] private Player player;

    private Dictionary<string, string> localiziations;
    private Dictionary<string, Choice> choiceEvent;
    public void GetExeclData()
    {
        GetLocalization();
        GetChoiceEvent();
    }
    private void GetLocalization()
    {
        TextAsset csvData = Resources.Load<TextAsset>(File.Localization.ToString());

        string[] data = csvData.text.Split(new char[] { '\n' });

        for (int i = 1; i < data.Length; i++)
        {
            string[] row = data[i].Split(new char[] { ',' });
            localiziations.Add(row[0], row[player.Language + 1]);
        }
    }
    private void GetChoiceEvent()
    {
        TextAsset csvData = Resources.Load<TextAsset>(File.ChoiceEvent.ToString());

        string[] data = csvData.text.Split(new char[] { '\n' });

        for (int i = 1; i < data.Length; i++)
        {
            string[] row = data[i].Split(new char[] { ',' });

            Choice choice = new Choice();
            choice.eventID = row[0];
            choice.choice_A = row[2];
            choice.choice_B = row[3];
            choice.result_A = row[4];
            choice.result_B = row[5];

            choiceEvent.Add(choice.eventID, choice);
        }
    }
    public Dialogue[] Parse(string _CSVFilieName)
    {
        List<Dialogue> dialgoueList = new List<Dialogue>(); // 대사 리스트 생성
        TextAsset csvData = Resources.Load<TextAsset>(_CSVFilieName); // CSV 데이터를 받기 위한 그릇 

        string[] data = csvData.text.Split(new char[] { '\n' }); //엔터를 만나면 쪼개어 넣음
        //엔터를 만났다 data[0] - 엑셀시트의 맨 1번째 줄 의미 

        for (int i = 1; i < data.Length;) // i++는 대한 내용은 그다음 내용은 조건문을 통해서 
        {
            string[] row = data[i].Split(new char[] { ',' }); //, 단위로 row 줄에 저장

            Dialogue dialogue = new Dialogue(); // 대사 리스트 생성
            dialogue.id = row[0];
            dialogue.characterName = row[1];
            dialogue.localizations = row[2];
            dialogue.eventID = row[3]; // 이벤트 넘버 생성
            dialogue.skipLine = row[4]; // 엑셀 맨끝줄 비고 추가 안하면 오류남

            dialgoueList.Add(dialogue);
        }

        return dialgoueList.ToArray();
    }

    public string Dialogue(string key)
    {
        string value;
        if (localiziations.TryGetValue(key, out value)) return value;
        else return null;
    }
    public Choice GetChoiceEvent(string key)
    {
        Choice value;
        if (choiceEvent.TryGetValue(key, out value)) return value;
        else return null;
    }
}