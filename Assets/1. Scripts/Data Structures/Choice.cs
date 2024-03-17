using System.Collections.Generic;

public class Choice
{
    public string ChoiceID { get; private set; }
    public List<ChoiceLine> Lines { get; private set; }

    // initialize function
    public Choice(string choiceId)
    {
        ChoiceID = choiceId;
        Lines = new List<ChoiceLine>();
    }
    
    public void AddLine(string scriptID, string next)
    {
        Lines.Add(new ChoiceLine(scriptID, next));
    }
}