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
        if (action.selectedUnit == null || action.targetUnit == null || action.selectedUnit.role.roleType != RoleType.Unique || action.targetUnit.role.roleType != RoleType.Unique)
        {
            return false;
        }        
        if ((action.selectedUnit.role.characterId == characterId
             && action.targetUnit.role.characterId == targetId)
            || (action.selectedUnit.role.characterId == targetId
                && action.targetUnit.role.characterId == characterId))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
