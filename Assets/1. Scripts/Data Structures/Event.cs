using System.Collections.Generic;
using Unity.VisualScripting;

public class Event
{
    public string EventID { get; private set; }
    public string EventName { get; private set; }
    public string EventDescription { get; private set; }
    public List<EventLine> EventLine { get; private set; }

    // initialize function
    public Event(string id, string name, string description)
    {
        EventID = id;
        EventName = name;
        EventDescription = description;
        EventLine = new List<EventLine>();
    }

    public void AddEventLine(string logic, List<Condition> conditions, List<Result> results)
    {
        EventLine.Add(new EventLine(logic, conditions, results));
    }
}