using System;
using System.Collections;
using System.Collections.Generic;
using Arycs_Fe.ScriptManagement;
using UnityEngine;

[Serializable]
public class RoleTalkCondition : RoleCondition
{
    public int targetId;

    public override MapEventConditionType type
    {
        get { return MapEventConditionType.RoleTalkCondition; }
    }

    public override bool GetResult(MapAction action)
    {
        if (action.selectedUnit == null || action.targetUnit == null || action.selectedUnit.role.roleType != RoleType.Unique || action.targetUnit.role.roleType != RoleType.Unique)
        {
            return false;
        }

        if (action.selectedUnit.role.characterId != characterId || action.targetUnit.role.characterId != targetId)
        {
            return false;
        }

        return true;
    }
}
