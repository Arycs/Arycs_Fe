using System;
using System.Collections;
using System.Collections.Generic;
using Arycs_Fe.Models;
using Arycs_Fe.ScriptManagement;
using UnityEngine;

[Serializable]
public class PropertyCondition : Condition
{
    public FightPropertyType propertyType;

    public int minValue;

    public int maxValue;

    public override MapEventConditionType type
    {
        get { return MapEventConditionType.PropertyCondition; }
    }

    public override bool GetResult(MapAction action)
    {
        if (action.SelectedUnit == null)
        {
            return false;
        }

        Role role = action.SelectedUnit.role;
        FightProperties fightProperties = role.fightProperties;
        return fightProperties[propertyType] >= minValue && fightProperties[propertyType] <= maxValue;
    }
}
