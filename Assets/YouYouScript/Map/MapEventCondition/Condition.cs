using System;
using System.Collections;
using System.Collections.Generic;
using Arycs_Fe.ScriptManagement;
using Sirenix.OdinInspector;
using UnityEngine;

public abstract class Condition
{
    public virtual MapEventConditionType type
    {
        get { return MapEventConditionType.NoneCondition; }
    }

    public virtual bool GetResult(MapAction action)
    {
        return true;
    }
}
