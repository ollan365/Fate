using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultManager : MonoBehaviour
{
    public TextAsset resultsCSV;
    
    // results: dictionary of "Results"s indexed by string "Result ID"
    public Dictionary<string, Result> results = new Dictionary<string, Result>();

    public DisplayLog displayLog;
    public FlashImage flashImage;
    
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
    
    public void ExcuteResult(string resultID)
    {
        switch (resultID)
        {
            case "r1":
                displayLog.DisplayLogOnScreen("아무 소리도 들리지 않는다");
                break;
            case "r2":
                displayLog.DisplayLogOnScreen("낯선 자: 열쇠가 필요해");
                break;
            case "r3":
                flashImage.ImageFlash();
                break;
            case "r4":
                displayLog.DisplayLogOnScreen("금고가 열렸다");
                break;
        }
    }
}
