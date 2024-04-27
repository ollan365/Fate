#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.IO;

public class CSVDownloaderEditor : EditorWindow
{
    private static readonly Dictionary<string, string> sheetIds = new Dictionary<string, string>
    {
        {"events", "2115772135"},
        {"Conditions", "1563155608"},
        {"results", "1535296605"},
        {"dialogues", "1768256627"},
        {"choices", "1943406759"},
        {"scripts", "888011379"},
        {"image paths", "1449854056"},
        {"memos", "1578497812"}
    };

    [MenuItem("Tools/Download CSVs")]
    public static void DownloadCSVs()
    {
        foreach (var sheet in sheetIds)
        {
            string url = $"https://docs.google.com/spreadsheets/u/0/d/1HKmv7JCwqAQGGxA361nO0V_E8HfC5RkxSU-OMMx127Y/export?format=csv&id=1HKmv7JCwqAQGGxA361nO0V_E8HfC5RkxSU-OMMx127Y&gid={sheet.Value}";
            string filePath = Path.Combine(Application.dataPath, "Resources/Datas", $"{sheet.Key}.csv");

            using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
            {
                var asyncOperation = webRequest.SendWebRequest();

                // 동기적 대기
                while (!asyncOperation.isDone)
                {
                    System.Threading.Thread.Sleep(100);
                }

                if (webRequest.result == UnityWebRequest.Result.Success)
                {
                    File.WriteAllText(filePath, webRequest.downloadHandler.text);
                    Debug.Log($"Successfully downloaded and saved: {sheet.Key}.csv");
                }
                else
                {
                    Debug.LogError($"Failed to download {sheet.Key}.csv: {webRequest.error}");
                }
            }
        }

        AssetDatabase.Refresh();
    }
}
#endif