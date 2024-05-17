using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DiaryManager : MonoBehaviour
{
    public static MemoManager Instance { get; private set; }
    private TextAsset diaryPagesCSV;

    private Dictionary<string, string> diaryPages = new Dictionary<string, string>();
    [SerializeField] private TextMeshProUGUI rightPage;

    private int currentPage;

    private void Awake()
    {
        diaryPagesCSV = Resources.Load<TextAsset>("Datas/diary");
        ParseDiaryPages();

        currentPage = 1;
        DisplayPage();
    }

    private void DisplayPage()
    {
        string diaryID = $"Diary_{currentPage.ToString().PadLeft(3, '0')}";
        rightPage.text = diaryPages[diaryID];
    }

    public void ParseDiaryPages()
    {
        string[] lines = diaryPagesCSV.text.Split('\n');

        string previousDiaryPageID = "";
        
        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;

            string[] fields = lines[i].Split(',');

            string diaryPageID = fields[0].Trim();
            if (diaryPageID == "") diaryPageID = previousDiaryPageID;
            else previousDiaryPageID = diaryPageID;
            
            string scriptID = fields[2].Trim();
            string script = DialogueManager.Instance.scripts[scriptID].GetScript();
            if (diaryPages.ContainsKey(diaryPageID)) diaryPages[diaryPageID] += "\n\n" + script;
            else diaryPages.Add(diaryPageID, script);
        }
    }
}