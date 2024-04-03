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
    string TextFormat(string text)
    {
        // 검색하여 특정 텍스트를 찾아 붉은색으로 변경
        int startIndex = text.IndexOf('[');
        int endIndex = text.IndexOf(']');
        if (startIndex != -1 && endIndex != -1 && endIndex > startIndex)
        {
            string newText = text.Substring(0, startIndex);
            newText += "<color=red>";
            newText += text.Substring(startIndex + 1, endIndex - startIndex - 1);
            newText += "</color>";
            newText += text.Substring(endIndex + 1);
            return newText;
        }
        return text;
    }
}