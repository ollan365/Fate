public class Script
{
    public string ScriptID { get; private set; }
    public string EngScript { get; private set; }
    public string KorScript { get; private set; }
    public string JP_M_Script { get; private set; }
    public string JP_W_Script { get; private set; }
    public string Placeholder { get; private set; }
    
    public class ScriptPlaceholderResult
    {
        public string ProcessedText;
        public bool Auto;
        public bool Fast;
        public bool Multi;
        public bool Ending;
    }

    public Script(string id, string eng, string kor, string jp_m, string jp_w, string placeholder)
    {
        ScriptID = id;
        EngScript = eng;
        KorScript = kor;
        JP_M_Script = jp_m;
        JP_W_Script = jp_w;
        Placeholder = placeholder;
    }
    
    public string GetScript()
    {
        return GetScriptByLanguageIndex((int)GameManager.Instance.GetVariable("Language"));
    }

    public string GetScriptByLanguageIndex(int languageIndex)
    {
        string localizedText = EngScript;
        switch (languageIndex) {
            case 1:
                localizedText = KorScript;
                break;
        }
        return GetFormattedText(localizedText);
    }
    
    public ScriptPlaceholderResult GetProcessedScript()
    {
        string sentence = GetScript();
        return ProcessPlaceholders(sentence);
    }
    
    public ScriptPlaceholderResult ProcessPlaceholders(string baseText)
    {
        var result = new ScriptPlaceholderResult
        {
            ProcessedText = baseText,
            Auto = false,
            Fast = false,
            Multi = false,
            Ending = false
        };
        
        if (!string.IsNullOrEmpty(Placeholder))
        {
            var effects = Placeholder.Split('/');
            foreach (var effect in effects)
            {
                switch (effect)
                {
                    case "RED":
                        result.ProcessedText = $"<color=red>{result.ProcessedText}</color>";
                        break;
                    case "AUTO":
                        result.Auto = true;
                        break;
                    case "FAST":
                        result.Fast = true;
                        break;
                    case "TRUE":
                        var fateName = (string)GameManager.Instance.GetVariable("FateName");
                        result.ProcessedText = result.ProcessedText.Replace("{PlayerName}", fateName);
                        break;
                    case "ENDING":
                        result.Ending = true;
                        break;
                    case "MULTI":
                        result.Multi = true;
                        break;
                }
            }
        }
        return result;
    }
    private string GetFormattedText(string text)
    {
        string newText = text;
        bool textEffect = false;
        int startIndex = -1;
        for (int i = 0; i < text.Length; i++)
        {
            if (text[i] == '[') {
                textEffect = true;
                startIndex = i;
            }

            if (!textEffect || text[i] != ']') 
                continue;
            
            int endIndex = i;
            newText = text.Substring(0, startIndex);
            newText += "<color=red>";
            newText += text.Substring(startIndex + 1, endIndex - startIndex - 1);
            newText += "</color>";
            newText += text.Substring(endIndex + 1);

            text = newText;
        }
        return newText;
    }
}