using System.Collections.Generic;

public class Event
{
    public string EventID { get; private set; }
    public string EventName { get; private set; }
    public string EventDescription { get; private set; }
    public List<EventLine> LogicConditionsResults { get; private set; }


    // initialize function
    public Event(string id, string name, string description)
    {
        EventID = id;
        EventName = name;
        EventDescription = description;
        LogicConditionsResults = new List<EventLine>();
    }

    public void AddLogicConditionResult(string logic, List<Condition> conditions, List<Result> results)
    {
        LogicConditionsResults.Add(new EventLine(logic, conditions, results));
    }
}