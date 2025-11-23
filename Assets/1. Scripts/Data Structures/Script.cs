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
    
    public ScriptPlaceholderResult GetScript(int languageIndex=-1)
    {
        if (languageIndex == -1)
            languageIndex = LocalizationManager.Instance != null ? LocalizationManager.Instance.GetLanguage() : 0;
        
        string localizedText = EngScript;
        switch (languageIndex) {
            case 1:
                localizedText = KorScript;
                break;
        }
        return ProcessPlaceholders(localizedText, languageIndex);
    }
    
    private ScriptPlaceholderResult ProcessPlaceholders(string baseText, int languageIndex)
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
        
        if (languageIndex == 0)
            result.ProcessedText = ApplyPronounPlaceholders(result.ProcessedText);
        result.ProcessedText = ReplaceBracketMarkup(result.ProcessedText);
        return result;
    }

    private string ApplyPronounPlaceholders(string text)
    {
        if (string.IsNullOrEmpty(text)) 
            return text;

        int accidyGender = GameManager.Instance != null ? (int)GameManager.Instance.GetVariable("AccidyGender") : 0;
        bool isMale = accidyGender == 1;

        text = text.Replace("{subject}", isMale ? "he" : "she");
        text = text.Replace("{Subject}", isMale ? "He" : "She");
        text = text.Replace("{object}", isMale ? "him" : "her");
        text = text.Replace("{Object}", isMale ? "Him" : "Her");
        text = text.Replace("{possessive}", isMale ? "his" : "her");
        text = text.Replace("{Possessive}", isMale ? "His" : "Her");

        return text;
    }

    private string ReplaceBracketMarkup(string text)
    {
        if (string.IsNullOrEmpty(text)) return text;

        int searchStart = 0;
        while (true)
        {
            int openIdx = text.IndexOf('[', searchStart);
            if (openIdx == -1) break;
            int closeIdx = text.IndexOf(']', openIdx + 1);
            if (closeIdx == -1) break;

            string inner = text.Substring(openIdx + 1, closeIdx - openIdx - 1);
            string replacement = $"<color=red>{inner}</color>";
            text = text.Substring(0, openIdx) + replacement + text.Substring(closeIdx + 1);
            searchStart = openIdx + replacement.Length;
        }

        return text;
    }
}