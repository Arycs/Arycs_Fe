using System;
using System.Collections;
using System.Collections.Generic;
using Arycs_Fe.ScriptManagement;
using UnityEngine;

public class RoleTalkCondition : RoleCondition
{
    public int targetId;

    public override MapEventConditionType type
    {
        get { return MapEventConditionType.RoleTalkCondition; }
    }

    public override bool GetResult(MapAction action)
    {
        if (action.SelectedUnit == null || action.TargetUnit == null || action.SelectedUnit.role.roleType != RoleType.Unique || action.TargetUnit.role.roleType != RoleType.Unique)
        {
            return false;
        }

        if (action.SelectedUnit.role.characterId != characterId || action.TargetUnit.role.characterId != targetId)
        {
            return false;
        }

        return true;
    }
}
