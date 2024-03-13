using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Result
{
    public string ResultID { get; private set; }
    public string Description { get; private set; }
    public string Action { get; private set; }

    public Result(string id, string description, string action)
    {
        ResultID = id;
        Description = description;
        Action = action;
    }
}
