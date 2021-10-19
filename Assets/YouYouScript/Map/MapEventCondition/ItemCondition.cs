using System;
using System.Linq;
using Arycs_Fe.Models;
using Arycs_Fe.ScriptManagement;
using Sirenix.OdinInspector;
using UnityEngine;
using YouYou;

public class ItemCondition : Condition
{
    public int characterId;

    public int itemId;

    public bool hold; //是否持有

    public override MapEventConditionType type
    {
        get { return MapEventConditionType.ItemCondition; }
    }

    public override bool GetResult(MapAction action)
    {
        Role role;
        if (characterId < 0)
        {
            if (action.SelectedUnit == null)
            {
                return false;
            }

            role = action.SelectedUnit.role;
            if (role.roleType == RoleType.Following)
            {
                return false;
            }
        }
        else
        {
            if (!GameEntry.Data.RoleDataManager.m_UniqueRoles.ContainsKey(characterId))
            {
                return false;
            }

            role = GameEntry.Data.RoleDataManager.GetOrCreateRole(characterId, RoleType.Unique);
        }

        Item[] items = role.items;
        bool hasItem = items.Any(item => item != null && item.itemId == itemId);
        return hasItem == hold;
    }
}
