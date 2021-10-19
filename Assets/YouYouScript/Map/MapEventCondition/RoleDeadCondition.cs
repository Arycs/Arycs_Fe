using System;
using System.Collections;
using System.Collections.Generic;
using Arycs_Fe.ScriptManagement;
using UnityEngine;
using YouYou;

public class RoleDeadCondition : RoleCondition
{
    public override MapEventConditionType type
    {
        get { return MapEventConditionType.RoleDeadCondition; }
    }

    public override bool GetResult(MapAction action)
    {
        if (!GameEntry.Data.RoleDataManager.m_UniqueRoles.ContainsKey(this.characterId))
        {
            return false;
        }

        return GameEntry.Data.RoleDataManager.GetOrCreateRole(this.characterId, RoleType.Unique).isDead;
    }
}