using System;
using System.Collections;
using System.Collections.Generic;
using Arycs_Fe.ScriptManagement;
using UnityEngine;

public abstract class Result
{
    public virtual MapEventResultType type
    {
        get { return MapEventResultType.NoneResult; }
    }

    public virtual bool Trigger(MapAction action)
    {
        return true;
    }
}
