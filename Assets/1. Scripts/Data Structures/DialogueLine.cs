public class DialogueLine
{
    public string SpeakerID { get; private set; }
    public string ScriptID { get; private set; }
    public string ImageID { get; private set; }
    public string Next { get; private set; }

    // initialize function
    public DialogueLine(string speakerID, string scriptID, string imageID, string next)
    {
        SpeakerID = speakerID;
        ScriptID = scriptID;
        ImageID = imageID;
        Next = next;
    }
}