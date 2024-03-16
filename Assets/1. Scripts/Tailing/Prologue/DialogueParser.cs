// using System.Collections.Generic;
// using UnityEngine;
// using static Constants;
// public class DialgoueParser : MonoBehaviour
// {
//     [SerializeField] private Player player;
//
//     private Dictionary<string, string> localiziations;
//     private Dictionary<string, Choice> choiceEvent;
//     private void Start()
//     {
//         localiziations = new();
//         choiceEvent = new();
//     }
//     public void GetExeclData()
//     {
//         GetLocalization();
//         GetChoiceEvent();
//     }
//     private void GetLocalization()
//     {
//         TextAsset csvData = Resources.Load<TextAsset>(File.Localization.ToString());
//
//         string[] data = csvData.text.Split(new char[] { '\n' });
//
//         for (int i = 1; i < data.Length; i++)
//         {
//             string[] row = data[i].Split(new char[] { ',' });
//
//             if (!localiziations.ContainsKey(row[0]))
//                 localiziations.Add(row[0], row[player.Language + 1]);
//         }
//     }
//     private void GetChoiceEvent()
//     {
//         TextAsset csvData = Resources.Load<TextAsset>(File.ChoiceEvent.ToString());
//
//         string[] data = csvData.text.Split(new char[] { '\n' });
//
//         for (int i = 1; i < data.Length; i++)
//         {
//             string[] row = data[i].Split(new char[] { ',' });
//
//             Choice choice = new Choice();
//             choice.eventID = row[0];
//
//             if (row[0] == "" || choiceEvent.ContainsKey(row[0])) continue;
//
//             choice.choice_A = row[2];
//             choice.choice_B = row[3];
//             choice.result_A = row[4];
//             choice.result_B = row[5];
//
//             choiceEvent.Add(choice.eventID, choice);
//         }
//     }
//     public Dialogue[] Parse(string _CSVFilieName)
//     {
//         List<Dialogue> dialgoueList = new List<Dialogue>(); // ��� ����Ʈ ����
//         TextAsset csvData = Resources.Load<TextAsset>(_CSVFilieName); // CSV �����͸� �ޱ� ���� �׸� 
//         
//         string[] data = csvData.text.Split(new char[] { '\n' }); //���͸� ������ �ɰ��� ����
//         //���͸� ������ data[0] - ������Ʈ�� �� 1��° �� �ǹ� 
//
//         for (int i = 1; i < data.Length; i++) // i++�� ���� ������ �״��� ������ ���ǹ��� ���ؼ� 
//         {
//             string[] row = data[i].Split(new char[] { ',' }); //, ������ row �ٿ� ����
//             if (row.Length < 3 || row[2] == "") break;
//
//             Dialogue dialogue = new Dialogue(); // ��� ����Ʈ ����
//             dialogue.id = row[0];
//             dialogue.characterName = row[1];
//             dialogue.localizations = row[2];
//
//             dialogue.eventID = row[3]; // �̺�Ʈ �ѹ� ����
//             dialogue.skipLine = row[4]; // ���� �ǳ��� ��� �߰� ���ϸ� ������
//
//             dialgoueList.Add(dialogue);
//         }
//
//         return dialgoueList.ToArray();
//     }
//
//     public string Dialogue(string key)
//     {
//         string value;
//         if (localiziations.TryGetValue(key, out value)) return value;
//         else return null;
//     }
//     public Choice GetChoiceEvent(string key)
//     {
//         Choice value;
//         if (choiceEvent.TryGetValue(key, out value)) return value;
//         else return null;
//     }
// }