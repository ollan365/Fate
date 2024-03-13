public class Condition
{
    public string ConditionID { get; private set; }
    public string Description { get; private set; }
    public string VariableName { get; private set; }
    public string Logic { get; private set; }
    public string Value { get; private set; }

    // initialize function
    public Condition(string id, string description, string variableName, string logic, string value)
    {
        ConditionID = id;
        Description = description;
        VariableName = variableName;
        Logic = logic;
        Value = value;
    }
}