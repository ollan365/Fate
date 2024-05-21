using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum PageType
{
    Left,
    Right,
    Back,
    Front
}

public class DiaryManager : MonoBehaviour
{
    public static MemoManager Instance { get; private set; }
    private TextAsset diaryPagesCSV;

    private Dictionary<string, string> diaryPages = new Dictionary<string, string>();
    [SerializeField] private TextMeshProUGUI leftPage;
    [SerializeField] private TextMeshProUGUI rightPage;
    [SerializeField] private TextMeshProUGUI backPage;
    [SerializeField] private TextMeshProUGUI frontPage;

    private void Awake()
    {
        diaryPagesCSV = Resources.Load<TextAsset>("Datas/diary");
        ParseDiaryPages();
    }

    private void DisplayPage(PageType pageType, int pageNum)
    {
        string diaryID = $"Diary_{pageNum.ToString().PadLeft(3, '0')}";

        switch (pageType)
        {
            case PageType.Left:
                leftPage.text = pageNum == 0 ? "" : diaryPages[diaryID];
                break;
            
            case PageType.Right:
                rightPage.text = pageNum > diaryPages.Count ? "" : diaryPages[diaryID];
                break;
            
            case PageType.Back:
                backPage.text = diaryPages[diaryID];
                break;
            
            case PageType.Front:
                frontPage.text = diaryPages[diaryID];
                break;
        }
    }

    public void DisplayPagesDynamic(int currentPage)
    {
        DisplayPage(PageType.Left, currentPage);
        DisplayPage(PageType.Right, currentPage + 3);
        DisplayPage(PageType.Back, currentPage + 1);
        DisplayPage(PageType.Front, currentPage + 2);
    }
    
    public void DisplayPagesStatic(int currentPage)
    {
        DisplayPage(PageType.Left, currentPage);
        DisplayPage(PageType.Right, currentPage + 1);
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