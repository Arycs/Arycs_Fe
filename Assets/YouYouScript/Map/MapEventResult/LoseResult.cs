using System.Collections;
using System.Collections.Generic;
using Arycs_Fe.ScriptManagement;
using UnityEngine;

public class LoseResult : Result
{
    public override MapEventResultType type
    {
        get { return MapEventResultType.LoseResult; }
    }

    public override bool Trigger(MapAction action)
    {
        action.MapEndCommand(-1);
        return true;
    }
}
