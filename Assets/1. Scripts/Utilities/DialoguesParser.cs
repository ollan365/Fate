using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialoguesParser : MonoBehaviour
{
    // CSV 파일
    private TextAsset dialoguesCSV = Resources.Load<TextAsset>("Datas/dialogues");
    private TextAsset scriptsCSV = Resources.Load<TextAsset>("Datas/scripts");
    private TextAsset choicesCSV = Resources.Load<TextAsset>("Datas/choices");
    private TextAsset imagePathsCSV = Resources.Load<TextAsset>("Datas/image paths");
    
    // 자료 구조
    private Dictionary<string, Dialogue> dialogues = new Dictionary<string, Dialogue>();
    private Dictionary<string, Script> scripts = new Dictionary<string, Script>();
    private Dictionary<string, Choice> choices = new Dictionary<string, Choice>();
    private Dictionary<string, ImagePath> imagePaths = new Dictionary<string, ImagePath>();

    private string Escaper(string originalString)
    {
        string modifiedString = originalString.Replace("\\n", "\n");
        modifiedString = modifiedString.Replace("`", ",");
        modifiedString = modifiedString.Replace("", "");

        return modifiedString;
    }
    
    public Dictionary<string, Dialogue> ParseDialogues()
    {
        string[] lines = dialoguesCSV.text.Split('\n');

        string lastDialogueID = "";

        for (int i = 1; i < lines.Length; i++)
        {
            string[] fields = lines[i].Split(',');

            if ((string.IsNullOrWhiteSpace(lines[i])) || (fields[0] == "" && fields[1] == "")) continue;

            string dialogueID = fields[0].Trim();
            if (string.IsNullOrWhiteSpace(dialogueID)) dialogueID = lastDialogueID;
            else lastDialogueID = dialogueID;

            string speakerID = fields[1].Trim();
            string scriptID = fields[2].Trim();
            string imageID = fields[3].Trim();
            string eventID = fields[4].Trim();
            string next = fields[6].Trim();

            if (!dialogues.ContainsKey(dialogueID))
            {
                Dialogue dialogue = new Dialogue(dialogueID);
                dialogues[dialogueID] = dialogue;
            }

            dialogues[dialogueID].AddLine(speakerID, scriptID, imageID, eventID, next);
        }

        return dialogues;
    }

    public Dictionary<string, Script> ParseScripts()
    {
        string[] lines = scriptsCSV.text.Split("EOL");

        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;

            string[] fields = lines[i].Split(',');

            string scriptID = fields[0].Trim().Trim('\n');
            string engScript = Escaper(fields[1].Trim());
            string korScript = Escaper(fields[2].Trim());
            string jpMScript = Escaper(fields[3].Trim());
            string jpWScript = Escaper(fields[4].Trim());
            string placeholder = Escaper(fields[5].Trim());

            Script script = new Script(
                scriptID,
                engScript,
                korScript,
                jpMScript,
                jpWScript,
                placeholder
            );
            scripts[scriptID] = script;
        }

        return scripts;
    }

    public Dictionary<string, Choice> ParseChoices()
    {
        string[] lines = choicesCSV.text.Split('\n');

        string lastChoiceID = "";
        
        for (int i = 1; i < lines.Length; i++)
        {
            if(string.IsNullOrWhiteSpace(lines[i])) continue;

            string[] fields = lines[i].Split(',');

            string choiceID = fields[0].Trim();
            if (string.IsNullOrWhiteSpace(choiceID)) choiceID = lastChoiceID;
            else lastChoiceID = choiceID;
            
            string scriptID = fields[1].Trim();
            string next = fields[2].Trim();

            if (!choices.ContainsKey(choiceID))
            {
                choices[choiceID] = new Choice(choiceID);
            }
            
            choices[choiceID].AddLine(scriptID, next);
        }

        return choices;
    }

    public Dictionary<string, ImagePath> ParseImagePaths()
    {
        string[] lines = imagePathsCSV.text.Split('\n');

        for (int i = 1; i < lines.Length; i++)
        {
            if(string.IsNullOrWhiteSpace(lines[i])) continue;

            string[] fields = lines[i].Split(',');

            string imageID = fields[0].Trim();
            string girlPath = fields[1].Trim();
            string boyPath = fields[2].Trim();
            girlPath = $"Characters/{girlPath}";
            boyPath = $"Characters/{boyPath}";
            ImagePath imagePath = new ImagePath(
                imageID,
                girlPath,
                boyPath
            );
            imagePaths[imageID] = imagePath;
        }

        return imagePaths;
    }
    
}
