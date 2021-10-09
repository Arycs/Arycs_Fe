using System.Collections;
using System.Collections.Generic;
using Arycs_Fe.Models;
using Arycs_Fe.ScriptManagement;
using UnityEngine;

public class RoleCombatTalkCondition : RoleTalkCondition
{
    public override MapEventConditionType type
    {
        get { return MapEventConditionType.RoleCombatTalkCondition; }
    }

    public override bool GetResult(MapAction action)
    {
        if (action.SelectedUnit == null || action.TargetUnit == null || action.SelectedUnit.role.roleType != RoleType.Unique || action.TargetUnit.role.roleType != RoleType.Unique)
        {
            return false;
        }        
        if ((action.SelectedUnit.role.characterId == characterId
             && action.TargetUnit.role.characterId == targetId)
            || (action.SelectedUnit.role.characterId == targetId
                && action.TargetUnit.role.characterId == characterId))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
