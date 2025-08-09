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
        {"events", "202180038"},
        {"conditions", "1563155608"},
        {"results", "445642289"},
        {"dialogues", "1768256627"},
        {"choices", "1943406759"},
        {"scripts", "888011379"},
        {"image paths", "1449854056"},
        {"memos", "1578497812"},
        {"diary", "1694526261"},
        {"backgrounds", "1143880704"},
        {"laptop_chat", "801842851"},
        {"laptop_SNS", "1372899183"},
        {"variables", "762639181"}
    };

    [MenuItem("Tools/Download CSVs")]
    public static void DownloadCSVs()
    {
        foreach (var sheet in sheetIds)
        {
            string url = $"https://docs.google.com/spreadsheets/d/1NHI0e1GgRrxN6Y73YEjZELX_az2mdZl6VuVrVqbYbJI/export?format=csv&id=1NHI0e1GgRrxN6Y73YEjZELX_az2mdZl6VuVrVqbYbJI&gid={sheet.Value}";
            string filePath = Path.Combine(Application.dataPath, "Resources/Datas", $"{sheet.Key}.csv");

            using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
            {
                var asyncOperation = webRequest.SendWebRequest();
                while (!asyncOperation.isDone)
                    System.Threading.Thread.Sleep(100); // 동기적 대기

                if (webRequest.result == UnityWebRequest.Result.Success)
                {
                    File.WriteAllText(filePath, webRequest.downloadHandler.text);
                    Debug.Log($"Successfully downloaded and saved: {sheet.Key}.csv");
                }
                else
                    Debug.LogError($"Failed to download {sheet.Key}.csv: {webRequest.error}");
            }
        }
        AssetDatabase.Refresh();
    }
}
#endif