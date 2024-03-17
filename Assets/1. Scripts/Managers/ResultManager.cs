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
        }
    }
}
