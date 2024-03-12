using System.Collections.Generic;

public class LogicConditionResult
{
    public string Logic { get; private set; }
    public List<Condition> Conditions { get; private set; }
    public List<Result> Results { get; private set; }

    public LogicConditionResult(string logic, List<Condition> conditions, List<Result> results)
    {
        Logic = logic;
        Conditions = conditions;
        Results = results;
    }
}