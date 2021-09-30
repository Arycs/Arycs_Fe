using System.Collections;
using System.Collections.Generic;
using Arycs_Fe.ScriptManagement;
using UnityEngine;

public class WinResult : Result
{
    public override MapEventResultType type
    {
        get { return MapEventResultType.WinResult; }
    }

    public override bool Trigger(MapAction action)
    {
        action.MapEndCommand(1);
        return true;
    }
}
