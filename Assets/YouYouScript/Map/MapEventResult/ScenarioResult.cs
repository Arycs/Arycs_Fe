using System.Collections;
using System.Collections.Generic;
using Arycs_Fe.ScriptManagement;
using UnityEngine;

public class ScenarioResult : Result
{
    public string flag;

    public override MapEventResultType type
    {
        get { return MapEventResultType.ScenarioResult; }
    }

    public override bool Trigger(MapAction action)
    {
        return action.ScenarioCommand(flag);
    }
}
