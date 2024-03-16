public class ChoiceLine
{
    public string ScriptID { get; private set; }
    public string Next { get; private set; }

    // initialize function
    public ChoiceLine(string scriptID, string next)
    {
        ScriptID = scriptID;
        Next = next;
    }
}