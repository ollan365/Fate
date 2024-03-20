using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultManager : MonoBehaviour
{
    public TextAsset resultsCSV;
    
    // results: dictionary of "Results"s indexed by string "Result ID"
    public Dictionary<string, Result> results = new Dictionary<string, Result>();

    public void ParseResults()
    {
        string[] lines = resultsCSV.text.Split('\n');
        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;

            string[] fields = lines[i].Split(',');
            
            Result result = new Result(
                fields[0].Trim(),
                fields[1].Trim(),
                fields[2].Trim()
                );
            
            results[result.ResultID] = result;
        }
    }
    
    public void ExecuteResult(string resultID)
    {
        switch (resultID)
        {
            case "Result_001":
                break;

            case "Result_002":
                break;

            case "Result_003":
                Debug.Log(results[resultID].Description);
                break;

            case "Result_004":
                Debug.Log(results[resultID].Description);
                DialogueManager.Instance.StartDialogue("RoomEscape_008");
                break;

            case "Result_005":
                Debug.Log(results[resultID].Description);
                DialogueManager.Instance.StartDialogue("RoomEscape_001");
                break;

            case "Result_006":
                Debug.Log(results[resultID].Description);
                DialogueManager.Instance.StartDialogue("RoomEscape_002");
                break;

            case "Result_007":
                Debug.Log(results[resultID].Description);
                DialogueManager.Instance.StartDialogue("RoomEscape_003");
                break;

            case "Result_008":
                Debug.Log(results[resultID].Description);
                DialogueManager.Instance.StartDialogue("RoomEscape_004");
                break;

            case "Result_009":
                Debug.Log(results[resultID].Description);
                DialogueManager.Instance.StartDialogue("RoomEscape_005");
                break;

            case "Result_010":
                Debug.Log(results[resultID].Description);
                DialogueManager.Instance.StartDialogue("RoomEscape_006");
                break;

            case "Result_011":
                Debug.Log(results[resultID].Description);
                DialogueManager.Instance.StartDialogue("RoomEscape_007");
                break;

            case "Result_012":
                Debug.Log(results[resultID].Description);
                break;

            case "Result_013":
                Debug.Log(results[resultID].Description);
                DialogueManager.Instance.StartDialogue("RoomEscape_009");
                break;

            case "Result_014":
                Debug.Log(results[resultID].Description);
                break;

            case "Result_015":
                Debug.Log(results[resultID].Description);
                break;

            case "Result_016":
                Debug.Log(results[resultID].Description);
                break;

            case "Result_017":
                Debug.Log(results[resultID].Description);
                DialogueManager.Instance.StartDialogue("RoomEscape_010");
                break;

            case "Result_018":
                Debug.Log(results[resultID].Description);
                DialogueManager.Instance.StartDialogue("RoomEscape_011");
                break;

            case "Result_019":
                Debug.Log(results[resultID].Description);
                DialogueManager.Instance.StartDialogue("RoomEscape_012");
                break;

            case "Result_020":
                Debug.Log(results[resultID].Description);
                break;

            case "Result_021":
                Debug.Log(results[resultID].Description);
                break;

            case "Result_022":
                Debug.Log(results[resultID].Description);
                DialogueManager.Instance.StartDialogue("RoomEscape_013");
                break;

            case "Result_023":
                Debug.Log(results[resultID].Description);
                break;

            case "Result_024":
                Debug.Log(results[resultID].Description); 
                break;

            case "Result_025":
                Debug.Log(results[resultID].Description);
                DialogueManager.Instance.StartDialogue("RoomEscape_014");
                break;

            case "Result_026":
                Debug.Log(results[resultID].Description);
                break;

            case "Result_027":
                Debug.Log(results[resultID].Description);
                break;

            case "Result_028":
                Debug.Log(results[resultID].Description);
                break;

            case "Result_029":
                Debug.Log(results[resultID].Description);
                DialogueManager.Instance.StartDialogue("RoomEscape_015");
                break;

            case "Result_030":
                Debug.Log(results[resultID].Description);
                DialogueManager.Instance.StartDialogue("RoomEscape_016");
                break;

            case "Result_031":
                Debug.Log(results[resultID].Description);
                break;

            case "Result_032":
                Debug.Log(results[resultID].Description);
                break;

            case "Result_033":
                Debug.Log(results[resultID].Description);
                DialogueManager.Instance.StartDialogue("RoomEscape_017");
                break;

            case "Result_034":
                Debug.Log(results[resultID].Description);
                DialogueManager.Instance.StartDialogue("RoomEscape_018");
                break;
        }
    }
}
