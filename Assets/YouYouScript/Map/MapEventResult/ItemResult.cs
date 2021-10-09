using System;
using System.Collections;
using System.Collections.Generic;
using Arycs_Fe.Models;
using Arycs_Fe.ScriptManagement;
using UnityEngine;
using YouYou;

[Serializable]
public class ItemResult : Result
{
    public int characterId = -1;

    public bool give;

    public int id;

    public override MapEventResultType type
    {
        get { return MapEventResultType.ItemResult; }
    }

    public override bool Trigger(MapAction action)
    {
        Role role;
        if (characterId  < 0)
        {
            if (action.SelectedUnit == null)
            {
                return false;
            }

            role = action.SelectedUnit.role;
        }
        else
        {
           role = GameEntry.Data.RoleDataManager.GetOrCreateRole(characterId,RoleType.Unique);
        }

        Item item = GameEntry.Data.ItemDataManager.CreateItem(id);
        if (give)
        {
            int index = role.AddItem(item);
            if (index == -1)
            {
                item.Dispose();
                return false;
            }
        }
        else
        {
            int index = -1;
            for (int i = 0; i < role.items.Length; i++)
            {
                if (role.items[i] != null && role.items[i].itemId == id)
                {
                    index = i;
                    break;
                }
            }

            if (index == -1)
            {
                return false;
            }

            item = role.RemoveItem(index);
            item.Dispose();
        }

        return true;
    }
}
