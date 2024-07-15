using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DiaryManager : PageContentsManager
{
    private void Awake()
    {
        ParsePageContents();
    }

    public override void DisplayPage(PageType pageType, int pageNum)
    {
        string diaryID = $"Diary_{pageNum.ToString().PadLeft(3, '0')}";

        switch (pageType)
        {
            case PageType.Left:
                leftPage.text = pageNum == 0 ? "" : PagesDictionary[diaryID];
                break;
            
            case PageType.Right:
                rightPage.text = pageNum > PagesDictionary.Count ? "" : PagesDictionary[diaryID];
                break;
            
            case PageType.Back:
                backPage.text = PagesDictionary[diaryID];
                break;
            
            case PageType.Front:
                frontPage.text = PagesDictionary[diaryID];
                break;
        }
    }

    public override void DisplayPagesDynamic(int currentPage)
    {
        DisplayPage(PageType.Left, currentPage);
        DisplayPage(PageType.Right, currentPage + 3);
        DisplayPage(PageType.Back, currentPage + 1);
        DisplayPage(PageType.Front, currentPage + 2);
    }
    
    public override void DisplayPagesStatic(int currentPage)
    {
        DisplayPage(PageType.Left, currentPage);
        DisplayPage(PageType.Right, currentPage + 1);
        
        flipLeftButton.SetActive(currentPage > 0);
        flipRightButton.SetActive(currentPage < PagesDictionary.Count - 1);
    }

    public override void ParsePageContents()
    {
        TextAsset diaryCsv = Resources.Load<TextAsset>("Datas/diary");
        string[] lines = diaryCsv.text.Split('\n');

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
            if (PagesDictionary.ContainsKey(diaryPageID)) PagesDictionary[diaryPageID] += "\n\n" + script;
            else PagesDictionary.Add(diaryPageID, script);
        }
    }
}