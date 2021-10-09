using System;
using System.Collections;
using System.Collections.Generic;
using Arycs_Fe.Models;
using Arycs_Fe.ScriptManagement;
using UnityEngine;

[Serializable]
public class PropertyResult : Result
{
    public FightPropertyType propertyType;

    public int value;

    public override MapEventResultType type
    {
        get { return MapEventResultType.PropertyResult; }
    }

    public override bool Trigger(MapAction action)
    {
        if (action.SelectedUnit == null)
        {
            return false;
        }

        Role role = action.SelectedUnit.role;
        role.AddFightProperty(propertyType, value);
        return true;
    }
}
