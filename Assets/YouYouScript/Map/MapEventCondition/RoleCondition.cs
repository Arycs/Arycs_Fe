using System;
using System.Collections;
using System.Collections.Generic;
using Arycs_Fe.Models;
using Arycs_Fe.ScriptManagement;
using UnityEngine;

[Serializable]
public class RoleCondition : Condition
{
    public int characterId;

    public override MapEventConditionType type
    {
        get { return MapEventConditionType.RoleCondition; }
    }

    public override bool GetResult(MapAction action)
    {
        if (action.SelectedUnit == null)
        {
            return false;
        }

        Role role = action.SelectedUnit.role;
        if (role.roleType == RoleType.Following)
        {
            return false;
        }

        return role.characterId == characterId;
    }
}
