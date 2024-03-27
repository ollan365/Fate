public class Script
{
    public string ScriptID { get; private set; }
    public string EngScript { get; private set; }
    public string KorScript { get; private set; }
    public string JP_M_Script { get; private set; }
    public string JP_W_Script { get; private set; }

    public Script(string id, string eng, string kor, string jp_m, string jp_w)
    {
        ScriptID = id;
        EngScript = eng;
        KorScript = kor;
        JP_M_Script = jp_m;
        JP_W_Script = jp_w;
    }

    public string GetScript()
    {
        switch (GameManager.Instance.GetVariable("Language"))
        {
            case 0: return EngScript;
            case 1: return KorScript;
            case 2: return JP_M_Script;
            case 3: return JP_W_Script;
            default: return KorScript;
        }
    }
}