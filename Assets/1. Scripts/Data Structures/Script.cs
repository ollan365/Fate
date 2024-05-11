public class Script
{
    public string ScriptID { get; private set; }
    public string EngScript { get; private set; }
    public string KorScript { get; private set; }
    public string JP_M_Script { get; private set; }
    public string JP_W_Script { get; private set; }
    public string Placeholder { get; private set; }

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
        switch (GameManager.Instance.GetVariable("Language"))
        {
            case 0: return TextFormat(EngScript);
            case 1: return TextFormat(KorScript);
            case 2: return TextFormat(JP_M_Script);
            case 3: return TextFormat(JP_W_Script);
            default: return TextFormat(KorScript);
        }
    }
    private string TextFormat(string text)
    {
        string newText = text;
        bool textEffect = false;
        int startIndex = -1, endIndex = -1;
        for (int i = 0; i < text.Length; i++)
        {
            if(text[i] == '[')
            {
                textEffect = true;
                startIndex = i;
            }
            if(textEffect && text[i] == ']')
            {
                endIndex = i;

                newText = text.Substring(0, startIndex);
                newText += "<color=red>";
                newText += text.Substring(startIndex + 1, endIndex - startIndex - 1);
                newText += "</color>";
                newText += text.Substring(endIndex + 1);

                text = newText;
            }
        }
        return newText;
    }

    private string CheckEffects(string text)
    {
        string[] effects = Placeholder.Split('/');
        for(int i = 0; i < effects.Length; i++)
        {
            switch (effects[i])
            {
                case "RED":
                    break;
                case "FAST":
                    break;
                case "AUTO":
                    break;
            }
        }
        return text;
    }
}